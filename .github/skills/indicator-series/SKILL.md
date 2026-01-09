---
name: indicator-series
description: Implement Series-style batch indicators with mathematical precision. Use for new StaticSeries implementations or optimization. Series results are the canonical reference—all other styles must match exactly. Focus on cross-cutting requirements and performance optimization decisions.
---

# Series indicator development

## File structure

- Implementation: `src/{category}/{Indicator}/{Indicator}.StaticSeries.cs`
- Test: `tests/indicators/{category}/{Indicator}/{Indicator}.StaticSeries.Tests.cs`
- Catalog: `src/{category}/{Indicator}/{Indicator}.Catalog.cs`
- Categories: a-d, e-k, m-r, s-z (alphabetical)

## Performance optimization

**Array allocation pattern** (recommended for new implementations):

```csharp
TResult[] results = new TResult[length];
// ... assign results[i] = new TResult(...);
return new List<TResult>(results);  // NOT results.ToList()
```

**When to use**: Indicators with predictable result counts show ~2x improvement (Issue #1259)

**When NOT to use**: Benchmark first. Some indicators (ADL) remain faster with `List.Add()`

**Conversion strategy**:

1. Benchmark existing List-based implementation
2. Convert to array pattern
3. Benchmark again
4. Revert if no improvement or regression

## Required implementation

Beyond the .StaticSeries.cs file, ensure:

- [ ] **Catalog registration**: Create `src/**/{IndicatorName}.Catalog.cs` and register in `src/_common/Catalog/Catalog.Listings.cs`
- [ ] **Unit tests**: Create `tests/indicators/**/{IndicatorName}.StaticSeries.Tests.cs`
  - Inherit from `StaticSeriesTestBase`
  - Include `[TestCategory("Regression")]` for baseline validation
  - Verify against manually calculated reference values
- [ ] **Performance benchmark**: Add to `tools/performance/SeriesIndicators.cs`
- [ ] **Public documentation**: Update `docs/_indicators/{IndicatorName}.md`

## Precision testing patterns

- **Store reference data separately**: Create `{Indicator}.Data.cs` files with arrays of expected values at maximum precision
- **Excel manual calculations**: Export at highest precision available (~14 decimal places for `default.csv` values ~200)
- **Baseline regression validation**: Compare full dataset against reference arrays using Money10-Money12 precision
- **Spot check assertions**: Use Money4 for individual sample value readability (sanity checks, not proofs)
- **Longer datasets**: May require lower precision (e.g., Money10 for 15k quotes) due to accumulated floating-point error
- **Document degradation**: When precision must be lowered, explain why in test comments

## Examples

- **Simple single-value**: `src/s-z/Sma/Sma.StaticSeries.cs`
- **Exponential smoothing**: `src/e-k/Ema/Ema.StaticSeries.cs`
- **Complex multi-stage**: `src/a-d/Adx/Adx.StaticSeries.cs`
- **Multi-line results**: `src/a-d/Alligator/Alligator.StaticSeries.cs`

See `references/decision-tree.md` for result interface selection guidance.

## Constitutional constraints

- **Series is truth**: All other styles (BufferList, StreamHub) MUST match Series results exactly
- **Verify against authoritative sources**: NEVER trust other libraries—use reference publications only
- **Algebraic stability**: Prefer boundary detection over clamping
- **Real-world testing**: Synthetic boundary data may miss precision edge cases
- **Fix formulas, not symptoms**: When all styles fail identically, fix the core algorithm

NEVER modify formulas without verification against authoritative mathematical references. See `src/AGENTS.md` for formula protection rules.

---
Last updated: December 31, 2025
