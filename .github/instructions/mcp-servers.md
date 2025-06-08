# MCP Servers

This workspace is configured with the following MCP servers:

1. Memory MCP server that provides persistent memory capabilities for GitHub Copilot Chat
2. Fetch MCP server that provides web content retrieval capabilities for GitHub Copilot Chat
3. Sequential Thinking MCP server that provides step-by-step reasoning capabilities for GitHub Copilot Chat
4. File System MCP server that provides file system capabilities for GitHub Copilot Chat

## General Configuration

All MCP servers are configured in `.vscode/mcp.json` and will be automatically started by VS Code when using GitHub Copilot Chat.

### Configuration Files

- `.vscode/mcp.json` - MCP server configuration for VS Code
- `.vscode/settings.json` - Settings that enable MCP functionality

### Server Packages

- `@modelcontextprotocol/server-memory` - Memory MCP server executed via `npx` (no local installation needed)
- `mcp-server-fetch` - Fetch MCP server executed via `uvx` (installed automatically via startup.sh)
- `@mcp/sequential-thinking` - Sequential Thinking MCP server executed via `npx` (no local installation needed)
- `@mcp/filesystem` - File System MCP server executed via `npx` (no local installation needed)

# Memory MCP Server Configuration

This workspace is configured with a Memory MCP server that provides persistent memory capabilities for GitHub Copilot Chat.

## Memory Server Configuration

The Memory MCP server is configured in `.vscode/mcp.json` and will be automatically started by VS Code when using GitHub Copilot Chat.

### Memory Server Package

- `@modelcontextprotocol/server-memory` - Executed via `npx` (no local installation needed)

## Memory Server Usage

Once configured, the Memory MCP server will:

1. Automatically start when you use GitHub Copilot Chat
2. Provide persistent memory across chat sessions
3. Remember context and information you share during conversations

## Memory Storage

The server stores memory data in memory files within the workspace. This allows for persistent context across VS Code sessions.

## Memory Server Verification

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

# Fetch MCP Server Configuration

This workspace is configured with a Fetch MCP server that provides web content retrieval capabilities for GitHub Copilot Chat.

## Fetch Server Configuration

The Fetch MCP server is configured in `.vscode/mcp.json` and will be automatically started by VS Code when using GitHub Copilot Chat.

### Fetch Server Package

- `mcp-server-fetch` - Executed via `uvx` (installed automatically via startup.sh)

### Fetch Server Requirements

The Fetch MCP server requires the uv package manager:

1. **uv Installation**: The `.devcontainer/startup.sh` script automatically installs uv during container startup
2. **PATH Configuration**: The startup script adds `$HOME/.cargo/bin` to the PATH to make `uvx` available
3. **Automatic Execution**: When used, uvx will automatically download and run `mcp-server-fetch`

## Fetch Server Usage

Once configured, the Fetch MCP server will:

1. Automatically start when you use GitHub Copilot Chat
2. Enable Copilot to retrieve content from web resources
3. Provide up-to-date information from online documentation and resources
4. Support non-mutating web content operations through the MCP protocol

## Fetch Server Functionality

The Fetch MCP server provides several web content tools:

1. **Web Page Retrieval**: Get content from web pages to answer questions
2. **API Documentation Access**: Access and search API documentation
3. **Code Examples**: Find code examples from online resources
4. **Library Information**: Get information about libraries and frameworks

## Fetch Server Verification

To verify the Fetch MCP server is working correctly:

1. Open GitHub Copilot Chat
2. Ask it questions that require up-to-date web information, such as:
   - "What is the latest version of .NET?"
   - "How do I use the latest HTTP client in C#?"
   - "Find me documentation about stock indicators"
3. Copilot should be able to provide accurate information from web resources

Note that the Fetch MCP server is read-only (non-mutating) and cannot make changes to web resources.

# Sequential Thinking MCP Server Configuration

This workspace is configured with a Sequential Thinking MCP server that provides step-by-step reasoning capabilities for GitHub Copilot Chat.

## Sequential Thinking Server Configuration

The Sequential Thinking MCP server is configured in `.vscode/mcp.json` and will be automatically started by VS Code when using GitHub Copilot Chat.

### Sequential Thinking Server Package

- `@mcp/sequential-thinking` - Executed via `npx` (no local installation needed)

## Sequential Thinking Server Usage

Once configured, the Sequential Thinking MCP server will:

1. Automatically start when you use GitHub Copilot Chat
2. Provide step-by-step reasoning capabilities to Copilot Chat
3. Assist in breaking down complex problems and generating solutions

## Sequential Thinking Server Functionality

The Sequential Thinking MCP server provides step-by-step reasoning tools:

1. **Problem Analysis**: Analyze and understand complex problems
2. **Solution Generation**: Generate solutions by breaking down problems into smaller parts
3. **Code Generation**: Assist in writing code by providing step-by-step guidance
4. **Debugging Assistance**: Help identify and fix issues in code

## Sequential Thinking Server Verification

To verify the Sequential Thinking MCP server is working correctly:

1. Open GitHub Copilot Chat
2. Ask it to help with a complex problem or code issue
3. Copilot should be able to provide step-by-step assistance and generate solutions

# File System MCP Server Configuration

This workspace is configured with a File System MCP server that provides file system capabilities for GitHub Copilot Chat.

## File System Server Configuration

The File System MCP server is configured in `.vscode/mcp.json` and will be automatically started by VS Code when using GitHub Copilot Chat.

### File System Server Package

- `@mcp/filesystem` - Executed via `npx` (no local installation needed)

## File System Server Usage

Once configured, the File System MCP server will:

1. Automatically start when you use GitHub Copilot Chat
2. Provide file system capabilities to Copilot Chat
3. Allow Copilot to interact with the file system, such as reading and writing files

## File System Server Functionality

The File System MCP server provides file system tools:

1. **File Reading**: Read files from the file system
2. **File Writing**: Write files to the file system
3. **Directory Listing**: List files and directories in a given path
4. **File Searching**: Search for files matching specific criteria

## File System Server Verification

To verify the File System MCP server is working correctly:

1. Open GitHub Copilot Chat
2. Ask it to read, write, or search for files in your workspace
3. Copilot should be able to perform file system operations as requested
