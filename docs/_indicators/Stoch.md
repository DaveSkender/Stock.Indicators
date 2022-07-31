---
title: Stochastic Oscillator
description: Stochastic Oscillator and KDJ Index
permalink: /indicators/Stoch/
type: oscillator
layout: indicator
redirect_from:
 - /indicators/Stochastic/README.md
---

# {{ page.title }}

Created by George Lane, the [Stochastic Oscillator](https://en.wikipedia.org/wiki/Stochastic_oscillator) is a momentum indicator that looks back `N` periods to produce a scale of 0 to 100.  %J is also included for the KDJ Index extension.
[[Discuss] :speech_balloon:]({{site.github.repository_url}}/discussions/237 "Community discussion about this indicator")

![image]({{site.baseurl}}/assets/charts/Stoch.png)

```csharp
// usage (standard)
IEnumerable<StochResult> results =
  quotes.GetStoch(lookbackPeriods, signalPeriods, smoothPeriods);

// advanced customization
IEnumerable<StochResult> results =
  quotes.GetStoch(lookbackPeriods, signalPeriods, smoothPeriods,
                  kFactor, dFactor, movingAverageType);
```

## Parameters

| name | type | notes
| -- |-- |--
| `lookbackPeriods` | int | Lookback period (`N`) for the oscillator (%K).  Must be greater than 0.  Default is 14.
| `signalPeriods` | int | Smoothing period for the signal (%D).  Must be greater than 0.  Default is 3.
| `smoothPeriods` | int | Smoothing period (`S`) for the Oscillator (%K).  "Slow" stochastic uses 3, "Fast" stochastic uses 1.  Must be greater than 0.  Default is 3.
| `kFactor` | double | Optional. Weight of %K in the %J calculation.  Must be greater than 0. Default is 3.
| `dFactor` | double | Optional. Weight of %D in the %J calculation.  Must be greater than 0. Default is 2.
| `movingAverageType` | MaType | Optional. Type of moving average (SMA or SMMA) used for smoothing.  See [MaType options](#matype-options) below.  Default is `MaType.SMA`.

### Historical quotes requirements

You must have at least `N+S` periods of `quotes` to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

### MaType options

These are the supported moving average types:

| type | description
|-- |--
| `MaType.SMA` | [Simple Moving Average]({{site.baseurl}}/indicators/Sma/#content) (default)
| `MaType.SMMA` | [Smoothed Moving Average]({{site.baseurl}}/indicators/Smma/#content)

## Response

```csharp
IEnumerable<StochResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N+S-2` periods will have `null` Oscillator values since there's not enough data to calculate.

:hourglass: **Convergence Warning**: The first `N+100` periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods when using `MaType.SMMA`.  Standard use of `MaType.SMA` does not have convergence-related precision errors.

### StochResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Oscillator` or `K` | double | %K Oscillator over prior `N` lookback periods
| `Signal` or `D` | double | %D Simple moving average of Oscillator
| `PercentJ` or `J` | double | %J is the weighted divergence of %K and %D: `%J=kFactor×%K-dFactor×%D`

Note: aliases of `K`, `D`, and `J` are also provided.  They can be used interchangeably with the standard outputs.

### Utilities

- [.Condense()]({{site.baseurl}}/utilities#condense)
- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()]({{site.baseurl}}/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and Helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Chaining

Results can be further processed on `Oscillator` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .GetStoch(..)
    .GetSlope(..);
```

This indicator must be generated from `quotes` and **cannot** be generated from results of another chain-enabled indicator or method.
