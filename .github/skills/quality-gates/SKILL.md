---
name: quality-gates
description: Pre-commit quality checklist for Stock Indicators development. Use before completing work to ensure clean build, passing tests, documentation updates, and migration bridge requirements.
---

# Quality gates

## Preparation

### Remove dead code

- MUST delete obsolete or commented-out code
- MUST remove unused imports, variables, helper methods
- MUST strip debugging aids (`Console.WriteLine`, breakpoints)
- MUST remove scratch files (`.bak`, `.new`, `.debug.*`)

## Validation sequence

Execute in order. Fix issues before proceeding to next step.

### Step 1: Prerequisites

```bash
dotnet restore
dotnet format
npx markdownlint-cli2 --fix
```

### Step 2: Format verification

```bash
dotnet format --verify-no-changes
dotnet build "Stock.Indicators.sln" -v minimal --nologo
```

MUST resolve all warnings. NEVER introduce suppressions without approval.

### Step 3: Testing

```bash
dotnet test "Stock.Indicators.sln" --no-restore --nologo
```

Note: 98% code coverage threshold is validated in CI/CD via Codacy. Local testing does not collect coverage metrics.

### Step 4: Documentation

- Update XML documentation for changed public APIs
- Update `docs/_indicators/{IndicatorName}.md` for indicator changes
- Update `src/MigrationGuide.V3.md` for breaking changes
- Update `src/Obsolete.V3.Indicators.cs` for deprecated APIs

### Step 5: Markdown linting

```bash
npx markdownlint-cli2
```

### Step 6: Final verification

```bash
dotnet format --verify-no-changes
dotnet build "Stock.Indicators.sln" -v minimal --nologo
dotnet test "Stock.Indicators.sln" --no-build --nologo
npx markdownlint-cli2
```

All commands must complete successfully.

## Required gates

All MUST pass before completing work:

- Dead/debugging code removed
- `dotnet format --verify-no-changes` passes
- `dotnet build` succeeds with zero warnings
- All tests pass
- Coverage ≥ 98% (validated in CI/CD)
- Documentation updated
- Markdown linting succeeds

## Required indicator components

For new or updated indicators, MUST include:

- Series implementation (`*.StaticSeries.cs`)
- Catalog entry and registration (`*.Catalog.cs`)
- Unit tests with full coverage (`*.Tests.cs`)
- Documentation (`docs/_indicators/{Name}.md`)
- Regression test baseline (if algorithm changed)
- Performance benchmark (for complex indicators)

## Required migration bridge

When changing public APIs, MUST:

- Add `[Obsolete]` attribute with migration message
- Update `src/MigrationGuide.V3.md`
- Update bridge files:
  - `src/Obsolete.V3.Indicators.cs`
  - `src/Obsolete.V3.Other.cs`

## Recovery from failures

If any verification step fails:

1. Fix the issue
2. Restart from Step 2 (Format verification)
3. Complete all remaining steps

---
Last updated: December 31, 2025
