---
title: Klinger Volume Oscillator
description: Created by Stephen Klinger, the Klinger Volume Oscillator depicts volume-based trend reversal and divergence between short and long-term money flow.
---

# Klinger Volume Oscillator

Created by Stephen Klinger, the [Klinger Volume Oscillator](https://www.investopedia.com/terms/k/klingeroscillator.asp) depicts volume-based trend reversal and divergence between short and long-term money flow.
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/446 "Community discussion about this indicator")

<ClientOnly>
  <IndicatorChart src="/data/Kvo.json" :height="360" />
</ClientOnly>

```csharp
// C# usage syntax
IReadOnlyList<KvoResult> results =
  quotes.ToKvo(shortPeriods, longPeriods, signalPeriods);
```

## Parameters

| param | type | description |
| ----- | ---- | ----------- |
| `fastPeriods` | int | Number of lookback periods (`F`) for the short-term EMA.  Must be greater than 2.  Default is 34. |
| `slowPeriods` | int | Number of lookback periods (`L`) for the long-term EMA.  Must be greater than `F`.  Default is 55. |
| `signalPeriods` | int | Number of lookback periods for the signal line.  Must be greater than 0.  Default is 13. |

### Historical quotes requirements

You must have at least `L+100` periods of `quotes` to cover the [warmup and convergence](https://github.com/DaveSkender/Stock.Indicators/discussions/688) periods.  Since this uses a smoothing technique, we recommend you use at least `L+150` data points prior to the intended usage date for better precision.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](/guide#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<KvoResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `L+1` periods will have `null` values since there's not enough data to calculate.

::: warning ⚞ Convergence warning
The first `L+150` periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods.
:::

### `KvoResult`

| property | type | description |
| -------- | ---- | ----------- |
| `Timestamp` | DateTime | Date from evaluated `TQuote` |
| `Oscillator` | double | Klinger Oscillator |
| `Signal` | double | EMA of Klinger Oscillator (signal line) |

### Utilities

- [.Condense()](/utilities/results#condense)
- [.Find(lookupDate)](/utilities/results#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()](/utilities/results#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)](/utilities/results#remove-warmup-periods)

See [Utilities and helpers](/utilities/results) for more information.

## Chaining

Results can be further processed on `Kvo` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .ToKvo(..)
    .ToSlope(..);
```

This indicator must be generated from `quotes` and **cannot** be generated from results of another chain-enabled indicator or method.

## Streaming

Use the buffer-style `List<T>` when you need incremental calculations without a hub:

```csharp
KvoList kvoList = new(34, 55, 13);

foreach (IQuote quote in quotes)  // simulating stream
{
  kvoList.Add(quote);
}

// based on `ICollection<KvoResult>`
IReadOnlyList<KvoResult> results = kvoList;
```

Subscribe to a `QuoteHub` for advanced streaming scenarios:

```csharp
QuoteHub quoteHub = new();
KvoHub observer = quoteHub.ToKvoHub(34, 55, 13);

foreach (IQuote quote in quotes)  // simulating stream
{
  quoteHub.Add(quote);
}

IReadOnlyList<KvoResult> results = observer.Results;
```
