---
title: Double Exponential Moving Average (DEMA)
description: Created by Patrick G. Mulloy, the Double Exponential Moving Average is a faster smoothed EMA of financial market prices.
redirect_from:
 - /indicators/DoubleEma/
---

# Double Exponential Moving Average (DEMA)

Created by Patrick G. Mulloy, the [Double exponential moving average](https://en.wikipedia.org/wiki/Double_exponential_moving_average) is a faster smoothed EMA of the price over a lookback window.
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/807 "Community discussion about this indicator")

<ClientOnly>
  <IndicatorChart src="/data/Dema.json" :height="360" />
</ClientOnly>

```csharp
// C# usage syntax
IReadOnlyList<DemaResult> results =
  quotes.ToDema(lookbackPeriods);
```

## Parameters

| param | type | description |
| ----- | ---- | ----------- |
| `lookbackPeriods` | int | Number of periods (`N`) in the moving average.  Must be greater than 0. |

### Historical quotes requirements

You must have at least `3×N` or `2×N+100` periods of `quotes`, whichever is more, to cover the [warmup and convergence](https://github.com/DaveSkender/Stock.Indicators/discussions/688) periods.  Since this uses a smoothing technique, we recommend you use at least `2×N+250` data points prior to the intended usage date for better precision.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](/guide#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<DemaResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N-1` periods will have `null` values since there's not enough data to calculate.

::: warning ⚞ Convergence warning
The first `2×N+100` periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods.
:::

### `DemaResult`

| property | type | description |
| -------- | ---- | ----------- |
| `Timestamp` | DateTime | Date from evaluated `TQuote` |
| `Dema` | double | Double exponential moving average |

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
    .ToDema(..);
```

Results can be further processed on `Dema` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .ToDema(..)
    .ToRsi(..);
```

## Streaming

Use the buffer-style `List<T>` when you need incremental calculations without a hub:

```csharp
DemaList demaList = new(lookbackPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  demaList.Add(quote);
}

// based on `ICollection<DemaResult>`
IReadOnlyList<DemaResult> results = demaList;
```

Subscribe to a `QuoteHub` for advanced streaming scenarios:

```csharp
QuoteHub quoteHub = new();
DemaHub observer = quoteHub.ToDemaHub(lookbackPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  quoteHub.Add(quote);
}

IReadOnlyList<DemaResult> results = observer.Results;
```
