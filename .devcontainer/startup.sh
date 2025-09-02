#!/bin/bash

# Dev Container Startup Script for Stock Indicators
# Handles initialization of development dependencies

echo "🚀 Starting Stock Indicators dev container setup..."

# Verify Node.js and npm are available
echo "🔍 Verifying Node.js environment..."
node --version
npm install -g npm@latest

# Restore .NET packages
echo "📦 Restoring .NET packages..."
dotnet restore

echo "✅ Dev container setup complete!"
echo ""
echo "📋 Available MCP servers:"
echo "  - Memory Server (via npx)"
echo "  - Sequential Thinking Server (via npx)"
echo "  - File System Server (via npx)"
echo ""
echo "🔍 Next steps:"
echo "  - Open GitHub Copilot Chat to test MCP server functionality"
echo "  - See .github/instructions/mcp-servers.md for details"
echo ""
