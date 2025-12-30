# Stock Indicators for .NET

This repository hosts **Stock Indicators for .NET**, the production source for the widely used <a href="https://www.nuget.org/packages/Skender.Stock.Indicators">Skender.Stock.Indicators</a> NuGet package. The library offers more than 200 technical analysis indicators with a focus on accuracy, performance, and ergonomics for financial analytics.

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
└── docs/                  # Public documentation site (Jekyll)
```

## Build and verification

- Use the solution tasks (`Restore`, `Build`, `Test`) or run `dotnet restore`, `dotnet build`, and `dotnet test --no-restore` from the repository root.
- Keep analyzers clean; treat warnings as build failures.
- Update documentation and samples when indicators change behavior, especially for streaming scenarios.
- Update the matching `docs/_indicators/<Indicator>.md` file whenever an indicator changes. Keep the primary public API example, parameter details, warmup guidance, and outputs in sync with the implementation.

### Linting tasks

VS Code tasks are organized for speed vs. completeness:

- **Fast development** (Roslynator only, ~seconds): Use `Lint: .NET code & markdown files (fix)` for regular development
- **Comprehensive** (Roslynator + dotnet format, slower): Use `Lint: All (fix)` before committing to ensure CI compliance

Individual tools available: `Lint: .NET format`, `Lint: .NET Roslynator (analyze)`, `Lint: Markdown` and their `(fix)` variants.

## Common pitfalls to avoid

1. **Off-by-one windows** when calculating lookback or warmup periods.
2. **Null or empty quotes** causing stateful streaming regressions—always validate input sequences.
3. **Precision loss** in chained calculations. Favor `double` for performance, switching to `decimal` only when business accuracy demands it.
4. **Index out of range** and buffer reuse issues in streaming indicators—guard shared spans and caches.
5. **Performance regressions** from unnecessary allocations or LINQ. Prefer span-friendly loops and avoid boxing.
6. **Documentation drift** between code comments, XML docs, and the published docs site.
7. **Improper NaN handling** - Do not reject NaN inputs; however, always guard against division by zero when denominators can be zero. See NaN handling policy below.

## NaN handling policy

This library uses non-nullable `double` types internally for performance, with intentional NaN propagation through calculations:

### Core principles

1. **Natural propagation** - NaN values propagate naturally through calculations (any operation with NaN produces NaN)
2. **Internal representation** - Use `double.NaN` internally when a value cannot be calculated
3. **External representation** - Convert NaN to `null` (via `.NaN2Null()`) **only at the final result boundary** when returning to users
4. **No rejection** - Never reject NaN inputs with validation; allow them to flow through the system
5. **Performance first** - Non-nullable `double` provides significant performance gains over `double?`

### Implementation guidelines

- **Division by zero** - MUST guard variable denominators with ternary checks (e.g., `denom != 0 ? num / denom : double.NaN`); choose fallback (NaN, 0, null) based on mathematical meaning
- **No epsilon comparisons** - NEVER use epsilon values (e.g., `1e-8`, `1e-9`) for zero checks in division guards. Use exact zero comparison (`!= 0` or `== 0`). Epsilon comparisons assume floating-point precision issues that don't exist in our calculations and cause incorrect results by treating near-zero values as zero.
- **NaN propagation** - Accept NaN inputs and allow natural propagation through calculations; never reject or filter NaN values
- **RollingWindow utilities** - Accept NaN values and return NaN for Min/Max when NaN is present in the window
- **Quote validation** - Only validate for null/missing quotes, not for NaN values in quote properties (High/Low/Close/etc.)
- **State initialization** - Use `double.NaN` to represent uninitialized state instead of sentinel values like `0` or `-1`

### Rationale

This approach aligns with **Constitution §1: Mathematical Precision** and **Constitution §2: Performance First**:

- Maintains numerical correctness (NaN is mathematically correct for undefined values)
- Prevents silent data corruption from substituting invalid placeholders
- Follows established IEEE 754 standard
- Achieves performance gains from non-nullable types while maintaining mathematical integrity

See [src/_common/README.md](../src/_common/README.md#nan-handling-policy) for complete policy documentation.

## Guiding principles

This library follows the [guiding principles](https://github.com/DaveSkender/Stock.Indicators/discussions/648) that balance usability, performance, precision, and security. The six core principles emphasize **Mathematical Precision** (non-negotiable), **Performance First** (critical), **Comprehensive Validation**, **Test-Driven Quality**, **Documentation Excellence**, and **Scope & Stewardship**.

See [PRINCIPLES.md](docs/PRINCIPLES.md) for complete constitutional details.

## Skills for indicator development

This repository uses Agent Skills (`.github/skills/`) for domain-specific guidance. Skills are automatically loaded when relevant:

| Skill | Primary Focus |
| ----- | ------------- |
| `indicator-series` | Series-style batch indicator development and testing |
| `indicator-buffer` | BufferList incremental indicator development |
| `indicator-stream` | StreamHub real-time indicator development |
| `indicator-catalog` | Catalog entry conventions and registration |
| `performance-testing` | BenchmarkDotNet performance testing |
| `quality-gates` | Pre-commit checklist and validation |
| `testing-standards` | Test naming, FluentAssertions, and coverage |

Additional path-specific instruction files:

| Pattern | File | Purpose |
| ------- | ---- | ------- |
| `**/*.md` | `.github/instructions/markdown.instructions.md` | Markdown authoring standards |
| `docs/**` | `.github/instructions/docs.instructions.md` | Jekyll documentation site |
| `src/**` | `src/AGENTS.md` | Formula protection rules |

## Skills for focused assistance

This repository provides Agent Skills in `.github/skills/` that are automatically loaded when relevant. Use skill invocations for expert guidance:

| Skill | Description | When to Use |
| ----- | ----------- | ----------- |
| `indicator-series` | Series indicator development - mathematical precision, validation patterns, test coverage | Implementing new Series indicators, validating calculations, structuring tests |
| `indicator-buffer` | BufferList indicator development - incremental processing, interface selection, buffer management | Implementing BufferList indicators, choosing interfaces, managing state efficiently |
| `indicator-stream` | StreamHub indicator development - implementation patterns, provider selection, state management | Implementing StreamHub indicators, choosing provider base classes, optimizing real-time processing |
| `indicator-catalog` | Catalog entry creation and registration | Creating indicator catalog entries for automation |
| `performance-testing` | Benchmarking with BenchmarkDotNet, regression detection | Adding performance tests, optimizing indicator performance |
| `quality-gates` | Pre-commit validation checklist | Completing work, ensuring build/test/lint pass |
| `testing-standards` | Test naming, FluentAssertions, Series parity | Writing comprehensive tests, debugging test failures |

**Usage**: Skills are auto-loaded when working with matching file patterns. Reference the SKILL.md content when asking questions about that domain:

```text
# When working on Series indicators, ask about patterns from that skill:
"How do I validate parameters for a new Series indicator?"

# When working on StreamHub files, ask about provider selection:
"What provider base should I use for a new VWAP StreamHub?"

# When finishing work, ask about quality gates:
"What's the pre-commit checklist?"
```

Skills are defined in `.github/skills/` following the Agent Skills specification.

## Common indicator requirements (all styles)

Use these cross-cutting requirements for Series, Stream, and Buffer indicators. Each style guide adds its own specifics, but these apply to all:

### Code completion checklist for indicator implementations

- [ ] **Catalog entry exists and is registered**:
  - Create `src/**/{IndicatorName}.Catalog.cs` and register in `src/_common/Catalog/Catalog.Listings.cs` (PopulateCatalog)
- [ ] **Regression tests include the indicator type**:
  - Add to `tests/indicators/**/{IndicatorName}.Regression.Tests.cs`
- [ ] **Performance benchmarks include a default case**:
  - Add to the appropriate file in `tools/performance` (Series/Stream/Buffer)
- [ ] **Public documentation is accurate**:
  - Update `docs/_indicators/{IndicatorName}.md` (usage, parameters, warmup, outputs)
- [ ] **Migration notes and bridges when behavior changes**:
  - Update `src/MigrationGuide.V3.md`
  - Update migration bridges in `src/Obsolete.V3.Indicators.cs` and `src/Obsolete.V3.Other.cs` to reflect new/renamed APIs or deprecations

See the skills for implementation requirements and additional checklist items:

- Series: `.github/skills/indicator-series/SKILL.md`
- Buffer: `.github/skills/indicator-buffer/SKILL.md`
- Stream: `.github/skills/indicator-stream/SKILL.md`

### Series as the canonical reference

- Series indicators are the canonical source of truth for numerical correctness across styles.
- Series results are based on authoritative author publications and manually verified calculations.
- Stream and Buffer implementations must match the Series results for the same inputs once warmed up.
- For discrepancies, fix Stream/Buffer unless there is a verified issue with Series and the reference data.

## Code review guidelines

### What to look for

- Comprehensive validation of periods, warmup requirements, and null checks.
- Accurate math across both batch and streaming paths; compare against reference data.
- Performance characteristics, especially allocations within hot loops.
- XML documentation completeness and clarity for public APIs.
- Consistent error messages and exception types that match established patterns.

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

### Folder-specific guidance

- **src/**: See `src/AGENTS.md` for numerical precision and formula protection rules
- **docs/**: Follow documentation instructions in `.github/instructions/docs.instructions.md`
- **All markdown files**: Follow markdown instructions in `.github/instructions/markdown.instructions.md`

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
- Ensure `dotnet test --no-restore` passes and the docs site builds when content changes.
- Provide before/after validation notes or benchmarks when touching performance-critical code.

Examples:

- `feat: Add RSI indicator`
- `fix: Resolve MACD calculation error`
- `plan: Define streaming indicators approach`
- `docs: Update API documentation`

---
Last updated: December 30, 2025
