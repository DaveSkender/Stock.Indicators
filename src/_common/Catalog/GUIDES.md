# Quick Reference Guides

## Fluent Builder API

Use the `IndicatorListingBuilder` to create catalog listings with a fluent interface:

```csharp
internal static readonly IndicatorListing SeriesListing = new IndicatorListingBuilder()
    .WithName("Exponential Moving Average")
    .WithId("EMA")
    .WithStyle(Style.Series)
    .WithCategory(Category.MovingAverage)
    .AddParameter<int>("lookbackPeriods", "Lookback Period",
        description: "Number of periods for the EMA calculation",
        isRequired: true,
        defaultValue: 20,
        minimum: 2,
        maximum: 250)
    .AddResult("Ema", "EMA", ResultType.Default, isDefault: true)
    .Build();
```

**Core Methods:**

- `.WithName(string)` - Set display name
- `.WithId(string)` - Set unique identifier  
- `.WithStyle(Style)` - Set indicator style (Series, Stream, Buffer)
- `.WithCategory(Category)` - Set classification category
- `.AddParameter<T>(name, displayName, ...)` - Add parameters with validation
- `.AddResult(dataName, displayName, type, isDefault)` - Add result properties
- `.Build()` - Create the final listing

## Multi-Style Catalog Listings

For indicators supporting multiple styles, create separate listings for each style:

```csharp
public static partial class Ema
{
    [SeriesIndicator("EMA")]
    public static IReadOnlyList<EmaResult> ToEma<T>(
        this IReadOnlyList<T> source, int lookbackPeriods) { }

    [StreamIndicator("EMA")]
    public static EmaHub<T> ToEma<T>(
        this IChainProvider<T> chainProvider, int lookbackPeriods) { }

    [BufferIndicator("EMA")]
    public EmaList(int lookbackPeriods) { }

    // Separate listing for each style
    internal static readonly IndicatorListing SeriesListing = /* ... */;
    internal static readonly IndicatorListing StreamListing = /* ... */;
    internal static readonly IndicatorListing BufferListing = /* ... */;
}
```

**Key Principles:**

- Use same identifier across all styles (e.g., "EMA")
- Create separate listing properties: `SeriesListing`, `StreamListing`, `BufferListing`
- Match parameter names exactly with method signatures
- All listings must be explicitly defined

**Registry Usage:**

```csharp
// Find by style
var seriesIndicators = IndicatorRegistry.GetByStyle(Style.Series);
var streamIndicators = IndicatorRegistry.GetByStyle(Style.Stream);

// Find by ID (returns all styles)
var emaIndicators = IndicatorRegistry.GetById("EMA");

// Find specific style variant
var emaSeries = IndicatorRegistry.GetById("EMA")
    .FirstOrDefault(i => i.Style == Style.Series);
```

## Best Practices

1. **Parameter Matching** - Ensure parameter names match method signatures exactly
2. **Validation** - Include minimum/maximum constraints where appropriate  
3. **Documentation** - Provide clear descriptions for all parameters and results
4. **Separation** - Keep catalog listings in separate `.Catalog.cs` files
5. **Testing** - Verify catalog correctness with unit tests
6. **Consistency** - Use consistent naming patterns across similar indicators
