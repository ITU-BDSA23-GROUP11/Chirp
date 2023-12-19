# Chirp
Chirp project for 3rd Semester

# Contents

- [How to set up](#how-to-set-up)
  - [How to set up **_Chirp_**](#how-to-set-up-chirp)
    - [Set up Sql Server with Docker](#set-up-sql-server-with-docker)
  - [How to set up tests](#how-to-set-up-tests)
- [How to run **_Chirp_**](#how-to-run-chirp)
- [How to run tests](#how-to-run-tests)

---

# How to set up
First, clone the repository with the following command if you have SSH keys set up for Github:
```shell
git clone git@github.com:ITU-BDSA23-GROUP11/Chirp.git
```
otherwise, if you don't have SSH keys set up for Github, the following command can be used:
```shell
git clone https://github.com/ITU-BDSA23-GROUP11/Chirp.git
```

Thereafter, in order to set up the project, the main dependency you need is `.NET 7.0`.
It can be downloaded from the from [the _'Download .NET 7.0'_ website](https://dotnet.microsoft.com/en-us/download/dotnet/7.0).
> _Make sure to download `.NET 7.0` and not `.NET 8.0`, as **Chirp** will not work otherwise_.

Thereafter, depending on what you want to do, here are the setup guides for the main application, and for tests:
- [How to set up **_Chirp_**]()
- [How to set up tests]()

## How to set up **_Chirp_**
The main requirement needed to run **_Chirp_**, other than .NET, is an `azure-sql-edge` Docker container, for which the setup guide is below.

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

## How to set up tests
Tests have only one requirement, which is needed to run end-to-end tests: playwright.

First of all, the powershell dotnet tool is needed, which can be installed with the following command:
```shell
dotnet tool install PowerShell --version 7.4.0
```

After running the tests the first time, and failing, the cause will be due to playwright not be installed. This can can be solved by running the following command:
```shell
dotnet pwsh test/Chirp.WebService.Tests/bin/Debug/net7.0/playwright.ps1 install
```

Everything should now be set up in order to enable tests to run.

---

# How to run _Chirp_
After the project is set up (see [How to set up](#how-to-set-up)), it can now be run.

When running the profile, make sure to run with the https profile.
This can be done with the following command:
```shell
dotnet run --launch-profile https --project src/Chirp.WebService
```
We would however recommend running it through an IDE such a Rider, which automatically detects launch profiles.

Furthermore, since the project needs to be run on HTTPS, a certificate will be needed.

In our case, using Rider as our IDE, a HTTPS certificate was added after being prompted when running _Chirp_ the first time.
However, a certificate can also be added with:
```shell
dotnet dev-certs https
```

---

# How to run tests
To run tests, given the it is set up (see [How to set up tests](#how-to-set-up-tests)), simply run the following command:
```shell
dotnet test --verbosity normal
```