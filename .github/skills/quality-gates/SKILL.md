---
name: quality-gates
description: Pre-commit quality checklist for Stock Indicators development. Use before completing work to ensure clean build, passing tests, documentation updates, and migration bridge requirements.
---

# Quality gates

Execute all quality gates before completing work and yielding to human review.

## Preparation

### Remove dead code

- Delete obsolete or commented-out code
- Remove unused imports, variables, helper methods
- Strip debugging aids (`Console.WriteLine`, breakpoints)
- Remove scratch files (`.bak`, `.new`, `.debug.*`)

## Validation sequence

Execute in order; fix issues before proceeding.

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

Resolve all warnings. Do not introduce suppressions without approval.

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

## Sign-off checklist

- [ ] All dead/debugging code removed
- [ ] `dotnet format --verify-no-changes` passes
- [ ] `dotnet build` succeeds with zero warnings
- [ ] All tests pass
- [ ] Coverage ≥ 98%
- [ ] Documentation updated
- [ ] Markdown linting succeeds

## Indicator implementation checklist

For new or updated indicators:

- [ ] Series implementation exists (`*.StaticSeries.cs`)
- [ ] Catalog entry exists and registered (`*.Catalog.cs`)
- [ ] Unit tests exist with full coverage (`*.Tests.cs`)
- [ ] Documentation exists (`docs/_indicators/{Name}.md`)
- [ ] Regression test baseline updated if algorithm changed
- [ ] Performance benchmark added for complex indicators

## Migration bridge requirements

When changing public APIs:

- [ ] Add `[Obsolete]` attribute with migration message
- [ ] Update `src/MigrationGuide.V3.md`
- [ ] Update bridge files:
  - `src/Obsolete.V3.Indicators.cs`
  - `src/Obsolete.V3.Other.cs`

## Recovery from failures

If any verification step fails:

1. Fix the issue
2. Restart from Step 2 (Format verification)
3. Complete all remaining steps

---
Last updated: December 31, 2025
