# ######################################################
# Build docker image
# ######################################################
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /app/src/

# For debug build pass --build-arg target=Debug
ARG build_proxy
ARG datapath=/web
ARG dependency_track_api_key
ARG dependency_track_host_url
ARG dependency_track_project_name
ARG sonar_host_url
ARG sonar_project_key
ARG sonar_token
ARG target_version=develop
ARG target=Release

# Configure env
ENV DEBCONF_NONINTERACTIVE_SEEN=true
ENV DEBIAN_FRONTEND=noninteractive
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
ENV http_proxy="${build_proxy}"
ENV https_proxy="${build_proxy}"
ENV PATH="$PATH:/root/.dotnet/tools"
ENV SONAR_SCANNER_OPTS="-Djava.net.useSystemProxies=true"

# Install certificates
# attention: COPY does not overwrite existing files!
COPY ./src/docker/certs/ /tmp/certs
RUN cp -rf /tmp/certs/* /etc/ssl/certs/

# Extend the base image by required tools
RUN apt-get update && \
    apt-get install --yes --no-install-recommends wget tar curl git nodejs npm && \
    npm install -g npm
RUN mkdir -p /usr/share/man/man1 && \
    apt-get install --yes --no-install-recommends default-jre
# WORKAROUND - install 3.1 sdk for dotnet test
RUN wget https://packages.microsoft.com/config/debian/10/packages-microsoft-prod.deb -O packages-microsoft-prod.deb && \
    dpkg -i packages-microsoft-prod.deb && \
    apt-get update && \
    apt-get install -y apt-transport-https && \
    apt-get update && \
    apt-get install -y dotnet-sdk-3.1
RUN dotnet tool install --global dotnet-sonarscanner --version 5.0.4 && \
    dotnet tool install --global coverlet.console --version 3.0.2 && \
    dotnet tool install --global CycloneDX --version 1.3.0 && \
    npm install -g @cyclonedx/bom@2.0.2
RUN wget -q -O docker.tgz https://download.docker.com/linux/static/stable/x86_64/docker-20.10.4.tgz && \
    tar xzvf docker.tgz -C /app

# Copy the sources
COPY . /app

# Install and pack the npm project
RUN cd ./SubmissionEvaluation/Client && \
    npm config set proxy $build_proxy && \
    npm config set https-proxy $build_proxy && \
    npm install && \
    npm audit fix && \
    npm run wbp

# Build, Publish and Test the dotnet project
RUN /bin/bash -c "if [ -z \"${sonar_project_key}\" ] || [ -z \"${sonar_host_url}\" ] || [ -z \"${sonar_token}\" ]; then echo \"SonarQube not enabled due to missing sonarqube arguments.\"; else dotnet sonarscanner begin /k:\"${sonar_project_key}\" /v:\"${target_version}\" /d:sonar.host.url=\"${sonar_host_url}\" /d:sonar.login=\"${sonar_token}\" /d:sonar.cs.opencover.reportsPaths=/coverage.opencover.xml; fi" && \
    dotnet build -c ${target} && \
    dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput="/coverage" && \
    dotnet publish ./SubmissionEvaluation/Server -c ${target} -o /app/out && \
    /bin/bash -c "if [ ! -z \"${sonar_token}\" ]; then dotnet sonarscanner end /d:sonar.login=\"${sonar_token}\"; fi"

# Configure appsettings and version
RUN echo "{\n  \"DataPath\": \"$datapath\"\n}" > /app/out/appsettings.json && \
    git rev-parse HEAD > /app/out/version.txt

# Generate SBOM for both projects & upload
RUN rm -rf bom.xml && \
    dotnet CycloneDX -d -r -o . SubmissionEvaluation.sln && \
    cyclonedx-bom -l -a ./bom.xml ./SubmissionEvaluation/Client && \
    /bin/bash -c "echo \"{\\\"projectName\\\": \\\"${dependency_track_project_name}\\\", \\\"projectVersion\\\": \\\"${target_version}\\\", \\\"autoCreate\\\": true, \\\"bom\\\": \\\"\`cat bom.xml | base64\`\\\"}\" > payload.json" && \
    # Attention: The ca-certificates.crt is overwriten in the previous steps, due to are the system certificates "reinstalled" ...
    cp -rf /tmp/certs/* /etc/ssl/certs/ && \
    /bin/bash -c "if [ -z \"${dependency_track_host_url}\" ] || [ -z \"${dependency_track_api_key}\" ]; then echo \"SBOM is not uploaded due to missing dependency track arguments.\"; else curl --cacert /etc/ssl/certs/ca-certificates.crt -X \"PUT\" \"${dependency_track_host_url}/api/v1/bom\" -H \"Content-Type: application/json\" -H \"X-API-Key: ${dependency_track_api_key}\" -d @payload.json; fi"

# ######################################################
# Build runtime image
# ######################################################
FROM mcr.microsoft.com/dotnet/aspnet:3.1

ARG proxy

# Prepare System
ENV DEBCONF_NONINTERACTIVE_SEEN=true
ENV DEBIAN_FRONTEND=noninteractive
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1

# Deploy
WORKDIR /app
COPY --from=build-env /app/out .
COPY --from=build-env /app/docker/docker /usr/bin

# Configure
EXPOSE 80
ENV http_proxy="${proxy}"
ENV https_proxy="${proxy}"
ENTRYPOINT ["dotnet", "SubmissionEvaluation.Server.dll"]
