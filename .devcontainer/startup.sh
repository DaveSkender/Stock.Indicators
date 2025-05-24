#!/bin/bash

# Dev Container Startup Script for Stock Indicators
# Handles initialization of development dependencies

echo "🚀 Starting Stock Indicators dev container setup..."

# Verify Node.js and npm are available
echo "🔍 Verifying Node.js environment..."
node --version
npm install -g npm@latest

# Install uv package manager if not available
echo "📦 Installing uv package manager for Fetch MCP server..."
if ! command -v uv &> /dev/null; then
  curl -fsSL https://pkg.astral.sh/uv-installer.sh | bash
  echo "✅ uv installed successfully"
else
  echo "✅ uv already installed"
fi

# Add uv to PATH if not already there
if ! command -v uvx &> /dev/null; then
  export PATH="$HOME/.cargo/bin:$PATH"
  echo 'export PATH="$HOME/.cargo/bin:$PATH"' >> ~/.bashrc
  echo "✅ uvx now available in PATH"
else
  echo "✅ uvx already in PATH"
fi

# Restore .NET packages (at end to avoid conflicts)
echo "📦 Restoring .NET packages..."
dotnet restore

echo "✅ Dev container setup complete!"
echo ""
echo "📋 Available MCP servers:"
echo "  - Memory MCP Server (configured in .vscode/mcp.json, via npx)"
echo "  - Fetch MCP Server (configured in .vscode/mcp.json, via uvx)"
echo "  - Sequential Thinking MCP Server (configured in .vscode/mcp.json, via npx)"
echo "  - File System MCP Server (configured in .vscode/mcp.json, via npx)"
echo ""
echo "🔍 Next steps:"
echo "  - Open GitHub Copilot Chat to test MCP server functionality"
echo "  - See .github/instructions/mcp-servers.md for details"
echo ""
