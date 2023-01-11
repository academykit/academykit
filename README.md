## Vurilo StandAlone

## Technologies

* [ASP.NET Core 6](https://docs.microsoft.com/en-us/aspnet/core/introduction-to-aspnet-core)
* [Entity Framework Core 6](https://docs.microsoft.com/en-us/ef/core/)
* [NodeJS 18.x](https://nodejs.org)
* [FluentValidation](https://fluentvalidation.net/)
* [NUnit](https://nunit.org/), [FluentAssertions](https://fluentassertions.com/), [Moq](https://github.com/moq)

## Run In Development

```bash
dotnet watch run --project=src/Api/Api.csproj
```

## Database Migrations

To add a new migration from the root folder

```
dotnet ef migrations add "migration message" --project src/Infrastructure --startup-project src/Api -o Persistence/Migrations
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

