---
name: indicator-series
description: Implement Series-style batch indicators with mathematical precision. Use for new StaticSeries implementations, warmup period calculations, validation patterns, and test coverage. Series results are the canonical reference—all other styles must match exactly.
---

# Series indicator development

Series indicators process complete datasets and return results all at once. They are the foundation style that establishes mathematical correctness.

## File structure

- Implementation: `src/{category}/{Indicator}/{Indicator}.StaticSeries.cs`
- Test: `tests/indicators/{category}/{Indicator}/{Indicator}.StaticSeries.Tests.cs`
- Categories: a-d, e-k, m-r, s-z (alphabetical)

## Core pattern

```csharp
public static IReadOnlyList<TResult> ToIndicator(
    this IReadOnlyList<IQuote> quotes,
    int lookbackPeriods)
{
    // 1. Validate parameters
    ArgumentOutOfRangeException.ThrowIfLessThan(lookbackPeriods, 1);

    // 2. Initialize results
    int length = quotes.Count;
    List<TResult> results = new(length);

    // 3. Calculate warmup period results (null values)
    // 4. Calculate main results
    // 5. Return results
    return results;
}
```

## Warmup period calculation

| Indicator Type | Formula | Example |
| -------------- | ------- | ------- |
| Simple MA | `lookback - 1` | SMA(20) → 19 warmup |
| Exponential | `lookback` | EMA(12) → 12 warmup |
| Multi-stage | Sum of stages | MACD(12,26,9) → 34 warmup |

## Validation patterns

- Null quotes: `ArgumentNullException.ThrowIfNull(quotes)`
- Invalid range: `ArgumentOutOfRangeException.ThrowIfLessThan(period, 1)`
- Semantic error: `ArgumentException` for logical constraints

## Testing requirements

1. Inherit from `StaticSeriesTestBase`
2. Implement Standard, Boundary, BadData, InsufficientData tests
3. Verify against manually calculated reference values
4. Use exact equality: `result.Value.Should().Be(expected)`

## Code completion checklist

- [ ] Source code: `src/**/{IndicatorName}.StaticSeries.cs` file exists
  - [ ] Validates parameters up front; throws consistent exceptions/messages
  - [ ] Uses efficient loops; avoids unnecessary allocations and LINQ in hot paths
  - [ ] Implements warmup logic and returns results with correct timestamps
- [ ] Catalog: `src/**/{IndicatorName}.Catalog.cs` exists and registered
- [ ] Unit testing: `tests/indicators/**/{IndicatorName}.StaticSeries.Tests.cs` exists
  - [ ] Covers happy path, boundary, bad data, insufficient data, and precision checks
  - [ ] Verifies alignment with manually calculated values

## Mathematical accuracy

- Verify calculations against reference implementations
- Include Excel/manual calculation files in test folders when applicable
- Handle floating-point precision appropriately (typically 6 decimal places)
- Document known precision limitations
- Use algebraically stable formulas—prefer boundary detection over clamping
- Test with real-world data—synthetic boundary data may not expose precision edge cases
- Fix formulas, not symptoms—when all styles fail identically, fix the core algorithm

## Reference implementations

- Simple: `src/s-z/Sma/Sma.StaticSeries.cs`
- Exponential: `src/e-k/Ema/Ema.StaticSeries.cs`
- Complex: `src/a-d/Adx/Adx.StaticSeries.cs`
- Multi-line: `src/a-d/Alligator/Alligator.StaticSeries.cs`

## Constitutional rules

**Warning**: See `src/AGENTS.md` for formula protection rules.
Series is the canonical source of mathematical truth. All other styles (BufferList, StreamHub) must match Series results exactly.

See `references/decision-tree.md` for implementation guidance.

---
Last updated: December 30, 2025
