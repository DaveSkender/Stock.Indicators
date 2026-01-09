---
name: indicator-catalog
description: Create and register indicator catalog entries for automation. Use for Catalog.cs files, CatalogListingBuilder patterns, parameter/result definitions, and PopulateCatalog registration.
---

# Indicator catalog development

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

## Required parameter patterns

- Use `AddParameter<T>()` for basic types (int, double, bool)
- Use `AddEnumParameter<T>()` for enum types
- Use `AddDateParameter()` for DateTime parameters
- Use `AddSeriesParameter()` for `IReadOnlyList<T> where T : IReusable`
- MUST set `minimum` and `maximum` for all numeric parameters

## Required result patterns

- `dataName` MUST match property name in Models file exactly
- Set `isReusable: true` ONLY for property mapping to `IReusable.Value`
- `ISeries` models: ALL results MUST have `isReusable: false`
- Exactly ONE result with `isReusable: true` per `IReusable` indicator

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

Add to `src/_common/Catalog/Catalog.Listings.cs` in `PopulateCatalog()`:

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

## Prohibited patterns

- ❌ `.WithMethodName()` in CommonListing (MUST be in style-specific listings)
- ❌ Wrong indicator method name (breaks extension method discovery)
- ❌ `isReusable: true` for `ISeries` models (violates interface contract)
- ❌ Multiple `isReusable: true` results (ambiguous Value mapping)

---
Last updated: December 31, 2025
