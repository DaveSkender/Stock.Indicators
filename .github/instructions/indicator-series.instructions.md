---
applyTo: "src/**/*.*Series.cs,tests/**/*.*Series.Tests.cs"
description: "Series-style indicator development and testing guidelines"
---

# Series indicator development guidelines

These instructions apply to series-style indicators that process complete datasets and return results all at once. Series indicators are the foundation style that all indicators must implement.

## Code completion checklist

When implementing or updating an indicator, you must complete:

- [ ] Source code: `src/**/{IndicatorName}.StaticSeries.cs` files exist and adhere to these instructions
  - [ ] <!-- TODO: add imperatives -->
- [ ] Testing : `tests/indicators/**/{IndicatorName}.StaticSeries.Tests.cs` files exist adhere to these instructions
  - [ ] <!-- TODO: add imperatives -->
- [ ] User documentation: `docs/_indicators/{IndicatorName}.md` file exists and is accurate
- [ ] Release/migration notes: [MigrationGuide.V3.md](/src/MigrationGuide.V3.md) has been updated for changes from the v2 package version.


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
/// <typeparam name="TQuote">Type that implements IQuote interface</typeparam>
/// <param name="quotes">Historical price quotes</param>
/// <param name="lookbackPeriods">Lookback period (default value)</param>
/// <returns>Collection of {IndicatorName}Result records</returns>
public static IReadOnlyList<{IndicatorName}Result> To{IndicatorName}<TQuote>(
    this IReadOnlyList<TQuote> quotes,
    int lookbackPeriods = {defaultValue})
    where TQuote : IQuote
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
Last updated: September 29, 2025
