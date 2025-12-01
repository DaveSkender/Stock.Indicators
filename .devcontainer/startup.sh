#!/bin/bash

# Dev Container Startup Script for Stock Indicators
# Handles initialization of development dependencies

echo "🚀 Starting Stock Indicators dev container setup..."

# Verify .NET is available

echo "🔍 Verifying .NET environment..."
dotnet --version

# Install or activate pnpm@10.24.0 for docs site
echo "🔧 Ensuring pnpm@10.24.0 is available..."
if command -v corepack >/dev/null 2>&1; then
	corepack enable
	corepack prepare pnpm@10.24.0 --activate
else
	npm install -g pnpm@10.24.0
fi
pnpm --version

echo "🧰 Installing .NET-based tools..."
dotnet tool install --global dotnet-format
dotnet tool install --global roslynator.dotnet.cli
dotnet tool install --global dotnet-outdated-tool
dotnet tool list --global

# Restore .NET packages
echo "📦 Restoring .NET packages..."
dotnet restore

echo "✅ Dev environment setup complete!"
