---
title: Kaufman's Adaptive Moving Average (KAMA)
description: Created by Perry Kaufman, KAMA is an volatility adaptive (adjusted) moving average of price over configurable lookback periods.
---

# Kaufman's Adaptive Moving Average (KAMA)

Created by Perry Kaufman, [KAMA](https://school.stockcharts.com/doku.php?id=technical_indicators:kaufman_s_adaptive_moving_average) is an volatility adaptive moving average of price over configurable lookback periods.
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/210 "Community discussion about this indicator")

<ClientOnly>
  <IndicatorChart src="/data/Kama.json" :height="360" />
</ClientOnly>

```csharp
// C# usage syntax
IReadOnlyList<KamaResult> results =
  quotes.ToKama(erPeriods, fastPeriods, slowPeriods);
```

## Parameters

| param | type | description |
| ----- | ---- | ----------- |
| `erPeriods` | int | Number of Efficiency Ratio (volatility) periods (`E`).  Must be greater than 0.  Default is 10. |
| `fastPeriods` | int | Number of Fast EMA periods.  Must be greater than 0.  Default is 2. |
| `slowPeriods` | int | Number of Slow EMA periods.  Must be greater than `fastPeriods`.  Default is 30. |

### Historical quotes requirements

You must have at least `6×E` or `E+100` periods of `quotes`, whichever is more, to cover the [warmup and convergence](https://github.com/DaveSkender/Stock.Indicators/discussions/688) periods.  Since this uses a smoothing technique, we recommend you use at least `10×E` data points prior to the intended usage date for better precision.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](/guide#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<KamaResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `E-1` periods will have `null` values since there's not enough data to calculate.

::: warning ⚞ Convergence warning
The first `10×E` periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods.
:::

### `KamaResult`

| property | type | description |
| -------- | ---- | ----------- |
| `Timestamp` | DateTime | Date from evaluated `TQuote` |
| `ER` | double | Efficiency Ratio is the fractal efficiency of price changes |
| `Kama` | double | Kaufman's adaptive moving average |

More about Efficiency Ratio: ER fluctuates between 0 and 1, but these extremes are the exception, not the norm. ER would be 1 if prices moved up or down consistently over the `erPeriods` window. ER would be zero if prices are unchanged over the `erPeriods` window.

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
    .ToKama(..);
```

Results can be further processed on `Kama` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .ToKama(..)
    .ToRsi(..);
```

## Streaming

Use the buffer-style `List<T>` when you need incremental calculations without a hub:

```csharp
KamaList kamaList = new(erPeriods, fastPeriods, slowPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  kamaList.Add(quote);
}

// based on `ICollection<KamaResult>`
IReadOnlyList<KamaResult> results = kamaList;
```

Subscribe to a `QuoteHub` for advanced streaming scenarios:

```csharp
QuoteHub quoteHub = new();
KamaHub observer = quoteHub.ToKamaHub(erPeriods, fastPeriods, slowPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  quoteHub.Add(quote);
}

IReadOnlyList<KamaResult> results = observer.Results;
```
