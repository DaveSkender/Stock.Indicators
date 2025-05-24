#!/bin/bash

# Dev Container Startup Script for Stock Indicators
# Handles initialization of development dependencies

echo "🚀 Starting Stock Indicators dev container setup..."

# Verify Node.js and npm are available
echo "🔍 Verifying Node.js environment..."
node --version
npm --version

# Restore .NET packages (at end to avoid conflicts)
echo "📦 Restoring .NET packages..."
dotnet restore

echo "✅ Dev container setup complete!"
echo ""
echo "📋 Available MCP servers:"
echo "  - Memory MCP Server (configured in .vscode/mcp.json)"
echo "  - Server will be downloaded via npx when needed"
echo ""
echo "�� Next steps:"
echo "  - Open GitHub Copilot Chat to test memory functionality"
echo "  - See .github/instructions/mcp-memory-server.md for details"
echo ""
