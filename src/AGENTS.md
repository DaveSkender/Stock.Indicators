# .NET source code development

This folder contains the Stock Indicators library source code. See `.github/skills/` for indicator development guidance (Series, Buffer, Stream).

## Technical constraints

### Performance & compatibility

- **Targets**: net10.0, net9.0, net8.0 (all must build and pass tests)
- **Complexity**: Single-pass O(n) unless mathematically impossible (justify exceptions in PR)
- **Warmup**: Provide deterministic `WarmupPeriod` helper or documented rule for each indicator
- **Precision**: Use `double` for speed; escalate to `decimal` only when rounding affects financial correctness (>0.5 tick at 4-decimal pricing)
- **Allocation**: Result list + minimal working buffers only; no temporary per-step Lists or LINQ in loops
- **Thread safety**: Stateless calculations are thread-safe; streaming hubs isolate instance state (no static mutable fields)
- **Backward compatibility**: Renaming public members or altering defaults requires MAJOR version bump

### Error conventions

- Use `ArgumentOutOfRangeException` for invalid numeric parameter ranges
- Use `ArgumentException` for semantic misuse (e.g., insufficient history)
- Never swallow exceptions; wrap only to add context
- Messages MUST include parameter name and offending value when relevant

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

See [_common/README.md](_common/README.md#nan-handling-policy) for complete policy documentation.

## Common pitfalls to avoid

1. **Off-by-one windows** when calculating lookback or warmup periods.
2. **Null or empty quotes** causing stateful streaming regressions—always validate input sequences.
3. **Precision loss** in chained calculations. Favor `double` for performance, switching to `decimal` only when business accuracy demands it.
4. **Index out of range** and buffer reuse issues in streaming indicators—guard shared spans and caches.
5. **Performance regressions** from unnecessary allocations or LINQ. Prefer span-friendly loops and avoid boxing.
6. **Documentation drift** between code comments, XML docs, and the published docs site.
7. **Improper NaN handling** - Do not reject NaN inputs; however, always guard against division by zero when denominators can be zero.

## Common indicator requirements (all styles)

Use these cross-cutting requirements for Series, Stream, and Buffer indicators. Each style guide adds its own specifics, but these apply to all:

### Code completion checklist

- [ ] **Catalog entry exists and is registered**:
  - Create `src/**/{IndicatorName}.Catalog.cs` and register in `_common/Catalog/Catalog.Listings.cs` (PopulateCatalog)
- [ ] **Regression tests include the indicator type**:
  - Add to `tests/indicators/**/{IndicatorName}.Regression.Tests.cs`
- [ ] **Performance benchmarks include a default case**:
  - Add to the appropriate file in `tools/performance` (Series/Stream/Buffer)
- [ ] **Public documentation is accurate**:
  - Update `docs/_indicators/{IndicatorName}.md` (usage, parameters, warmup, outputs)
- [ ] **Migration notes and bridges when behavior changes**:
  - Update `MigrationGuide.V3.md`
  - Update migration bridges in `Obsolete.V3.Indicators.cs` and `Obsolete.V3.Other.cs` to reflect new/renamed APIs or deprecations

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
# Build from solution root
dotnet build

# Run tests from solution root
dotnet test

# Format code
dotnet format

# Lint markdown files
npx markdownlint-cli2 --fix
```

---
Last updated: December 31, 2025
