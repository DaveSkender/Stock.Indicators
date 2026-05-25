---
name: indicator-catalog
description: Create and register indicator catalog entries for automation. Use for Catalog.cs files, CatalogListingBuilder patterns, parameter/result definitions, and PopulateCatalog registration.
---

# Indicator catalog development

## File

`src/{category}/{Indicator}/{Indicator}.Catalog.cs`

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

## Method naming

| Style | Pattern | Example |
| ----- | ------- | ------- |
| Series | `To{Name}` | `ToEma` |
| Stream | `To{Name}Hub` | `ToEmaHub` |
| Buffer | `To{Name}List` | `ToEmaList` |

`.WithMethodName()` must be in style-specific listings, NOT in `CommonListing`.

## Parameter patterns

- `AddParameter<T>()` — int, double, bool
- `AddEnumParameter<T>()` — enum types
- `AddDateParameter()` — DateTime
- `AddSeriesParameter()` — `IReadOnlyList<T> where T : IReusable`
- `minimum` and `maximum` required for all numeric parameters

## Result patterns

- `dataName` must match property name in Models file exactly
- `isReusable: true` only for the property mapping to `IReusable.Value`
- `ISeries` models: all results must have `isReusable: false`
- Exactly one `isReusable: true` per `IReusable` indicator

## Categories

| Category | Examples |
| -------- | -------- |
| `CandlestickPattern` | Doji, Marubozu |
| `MovingAverage` | EMA, SMA, HMA, TEMA, WMA, DEMA |
| `Oscillator` | RSI, Stochastic, MACD, CCI, BOP, CMO, Chop, DPO |
| `PriceChannel` | Bollinger Bands, Keltner, Donchian, VWAP |
| `PriceCharacteristic` | ATR, Beta, Standard Deviation, True Range |
| `PricePattern` | Fractal, Pivot Points |
| `PriceTransform` | Quote Part, ZigZag |
| `PriceTrend` | ADX, Aroon, Alligator, AtrStop, SuperTrend, Vortex |
| `StopAndReverse` | Chandelier, Parabolic SAR, Volatility Stop |
| `VolumeBased` | OBV, Chaikin Money Flow, Chaikin Oscillator |

## Registration

Add to `src/_common/Catalog/Catalog.Listings.cs` in `PopulateCatalog()`. Indicators are grouped alphabetically by indicator name, and within each indicator the three style listings register in **Buffer → Series → Stream** order (matches the existing file convention; see `Adl`, `Adx`, `Alligator`, `Alma` for examples):

```csharp
// EMA (Exponential Moving Average)
_listings.Add(Ema.BufferListing);
_listings.Add(Ema.SeriesListing);
_listings.Add(Ema.StreamListing);
```

The backing field is `_listings` (a `private static readonly List<IndicatorListing>` at the top of `Catalog.Listings.cs`). A short comment header `// {ABBR} ({Full Name})` precedes each indicator block, separated by a blank line.

## Prohibited

- `.WithMethodName()` in `CommonListing`
- Wrong indicator method name
- `isReusable: true` for `ISeries` models
- Multiple `isReusable: true` results per indicator

## Testing

`tests/indicators/{folder}/{Indicator}/{Indicator}.Catalog.Tests.cs`:

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

## Plan reference

Catalog evolution and assertion-sharpening items (e.g. exact-count assertions in `Catalog.Metrics.Tests.cs`) are tracked in [docs/plans/streaming-indicators.plan.md](../../../docs/plans/streaming-indicators.plan.md).
