#!/bin/bash

set -e  # Exit immediately if a command fails

echo "🚀 Starting deployment..."

PROJECT_PATH="src/PayVault.API/PayVault.API.csproj"
OUTPUT_DIR="out"
DLL_NAME="PayVault.API.dll"

echo "📦 Building app..."
dotnet build $PROJECT_PATH

echo "📤 Publishing app..."
dotnet publish $PROJECT_PATH -c Release -o $OUTPUT_DIR

echo "▶️ Running app..."
cd $OUTPUT_DIR

if [ ! -f "$DLL_NAME" ]; then
    echo "❌ Error: $DLL_NAME not found in $OUTPUT_DIR"
    exit 1
fi

dotnet $DLL_NAME