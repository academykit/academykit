FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

RUN apt-get update
RUN apt-get install -y curl
RUN apt-get install -y libpng-dev libjpeg-dev curl libxi6 build-essential libgl1-mesa-glx
RUN curl -sL https://deb.nodesource.com/setup_18.x | bash -
RUN apt-get install -y nodejs

#####################
#PUPPETEER RECIPE
#####################
RUN apt-get update \
    && apt-get install -y wget gnupg \
    && wget -q -O - https://dl-ssl.google.com/linux/linux_signing_key.pub | gpg --dearmor -o /usr/share/keyrings/googlechrome-linux-keyring.gpg \
    && sh -c 'echo "deb [arch=amd64 signed-by=/usr/share/keyrings/googlechrome-linux-keyring.gpg] http://dl.google.com/linux/chrome/deb/ stable main" >> /etc/apt/sources.list.d/google.list' \
    && apt-get update \
    && apt-get install -y google-chrome-stable fonts-ipafont-gothic fonts-wqy-zenhei fonts-thai-tlwg fonts-khmeros fonts-kacst fonts-freefont-ttf libxss1 dbus dbus-x11 \
    --no-install-recommends \
    && service dbus start \
    && rm -rf /var/lib/apt/lists/*

ENV PUPPETEER_EXECUTABLE_PATH "/usr/bin/google-chrome-stable"

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

# run build over the API project
WORKDIR "src/Api"
RUN dotnet build "Api.csproj" -c Release -o /app/build

# run publish over the API project
FROM build AS publish
RUN dotnet publish "Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
RUN apt-get update && apt-get install -y ffmpeg
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Api.dll"]