#!/bin/bash

# Dev Container Startup Script for Stock Indicators
# Handles initialization of development dependencies

echo "🚀 Starting Stock Indicators dev container setup..."

# Verify .NET is available
echo "🔍 Verifying .NET environment..."
dotnet --version

echo "🧰 Installing .NET-based tools..."
dotnet tool install --global dotnet-format
dotnet tool install --global roslynator.dotnet.cli@0.11.0  # bug in 0.12.0
dotnet tool install --global dotnet-outdated-tool
dotnet tool list --global

# Refresh git repo
echo "🗂️ Fetch and pull from git..."
git fetch && git pull

# Restore .NET packages
echo "📦 Restoring .NET packages..."
dotnet restore

echo "✅ Dev environment setup complete!"
