## Vurilo StandAlone

## Technologies

* [ASP.NET Core 6](https://docs.microsoft.com/en-us/aspnet/core/introduction-to-aspnet-core)
* [Entity Framework Core 6](https://docs.microsoft.com/en-us/ef/core/)
* [NodeJS 18.x](https://nodejs.org)
* [FluentValidation](https://fluentvalidation.net/)
* [NUnit](https://nunit.org/), [FluentAssertions](https://fluentassertions.com/), [Moq](https://github.com/moq)

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

The Hangfire Dashboard is available at https://localhost:7042/jobs 

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

