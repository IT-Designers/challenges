ITD-Challenge Platform - Developer Guide
========================

Documentation
---------------------

- Please use `draw.io` and export the charts always as svg with the embedded diagram.
- Please document only with `Markdown`.

VCS
---------------------

- Please develop according to the [Gitflow Workflow](https://www.atlassian.com/git/tutorials/comparing-workflows/gitflow-workflow)
- Please document the issue and git commit type in each git commit
- Please ensure code formatting rules are applied before committing (we use `Resharper`, but also enforced basic rules via `Editorconfig`)

Open Source Compliance
---------------------

Each developer must ensure that only open source compliant third party dependencies or code snippets are used!
For third party libraries, we also track vulnerabilities and OSS compliance of licenses with [OWASP Dependency Track](https://docs.dependencytrack.org/).
To execute Dependency Track locally, simply run:

1. `docker volume create --name dependency-track`
2. `docker run --rm -p 8080:8080 --name dependency-track -v dependency-track:/data owasp/dependency-track`

Then upload the SBOMs generated with [CycloneDX module for .NET](https://github.com/CycloneDX/cyclonedx-dotnet) and [CycloneDX Node.js Module](https://www.npmjs.com/package/@cyclonedx/bom):

1. `dotnet tool install --global CycloneDX; npm install -g @cyclonedx/bom`
2. `dotnet CycloneDX .\src\SubmissionEvaluation.sln -o . -gu <github-username> -gt <github-token>; mv bom.xml bom-dotnet.xml`
3. `cyclonedx-bom -o bom-nodejs.xml .\src\SubmissionEvaluation\Client`

> Note: to resolve github licences, generate a github token without specific rights (<https://github.com/settings/tokens/new>)

Development
---------------------

**Prerequesites**

- Team preference:
  - [Docker/Windows](https://docs.docker.com/docker-for-windows/install/)
  - [Visual Studio 2019](https://www.visualstudio.com/de/downloads)
  - [Resharper](https://www.jetbrains.com/resharper/)
- But feel free to use your own preferred tooling, e.g.:
  - [Docker/Linux](https://docs.docker.com/install/linux/docker-ce/ubuntu/)
  - [Visual Studio Code](https://code.visualstudio.com/)

**Building Developer-Version**

1. You need either Windows or Linux with Docker installed
2. Clone repository and switch into the cloned repository
3. Build docker test image `docker build -t test -f src/docker/test.dockerfile src/docker`
4. Build the webhost image `docker build -t webhost --build-arg target=Debug -f src/docker/aspnet.dockerfile .`
5. Run system:
   1. Windows: `docker run -it --rm -v %cd%/web:/web -v /var/run/docker.sock:/var/run/docker.sock -p 80:80 webhost`
   2. Linux:   `docker run -it --rm -v`pwd`/web:/web -v /var/run/docker.sock:/var/run/docker.sock -p 80:80 webhost`
6. Finally attach the debugger of your choice to the running docker container

**Building Release-Version**

1. You need either Windows or Linux with Docker installed
2. Clone repository and switch into the cloned repository
3. Build docker test image `docker build -t test -f src/docker/test.dockerfile src/docker`
4. Build the webhost image `docker build -t webhost --build-arg target=Release -f src/docker/aspnet.dockerfile .`
5. Run system:
   1. Windows: `docker run -it --rm -v %cd%/web:/web -v /var/run/docker.sock:/var/run/docker.sock -p 80:80 webhost`
   2. Linux:   `docker run -it --rm -v`pwd`/web:/web -v /var/run/docker.sock:/var/run/docker.sock -p 80:80 webhost`

Development almost without Docker (legacy)
---------------------

> “Almost without Docker” means, that the sln project itself can be build without docker.
> However, the `test` image must still be build `docker build -t test -f src/docker/test.dockerfile src/docker` from project root folder, as it provides the necessary compilers.

**Prerequesites**

- dotnet Core SDK 3.1
- NodeJs and NPM
- Docker (is still needed to run the submissions)

**Building & Running the Developer-Version**

Steps:

- Build docker test image:              `docker build -t test -f src/docker/test.dockerfile src/docker`
- Install & pack the node dependencies: `pushd src\SubmissionEvaluation\Client; npm install; npm run wbp; popd`
- Build & publish the dotnet project:   `pushd src\; dotnet restore; dotnet publish; popd`
- Ensure the path in `src\SubmissionEvaluation\Server\bin\Debug\netcoreapp3.1\publish\appsettings.json` points to `"DataPath": "../../../../../../../web"`
- To run the system, execute:           `pushd src\SubmissionEvaluation\Server\bin\Debug\netcoreapp3.1\publish\; dotnet .\SubmissionEvaluation.Server.dll`

Setup your platform
---------------------

- Tailor the challenge platform to your needs via the `web\settings.json` file.
- At the first start, opens the website a setup page to add an admin account. Therefore, you have to enter the security key which you can either find in the generated file `web\security_token.txt`.

<!-- styling section -->
<style>
    body {text-align: justify}
</style>
