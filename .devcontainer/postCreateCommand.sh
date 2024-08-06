#!/bin/bash

echo "🛠 cleaning the solution"
# npm i -g rimraf
# npm run clean

dotnet dev-certs https --trust

echo "restoring the solution file"
dotnet restore academyKit.sln

echo "installing dotnet tools"
dotnet tool restore

echo "updating the database"
dotnet ef database update  --project AcademyKit.Server --startup-project AcademyKit.Server

echo "✅ All setup done ✅"