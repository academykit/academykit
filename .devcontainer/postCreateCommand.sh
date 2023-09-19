#!/bin/bash

echo "🛠 cleaning the solution"
# npm i -g rimraf
# npm run clean

dotnet dev-certs https --trust

echo "restoring the solution file"
dotnet restore lingtren.sln

echo "installing dotnet tools"
dotnet tool restore

echo "installing client app"
npm i 
cd src/Api/ClientApp && npm i && cd ../../../


echo "updating the database"
dotnet ef database update  --project src/Infrastructure --startup-project src/Api

echo "✅ All setup done ✅"