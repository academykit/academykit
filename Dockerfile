# Use multi-stage builds to separate building and runtime environments
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS prepare-restore-files
ENV PATH="${PATH}:/root/.dotnet/tools"

WORKDIR /source
COPY .config/dotnet-tools.json .config/
RUN dotnet tool restore
COPY . .

RUN dotnet subset restore ./AcademyKit.Server/AcademyKit.Server.csproj --root-directory /source --output restore_subset/

# Set the target architecture environment variable for conditional setups
ARG TARGETARCH

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source
COPY --from=prepare-restore-files /source/restore_subset .
RUN dotnet restore ./AcademyKit.Server/AcademyKit.Server.csproj

# Build stage for compiling the application
# RUN apt-get update && apt-get install -y curl
RUN curl -sL https://deb.nodesource.com/setup_18.x | bash - && apt-get install -y nodejs

# COPY ["./academykit.client/academykit.client.esproj", "./academykit.client/"]
# COPY ["./AcademyKit.Server/AcademyKit.Server.csproj", "./AcademyKit.Server/"]
# RUN dotnet restore "./AcademyKit.Server/AcademyKit.Server.csproj"
COPY . .

# Publish the application
RUN dotnet publish "./AcademyKit.Server/AcademyKit.Server.csproj" --no-restore -c Release -o /app/publish /p:UseAppHost=false

# Final stage/image
FROM ghcr.io/academykit/academykit-base:main AS final

WORKDIR /app
EXPOSE 80
EXPOSE 443

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AcademyKit.Server.dll"]
