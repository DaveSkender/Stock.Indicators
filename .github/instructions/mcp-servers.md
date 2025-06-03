# MCP Servers

This workspace is configured with the following MCP servers:

1. **Memory** - Provides persistent memory capabilities for GitHub Copilot Chat
2. **Sequential Thinking** - Provides step-by-step reasoning capabilities
3. **File System** - Provides file system access capabilities

## Configuration

All MCP servers are configured in `.vscode/mcp.json` and automatically start when using GitHub Copilot Chat.

### Server Packages

- `@modelcontextprotocol/server-memory` - Memory server (via npx)
- `@modelcontextprotocol/server-sequential-thinking` - Sequential thinking server (via npx)  
- `@modelcontextprotocol/server-filesystem` - File system server (via npx)

## Memory Server

The Memory server provides persistent memory through a knowledge graph that:

- Maintains context across chat sessions
- Stores information you share during conversations
- Persists data within the workspace

## Sequential Thinking Server

The Sequential Thinking server provides step-by-step reasoning to:

- Analyze complex problems systematically
- Generate solutions by breaking down tasks
- Assist with debugging and code analysis

## File System Server

The File System server enables file operations including:

- Reading and writing files
- Directory listing and searching
- File system navigation within the workspace
- Use compatible path name syntax for bash/shell

## Verification

To verify MCP servers are working:

1. Open GitHub Copilot Chat
2. Test each server:
   - **Memory**: Ask Copilot to remember something, then restart VS Code and ask about it
   - **Sequential Thinking**: Request help with a complex problem
   - **File System**: Ask Copilot to read or search files in your workspace

All servers should function automatically without additional setup.
