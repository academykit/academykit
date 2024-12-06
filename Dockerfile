# Final stage/image
FROM ghcr.io/academykit/academykit-base:latest AS base

WORKDIR /app
EXPOSE 80
EXPOSE 443

# Set the target architecture environment variable for conditional setups
ARG TARGETARCH

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# RUN apt-get update && apt-get install -y curl
RUN curl -sL https://deb.nodesource.com/setup_18.x | bash - && apt-get install -y nodejs

WORKDIR /src

COPY ["./academykit.client/academykit.client.esproj", "./academykit.client/"]
COPY ["./AcademyKit.Server/AcademyKit.Server.csproj", "./AcademyKit.Server/"]

RUN dotnet restore ./AcademyKit.Server/AcademyKit.Server.csproj

COPY . .

# Publish the application
RUN dotnet publish "./AcademyKit.Server/AcademyKit.Server.csproj" --no-restore -c Release -o /app/publish /p:UseAppHost=false


FROM base AS final
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AcademyKit.Server.dll"]
