# ######################################################
# Build docker image
# ######################################################
FROM ubuntu:20.04 AS build

ARG build_proxy
ARG proxy
ENV DEBCONF_NONINTERACTIVE_SEEN=true
ENV DEBIAN_FRONTEND=noninteractive
ENV http_proxy="${build_proxy}"
ENV https_proxy="${build_proxy}"

RUN apt-get update && \
    apt-get upgrade -y && \
    apt-get install -y apt-utils curl wget tar  software-properties-common

# Configure timezone
RUN echo "tzdata tzdata/Areas select Europe" > /tmp/preseed.txt; \
    echo "tzdata tzdata/Zones/Europe select Berlin" >> /tmp/preseed.txt; \
    debconf-set-selections /tmp/preseed.txt && \
    apt-get install -y tzdata

# Configure locale
RUN apt-get install -y locales && \
    locale-gen de_DE.UTF-8 && \
    update-locale LANG=de_DE.UTF-8

# C / C++
RUN apt-get install -y gcc g++ cppcheck make && \
    gcc --version && \
    g++ --version

# CMake
ENV PATH=$PATH:/usr/lib/cmake/bin
RUN wget -q --no-check-certificate -O cmake_inst.sh https://cmake.org/files/v3.15/cmake-3.15.1-Linux-x86_64.sh && \
    chmod +x cmake_inst.sh && \
    mkdir /usr/lib/cmake && \
    ./cmake_inst.sh --skip-license --prefix=/usr/lib/cmake && \
    rm cmake_inst.sh && \
    cmake --version

# Dart
RUN wget https://storage.googleapis.com/dart-archive/channels/stable/release/2.14.4/linux_packages/dart_2.14.4-1_amd64.deb && \
    dpkg -i dart_2.14.4-1_amd64.deb

# Python
RUN apt-get install -y python3 && \
    python3 --version

# Java
RUN apt-get update && \
    apt-get install -y openjdk-17-jdk && \
    java --version

# Maven
ARG MAVEN_VERSION=3.8.5
ARG SHA=89ab8ece99292476447ef6a6800d9842bbb60787b9b8a45c103aa61d2f205a971d8c3ddfb8b03e514455b4173602bd015e82958c0b3ddc1728a57126f773c743
ARG BASE_URL=https://dlcdn.apache.org/maven/maven-3/${MAVEN_VERSION}/binaries/
RUN mkdir -p /usr/share/maven \
  && curl -fsSL -o /tmp/apache-maven.tar.gz ${BASE_URL}/apache-maven-${MAVEN_VERSION}-bin.tar.gz \
  && echo "${SHA} /tmp/apache-maven.tar.gz" | sha512sum -c - \
  && tar -xzf /tmp/apache-maven.tar.gz -C /usr/share/maven --strip-components=1 \
  && rm -f /tmp/apache-maven.tar.gz
ENV PATH=$PATH:/usr/share/maven/bin

# Setup Maven for Proxy
RUN if [[ -z $proxy ]]; then \
    echo "No proxy"; \
    else \
    port=$(python3 -c "from urllib.parse import urlparse; print(urlparse(\"${proxy}\").port);") && \
    host=$(python3 -c "from urllib.parse import urlparse; print(urlparse(\"${proxy}\").hostname);") && \
    proto=$(python3 -c "from urllib.parse import urlparse; print(urlparse(\"${proxy}\").scheme);") && \
    mkdir -p ~/.m2/ && \
    nl='\n' && echo "\
<settings>${nl}\
  <proxies>${nl}\
    <proxy>${nl}\
      <id>proxy</id>${nl}\
      <active>true</active>${nl}\
      <protocol>${proto}</protocol>${nl}\
      <host>${host}</host>${nl}\
      <port>${port}</port>${nl}\
    </proxy>${nl}\
  </proxies>${nl}\
</settings>" > ~/.m2/settings.xml; fi

