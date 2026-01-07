---
title: Stochastic Oscillator
description: Created by George Lane, the Stochastic Oscillator, also known as KDJ Index, is a momentum oscillator that compares current financial market price with recent highs and lows and is presented on a scale of 0 to 100.  %J is also included for the KDJ Index extension.
---

# Stochastic Oscillator

Created by George Lane, the [Stochastic Oscillator](https://en.wikipedia.org/wiki/Stochastic_oscillator), also known as KDJ Index, is a momentum oscillator that compares current price with recent highs and lows and is presented on a scale of 0 to 100.
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/237 "Community discussion about this indicator")

<ClientOnly>
  <IndicatorChart src="/data/Stoch.json" :height="360" />
</ClientOnly>

```csharp
// C# usage syntax (standard)
IReadOnlyList<StochResult> results =
  quotes.ToStoch(lookbackPeriods, signalPeriods, smoothPeriods);

// advanced customization
IReadOnlyList<StochResult> results =
  quotes.ToStoch(lookbackPeriods, signalPeriods, smoothPeriods,
                  kFactor, dFactor, movingAverageType);
```

## Parameters

| param | type | description |
| ----- | ---- | ----------- |
| `lookbackPeriods` | int | Lookback period (`N`) for the oscillator (%K).  Must be greater than 0.  Default is 14. |
| `signalPeriods` | int | Smoothing period for the signal (%D).  Must be greater than 0.  Default is 3. |
| `smoothPeriods` | int | Smoothing period (`S`) for the Oscillator (%K).  "Slow" stochastic uses 3, "Fast" stochastic uses 1.  Must be greater than 0.  Default is 3. |
| `kFactor` | double | Optional. Weight of %K in the %J calculation.  Must be greater than 0. Default is 3. |
| `dFactor` | double | Optional. Weight of %D in the %J calculation.  Must be greater than 0. Default is 2. |
| `movingAverageType` | MaType | Optional. Type of moving average (SMA or SMMA) used for smoothing.  See [MaType options](#matype-options) below.  Default is `MaType.SMA`. |

### Historical quotes requirements

You must have at least `N+S` periods of `quotes` to cover the [warmup and convergence](https://github.com/DaveSkender/Stock.Indicators/discussions/688) periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](/guide#historical-quotes) for more information.

### MaType options

These are the supported moving average types:

**`MaType.SMA`** - [Simple Moving Average](/indicators/Sma) (default)

**`MaType.SMMA`** - [Smoothed Moving Average](/indicators/Smma)

## Response

```csharp
IReadOnlyList<StochResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N+S-2` periods will have `null` Oscillator values since there's not enough data to calculate.

::: warning ⚞ Convergence warning
The first `N+100` periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods when using `MaType.SMMA`.  Standard use of `MaType.SMA` does not have convergence-related precision errors.
:::

### `StochResult`

| property | type | description |
| -------- | ---- | ----------- |
| `Timestamp` | DateTime | Date from evaluated `TQuote` |
| `Oscillator` or `K` | double | %K Oscillator |
| `Signal` or `D` | double | %D Simple moving average of Oscillator |
| `PercentJ` or `J` | double | %J is the weighted divergence of %K and %D: `%J = kFactor × %K - dFactor × %D` |

Note: aliases of `K`, `D`, and `J` are also provided.  They can be used interchangeably with the standard outputs.

### Utilities

- [.Condense()](/utilities/results#condense)
- [.Find(lookupDate)](/utilities/results#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()](/utilities/results#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)](/utilities/results#remove-warmup-periods)

See [Utilities and helpers](/utilities/results) for more information.

## Chaining

Results can be further processed on `Oscillator` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .ToStoch(..)
    .ToSlope(..);
```

This indicator must be generated from `quotes` and **cannot** be generated from results of another chain-enabled indicator or method.

## Streaming

Use the buffer-style `List<T>` when you need incremental calculations without a hub:

```csharp
StochList stochList = new(lookbackPeriods, signalPeriods, smoothPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  stochList.Add(quote);
}

// based on `ICollection<StochResult>`
IReadOnlyList<StochResult> results = stochList;
```

Subscribe to a `QuoteHub` for advanced streaming scenarios:

```csharp
QuoteHub quoteHub = new();
StochHub observer = quoteHub.ToStochHub(lookbackPeriods, signalPeriods, smoothPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  quoteHub.Add(quote);
}

IReadOnlyList<StochResult> results = observer.Results;
```
