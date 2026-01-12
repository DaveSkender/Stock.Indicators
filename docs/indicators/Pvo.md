---
title: Percentage Volume Oscillator (PVO)
description: The Percentage Volume Oscillator is a simple oscillator view of the rate of change between two converging / diverging exponential moving averages of Volume.  It is presented similarly to MACD.
---

# Percentage Volume Oscillator (PVO)

The [Percentage Volume Oscillator](https://school.stockcharts.com/doku.php?id=technical_indicators:percentage_volume_oscillator_pvo) is a simple oscillator view of the rate of change between two converging / diverging exponential moving averages of Volume.
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/305 "Community discussion about this indicator")

<ClientOnly>
  <IndicatorChart src="/data/Pvo.json" :height="360" />
</ClientOnly>

```csharp
// C# usage syntax
IReadOnlyList<PvoResult> results =
  quotes.ToPvo(fastPeriods, slowPeriods, signalPeriods);
```

## Parameters

| param | type | description |
| ----- | ---- | ----------- |
| `fastPeriods` | int | Number of periods (`F`) for the faster moving average.  Must be greater than 0.  Default is 12. |
| `slowPeriods` | int | Number of periods (`S`) for the slower moving average.  Must be greater than `fastPeriods`.  Default is 26. |
| `signalPeriods` | int | Number of periods (`P`) for the moving average of PVO.  Must be greater than or equal to 0.  Default is 9. |

### Historical quotes requirements

You must have at least `2×(S+P)` or `S+P+100` worth of `quotes`, whichever is more, to cover the [warmup and convergence](https://github.com/DaveSkender/Stock.Indicators/discussions/688) periods.  Since this uses a smoothing technique, we recommend you use at least `S+P+250` data points prior to the intended usage date for better precision.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](/guide#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<PvoResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `S-1` slow periods will have `null` values since there's not enough data to calculate.

::: warning ⚞ Convergence warning
The first `S+P+250` periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods.
:::

### `PvoResult`

| property | type | description |
| -------- | ---- | ----------- |
| `Timestamp` | DateTime | Date from evaluated `TQuote` |
| `Pvo` | double | Normalized difference between two Volume moving averages |
| `Signal` | double | Moving average of the `Pvo` line |
| `Histogram` | double | Gap between the `Pvo` and `Signal` line |

### Utilities

- [.Condense()](/utilities/results#condense)
- [.Find(lookupDate)](/utilities/results#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()](/utilities/results#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)](/utilities/results#remove-warmup-periods)

See [Utilities and helpers](/utilities/results) for more information.

## Chaining

Results can be further processed on `Pvo` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .ToPvo(..)
    .ToSlope(..);
```

This indicator must be generated from `quotes` and **cannot** be generated from results of another chain-enabled indicator or method.

## Streaming

Use the buffer-style `List<T>` when you need incremental calculations without a hub:

```csharp
PvoList pvoList = new(fastPeriods, slowPeriods, signalPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  pvoList.Add(quote);
}

// based on `ICollection<PvoResult>`
IReadOnlyList<PvoResult> results = pvoList;
```

Subscribe to a `QuoteHub` for advanced streaming scenarios:

```csharp
QuoteHub quoteHub = new();
PvoHub observer = quoteHub.ToPvoHub(fastPeriods, slowPeriods, signalPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  quoteHub.Add(quote);
}

IReadOnlyList<PvoResult> results = observer.Results;
```
