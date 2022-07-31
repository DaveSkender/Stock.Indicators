---
title: Stochastic Momentum Index (SMI)
permalink: /indicators/Smi/
type: oscillator
layout: indicator
---

# {{ page.title }}

Created by William Blau, the Stochastic Momentum Index (SMI) is a double-smoothed variant of the [Stochastic Oscillator]({{site.baseurl}}/indicators/Stoch/#content) on a scale from -100 to 100.
[[Discuss] :speech_balloon:]({{site.github.repository_url}}/discussions/625 "Community discussion about this indicator")

![image]({{site.baseurl}}/assets/charts/Smi.png)

```csharp
// usage (standard)
IEnumerable<SmiResult> results =
  quotes.GetSmi(lookbackPeriods, firstSmoothPeriods,
                 secondSmoothPeriods, signalPeriods);
```

## Parameters

| name | type | notes
| -- |-- |--
| `lookbackPeriods` | int | Lookback period (`N`) for the stochastic.  Must be greater than 0.
| `firstSmoothPeriods` | int | First smoothing factor lookback.  Must be greater than 0.
| `secondSmoothPeriods` | int | Second smoothing factor lookback.  Must be greater than 0.
| `signalPeriods` | int | EMA of SMI lookback periods.  Must be greater than 0. Default is 3.

### Historical quotes requirements

You must have at least `N+100` periods of `quotes` to cover the convergence periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<SmiResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N-1` periods will have `null` SMI values since there's not enough data to calculate.

:hourglass: **Convergence Warning**: The first `N+100` periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods.

### SmiResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Smi` | double | Stochastic Momentum Index (SMI)
| `Signal` | double | Signal line: an Exponential Moving Average (EMA) of SMI

### Utilities

- [.Condense()]({{site.baseurl}}/utilities#condense)
- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()]({{site.baseurl}}/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and Helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Chaining

Results can be further processed on `Smi` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .GetSmi(..)
    .GetSlope(..);
```

This indicator must be generated from `quotes` and **cannot** be generated from results of another chain-enabled indicator or method.
