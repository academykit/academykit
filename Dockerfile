# Use multi-stage builds to separate building and runtime environments
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the target architecture environment variable for conditional setups
ARG TARGETARCH

# Update and install base packages, including Node.js and dependencies for Puppeteer
# RUN apt-get update && apt-get install -y \
#     curl \
#     wget \
#     gnupg \
#     libpng-dev \
#     libjpeg-dev \
#     libxi6 \
#     build-essential \
#     libgl1-mesa-glx \
#     ffmpeg \
#     libxss1 \
#     dbus \
#     dbus-x11 \
#     fonts-ipafont-gothic \
#     fonts-wqy-zenhei \
#     fonts-thai-tlwg \
#     fonts-khmeros \
#     fonts-kacst \
#     fonts-freefont-ttf \
#     --no-install-recommends && \
#     curl -sL https://deb.nodesource.com/setup_18.x | bash - && \
#     apt-get install -y nodejs && \
#     rm -rf /var/lib/apt/lists/*

# Setup Google Chrome repository based on architecture
# RUN if [ "$TARGETARCH" = "amd64" ]; then \
#     wget -q -O - https://dl-ssl.google.com/linux/linux_signing_key.pub | gpg --dearmor -o /usr/share/keyrings/googlechrome-linux-keyring.gpg && \
#     echo "deb [arch=amd64 signed-by=/usr/share/keyrings/googlechrome-linux-keyring.gpg] http://dl.google.com/linux/chrome/deb/ stable main" > /etc/apt/sources.list.d/google.list && \
#     apt-get update && \
#     apt-get install -y google-chrome-stable --no-install-recommends; \
#     fi

# # Start D-Bus to support Google Chrome
# RUN service dbus start

# # Set Puppeteer's executable path environment variable
# ENV PUPPETEER_EXECUTABLE_PATH="/usr/bin/google-chrome-stable"

# Build stage for compiling the application
# RUN apt-get update && apt-get install -y curl
RUN curl -sL https://deb.nodesource.com/setup_18.x | bash - && apt-get install -y nodejs

WORKDIR /AcademyKit

# COPY ["./academykit.client/academykit.client.esproj", "./academykit.client/"]
# COPY ["./AcademyKit.Server/AcademyKit.Server.csproj", "./AcademyKit.Server/"]
# RUN dotnet restore "./AcademyKit.Server/AcademyKit.Server.csproj"
COPY . .

# Publish the application
RUN dotnet publish "./AcademyKit.Server/AcademyKit.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage/image
FROM ghcr.io/academykit/academykit-base:main AS final

WORKDIR /app
EXPOSE 80
EXPOSE 443

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AcademyKit.Server.dll"]
