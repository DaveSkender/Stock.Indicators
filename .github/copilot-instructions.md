# GitHub Copilot Instructions for Stock Indicators for .NET

This is **Stock Indicators for .NET** - a comprehensive C# library providing 200+ 
technical analysis indicators for financial data analysis. The library focuses on 
performance, accuracy, and ease of use for .NET developers working with financial data.

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
```

## Common pitfalls to avoid

1. **Off-by-one errors** in lookback period calculations
2. **Null reference exceptions** with insufficient data
3. **Precision loss** in financial calculations - use `decimal` not `double`
4. **Index out of bounds** when accessing historical data
5. **Performance regression** from excessive LINQ chaining

## Code review guidelines

### What to look for

- Input validation completeness
- Edge case handling (insufficient data, zero/negative values)
- Mathematical accuracy vs reference implementations
- Performance characteristics
- XML documentation completeness
- Consistent error messages and exception types

### Code quality standards

- All public methods must have XML documentation
- Unit test coverage for all code paths
- Performance tests for computationally intensive indicators
- Validation for all user inputs
- Consistent formatting using `.editorconfig`

## Spec-Driven Development Integration

This repository uses [GitHub Spec-Kit](https://github.com/github/spec-kit) for structured development workflows. Use the following commands for new feature development:

- **`/constitution`** - Review or update project governance principles
- **`/specify`** - Create detailed feature specifications for new indicators
- **`/plan`** - Define technical implementation approach
- **`/tasks`** - Break down features into actionable development tasks
- **`/implement`** - Execute planned implementation

For detailed guidance, see [Spec-Kit Integration Guide](./.github/spec-kit-integration.md).

## Pull Request Guidelines

When creating pull requests, follow these guidelines:

- **PR titles must follow [Conventional Commits](https://www.conventionalcommits.org) format**: `type: Subject`
- **Keep titles under 65 characters** for better readability
- **Available types**: feat, fix, docs, style, refactor, perf, test, build, ci, chore, revert, plan
- **Subject must start with uppercase letter**

Examples:

- `feat: Add RSI indicator` ✓ (20 chars)
- `fix: Resolve MACD calculation error` ✓ (35 chars)
- `plan: Define streaming indicators approach` ✓ (42 chars)
- `docs: Update API documentation` ✓ (31 chars)