# Kotlin
ENV PATH=$PATH:/usr/lib/kotlinc/bin
RUN apt-get install -y unzip && \
    cd /usr/lib && \
    wget -q https://github.com/JetBrains/kotlin/releases/download/v1.3.41/kotlin-compiler-1.3.41.zip && \
    unzip kotlin-compiler-*.zip && \
    rm kotlin-compiler-*.zip && \
    rm kotlinc/bin/*.bat && \
    kotlin -version

# Haskell
RUN add-apt-repository -y ppa:hvr/ghc && \
    apt-get update && \
    apt-get install -y ghc && \
    ghc --version

# Scala
RUN apt-get install -y scala && \
    scala -version

# JavaScript & Typescript
RUN su -c "curl -sL https://deb.nodesource.com/setup_16.x | bash -" && \
    apt-get update && \
    apt-get install -y nodejs && \
    npm install -g typescript && \
    node --version && \
    npm --version && \
    tsc --version

# Perl
RUN apt-get install -y perl && \
    perl --version

# Go
RUN apt-get install -y golang  && \
    rm -rf `find /usr/share/go-* -type d -name test` && \
    go version

# Rust
ENV RUSTUP_HOME=/usr/lib/rust
ENV PATH=$PATH:/root/.cargo/bin
RUN wget -q -O rustup.rs https://sh.rustup.rs && \
    chmod +x rustup.rs && \
    ./rustup.rs -y && \
    rm rustup.rs && \
    rustc -V

# Julia
ENV PATH=$PATH:/usr/lib/julia/bin
RUN wget -q -O julia.tar.gz https://julialang-s3.julialang.org/bin/linux/x64/1.1/julia-1.1.1-linux-x86_64.tar.gz && \
    tar xf julia.tar.gz && \
    rm julia.tar.gz && \
    mv julia-* /usr/lib/julia && \
    julia --version

# C# (DotNetCore) / F#
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
RUN apt-get install -y ca-certificates apt-transport-https && \
    wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb && \
    dpkg -i packages-microsoft-prod.deb && \
    apt-get update && \
    apt-get install -y dotnet-sdk-5.0 && \
    dotnet --version

# Tu
RUN apt-get install -y git && \
    git clone --recursive https://github.com/genko/tu.git && \
    cd tu && \
    dotnet build && \
    cd tu-cli/bin/Debug/net5.0 && \
    cp -r * /usr/bin && \
    cd ../../../../.. && \
    rm -rf tu && \
    tu-cli --version



# F# NuGetPackage
ADD packagesfsharp.zip /root/.nuget/
RUN unzip -qo /root/.nuget/packagesfsharp.zip -d /root/.nuget/ && rm /root/.nuget/packagesfsharp.zip

# Uninstall unused packages
RUN apt-get remove -y gnupg dirmngr curl git unzip apt-transport-https wget && \
    apt-get clean && \
    rm -rf `find / -type d -name doc -name man -name tmp`

# Reduce duplicates
RUN apt-get install -y rdfind && \
    rdfind -makeresultsfile false -deleteduplicates true -makehardlinks true /usr && \
    apt-get remove -y rdfind && \
    apt-get clean

RUN mkdir testrun && \
    mkdir testrun/src && \
    mkdir testrun/bin && \
    mkdir testrun/runner


# ######################################################
# Build runtime image
# ######################################################
FROM scratch

ENV DEBCONF_NONINTERACTIVE_SEEN=true
ENV DEBIAN_FRONTEND=noninteractive
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
ENV LANG de_DE.UTF-8
ENV LANGUAGE de_DE:de
ENV LC_ALL de_DE.UTF-8
ENV PATH=$PATH:/root/.cargo/bin:/usr/lib/julia/bin:/usr/lib/kotlinc/bin:/usr/share/maven/bin:/usr/lib/cmake/bin:/usr/bin/processing
ENV RUSTUP_HOME=/usr/lib/rust

ARG proxy
ENV http_proxy=$proxy
ENV https_proxy=$proxy
COPY --from=build / /

# Run dotnet once, else it prints welcome messages with each build
RUN mkdir dotnettest && \
    cd dotnettest && \
    dotnet new console && \
    dotnet publish && \
    cd .. && \
    rm /dotnettest -rf

#Run Maven once, else it will download alot with each build
ADD TestProjectMaven.zip /tmp/
RUN apt-get update \
 && apt-get install -y \
    unzip \
 && rm -rf /var/lib/apt/lists/*
RUN unzip -q /tmp/TestProjectMaven.zip -d /tmp/testprojectmaven/
RUN apt-get -y remove unzip && apt-get clean
RUN mvn --file /tmp/testprojectmaven/pom.xml package
RUN rm /tmp/TestProjectMaven.zip /tmp/testprojectmaven -rf
