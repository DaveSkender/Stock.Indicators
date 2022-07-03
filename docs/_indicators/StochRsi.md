---
title: Stochastic RSI
permalink: /indicators/StochRsi/
type: oscillator
layout: indicator
---

# {{ page.title }}

Created by by Tushar Chande and Stanley Kroll, [Stochastic RSI](https://school.stockcharts.com/doku.php?id=technical_indicators:stochrsi) is a Stochastic interpretation of the Relative Strength Index.  It is different from, and often confused with the more traditional [Stochastic Oscillator]({{site.baseurl}}/indicators/Stoch/#content).
[[Discuss] :speech_balloon:]({{site.github.repository_url}}/discussions/236 "Community discussion about this indicator")

![image]({{site.baseurl}}/assets/charts/StochRsi.png)

```csharp
// usage
IEnumerable<StochRsiResult> results =
  quotes.GetStochRsi(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods);
```

## Parameters

| name | type | notes
| -- |-- |--
| `rsiPeriods` | int | Number of periods (`R`) in the lookback period.  Must be greater than 0.  Standard is 14.
| `stochPeriods` | int | Number of periods (`S`) in the lookback period.  Must be greater than 0.  Typically the same value as `rsiPeriods`.
| `signalPeriods` | int | Number of periods (`G`) in the signal line (SMA of the StochRSI).  Must be greater than 0.  Typically 3-5.
| `smoothPeriods` | int | Smoothing periods (`M`) for the Stochastic.  Must be greater than 0.  Default is 1 (Fast variant).

The original Stochastic RSI formula uses a the Fast variant of the Stochastic calculation (`smoothPeriods=1`).  For a standard period of 14, the original formula would be `quotes.GetStochRSI(14,14,3,1)`.  The "3" here is just for the Signal (%D), which is not present in the original formula, but useful for additional smoothing and analysis.

### Historical quotes requirements

You must have at least `N` periods of `quotes`, where `N` is the greater of `R+S+M` and `R+100` to cover the convergence periods.  Since this uses a smoothing technique in the underlying RSI value, we recommend you use at least `10×R` periods prior to the intended usage date for better precision.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<StochRsiResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `R+S+M` periods will have `null` values for `StochRsi` since there's not enough data to calculate.

:hourglass: **Convergence Warning**: The first `10×R` periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods.  We recommend pruning at least `R+S+M+100` initial values.

### StochRsiResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `StochRsi` | double | %K Oscillator = Stochastic RSI = Stoch(`S`,`G`,`M`) of RSI(`R`) of price
| `Signal` | double | %D Signal Line = Simple moving average of %K based on `G` periods

### Utilities

- [.Condense()]({{site.baseurl}}/utilities#condense)
- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()]({{site.baseurl}}/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and Helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Chaining

Results can be further processed on `StochRsi` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .GetStochRsi(..)
    .GetSlope(..);
```

This indicator must be generated from `quotes` and **cannot** be generated from results of another chain-enabled indicator or method.
