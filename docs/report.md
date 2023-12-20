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

## User activities

We have created the following diagrams to illustrate typical user activities/journeys through Chirp.
We strove to create a user-flow that is smooth and functional. Therefore the register/login process is handled with OAuth and Github. This removes the need for a complicated registration (assuming the user has a Github account). 

This can be seen in the diagrams after the user press the "Login" button. This starts the OAuth process. If the user has already been logged in to Chirp before, a Login press will simply handle the login and automatically redirect to the timeline without any further action from the user. 

To ease the understanding of the diagrams, the following picture explains symbols and color definitions:

-- INDSÆT BILLEDE AF DEFINITIONER --

-- INDSÆT DIAGRAMMER AF USER ACTIVITIES --

## Sequence of functionality/calls trough _Chirp!_

# Process

## Build, test, release, and deployment
![Workflows](docs/diagrams/Workflows.jpg "Workflows")

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

`docker pull mcr.microsoft.com/azure-sql-edge`


#### 2. Run the image in a container
Replacing `<YOUR_DB_PASSWORD>` with a strong password (requires 1 upper case, 1 lower case, 1 number, and no special characters), run

```shell
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=<YOUR_DB_PASSWORD>" -p 1433:1433 --name azure-sql-server -d mcr.microsoft.com/azure-sql-edge
```

#### 3. Init secrets
If not done yet, run the following to create a secrets file

```shell
dotnet user-secrets init --project ./src/Chirp.WebService
```

#### 4. Add DB password secret
Add the DB secret by running the following command, replacing `<YOUR_DB_PASSWORD>` with the strong password you generated earlier

```shell
dotnet user-secrets set "DB:Password" "<YOUR_DB_PASSWORD>" --project ./src/Chirp.WebService
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

## How to run test suite locally
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

### Install Playwright
Tests have only one requirement, which is needed to run end-to-end tests: Playwright.

First of all, the powershell dotnet tool is needed, which can be installed with the following command:
```shell
dotnet tool install PowerShell --version 7.4.0
```

After running the tests the first time, and failing, the cause will be due to playwright not be installed. This can can be solved by running the following command:
```shell
dotnet pwsh test/Chirp.WebService.Tests/bin/Debug/net7.0/playwright.ps1 install
```

Everything should now be set up in order to enable tests to run.

### Run tests
To run tests, given the it is set up, simply run the following command:
```shell
dotnet test --verbosity normal
```

# Ethics

## License

## LLMs, ChatGPT, CoPilot, and others
We have utilised AI tools in the development of Chirp, however only when we felt appropriate or necessary. 
Primarily we have used ChatGPT as a tool to troubleshoot code logic, if we have not been able to assess the faults ourselves.

We are using the Tailwind CSS Framework to quickly develop UI components for Chirp. Though we are able to write HTML and CSS ourselves, we have used the Vercel V0 tool to speed up development process, as it can generate HTML with Tailwind styling applied. 
Though this has been used extensively in the UI implementation, we found it appropriate as this course is not a UI-focused course, and we therefore did not wish to prioritize UI over more critical development issues.

V0 is currently in a closed beta, and can therefore not be publicly accessed.