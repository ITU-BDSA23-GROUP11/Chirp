# Chirp
Chirp project for 3rd Semester

## How to set up
### Sql Server with Docker
Here are the steps to set up the sql server with docker:

#### 1. Pull docker image

Run the following to pull the docker image

`docker pull mcr.microsoft.com/azure-sql-edge`


#### 2. Run the image in a container
Replacing `<YOUR_POSTGRES_PASSWORD>` with a strong password, run
```
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=<YOUR_POSTGRES_PASSWORD>" \                                                 
   -p 1433:1433 --name azure-sql-server \
   -d \
   mcr.microsoft.com/azure-sql-edge
```

#### 3. Init secrets
If not done yet, run the following to create a secrets file

`dotnet user-secrets init --project ./src/Chirp.WebService`

#### 4. Add DB password secret
Add the DB secret by running the following command, replacing `<YOUR_POSTGRES_PASSWORD>` with the strong password you generated earlier

`dotnet user-secrets set "DB:Password" "<YOUR_POSTGRES_PASSWORD>"`