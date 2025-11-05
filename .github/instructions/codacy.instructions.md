---
description: Configuration for AI behavior when interacting with Codacy's MCP Server
applyTo: '**'
---

# Codacy rules

Configuration for AI behavior when interacting with Codacy's MCP Server

## using any tool that accepts the arguments: `provider`, `organization`, or `repository`

- ALWAYS use:
  - provider: gh
  - organization: DaveSkender
  - repository: Stock.Indicators
  - Avoid calling `git remote -v` unless really necessary

## After ANY successful `edit_file` or `reapply` operation

- For complex or significant file changes run the `codacy_cli_analyze` tool from Codacy's MCP Server for each file that was edited before finishing your turn, with:
  - `rootPath`: set to the workspace path
  - `file`: set to the path of the edited file
  - `tool`: leave empty or unset
- If any issues are found in the new edits, apply fixes for them.
- This tool does not need to be run on every edit if you expect more edits or for small changes to a file.

## When you tried to run the `codacy_cli_analyze` tool and the Codacy CLI is not installed

- Simply use `npx @codacy/codacy-mcp` to invoke the Codacy CLI directly
- npx will automatically download and run the tool without requiring installation
- Do not ask the user to install anything
- Continue with the analysis using the npx command

## After every response

- If you made any file edits in this conversation, verify you ran `codacy_cli_analyze` tool from Codacy's MCP Server

## When there are no Codacy MCP Server tools available, or the MCP Server is not reachable

- Suggest the user the following troubleshooting steps:
  - Try to reset the MCP on the extension
  - If the user is using VSCode, suggest them to review their Copilot > MCP settings in Github, under their organization or personal account. Refer them to Settings > Copilot > Enable MCP servers in Copilot. Suggested URL (<https://github.com/settings/copilot/features>) or <https://github.com/organizations/{organization-name}/settings/copilot/features> (This can only be done by their organization admins / owners)
- If none of the above steps work, suggest the user to contact Codacy support

## Trying to call a tool that needs a rootPath as a parameter

- Always use the standard, non-URL-encoded file system path

## CRITICAL: Dependencies and Security Checks

- IMMEDIATELY after ANY of these actions:
  - Adding dependencies to requirements.txt
  - Adding dependencies to pom.xml
  - Adding dependencies to build.gradle
  - Adding dependencies to .csproj files
  - Any other package manager operations (excluding npm/yarn/pnpm as this project does not use Node.js packages)
- You MUST run the `codacy_cli_analyze` tool with:
  - `rootPath`: set to the workspace path
  - `tool`: set to "trivy"
  - `file`: leave empty or unset
- If any vulnerabilities are found because of the newly added packages:
  - Stop all other operations
  - Propose and apply fixes for the security issues
  - Only continue with the original task after security issues are resolved

## General

- Repeat the relevant steps for each modified file.
- "Propose fixes" means to both suggest and, if possible, automatically apply the fixes.
- You MUST NOT wait for the user to ask for analysis or remind you to run the tool.
- Do not run `codacy_cli_analyze` looking for changes in duplicated code or code complexity metrics.
- Complexity metrics are different from complexity issues. When trying to fix complexity in a repository or file, focus on solving the complexity issues and ignore the complexity metric.
- Do not run `codacy_cli_analyze` looking for changes in code coverage.
- The Codacy CLI can be invoked via `npx @codacy/codacy-mcp` when needed.
- When calling `codacy_cli_analyze`, only send provider, organization and repository if the project is a git repository.

## Whenever a call to a Codacy tool that uses `repository` or `organization` as a parameter returns a 404 error

- Offer to run the `codacy_setup_repository` tool to add the repository to Codacy
- If the user accepts, run the `codacy_setup_repository` tool
- Do not ever try to run the `codacy_setup_repository` tool on your own
- After setup, immediately retry the action that failed (only retry once)

---
Last updated: November 5, 2025
