---
title: Bollinger Bands®
description: Created by John Bollinger, the Bollinger Bands price channels depict volatility as standard deviation boundary line range from a moving average of price.  Bollinger Bands&#174; is a registered trademark of John A. Bollinger.
---

# Bollinger Bands®

Created by John Bollinger, [Bollinger Bands](https://en.wikipedia.org/wiki/Bollinger_Bands) price channels depict volatility as standard deviation boundary line range from a moving average of price.  Bollinger Bands® is a registered trademark of John A. Bollinger.
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/267 "Community discussion about this indicator")

<ClientOnly>
  <IndicatorChart src="/data/BollingerBands.json" :height="360" />
</ClientOnly>

```csharp
// C# usage syntax
IReadOnlyList<BollingerBandsResult> results =
  quotes.ToBollingerBands(lookbackPeriods, standardDeviations);
```

## Parameters

| param | type | description |
| ----- | ---- | ----------- |
| `lookbackPeriods` | int | Number of periods (`N`) for the center line moving average.  Must be greater than 1 to calculate; however we suggest a larger period for statistically appropriate sample size.  Default is 20. |
| `standardDeviations` | double | Width of bands.  Standard deviations (`D`) from the moving average.  Must be greater than 0.  Default is 2. |

### Historical quotes requirements

You must have at least `N` periods of `quotes` to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](/guide#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<BollingerBandsResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N-1` periods will have `null` values since there's not enough data to calculate.

### `BollingerBandsResult`

| property | type | description |
| -------- | ---- | ----------- |
| `Timestamp` | DateTime | Date from evaluated `TQuote` |
| `Sma` | double | Simple moving average (SMA) of price (center line) |
| `UpperBand` | double | Upper line is `D` standard deviations above the SMA |
| `LowerBand` | double | Lower line is `D` standard deviations below the SMA |
| `PercentB` | double | `%B` is the location within the bands.  `(Price-LowerBand)/(UpperBand-LowerBand)` |
| `ZScore` | double | Z-Score of current price (number of standard deviations from mean) |
| `Width` | double | Width as percent of SMA price.  `(UpperBand-LowerBand)/Sma` |

### Utilities

- [.Condense()](/utilities/results#condense)
- [.Find(lookupDate)](/utilities/results#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()](/utilities/results#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)](/utilities/results#remove-warmup-periods)

See [Utilities and helpers](/utilities/results) for more information.

## Chaining

This indicator may be generated from any chain-enabled indicator or method.

```csharp
// example
var results = quotes
    .Use(CandlePart.HL2)
    .ToBollingerBands(..);
```

Results can be further processed on `PercentB` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .ToBollingerBands(..)
    .ToRsi(..);
```

## Streaming

Use the buffer-style `List<T>` when you need incremental calculations without a hub:

```csharp
BollingerBandsList bbList = new(lookbackPeriods, standardDeviations);

foreach (IQuote quote in quotes)  // simulating stream
{
  bbList.Add(quote);
}

// based on `ICollection<BollingerBandsResult>`
IReadOnlyList<BollingerBandsResult> results = bbList;
```

Subscribe to a `QuoteHub` for advanced streaming scenarios:

```csharp
QuoteHub quoteHub = new();
BollingerBandsHub observer = quoteHub.ToBollingerBandsHub(lookbackPeriods, standardDeviations);

foreach (IQuote quote in quotes)  // simulating stream
{
  quoteHub.Add(quote);
}

IReadOnlyList<BollingerBandsResult> results = observer.Results;
```
