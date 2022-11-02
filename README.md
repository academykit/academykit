## Vurilo StandAlone

## Technologies

* [ASP.NET Core 6](https://docs.microsoft.com/en-us/aspnet/core/introduction-to-aspnet-core)
* [Entity Framework Core 6](https://docs.microsoft.com/en-us/ef/core/)
* [FluentValidation](https://fluentvalidation.net/)
* [NUnit](https://nunit.org/), [FluentAssertions](https://fluentassertions.com/), [Moq](https://github.com/moq)

## Run In Development
Navigate to src/Api 
`dotnet watch run`

## Database Migrations

To add a new migration from the root folder

`dotnet ef migrations add "migration message" --project src/Infrastructure --startup-project src/Api -o Persistence/Migrations`

To update database

`dotnet ef database update  --project src/Infrastructure --startup-project src/Api`

## Docker

Build docker image

` docker build -t standone . `

Run docker container

`docker run -d -p 8080:80 --name vurilo-standalone standone`

