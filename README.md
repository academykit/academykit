## Academykit LMS

## Technologies

- [ASP.NET Core 7](https://docs.microsoft.com/en-us/aspnet/core/introduction-to-aspnet-core)
- [NodeJS 18.x](https://nodejs.org)
- [Entity Framework Core tools](https://learn.microsoft.com/en-us/ef/core/cli/dotnet)
  - `dotnet tool install --global dotnet-ef`

## Database Setup

To setup database on development run below sql statements on the database

1. `/db/data/seed.sql`
2. `/db/data/testData.sql`

For production during setup or upgrade

1. TBD

## Run In Development

```bash
cd src/Api/ClientApp && npm i && cd ../../../
dotnet watch run --project=src/Api/Api.csproj
```

NOTE: if you are working on more frequent backend changes then, run the frontend separately, [learn more here](https://learn.microsoft.com/en-us/aspnet/core/client-side/spa/react?view=aspnetcore-7.0&tabs=netcore-cli#run-the-cra-server-independently)

```bash
cd ClientApp
npm i
npm start
```

Then run the backend in another terminal, the build and restart will be faster

```bash
dotnet watch run --project=src/Api/Api.csproj
```

The Hangfire Dashboard is available at https://localhost:7042/hangfire

## Database Migrations

To add a new migration from the root folder

```
dotnet ef migrations add "migration message" --project src/Infrastructure --startup-project src/Api -o Persistence/Migrations
```

To remove migrations

```
dotnet ef migrations  remove --project src/Infrastructure --startup-project src/Api
```

To update database

```bash
dotnet ef database update  --project src/Infrastructure --startup-project src/Api
```

## Docker

Build docker image

```bash
docker build -t standalone .
```

Run docker container

```bash
docker run -d -p 8080:80 --name vurilo-standalone standalone
```

## Known issues

1. If during debug, port 7042 is already in use kill the port as

```bash
# mac
sudo lsof -t -i tcp:7042 | xargs kill -9
```
