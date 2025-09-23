#!/usr/bin/env bash
set -e
echo "Building Api project..."
cd src/Api
dotnet restore
dotnet build
echo "Build finished."
