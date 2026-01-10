---
title: True Strength Index (TSI)
description: Created by William Blau, the True Strength Index is a momentum oscillator that uses a series of exponential moving averages to depicts trends in price changes.
---

# True Strength Index (TSI)

Created by William Blau, the [True Strength Index](https://en.wikipedia.org/wiki/True_strength_index) is a momentum oscillator that uses a series of exponential moving averages to depicts trends in price changes.
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/300 "Community discussion about this indicator")

<ClientOnly>
  <IndicatorChart src="/data/Tsi.json" :height="360" />
</ClientOnly>

```csharp
// C# usage syntax
IReadOnlyList<TsiResult> results =
  quotes.ToTsi(lookbackPeriods, smoothPeriods, signalPeriods);
```

## Parameters

| param | type | description |
| ----- | ---- | ----------- |
| `lookbackPeriods` | int | Number of periods (`N`) for the first EMA.  Must be greater than 0.  Default is 25. |
| `smoothPeriods` | int | Number of periods (`M`) for the second smoothing.  Must be greater than 0.  Default is 13. |
| `signalPeriods` | int | Number of periods (`S`) in the TSI moving average.  Must be greater than or equal to 0.  Default is 7. |

### Historical quotes requirements

You must have at least `N+M+100` periods of `quotes` to cover the [warmup and convergence](https://github.com/DaveSkender/Stock.Indicators/discussions/688) periods.  Since this uses a two-stage EMA smoothing technique, we recommend you use at least `N+M+250` data points prior to the intended usage date for better precision.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](/guide#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<TsiResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N+M-1` periods will have `null` values since there's not enough data to calculate.
- `Signal` will be `null` for all periods if `signalPeriods=0`.

::: warning ⚞ Convergence warning
The first `N+M+250` periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods.
:::

### `TsiResult`

| property | type | description |
| -------- | ---- | ----------- |
| `Timestamp` | DateTime | Date from evaluated `TQuote` |
| `Tsi` | double | True Strength Index |
| `Signal` | double | Signal line (EMA of TSI) |

### Utilities

- [.Condense()](/utilities/results#condense)
- [.Find(lookupDate)](/utilities/results#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()](/utilities/results#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)](/utilities/results#remove-warmup-periods)

See [Utilities and helpers](/utilities/results) for more information.

## Streaming

This indicator can be used with the buffer style for incremental streaming scenarios.  See [Streaming guide](/guide) for more information.

```csharp
// buffer-style streaming
TsiList buffer = new(lookbackPeriods, smoothPeriods, signalPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
    buffer.Add(quote);
    TsiResult result = buffer[^1];
}

// or initialize with historical quotes
TsiList buffer = quotes.ToTsiList(lookbackPeriods, smoothPeriods, signalPeriods);
```

## Chaining

This indicator may be generated from any chain-enabled indicator or method.

```csharp
// example
var results = quotes
    .Use(CandlePart.HL2)
    .ToTsi(..);
```

Results can be further processed on `Tsi` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .ToTsi(..)
    .ToSlope(..);
```
