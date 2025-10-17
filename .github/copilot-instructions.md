# GitHub Copilot instructions for Stock Indicators for .NET

This repository hosts **Stock Indicators for .NET**, the production source for the widely used <a href="https://www.nuget.org/packages/Skender.Stock.Indicators">Skender.Stock.Indicators</a> NuGet package. The library offers more than 200 technical analysis indicators with a focus on accuracy, performance, and ergonomics for financial analytics.

- Multi-targets `net9.0` and `net8.0` with analyzers enabled for strict code quality.
- Active development expands streaming indicator support—consult open specs before modifying stateful pipelines.
- Documentation at <a href="https://dotnet.stockindicators.dev">dotnet.stockindicators.dev</a> is sourced from the `docs/` content in this repository.

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
├── docs/                  # Public documentation site (Jekyll)
├── .specify/              # Spec Kit configuration and memory
└── specs/                 # Active feature specifications
```

## Build and verification

- Use the solution tasks (`Restore`, `Build`, `Test`) or run `dotnet restore`, `dotnet build`, and `dotnet test --no-restore` from the repository root.
- Keep analyzers clean; treat warnings as build failures.
- Update documentation and samples when indicators change behavior, especially for streaming scenarios.
- Update the matching `docs/_indicators/<Indicator>.md` file whenever an indicator changes. Keep the primary public API example, parameter details, warmup guidance, and outputs in sync with the implementation.

## Common pitfalls to avoid

1. **Off-by-one windows** when calculating lookback or warmup periods.
2. **Null or empty quotes** causing stateful streaming regressions—always validate input sequences.
3. **Precision loss** in chained calculations. Favor `double` for performance, switching to `decimal` only when business accuracy demands it.
4. **Index out of range** and buffer reuse issues in streaming indicators—guard shared spans and caches.
5. **Performance regressions** from unnecessary allocations or LINQ. Prefer span-friendly loops and avoid boxing.
6. **Documentation drift** between code comments, XML docs, and the published docs site.

## Guiding principles

This library follows the [guiding principles](https://github.com/DaveSkender/Stock.Indicators/discussions/648) that emphasize:

### I. Mathematical Precision

Default to `double` for performance and reach for `decimal` when price-sensitive precision requires it. All financial calculations must be mathematically accurate and verified against reference implementations.

### II. Performance First

Minimize memory allocations, avoid excessive LINQ operations in hot paths, and use span-friendly loops. Performance benchmarks are mandatory for computationally intensive indicators.

### III. Comprehensive Validation

All public methods require complete input validation with descriptive error messages. Handle edge cases (insufficient data, zero/negative values, null inputs) explicitly with consistent exception types.

### IV. Test-Driven Quality

Every indicator requires comprehensive unit tests covering all code paths. Mathematical accuracy must be verified against reference implementations. Performance tests are mandatory for computationally intensive indicators.

### V. Documentation Excellence

All public methods must have complete XML documentation. Code examples must be provided for complex indicators. Documentation must include parameter constraints, return value descriptions, and usage patterns.

## Scoped instruction files

This repository uses scoped instruction files for specific development areas. These files contain detailed guidelines that apply to particular file patterns:

| Pattern | File | Description |
| ------- | ---- | ----------- |
| `.specify/**,specs/**,.github/prompts/speckit.*` | [spec-kit.instructions.md](.github/instructions/spec-kit.instructions.md) | Spec Kit development workflow and artifact editing guidelines |
| `src/**` | [agents.md](../src/agents.md) | **CRITICAL**: Formula change rules and mathematical precision requirements for AI agents |
| `src/**/*.*Series.cs,tests/**/*.*Series.Tests.cs` | [indicator-series.instructions.md](.github/instructions/indicator-series.instructions.md) | Series-style indicator development and testing guidelines |
| `src/**/*.StreamHub.cs,tests/**/*.StreamHub.Tests.cs` | [indicator-stream.instructions.md](.github/instructions/indicator-stream.instructions.md) | Stream indicator development guidelines |
| `src/**/*.BufferList.cs,tests/**/*.BufferList.Tests.cs` | [indicator-buffer.instructions.md](.github/instructions/indicator-buffer.instructions.md) | Buffer indicator development guidelines |
| `**/src/**/*.Catalog.cs,**/tests/**/*.Catalog.Tests.cs` | [catalog.instructions.md](.github/instructions/catalog.instructions.md) | Catalog file conventions |
| `src/**,tests/**` | [source-code-completion.instructions.md](.github/instructions/source-code-completion.instructions.md) | Source code, testing, and pre-commit code completion checklist |
| `**/*.md` | [markdown.instructions.md](.github/instructions/markdown.instructions.md) | Markdown formatting rules |
| `docs/**` | [documentation.instructions.md](.github/instructions/documentation.instructions.md) | Documentation website instructions |
| `tools/performance/**` | [performance-testing.instructions.md](.github/instructions/performance-testing.instructions.md) | Performance testing and benchmarking guidelines |

These scoped files are automatically applied when working with files matching their patterns.

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

See the style-specific guides for implementation requirements and additional checklist items:

- Series: [.github/instructions/indicator-series.instructions.md](.github/instructions/indicator-series.instructions.md)
- Stream: [.github/instructions/indicator-stream.instructions.md](.github/instructions/indicator-stream.instructions.md)
- Buffer: [.github/instructions/indicator-buffer.instructions.md](.github/instructions/indicator-buffer.instructions.md)

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

- Provide XML comments for all public types and members.
- Use `/// <inheritdoc />` instead of repeating same XML code comments on implementations.
- Cover happy paths, edge cases, and streaming flows with unit tests.
- Add or update performance benchmarks when modifying core indicator loops.
- Maintain `.editorconfig` conventions; let analyzers and style rules guide formatting.
- Prefer explicit variable names over `var` (following `.editorconfig` conventions).
- Use filenames that match the containing class name, with modifiers for partial classes spread across files.
- Keep package metadata aligned with NuGet expectations (icon, README, license).
- Keep `docs/_indicators/*.md` pages aligned with their indicator APIs, including usage examples, parameter defaults, warmup guidance, and notable streaming behavior.

## Spec-driven development integration

This repository uses [Spec Kit](https://github.com/github/spec-kit) for Specification-Driven Development. Before adding or changing indicators, consult the relevant spec in `specs/` and use chat commands to align with the active plan.

**Core workflow commands:**

- **`/speckit.constitution`** — Create or update project governing principles
- **`/speckit.specify`** — Define what you want to build (requirements and user stories)
- **`/speckit.clarify`** — Clarify underspecified areas (recommended before planning)
- **`/speckit.plan`** — Create technical implementation plans with tech stack choices
- **`/speckit.tasks`** — Generate actionable task lists for implementation
- **`/speckit.analyze`** — Cross-artifact consistency & coverage analysis (before implementing)
- **`/speckit.implement`** — Execute all tasks to build the feature according to the plan

**Optional commands:**

- **`/speckit.checklist`** — Generate custom quality checklists for validation

For detailed Spec Kit workflow guidance, see [spec-kit.instructions.md](.github/instructions/spec-kit.instructions.md).

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
Last updated: October 12, 2025
