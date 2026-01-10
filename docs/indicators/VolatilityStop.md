---
title: Volatility Stop
description: Created by J. Welles Wilder, Volatility Stop, also known his Volatility System, is an ATR based indicator used to determine trend direction, stops, and reversals.  It is similar to Wilder's Parabolic SAR, SuperTrend, and more contemporary ATR Trailing Stop.
---

# Volatility Stop

Created by J. Welles Wilder, [Volatility Stop](https://archive.org/details/newconceptsintec00wild), also known his Volatility System, is an [ATR](/indicators/Atr) based indicator used to determine trend direction, stops, and reversals.  It is similar to Wilder's [Parabolic SAR](/indicators/ParabolicSar) and [SuperTrend](/indicators/SuperTrend).
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/564 "Community discussion about this indicator")

<ClientOnly>
  <IndicatorChart src="/data/VolatilityStop.json" :height="360" />
</ClientOnly>

```csharp
// C# usage syntax (Series)
IReadOnlyList<VolatilityStopResult> results =
  quotes.ToVolatilityStop(lookbackPeriods, multiplier);

// Usage with QuoteHub (streaming)
QuoteHub quoteHub = new();
VolatilityStopHub observer = quoteHub.ToVolatilityStopHub(lookbackPeriods, multiplier);
```

## Parameters

| param | type | description |
| ----- | ---- | ----------- |
| `lookbackPeriods` | int | Number of periods (`N`) ATR lookback window.  Must be greater than 1.  Default is 7. |
| `multiplier` | double | ATR multiplier for the offset.  Must be greater than 0.  Default is 3.0. |

### Historical quotes requirements

You must have at least `N+100` periods of `quotes` to cover the [warmup and convergence](https://github.com/DaveSkender/Stock.Indicators/discussions/688) periods.  Since the underlying ATR uses a smoothing technique, we recommend you use at least `N+250` data points prior to the intended usage date for better precision.  Initial values prior to the first reversal are not accurate and are excluded from the results.  Therefore, provide sufficient quotes to capture prior trend reversals.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](/guide#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<VolatilityStopResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first trend will have `null` values since it is not accurate and based on an initial guess.

::: warning ⚞ Convergence warning
The first `N+100` periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods.
:::

### `VolatilityStopResult`

| property | type | description |
| -------- | ---- | ----------- |
| `Timestamp` | DateTime | Date from evaluated `TQuote` |
| `Sar` | double | Stop and Reverse value contains both Upper and Lower segments |
| `IsStop` | bool | Indicates a trend reversal |
| `UpperBand` | double | Upper band only (bearish/red) |
| `LowerBand` | double | Lower band only (bullish/green) |

`UpperBand` and `LowerBand` values are provided to differentiate bullish vs bearish trends and to clearly demark trend reversal.  `Sar` is the contiguous combination of both upper and lower line data.

### Utilities

- [.Condense()](/utilities/results#condense)
- [.Find(lookupDate)](/utilities/results#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()](/utilities/results#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)](/utilities/results#remove-warmup-periods)

See [Utilities and helpers](/utilities/results) for more information.

## Chaining

Results can be further processed on `Sar` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .ToVolatilityStop(..)
    .ToEma(..);
```

This indicator must be generated from `quotes` and **cannot** be generated from results of another chain-enabled indicator or method.

## Streaming

Use the buffer-style `List<T>` when you need incremental calculations without a hub:

```csharp
VolatilityStopList volatilityStopList = new(lookbackPeriods, multiplier);

foreach (IQuote quote in quotes)  // simulating stream
{
  volatilityStopList.Add(quote);
}

// based on `ICollection<VolatilityStopResult>`
IReadOnlyList<VolatilityStopResult> results = volatilityStopList;
```

Subscribe to a `QuoteHub` for advanced streaming scenarios:

```csharp
QuoteHub quoteHub = new();
VolatilityStopHub observer = quoteHub.ToVolatilityStopHub(lookbackPeriods, multiplier);

foreach (IQuote quote in quotes)  // simulating stream
{
  quoteHub.Add(quote);
}

IReadOnlyList<VolatilityStopResult> results = observer.Results;
```
