FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY ["./src/Api/Api.csproj", "src/Api/"]
COPY ["./src/Application/Application.csproj", "src/Application/"]
COPY ["./src/Domain/Domain.csproj", "src/Domain/"]
COPY ["./src/Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]

RUN dotnet restore "src/Api/Api.csproj"

COPY . .

# run build over the API project
WORKDIR "src/Api"
RUN dotnet build "Api.csproj" -c Release -o /app/build

# run publish over the API project
FROM build AS publish
RUN dotnet publish "Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Api.dll"]