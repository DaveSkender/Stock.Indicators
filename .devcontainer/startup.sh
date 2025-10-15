#!/bin/bash

# Dev Container Startup Script for Stock Indicators
# Handles initialization of development dependencies

echo "🚀 Starting Stock Indicators dev container setup..."

# Add .NET tools directory to PATH for roslynator and other global tools
export PATH="/home/vscode/.local/bin:/home/vscode/.dotnet/tools:$PATH"

# Verify Node.js and npm are available
echo "🔍 Verifying Node.js environment..."
node --version
npm install --global npm@latest
npm --version

# Verify .NET is available
echo "🔍 Verifying .NET environment..."
dotnet --version

# Restore global tools
echo "🧰 Installing NPM-based tools..."
npm install --global @angular/cli
npm list --global

echo "🧰 Installing .NET-based tools..."
dotnet tool install --global dotnet-format
dotnet tool install --global roslynator.dotnet.cli
dotnet tool install --global dotnet-outdated-tool
dotnet tool list --global

echo "🧰 Installing UV-based (Python) tools..."
uv tool install --force specify-cli --from git+https://github.com/github/spec-kit.git
uv tool list

# Restore .NET packages
echo "📦 Restoring .NET packages..."
dotnet restore

echo "✅ Dev environment setup complete!"
