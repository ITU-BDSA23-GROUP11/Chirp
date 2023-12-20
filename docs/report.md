---
title: _Chirp!_ Project Report
subtitle: ITU BDSA 2023 Group `11`
author:
- "Andreas Bartholdy Christensen <anbc@itu.dk>"
- "Marcus Andreas Aandahl <maraa@itu.dk>"
- "Villads Grum-Schwensen <vilg@itu.dk>"
numbersections: true
---

# Design and Architecture of _Chirp!_

## Domain model

## Architecture — In the small

## Architecture of deployed application
![Cloud Architecture](docs/diagrams/CloudArchitecture.jpg "Cloud Architecture")

This diagram shows the cloud architecture of how the clients and different Azure services communicate, in order to serve the said clients.

## User activities

## Sequence of functionality/calls trough _Chirp!_

# Process

## Build, test, release, and deployment
![Workflows](docs/diagrams/Workflows.jpg "Workflows")

This diagram shows the usual flow starting from a pull request (PR) to the deployed application.
As shown, we manage to build, test, release, and deploy our application through the use of Github Workflows/Actions.

### Test and Coverage
Starting from when a PR is created with the default branch (`main`) as its base, the workflows `Build and Test` and `Code coverage` are run every time the PR is changed (fx with a new commit).
These workflows, although similar, differentiate by the `Code coverage` workflow also providing a sticky comment on the PR with the code coverage from the tests.
They both set up, restore, build and run tests.
Together, they act as required checks, for which the PR merging is blocked if any of the tests fail, or if the code coverage is too low (under 60%).

### Deployment
Once a PR has been merged to the main branch, a deployment is initiated through the `Build and deploy` workflow.
The workflow starts by setting up .NET, build the project, publishes the application, creates an artifact, and then deploys it to Azure App Services.

### Publishing
When a new release is created, the `Publish app` workflow runs.
Running as a matrix with common runtime identifiers (RID), it sets up .NET, publishes the application, and appends the published application as a `.zip` asset for the release.
In our case, it creates 3 `.zip` files (one for each RID), specific for the commit at which the workflow was run at.

## Team work

## How to make _Chirp!_ work locally
### Clone Github repository
To make _Chirp!_ work locally, first clone the repository with the following command if you have SSH keys set up for Github:
```shell
git clone git@github.com:ITU-BDSA23-GROUP11/Chirp.git
```
otherwise, if you don't have SSH keys set up for Github, the following command can be used:
```shell
git clone https://github.com/ITU-BDSA23-GROUP11/Chirp.git
```
### Install .NET
Thereafter, in order to set up the project, the main dependency you need is `.NET 7.0`.
It can be downloaded from the from [the _'Download .NET 7.0'_ website](https://dotnet.microsoft.com/en-us/download/dotnet/7.0).
> _Make sure to download `.NET 7.0` and not `.NET 8.0`, as **Chirp** will not work otherwise_.

### Set up Sql Server with Docker
Here are the steps to set up the sql server with docker:

#### 1. Pull docker image

Run the following to pull the docker image

```shell
docker pull mcr.microsoft.com/azure-sql-edge
```

#### 2. Run the image in a container
Replacing `<YOUR_DB_PASSWORD>` with a strong password (requires 1 upper case, 1 lower case, 1 number, and no special characters), run

```shell
docker run -e "ACCEPT_EULA=Y" \
   -e "MSSQL_SA_PASSWORD=<YOUR_DB_PASSWORD>" \
   -p 1433:1433 --name azure-sql-server \
   -d mcr.microsoft.com/azure-sql-edge
```

#### 3. Init secrets
If not done yet, run the following to create a secrets file

```shell
dotnet user-secrets init --project ./src/Chirp.WebService
```

#### 4. Add DB password secret
Add the DB secret by running the following command, replacing `<YOUR_DB_PASSWORD>` with the strong password you generated earlier

```shell
dotnet user-secrets set "DB:Password" "<YOUR_DB_PASSWORD>" \
   --project ./src/Chirp.WebService
```

### Run the project
After the project is set up, it can now be run.

When running the profile, make sure to run with the https profile.
This can be done with the following command:
```shell
dotnet run --launch-profile https --project src/Chirp.WebService
```
We would however recommend running it through an IDE, such a Rider, which automatically detects launch profiles.

Furthermore, since the project needs to be run on HTTPS, a certificate will be needed.

In our case, using Rider as our IDE, a HTTPS certificate was added after being prompted when running _Chirp_ the first time.
However, a certificate can also be added with:
```shell
dotnet dev-certs https
```

## How to run tests locally
### Install .NET
Thereafter, in order to set up the project, the main dependency you need is `.NET 7.0`.
It can be downloaded from the from [the _'Download .NET 7.0'_ website](https://dotnet.microsoft.com/en-us/download/dotnet/7.0).
> _Make sure to download `.NET 7.0` and not `.NET 8.0`, as **Chirp** will not work otherwise_.

### Install Playwright
Tests have only one requirement, which is needed to run end-to-end tests: Playwright.

First of all, the powershell dotnet tool is needed, which can be installed with the following command:
```shell
dotnet tool install PowerShell --version 7.4.0
```

After running the tests the first time, and failing, the cause will be due to playwright not be installed. This can can be solved by running the following command:
```shell
dotnet pwsh \
   test/Chirp.WebService.Tests/bin/Debug/net7.0/playwright.ps1 \
   install
```

Everything should now be set up in order to enable tests to run.

### Run tests
To run tests, given the it is set up, simply run the following command:
```shell
dotnet test --verbosity normal
```

### About the _Chirp_ test suite
During the project, having a robust test suite, along with great coverage, was one of our focus points.
Our tests are set up to reflect the structure of our source code, as to keep the structure coherent.
This was further set into action with the use of a required workflow check, which failed if test coverage was under 60%.

In `Chirp.Tests.Core`, we have included anything our tests might have in common, as to keep our code DRY.
This includes generated fake instances of our models (using [Bogus](https://github.com/bchavez/Bogus)), mocked repositories (using [Moq](https://github.com/devlooped/moq)), fixtures and application factories.

In `Chirp.Infrastructure.Tests`, we aim to cover DbContexts, Models and Repositories.
These tests are primarily unit tests, covering the different functional components found in `Chirp.Infrastructure`, mainly based off of [dotCover](https://www.jetbrains.com/dotcover/) reports.

In `Chirp.WebService.Tests` lies a combination of unit, integration and end-to-end (E2E) tests.
Whilst unit tests cover the functional aspects of our controllers and extension classes, our integration and E2E tests cover user flows an user might have foretaken.

# Ethics

## License

## LLMs, ChatGPT, CoPilot, and others