---
name: indicator-catalog
description: Create and register indicator catalog entries for automation. Use for Catalog.cs files, CatalogListingBuilder patterns, parameter/result definitions, and PopulateCatalog registration.
---

# Indicator catalog development

Create catalog entries that enable automation and UI discovery of indicators.

## File structure

- Catalog: `src/{category}/{Indicator}/{Indicator}.Catalog.cs`
- Location: Same directory as `{Indicator}.Models.cs`

## Builder pattern

```csharp
public static partial class Ema
{
    /// <summary>
    /// EMA Common Base Listing
    /// </summary>
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Exponential Moving Average")
            .WithId("EMA")
            .WithCategory(Category.MovingAverage)
            .AddParameter<int>("lookbackPeriods", "Lookback Period",
                description: "Number of periods for the EMA calculation",
                isRequired: true, defaultValue: 20, minimum: 2, maximum: 250)
            .AddResult("Ema", "EMA", ResultType.Default, isReusable: true)
            .Build();

    /// <summary>
    /// EMA Series Listing
    /// </summary>
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .WithMethodName("ToEma")
            .Build();

    /// <summary>
    /// EMA Stream Listing
    /// </summary>
    internal static readonly IndicatorListing StreamListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Stream)
            .WithMethodName("ToEmaHub")
            .Build();

    /// <summary>
    /// EMA Buffer Listing
    /// </summary>
    internal static readonly IndicatorListing BufferListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Buffer)
            .WithMethodName("ToEmaList")
            .Build();
}
```

## Method naming conventions

| Style | Pattern | Example |
| ----- | ------- | ------- |
| Series | `To{IndicatorName}` | `ToEma` |
| Stream | `To{IndicatorName}Hub` | `ToEmaHub` |
| Buffer | `To{IndicatorName}List` | `ToEmaList` |

**Critical**: `.WithMethodName()` must be in style-specific listings, NOT in CommonListing.

## Parameter guidelines

- Use `AddParameter<T>()` for basic types (int, double, bool)
- Use `AddEnumParameter<T>()` for enum types
- Use `AddDateParameter()` for DateTime parameters
- Use `AddSeriesParameter()` for `IReadOnlyList<T> where T : IReusable`
- Always set `minimum` and `maximum` for numeric parameters

## Result guidelines

- `dataName` must match property name in Models file exactly
- Set `isReusable: true` ONLY for property mapping to `IReusable.Value`
- `ISeries` models: ALL results must have `isReusable: false`
- Exactly one result with `isReusable: true` per `IReusable` indicator

## Categories

| Category | Examples |
| -------- | -------- |
| MovingAverage | EMA, SMA, HMA, TEMA |
| Oscillator | RSI, Stochastic, MACD |
| PriceChannel | Bollinger Bands, Keltner |
| Trend | ADX, Aroon, Parabolic SAR |
| Volume | OBV, Chaikin Money Flow |
| Volatility | ATR, Standard Deviation |

## Registration

Add to `src/_common/Catalog/Catalog.cs` in `PopulateCatalog()`:

```csharp
_catalog.Add(Ema.SeriesListing);
_catalog.Add(Ema.StreamListing);
_catalog.Add(Ema.BufferListing);
```

Register in alphabetical order by indicator name.

## Testing

Create `tests/indicators/{folder}/{Indicator}/{Indicator}.Catalog.Tests.cs`:

```csharp
[TestClass]
public class EmaCatalogTests : TestBase
{
    [TestMethod]
    public void EmaSeriesListing()
    {
        var listing = Ema.SeriesListing;
        listing.Name.Should().Be("Exponential Moving Average");
        listing.Style.Should().Be(Style.Series);
        listing.MethodName.Should().Be("ToEma");
    }
}
```

## Anti-patterns

- ❌ `.WithMethodName()` in CommonListing
- ❌ Wrong indicator method name
- ❌ `isReusable: true` for `ISeries` models
- ❌ Multiple `isReusable: true` results

---
Last updated: December 30, 2025
