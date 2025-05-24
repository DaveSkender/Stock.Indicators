# MCP Servers

This workspace is configured with a Memory MCP server that provides persistent memory capabilities for GitHub Copilot Chat.

## Configuration

The Memory MCP server is configured in `.vscode/mcp.json` and will be automatically started by VS Code when using GitHub Copilot Chat.

### Files

- `.vscode/mcp.json` - MCP server configuration for VS Code

### Server Package

- `@modelcontextprotocol/server-memory` - Executed via `npx` (no local installation needed)

# Memory MCP Server Configuration

This workspace is configured with a Memory MCP server that provides persistent memory capabilities for GitHub Copilot Chat.

## Configuration

The Memory MCP server is configured in `.vscode/mcp.json` and will be automatically started by VS Code when using GitHub Copilot Chat.

### Files

- `.vscode/mcp.json` - MCP server configuration for VS Code

### Server Package

- `@modelcontextprotocol/server-memory` - Executed via `npx` (no local installation needed)

## Usage

Once configured, the Memory MCP server will:

1. Automatically start when you use GitHub Copilot Chat
2. Provide persistent memory across chat sessions
3. Remember context and information you share during conversations

## Memory Storage

The server stores memory data in memory files within the workspace. This allows for persistent context across VS Code sessions.

## Verification

The Memory MCP server is confirmed working! You can test it by:

1. **Knowledge Graph Operations**: The server provides persistent memory through a knowledge graph
2. **Configuration Updated**: Fixed schema to use `"servers"` instead of `"mcpServers"` in `.vscode/mcp.json`
3. **No Global Installation Required**: Verified that global npm installation was removed and `npx` handles package execution automatically
4. **Dev Container Ready**: Added `.devcontainer/startup.sh` for proper initialization

To verify the setup is working:

1. Open GitHub Copilot Chat
2. Ask it to remember something specific about your project
3. Close and reopen VS Code
4. Ask Copilot about what you asked it to remember

The memory should persist across sessions if the MCP server is working correctly.
