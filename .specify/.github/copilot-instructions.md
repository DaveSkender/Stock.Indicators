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

## Common pitfalls to avoid

1. **Off-by-one windows** when calculating lookback or warmup periods.
2. **Null or empty quotes** causing stateful streaming regressions—always validate input sequences.
3. **Precision loss** in chained calculations. Favor `double` for performance, switching to `decimal` only when business accuracy demands it.
4. **Index out of range** and buffer reuse issues in streaming indicators—guard shared spans and caches.
5. **Performance regressions** from unnecessary allocations or LINQ. Prefer span-friendly loops and avoid boxing.
6. **Documentation drift** between code comments, XML docs, and the published docs site.

## Code review guidelines

### What to look for

- Comprehensive validation of periods, warmup requirements, and null checks.
- Accurate math across both batch and streaming paths; compare against reference data.
- Performance characteristics, especially allocations within hot loops.
- XML documentation completeness and clarity for public APIs.
- Consistent error messages and exception types that match established patterns.

### Code quality standards

- Provide XML comments for all public types and members.
- Cover happy paths, edge cases, and streaming flows with unit tests.
- Add or update performance benchmarks when modifying core indicator loops.
- Maintain `.editorconfig` conventions; let analyzers and style rules guide formatting.
- Keep package metadata aligned with NuGet expectations (icon, README, license).

## Spec-driven development integration

This repository relies on <a href="https://github.com/github/spec-kit">GitHub Spec-Kit</a> for structured development. Before adding or changing indicators, consult the relevant spec in `specs/` and use chat commands to align with the active plan:

- **`/constitution`** — Review or update project governance principles.
- **`/specify`** — Draft detailed specifications for new or revised indicators.
- **`/plan`** — Outline the implementation approach.
- **`/tasks`** — Break work into actionable units.
- **`/implement`** — Execute the agreed-upon plan and document outcomes.

Refer to <a>Spec-Kit Integration Guide</a> for usage details.

## Pull request guidelines

- Follow <a href="https://www.conventionalcommits.org">Conventional Commits</a> for titles: `type: Subject` (subject starts uppercase, ≤ 65 characters).
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
Last updated: January 27, 2025