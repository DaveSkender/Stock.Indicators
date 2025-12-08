---
description: Configuration for AI behavior when interacting with Codacy's MCP Server
applyTo: '**'
---

# Codacy rules

Configuration for AI behavior when interacting with Codacy's MCP Server

## Repository configuration

When using any tool that accepts the arguments `provider`, `organization`, or `repository`:

- provider: gh
- organization: DaveSkender
- repository: Stock.Indicators

Avoid calling `git remote -v` unless really necessary.

## Optional code quality analysis

Consider running the `codacy_cli_analyze` tool for:

- Complex architectural changes
- Large refactoring efforts
- When explicitly requested by the user

When running `codacy_cli_analyze`:

- `rootPath`: set to the workspace path
- `file`: set to the path of the edited file (optional)
- `tool`: leave empty or unset (or specify "trivy" for security scans)

If the Codacy CLI is not installed, use `npx @codacy/codacy-mcp` to invoke it directly.

## Security checks for dependency changes

IMMEDIATELY after adding or modifying dependencies (.csproj files or other package managers):

Run the `codacy_cli_analyze` tool with:

- `rootPath`: set to the workspace path
- `tool`: set to "trivy"
- `file`: leave empty or unset

If vulnerabilities are found:

- Propose and apply fixes for security issues
- Document the security concerns
- Only continue after security issues are resolved

## Error handling

### When repository is not found (404 error)

If a Codacy tool returns a 404 error:

- Offer to run the `codacy_setup_repository` tool
- Only run setup if the user explicitly accepts
- After setup, retry the failed action once

## General guidelines

- Always use standard, non-URL-encoded file system paths for `rootPath` parameters
- Do not run `codacy_cli_analyze` for code coverage, duplication metrics, or complexity metrics
- Focus on complexity issues, not complexity metrics, when addressing code complexity
- When calling `codacy_cli_analyze`, only send provider, organization, and repository if the project is a git repository

---
Last updated: December 8, 2025
