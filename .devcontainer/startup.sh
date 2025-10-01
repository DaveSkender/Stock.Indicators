#!/bin/bash

# Dev Container Startup Script for Stock Indicators
# Handles initialization of development dependencies

echo "🚀 Starting Stock Indicators dev container setup..."

# Verify Node.js and npm are available
echo "🔍 Verifying Node.js environment..."
node --version
npm i -g npm@latest

# Restore global tools
echo "📦 Restoring Angular CLI..."
npm install -g @angular/cli

echo "📦 Restoring Spec-Kit..."
uv tool install specify-cli --from git+https://github.com/github/spec-kit.git

# Restore .NET packages
echo "📦 Restoring .NET packages..."
dotnet restore

echo "✅ Dev container setup complete!"
