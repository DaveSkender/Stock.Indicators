---
title: Stochastic Momentum Index (SMI)
description: Created by William Blau, the Stochastic Momentum Index (SMI) oscillator is a double-smoothed variant of the traditional Stochastic Oscillator, depicted on a scale from -100 to 100.
---

# Stochastic Momentum Index (SMI)

Created by William Blau, the Stochastic Momentum Index (SMI) oscillator is a double-smoothed variant of the [Stochastic Oscillator](/indicators/Stoch), depicted on a scale from -100 to 100.
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/625 "Community discussion about this indicator")

<ClientOnly>
  <IndicatorChart src="/data/Smi.json" :height="360" />
</ClientOnly>

```csharp
// C# usage syntax (standard)
IReadOnlyList<SmiResult> results =
  quotes.ToSmi(lookbackPeriods, firstSmoothPeriods,
                 secondSmoothPeriods, signalPeriods);
```

## Parameters

| param | type | description |
| ----- | ---- | ----------- |
| `lookbackPeriods` | int | Lookback period (`N`) for the stochastic.  Must be greater than 0.  Default is 13. |
| `firstSmoothPeriods` | int | First smoothing factor lookback.  Must be greater than 0.  Default is 25. |
| `secondSmoothPeriods` | int | Second smoothing factor lookback.  Must be greater than 0.  Default is 2. |
| `signalPeriods` | int | EMA of SMI lookback periods.  Must be greater than 0. Default is 3. |

### Historical quotes requirements

You must have at least `N+100` periods of `quotes` to cover the [warmup and convergence](https://github.com/DaveSkender/Stock.Indicators/discussions/688) periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](/guide#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<SmiResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N-1` periods will have `null` SMI values since there's not enough data to calculate.

::: warning ⚞ Convergence warning
The first `N+100` periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods.
:::

### `SmiResult`

| property | type | description |
| -------- | ---- | ----------- |
| `Timestamp` | DateTime | Date from evaluated `TQuote` |
| `Smi` | double | Stochastic Momentum Index (SMI) |
| `Signal` | double | Signal line: an Exponential Moving Average (EMA) of SMI |

### Utilities

- [.Condense()](/utilities/results#condense)
- [.Find(lookupDate)](/utilities/results#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()](/utilities/results#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)](/utilities/results#remove-warmup-periods)

See [Utilities and helpers](/utilities/results) for more information.

## Chaining

Results can be further processed on `Smi` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .ToSmi(..)
    .ToSlope(..);
```

This indicator must be generated from `quotes` and **cannot** be generated from results of another chain-enabled indicator or method.

## Streaming

Use the buffer-style `List<T>` when you need incremental calculations without a hub:

```csharp
SmiList smiList = new(lookbackPeriods, firstSmoothPeriods,
                 secondSmoothPeriods, signalPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  smiList.Add(quote);
}

// based on `ICollection<SmiResult>`
IReadOnlyList<SmiResult> results = smiList;
```

Subscribe to a `QuoteHub` for advanced streaming scenarios:

```csharp
QuoteHub quoteHub = new();
SmiHub observer = quoteHub.ToSmiHub(lookbackPeriods, firstSmoothPeriods,
                 secondSmoothPeriods, signalPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  quoteHub.Add(quote);
}

IReadOnlyList<SmiResult> results = observer.Results;
```
