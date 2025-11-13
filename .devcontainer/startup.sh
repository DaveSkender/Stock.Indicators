#!/bin/bash

# Dev Container Startup Script for Stock Indicators
# Handles initialization of development dependencies

echo "🚀 Starting Stock Indicators dev container setup..."

# Verify .NET is available
echo "🔍 Verifying .NET environment..."
dotnet --version

echo "🧰 Installing .NET-based tools..."
dotnet tool install --global dotnet-format
dotnet tool install --global roslynator.dotnet.cli
dotnet tool install --global dotnet-outdated-tool
dotnet tool list --global

# Restore .NET packages
echo "📦 Restoring .NET packages..."
dotnet restore

# Symlink dotnet global tools to /usr/local/bin for immediate availability
DOTNET_TOOLS_DIR="${HOME}/.dotnet/tools"

if [ -d "$DOTNET_TOOLS_DIR" ]; then
  echo "🔧 Making dotnet global tools available system-wide..."

  for shim in "$DOTNET_TOOLS_DIR"/*; do
    if [ -f "$shim" ] && [ -x "$shim" ]; then
      sudo ln -sfn "$shim" "/usr/local/bin/$(basename "$shim")" || true
    fi
  done
fi

echo "✅ Dev environment setup complete!"
