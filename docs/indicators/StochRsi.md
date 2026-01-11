---
title: Stochastic RSI
description: Created by Tushar Chande and Stanley Kroll, Stochastic RSI is a Stochastic Oscillator interpretation of the Relative Strength Index.  It is different from, and often confused with the more traditional Stochastic Oscillator.
---

# Stochastic RSI

Created by Tushar Chande and Stanley Kroll, [Stochastic RSI](https://school.stockcharts.com/doku.php?id=technical_indicators:stochrsi) is a Stochastic interpretation of the Relative Strength Index.  It is different from, and often confused with the more traditional [Stochastic Oscillator](/indicators/Stoch).
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/236 "Community discussion about this indicator")

<ClientOnly>
  <IndicatorChart src="/data/StochRsi.json" :height="360" />
</ClientOnly>

```csharp
// C# usage syntax
IReadOnlyList<StochRsiResult> results =
  quotes.ToStochRsi(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods);
```

## Parameters

| param | type | description |
| ----- | ---- | ----------- |
| `rsiPeriods` | int | Number of periods (`R`) in the lookback period.  Must be greater than 0.  Standard is 14. |
| `stochPeriods` | int | Number of periods (`S`) in the lookback period.  Must be greater than 0.  Typically the same value as `rsiPeriods`. |
| `signalPeriods` | int | Number of periods (`G`) in the signal line (SMA of the StochRSI).  Must be greater than 0.  Typically 3-5. |
| `smoothPeriods` | int | Smoothing periods (`M`) for the Stochastic.  Must be greater than 0.  Default is 1 (Fast variant). |

The original Stochastic RSI formula uses a the Fast variant of the Stochastic calculation (`smoothPeriods=1`).  For a standard period of 14, the original formula would be `quotes.ToStochRSI(14,14,3,1)`.  The "3" here is just for the Signal (%D), which is not present in the original formula, but useful for additional smoothing and analysis.

### Historical quotes requirements

You must have at least `N` periods of `quotes`, where `N` is the greater of `R+S+M` and `R+100` to cover the [warmup and convergence](https://github.com/DaveSkender/Stock.Indicators/discussions/688) periods.  Since this uses a smoothing technique in the underlying RSI value, we recommend you use at least `10×R` periods prior to the intended usage date for better precision.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](/guide#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<StochRsiResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `R+S+M` periods will have `null` values for `StochRsi` since there's not enough data to calculate.

::: warning ⚞ Convergence warning
The first `10×R` periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods.  We recommend pruning at least `R+S+M+100` initial values.
:::

### `StochRsiResult`

| property | type | description |
| -------- | ---- | ----------- |
| `Timestamp` | DateTime | Date from evaluated `TQuote` |
| `StochRsi` | double | %K Oscillator = Stochastic RSI = Stoch(`S`,`G`,`M`) of RSI(`R`) of price |
| `Signal` | double | %D Signal Line = Simple moving average of %K based on `G` periods |

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
    .ToStochRsi(..);
```

Results can be further processed on `StochRsi` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .ToStochRsi(..)
    .ToSlope(..);
```

## Streaming

Use the buffer-style `List<T>` when you need incremental calculations without a hub:

```csharp
StochRsiList stochRsiList = new(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  stochRsiList.Add(quote);
}

// based on `ICollection<StochRsiResult>`
IReadOnlyList<StochRsiResult> results = stochRsiList;
```

Subscribe to a `QuoteHub` for advanced streaming scenarios:

```csharp
QuoteHub quoteHub = new();
StochRsiHub observer = quoteHub.ToStochRsiHub(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  quoteHub.Add(quote);
}

IReadOnlyList<StochRsiResult> results = observer.Results;
```
