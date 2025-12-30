# Stock Indicators for .NET

This is **Stock Indicators for .NET** - a comprehensive C# library providing 200+ technical analysis indicators for financial data analysis. The library focuses on performance, accuracy, and ease of use for .NET developers working with financial data.

## Project structure

```text
src/
├── _common/          # Shared utilities, base classes, and common types
├── a-d/              # Indicators A-D (alphabetical organization)
├── e-k/              # Indicators E-K
├── m-r/              # Indicators M-R
├── s-z/              # Indicators S-Z
└── Indicators.csproj # Main project file

tests/
├── indicators/       # Unit tests for indicators
├── other/            # Integration and utility tests
└── performance/      # Performance benchmarks

docs/
└── [Jekyll-based documentation website]
```

## Code review guidelines

### What to look for

- Input validation completeness
- Edge case handling (insufficient data, zero/negative values)
- Mathematical accuracy vs reference implementations
- Performance characteristics
- Appropriate data types: `decimal` for public quote inputs, `double` for internal calculations, choose result type based on precision needs
- XML documentation completeness
- Consistent error messages and exception types

### Code quality standards

- All public methods must have XML documentation
- Unit test coverage for all code paths
- Performance tests for computationally intensive indicators
- Validation for all user inputs
- Consistent formatting using `.editorconfig`

## Development workflow

### Building and testing

```bash
# Build the solution
dotnet build

# Run all tests
dotnet test

# Format code
dotnet format

# Lint markdown files
npx markdownlint-cli2 --fix
```

### Folder-specific instructions

- **src/**: Follow .NET development instructions in .github/instructions/dotnet.instructions.md
- **docs/**: Follow documentation instructions in .github/instructions/docs.instructions.md
- **All markdown files**: Follow markdown instructions in .github/instructions/markdown.instructions.md

## MCP tools guidance

### When to use MCP tools

The following MCP servers are configured in .vscode/mcp.json and should be used in these scenarios:

- `mslearn/*`: Research C# coding conventions, .NET best practices, performance optimization, and language features. Use when implementing indicators or utility functions that require knowledge of official Microsoft standards.
- `context7/*`: Look up documentation for NuGet package dependencies or external libraries used in the project. Use when integrating third-party functionality.
- `github/web_search`: Research indicator algorithms, financial calculations, and external technical analysis standards. Use for mathematical validation and algorithm research.
- `github/*`: Get recently failed CI workflow job details, research recent library changes, pull requests, issues, and discussions. Use when updating documentation or implementing features that depend on understanding recent repository context.

Do NOT use MCP tools for:

- Local file operations (use file read/edit tools)
- Simple code formatting (use `dotnet format`)
- Markdown linting (use `markdownlint-cli2`)
- Running local build tests (use `dotnet build` and `dotnet test`)

---
Last updated: December 30, 2025
