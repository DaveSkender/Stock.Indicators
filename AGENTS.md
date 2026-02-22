# Stock Indicators for .NET v2

This repository hosts **Stock Indicators for .NET**, the production source for the <a href="https://www.nuget.org/packages/Skender.Stock.Indicators">Skender.Stock.Indicators</a> NuGet package. The library offers financial market technical analysis indicators with a focus on accuracy, performance, and ergonomics for financial analytics.

## Repository layout

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

## Build and verification

- Use the solution tasks (`Restore`, `Build`, `Test`) or run `dotnet restore`, `dotnet build`, and `dotnet test --no-restore` from the repository root
- Keep analyzers clean; **treat all warnings and errors as failures that must be fixed**
- Do not accept or ignore warnings regardless of scope or reason
- Do not suppress issues as a way to remove warnings or errors—you must fix the underlying problem
- Update documentation as needed and in accordance with markdown instructions
- Update the matching `docs/_indicators/<Indicator>.md` file whenever an indicator changes. Keep the primary public API example, parameter details, warmup guidance, and outputs in sync with the implementation

See subfolder AGENTS.md files for detailed domain-specific guidance.

### Linting and testing

- **Markdown**: `npx markdownlint-cli2` (auto-fix with `npx markdownlint-cli2 --fix`)
- **Roslynator**: `dotnet tool run roslynator fix --properties TargetFramework=net10.0 --severity-level info` (fast for dev loop)
- **All linters**: `dotnet format && npx markdownlint-cli2` (auto-fix with both `--fix` flags)
- **Build**: `dotnet build "Stock.Indicators.sln" -v minimal --nologo`
- **Test**: `dotnet test "Stock.Indicators.sln" --no-restore --nologo`

VS Code tasks are organized for speed vs. completeness:

- **Fast development** (Roslynator only, ~seconds): Use `Lint: .NET code & markdown files (fix)` for regular development
- **Comprehensive** (Roslynator + dotnet format, slower): Use `Lint: All (fix)` before committing to ensure CI compliance

Individual tools available: `Lint: .NET format`, `Lint: .NET Roslynator (analyze)`, `Lint: Markdown` and their `(fix)` variants.

### Code completion checklist

**Before finishing any code work, execute these quality gates**:

```bash
dotnet format --no-restore && dotnet build && dotnet test --no-restore && npx markdownlint-cli2
```

This ensures:

1. All code passes linting (markdown + .NET) with **zero warnings and zero errors**
2. All projects build successfully with **zero warnings and zero errors**
3. All tests pass
4. **All warnings must be fixed**—do not ignore or defer warnings

See the [Code Completion skill](.agents/skills/code-completion/SKILL.md) for detailed checklist and troubleshooting.

## Skills for development

This repository uses Agent Skills (`.agents/skills/`) for domain-specific guidance. Skills are automatically loaded when relevant:

| Skill | Description | When to use |
| ----- | ----------- | ----------- |
| [code-completion](.agents/skills/code-completion/SKILL.md) | Quality gates checklist for completing code work | Before finishing any implementation, bug fix, or refactoring |

Additional path-specific instruction files:

| Pattern | File | Purpose |
| ------- | ---- | ------- |
| `**/*.md` | `.github/instructions/markdown.instructions.md` | Markdown authoring standards |
| `docs/**` | `.github/instructions/docs.instructions.md` | Jekyll documentation site |
| `src/**` | `src/AGENTS.md` | Formula protection rules |

Skills are defined in `.agents/skills/` following the Agent Skills specification.

## Folder-specific guidance

- **src/**: See `src/AGENTS.md` for implementation details, technical constraints, and code quality standards
- **tests/**: See `tests/AGENTS.md` for test organization and writing guidance
- **docs/**: See `docs/AGENTS.md` for documentation site development

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

## Pull request guidelines

- PR titles must follow <a href="https://www.conventionalcommits.org">Conventional Commits</a> format: `type: Subject` (subject starts uppercase, ≤ 65 characters).
- Supported types: feat, fix, docs, style, refactor, perf, test, build, ci, chore, revert, plan.
- Link or reference the governing spec/task thread when applicable.
- Ensure lint, build, and tests pass before finishing changes or requesting reviews

Examples of PR titles:

- `feat: Add RSI indicator`
- `fix: Resolve MACD calculation error`
- `plan: Define streaming indicators approach`
- `docs: Update API documentation`

---
Last updated: January 2, 2026
