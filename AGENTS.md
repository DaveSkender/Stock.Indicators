# Stock Indicators for .NET

This repository hosts **Stock Indicators for .NET**, the production source for the <a href="https://www.nuget.org/packages/Skender.Stock.Indicators">Skender.Stock.Indicators</a> NuGet package. The library offers financial market technical analysis indicators with a focus on accuracy, performance, and ergonomics for financial analytics.

- Multi-targets `net10.0`, `net9.0`, and `net8.0` with analyzers enabled for strict code quality.
- Active development expands streaming indicator support—consult open specs before modifying stateful pipelines.
- Documentation at <a href="https://dotnet.stockindicators.dev">dotnet.stockindicators.dev</a> is sourced from the `docs/` content in this repository.

## AI-first development model

**This repository is optimized for AI agent contributions, not human coding.**

Guidance follows industry standards for AI configuration:

- **Imperative, directive voice**: "Run the command" not "You should run"
- **Present tense**: Avoid historical context; focus on current directives
- **Autonomous decision-making**: Agents infer, proceed, and document assumptions
- **Outcome-focused**: Define "what" (results) and "why" (rationale), not "how" (micro-steps)
- **No approval loops**: Agents complete work (code + tests + docs) before yielding to humans
- **Succinctness over verbosity**: Prefer compact, scannable directives; eliminate filler words and redundant explanations

**AI agents contribute by:**

1. Performing the vast majority of actual coding work
2. Making educated guesses and documenting assumptions
3. Following standards defined in guidance files
4. Proceeding autonomously without human approval loops
5. Generating changes, tests, and documentation in one pass
6. Executing quality gates (build, test, lint) before yielding

**Humans contribute by:**

1. Setting strategic direction (what capabilities to build)
2. Making architectural decisions and defining quality standards
3. Reviewing agent-generated changes for alignment with project philosophy
4. Correcting agent assumptions when they guess incorrectly
5. Providing context agents cannot infer from repository artifacts
6. Validating outcomes, not micromanaging implementation details

**Human coding is an anti-pattern** in this workflow. If you are a human developer who wants to write code, you can—but recognize that this repository's instructions are optimized for AI agent consumption, not human procedural guidance.

**For AI agents:**

- Make reasonable inferences and proceed autonomously
- Generate complete implementations (code + tests + docs) in one turn
- Execute quality gates (build, test, lint) before yielding to humans
- Document assumptions when making educated guesses
- Follow established patterns and standards without seeking approval
- Handle edge cases using documented principles and constitutional rules

## Guiding principles

This library follows six core principles that balance usability, performance, precision, and security: **Mathematical Precision** (non-negotiable), **Performance First** (critical), **Comprehensive Validation**, **Test-Driven Quality**, **Documentation Excellence**, and **Scope & Stewardship**.

See [PRINCIPLES.md](docs/PRINCIPLES.md) for constitutional philosophy and rationale. This file (AGENTS.md) provides operational implementation guidance.

## Repository layout

```text
(root)
├── src/                   # Library source code
│    ├── _common/          # Shared utilities, base classes, and common types
│    ├── a-d/              # Indicators A-D (alphabetical organization)
│    ├── e-k/              # Indicators E-K
│    ├── m-r/              # Indicators M-R
│    ├── s-z/              # Indicators S-Z
│    └── Indicators.csproj # Main project file
├── tests/                 # Unit, integration, performance, and simulation suites
└── Stock.Indicators.sln   # Primary solution for src + tests
.
├── docs/                  # Public documentation site (VitePress)
└── .specify/              # Spec Kit configuration and active specifications
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
- **Roslynator**: `roslynator fix --properties TargetFramework=net10.0 --severity-level info` (fast for dev loop)
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

See the [Code Completion skill](.github/skills/code-completion/SKILL.md) for detailed checklist and troubleshooting.

## Skills for development

This repository uses Agent Skills (`.github/skills/`) for domain-specific guidance. Skills are automatically loaded when relevant:

| Skill | Description | When to use |
| ----- | ----------- | ----------- |
| `indicator-series` | Series indicator development - mathematical precision, validation patterns, test coverage | Implementing new Series indicators, validating calculations, structuring tests |
| `indicator-buffer` | BufferList indicator development - incremental processing, interface selection, buffer management | Implementing BufferList indicators, choosing interfaces, managing state efficiently |
| `indicator-stream` | StreamHub indicator development - implementation patterns, provider selection, state management | Implementing StreamHub indicators, choosing provider base classes, optimizing real-time processing |
| `indicator-catalog` | Catalog entry creation and registration | Creating indicator catalog entries for automation |
| `performance-testing` | Benchmarking with BenchmarkDotNet, regression detection | Adding performance tests, optimizing indicator performance |
| [code-completion](.github/skills/code-completion/SKILL.md) | Quality gates checklist for completing code work | Before finishing any implementation, bug fix, or refactoring |
| `testing-standards` | Test naming, FluentAssertions, Series parity | Writing comprehensive tests, debugging test failures |

Additional path-specific instruction files:

| Pattern | File | Purpose |
| ------- | ---- | ------- |
| `**/*.md` | `.github/instructions/markdown.instructions.md` | Markdown authoring standards |
| `docs/**` | `.github/instructions/docs.instructions.md` | Jekyll documentation site |
| `src/**` | `src/AGENTS.md` | Formula protection rules |

**Usage**: Skills are auto-loaded when working with matching file patterns. Reference the SKILL.md content when asking questions about that domain:

```text
# When working on Series indicators, ask about patterns from that skill:
"How do I validate parameters for a new Series indicator?"

# When working on StreamHub files, ask about provider selection:
"What provider base should I use for a new VWAP StreamHub?"

# When finishing work, ask about quality gates:
"What's the pre-commit checklist?"
```

Skills are defined in `.github/skills/` following the Agent Skills specification.  Refer to our skills instruction when developing new skills.

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
