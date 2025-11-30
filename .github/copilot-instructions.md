# GitHub Copilot instructions for Stock Indicators for .NET

This repository hosts **Stock Indicators for .NET**, the production source for the widely used <a href="https://www.nuget.org/packages/Skender.Stock.Indicators">Skender.Stock.Indicators</a> NuGet package. The library offers more than 200 technical analysis indicators with a focus on accuracy, performance, and ergonomics for financial analytics.

- Multi-targets `net10.0`, `net9.0`, and `net8.0` with analyzers enabled for strict code quality.
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
├── docs/                  # Public documentation site (VitePress)
└── .specify/              # Spec Kit configuration and active specifications
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

This library follows the [guiding principles](https://github.com/DaveSkender/Stock.Indicators/discussions/648) that emphasize:

### I. Mathematical Precision

Default to `double` for performance and reach for `decimal` when price-sensitive precision requires it. All financial calculations must be mathematically accurate and verified against reference implementations.

### II. Performance First

Minimize memory allocations, avoid excessive LINQ operations in hot paths, and use span-friendly loops. Performance benchmarks are mandatory for computationally intensive indicators.

### III. Comprehensive Validation

All public methods require complete input validation with descriptive error messages. Handle edge cases (insufficient data, zero/negative values, null inputs) explicitly with consistent exception types.

### IV. Test-Driven Quality

Every indicator requires comprehensive unit tests covering all code paths. Mathematical accuracy must be verified against reference implementations. Performance tests are mandatory for computationally intensive indicators.

**Test epsilon usage rules:**

- ✅ **Use epsilon** (`BeApproximately`) ONLY when comparing against **manually calculated reference values** (e.g., `Money4 = 0.00005` for 4 decimal places) or for **recursive algorithms** where calculation order legitimately differs (e.g., Fisher Transform)
- ❌ **DO NOT use epsilon** for regression tests comparing calculated-vs-calculated results - use exact equality (default `AssertEquals()`)
- ❌ **DO NOT use epsilon** when comparing computed formulas or constants (e.g., `2d / (period + 1)`) - these should use exact comparison (`.Be()`)
- ❌ **DO NOT use epsilon** for zero evaluations in production code - use exact comparison (`!= 0` or `== 0`)

### V. Documentation Excellence

All public methods must have complete XML documentation. Code examples must be provided for complex indicators. Documentation must include parameter constraints, return value descriptions, and usage patterns.

## Scoped instruction files

This repository uses scoped instruction files for specific development areas. These files contain detailed guidelines that apply to particular file patterns:

| Pattern | File | Description |
| ------- | ---- | ----------- |
| `.specify/**,.github/prompts/speckit.*` | [spec-kit.instructions.md](instructions/spec-kit.instructions.md) | Spec Kit development workflow and artifact editing guidelines |
| `src/**` | [agents.md](../src/agents.md) | **CRITICAL**: Formula change rules and mathematical precision requirements for AI agents |
| `src/**/*.*Series.cs,tests/**/*.*Series.Tests.cs` | [indicator-series.instructions.md](instructions/indicator-series.instructions.md) | Series-style indicator development and testing guidelines |
| `src/**/*.StreamHub.cs,tests/**/*.StreamHub.Tests.cs` | [indicator-stream.instructions.md](instructions/indicator-stream.instructions.md) | Stream indicator development guidelines |
| `src/**/*.BufferList.cs,tests/**/*.BufferList.Tests.cs` | [indicator-buffer.instructions.md](instructions/indicator-buffer.instructions.md) | Buffer indicator development guidelines |
| `**/src/**/*.Catalog.cs,**/tests/**/*.Catalog.Tests.cs` | [catalog.instructions.md](instructions/catalog.instructions.md) | Catalog file conventions |
| `src/**,tests/**` | [source-code-completion.instructions.md](instructions/source-code-completion.instructions.md) | Source code, testing, and pre-commit code completion checklist |
| `**/*.md` | [markdown.instructions.md](instructions/markdown.instructions.md) | Markdown formatting rules |
| `docs/**` | [documentation.instructions.md](instructions/documentation.instructions.md) | Documentation website instructions |
| `tools/performance/**` | [performance-testing.instructions.md](instructions/performance-testing.instructions.md) | Performance testing and benchmarking guidelines |

These scoped files are automatically applied when working with files matching their patterns.

## Custom agents for focused assistance

This repository provides specialized custom agents that developers can invoke in GitHub Copilot Chat for expert guidance on specific development areas:

| Agent | Description | When to Use |
| ----- | ----------- | ----------- |
| `@series` | Series indicator development - mathematical precision, validation patterns, test coverage | Implementing new Series indicators, validating calculations, structuring tests |
| `@buffer` | BufferList indicator development - incremental processing, interface selection, buffer management | Implementing BufferList indicators, choosing interfaces, managing state efficiently |
| `@streamhub` | StreamHub indicator development - implementation patterns, provider selection, state management | Implementing new StreamHub indicators, choosing provider base classes, optimizing real-time processing |
| `@streamhub-state` | RollbackState patterns, cache replay strategies, window rebuilding | Implementing state management, handling provider history mutations (Insert/Remove) |
| `@streamhub-performance` | O(1) optimization patterns, RollingWindow utilities, avoiding O(n²) anti-patterns | Performance optimization, achieving ≤1.5x Series benchmark, eliminating bottlenecks |
| `@streamhub-testing` | Test interface selection, comprehensive rollback validation, Series parity checks | Writing StreamHub tests, selecting test interfaces, implementing rollback validation |
| `@streamhub-pairs` | PairsProvider dual-stream patterns, timestamp synchronization, dual-cache coordination | Implementing dual-stream indicators (Correlation, Beta), managing synchronized inputs |
| `@performance` | Performance optimization - algorithmic complexity, O(1) patterns, memory efficiency, benchmarking | Identifying bottlenecks, eliminating O(n²) anti-patterns, meeting performance targets across all styles |

**Usage examples:**

```text
@series I need to implement a new momentum indicator (RSI-style)

@buffer Which interface should I use for my indicator that needs OHLCV data?

@streamhub I need to implement a new VWAP StreamHub. What provider base should I use?

@streamhub-state How do I rebuild RollingWindowMax state after a provider Insert?

@streamhub-performance My StreamHub is 50x slower than Series. How do I optimize?

@streamhub-testing Which test interfaces should I implement for a ChainProvider hub?

@streamhub-pairs How do I handle timestamp synchronization for dual-stream indicators?

@performance My indicator is 10x slower than expected. How do I identify the bottleneck?
```

Custom agent definitions are in `.github/agents/`. Official GitHub Copilot documentation: [Configure custom agents for Copilot Chat](https://docs.github.com/en/copilot/reference/custom-agents-configuration).

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

- Series: [indicator-series.instructions.md](instructions/indicator-series.instructions.md)
- Buffer: [indicator-buffer.instructions.md](instructions/indicator-buffer.instructions.md)
- Stream: [indicator-stream.instructions.md](instructions/indicator-stream.instructions.md)

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

This repository uses [Spec Kit](https://github.com/github/spec-kit) for Specification-Driven Development. Before adding or changing indicators, consult the relevant spec in [.specify/specs/](../.specify/specs/) and use chat commands to align with the active plan.

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

For detailed Spec Kit workflow guidance, see [spec-kit.instructions.md](instructions/spec-kit.instructions.md).

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
Last updated: October 29, 2025
