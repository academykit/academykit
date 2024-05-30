# Use multi-stage builds to separate building and runtime environments
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Set the target architecture environment variable for conditional setups
ARG TARGETARCH

# Update and install base packages, including Node.js and dependencies for Puppeteer
RUN apt-get update && apt-get install -y \
    curl \
    wget \
    gnupg \
    libpng-dev \
    libjpeg-dev \
    libxi6 \
    build-essential \
    libgl1-mesa-glx \
    ffmpeg \
    libxss1 \
    dbus \
    dbus-x11 \
    fonts-ipafont-gothic \
    fonts-wqy-zenhei \
    fonts-thai-tlwg \
    fonts-khmeros \
    fonts-kacst \
    fonts-freefont-ttf \
    --no-install-recommends && \
    curl -sL https://deb.nodesource.com/setup_18.x | bash - && \
    apt-get install -y nodejs && \
    rm -rf /var/lib/apt/lists/*

# Setup Google Chrome repository based on architecture
RUN if [ "$TARGETARCH" = "amd64" ]; then \
        wget -q -O - https://dl-ssl.google.com/linux/linux_signing_key.pub | gpg --dearmor -o /usr/share/keyrings/googlechrome-linux-keyring.gpg && \
        echo "deb [arch=amd64 signed-by=/usr/share/keyrings/googlechrome-linux-keyring.gpg] http://dl.google.com/linux/chrome/deb/ stable main" > /etc/apt/sources.list.d/google.list && \
        apt-get update && \
        apt-get install -y google-chrome-stable --no-install-recommends; \
    fi

# Start D-Bus to support Google Chrome
RUN service dbus start

# Set Puppeteer's executable path environment variable
ENV PUPPETEER_EXECUTABLE_PATH="/usr/bin/google-chrome-stable"

# Build stage for compiling the application
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

RUN apt-get update
RUN apt-get install -y curl
RUN apt-get install -y libpng-dev libjpeg-dev curl libxi6 build-essential libgl1-mesa-glx
RUN curl -sL https://deb.nodesource.com/setup_18.x | bash -
RUN apt-get install -y nodejs

WORKDIR /src
COPY ["./src/Api/Api.csproj", "src/Api/"]
COPY ["./src/Application/Application.csproj", "src/Application/"]
COPY ["./src/Domain/Domain.csproj", "src/Domain/"]
COPY ["./src/Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]
RUN dotnet restore "src/Api/Api.csproj"
COPY . .
WORKDIR "src/Api"
RUN dotnet build "Api.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage/image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Api.dll"]
