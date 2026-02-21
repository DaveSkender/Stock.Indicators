#!/bin/bash

# Dev Container Startup Script for Stock Indicators
# Handles initialization of development dependencies

echo "🚀 Starting Stock Indicators dev container setup..."

# Verify .NET is available
echo "🔍 Verifying .NET environment..."
dotnet --version

echo "🧰 Installing .NET-based tools..."
dotnet tool restore

# Refresh git repo
echo "🗂️ Fetch and pull from git..."
git fetch && git pull

# Restore .NET packages
echo "📦 Restoring .NET packages..."
dotnet restore

echo "✅ Dev environment setup complete!"
