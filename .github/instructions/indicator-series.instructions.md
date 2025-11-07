---
applyTo: "src/**/*.*Series.cs,tests/**/*.*Series.Tests.cs"
description: "Series-style indicator development and testing guidelines"
---

# Series indicator development guidelines

These instructions apply to series-style indicators that process complete datasets and return results all at once. Series indicators are the foundation style that all indicators must implement.

## When to use this agent

Invoke `@series` when you need help with:

- Implementing new Series indicators with mathematical precision
- Validating calculation correctness against reference formulas
- Structuring input validation and exception handling patterns
- Determining warmup period requirements for your indicator
- Writing comprehensive test coverage (Standard, Boundary, BadData, etc.)
- Optimizing batch processing performance
- Debugging calculation discrepancies or formula issues

For quick decision guidance and pattern selection, use the agent. For comprehensive implementation details and complete checklists, continue reading this document.

## Related agents

- `@buffer` - BufferList indicator development guidance (incremental processing patterns)
- `@streamhub` - StreamHub indicator development guidance (real-time processing patterns)
- `@performance` - Performance optimization guidance (algorithmic complexity, O(1) patterns, benchmarking)

See also: [`.github/cheatsheets/indicator-series.agent.md`](../cheatsheets/indicator-series.agent.md) for decision trees and quick reference patterns.

## Code completion checklist

When implementing or updating an indicator, you must complete:

- [ ] Source code: `src/**/{IndicatorName}.StaticSeries.cs` file exists and adheres to these instructions
  - [ ] Validates parameters up front; throws consistent exceptions/messages
  - [ ] Uses efficient loops; avoids unnecessary allocations and LINQ in hot paths
  - [ ] Implements warmup logic and returns results with correct timestamps
  - [ ] Member order matches conventions in this document
- [ ] Catalog: `src/**/{IndicatorName}.Catalog.cs` exists, is accurate, and registered in `src\_common\Catalog\Catalog.Listings.cs` (`PopulateCatalog`)
- [ ] Unit testing: `tests/indicators/**/{IndicatorName}.StaticSeries.Tests.cs` file exists and adheres to these instructions
  - [ ] Covers happy path, boundary (min periods), bad data, insufficient data, and precision checks
  - [ ] Verifies alignment with manually calculated values, typically from a spreadsheet (`.xlsx`)
- [ ] Common items: Complete catalog, regression, performance, docs, and migration per `.github/copilot-instructions.md` (Common indicator requirements)
- [ ] Migration bridges updated: `src/Obsolete.V3.Indicators.cs`, `src/Obsolete.V3.Other.cs`; and `src/MigrationGuide.V3.md` as needed

## File naming conventions

Series indicators should follow these naming patterns:

- **Implementation**: `{IndicatorName}.StaticSeries.cs`
- **Tests**: `{IndicatorName}.StaticSeries.Tests.cs`
- **Interface**: `I{IndicatorName}.cs` (optional, for complex indicators)

## Implementation requirements

### Core method structure

```csharp
/// <summary>
/// {Indicator description with mathematical formula if applicable}
/// </summary>
/// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
/// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
/// <returns>Collection of {IndicatorName}Result records</returns>
public static IReadOnlyList<{IndicatorName}Result> To{IndicatorName}(
    this IReadOnlyList<IQuote> quotes,
    int lookbackPeriods = {defaultValue})
{
    // Input validation
    // Core calculation logic
    // Return results
}
```

### Input validation patterns

```csharp
// Validate quotes collection
quotes.ThrowIfNull();
List<TQuote> quotesList = quotes.ToList();

// Validate minimum periods
if (lookbackPeriods <= 0)
{
    throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), 
        "Lookback periods must be greater than 0.");
}

// Validate sufficient data
if (quotesList.Count < lookbackPeriods)
{
    return Enumerable.Empty<{IndicatorName}Result>();
}
```

## Testing requirements

### Test coverage expectations

Series indicator tests must cover:

1. **Happy path calculations** - Verify against manually calculated reference data
2. **Boundary conditions** - Test with minimum required periods
3. **Edge cases** - Empty quotes, insufficient data, invalid parameters
4. **Mathematical accuracy** - Compare against reference implementations
5. **Performance expectations** - Must not exceed baseline performance

### Test structure pattern

```csharp
[TestClass]
public class {IndicatorName}StaticSeriesTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        // Test against historical data with expected results
        IReadOnlyList<{IndicatorName}Result> results = quotes.To{IndicatorName}();
        
        // Verify specific data points
        {IndicatorName}Result result = results.ElementAt(index);
        result.{Property}.Should().BeApproximately(expectedValue, precision);
    }

    [TestMethod]
    public void InsufficientQuotes()
    {
        // Test with insufficient data
        IReadOnlyList<{IndicatorName}Result> results = TestData.GetDefault(10).To{IndicatorName}(20);
        results.Should().BeEmpty();
    }

    [TestMethod]
    public void BadData()
    {
        // Test parameter validation
        Action act = () => quotes.To{IndicatorName}(-1);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
```

### Performance benchmarking

Series indicators with significant computational complexity must include performance tests in the `tools/performance` project:

```csharp
// In tools/performance project
[MethodImpl(MethodImplOptions.NoInlining)]
public void SeriesIndicator{IndicatorName}()
{
    _ = quotes.To{IndicatorName}(lookbackPeriods);
}
```

See also: Common indicator requirements and Series-as-canonical policy in `.github/copilot-instructions.md`.

## Quality standards

### Mathematical accuracy

- All calculations must be verified against reference implementations
- Include Excel/manual calculation files in test folders when applicable
- Handle floating-point precision appropriately (typically 6 decimal places)
- Document any known precision limitations

### Performance expectations

- Series processing should complete within reasonable time bounds
- Avoid unnecessary memory allocations in loops
- Use efficient data structures and algorithms
- Profile memory usage for large datasets

### Documentation requirements

- Complete XML documentation for public methods
- Include mathematical formulas and references where applicable
- Document parameter constraints and typical usage patterns
- Provide code examples for complex indicators

## Common patterns

### Result initialization

```csharp
// Initialize results collection
List<{IndicatorName}Result> results = new(quotesList.Count);

// Pre-populate with empty results
for (int i = 0; i < lookbackPeriods - 1; i++)
{
    results.Add(new {IndicatorName}Result
    {
        Timestamp = quotesList[i].Date
    });
}
```

### Calculation loops

```csharp
// Main calculation loop
for (int i = lookbackPeriods - 1; i < quotesList.Count; i++)
{
    TQuote quote = quotesList[i];
    
    // Perform calculations
    double calculatedValue = PerformCalculation(quotesList, i, lookbackPeriods);
    
    results.Add(new {IndicatorName}Result
    {
        Timestamp = quote.Date,
        Value = calculatedValue
    });
}
```

## Reference examples

Study these exemplary series indicators:

- **EMA**: `src/e-k/Ema/Ema.StaticSeries.cs`
- **SMA**: `src/s-z/Sma/Sma.StaticSeries.cs`
- **ATRSTOP**: `src/a-d/AtrStop/AtrStop.StaticSeries.cs`
- **ALLIGATOR**: `src/a-d/Alligator/Alligator.StaticSeries.cs`

---
Last updated: October 12, 2025
