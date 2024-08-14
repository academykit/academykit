# AcademyKit

🔥 🔥 👋 🚀 AI-driven Lightweight Open Source Thinkific, Teachable, and fresh Moodle alternative. AcademyKit helps you build your academy for your team or customer in the simplest way possible.

## Technologies

- [ASP.NET Core 8](https://docs.microsoft.com/en-us/aspnet/core/introduction-to-aspnet-core)
- [NodeJS 18.x](https://nodejs.org)
- [Entity Framework Core tools](https://learn.microsoft.com/en-us/ef/core/cli/dotnet)
- [MySQL]

## Database Setup

1. Login into mysql using terminal(CLI)

```bash
mysql -u root -p
```

2. Create a Database

```bash
CREATE DATABASE academykit;
SHOW DATABASES;
```

3 Create a new User

```bash
CREATE USER 'dev'@'localhost' IDENTIFIED BY '4ubiY2A163@f';
```

4.Grant privileges to the new user

```bash
GRANT ALL PRIVILEGES ON academykit.* TO 'dev'@'localhost';
FLUSH PRIVILEGES;
```

5 Exit the MySQL CLI
EXIT;

To setup database on development run below sql statements on the database

1. `/db/data/seed.sql`
2. `/db/data/testData.sql`

For production during setup or upgrade

1. TBD

## Run In Development

```bash
dotnet watch run --project=AcademyKit.Server/AcademyKit.Server.csproj
```

The Hangfire Dashboard is available at https://localhost:7042/hangfire

## Database Migrations

To add a new migration from the root folder

```
dotnet ef migrations add "support content for lesson" --project AcademyKit.Server --startup-project AcademyKit.Server -o Infrastructure/Persistence/Migrations
```

To remove migrations

```
dotnet ef migrations  remove --project AcademyKit.Server --startup-project AcademyKit.Server
```

To update database

```bash
dotnet ef database update  --project AcademyKit.Server --startup-project AcademyKit.Server
```

## Docker

Build docker image

```bash
docker build -t academy .
```

Run docker container

```bash
docker run -d -p 8080:80 -v /var/run/docker.sock:/var/run/docker.sock --name academy academy
```

or on Windows

```cmd
docker run -d -p 8080:80 -v //var/run/docker.sock:/var/run/docker.sock --name academy academy
```

## Known issues

1. If during debug, port 7042 is already in use kill the port as

```bash
# mac
sudo lsof -t -i tcp:7042 | xargs kill -9
```

## Formatting
Run
```bash
dotnet csharpier .
```
