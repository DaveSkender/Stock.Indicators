---
title: Volatility Stop
permalink: /indicators/VolatilityStop/
type: stop-and-reverse
layout: indicator
---

# {{ page.title }}

Created by J. Welles Wilder, [Volatility Stop](https://archive.org/details/newconceptsintec00wild), also known his Volatility System, is an [ATR]({{site.baseurl}}/indicators/Atr/#content) based indicator used to determine trend direction, stops, and reversals.  It is similar to Wilder's [Parabolic SAR]({{site.baseurl}}/indicators/ParabolicSar/#content) and [SuperTrend]({{site.baseurl}}/indicators/SuperTrend/#content).
[[Discuss] :speech_balloon:]({{site.github.repository_url}}/discussions/564 "Community discussion about this indicator")

![image]({{site.baseurl}}/assets/charts/VolatilityStop.png)

```csharp
// usage
IEnumerable<VolatilityStopResult> results =
  quotes.GetVolatilityStop(lookbackPeriods, multiplier);
```

## Parameters

| name | type | notes
| -- |-- |--
| `lookbackPeriods` | int | Number of periods (`N`) ATR lookback window.  Must be greater than 1.  Default is 7.
| `multiplier` | double | ATR multiplier for the offset.  Must be greater than 0.  Default is 3.0.

### Historical quotes requirements

You must have at least `N+100` periods of `quotes` to cover the convergence periods.  Since the underlying ATR uses a smoothing technique, we recommend you use at least `N+250` data points prior to the intended usage date for better precision.  Initial values prior to the first reversal are not accurate and are excluded from the results.  Therefore, provide sufficient quotes to capture prior trend reversals.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<VolatilityStopResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first trend will have `null` values since it is not accurate and based on an initial guess.

:hourglass: **Convergence Warning**: The first `N+100` periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods.

### VolatilityStopResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Sar` | double | Stop and Reverse value contains both Upper and Lower segments
| `IsStop` | bool | Indicates a trend reversal
| `UpperBand` | double | Upper band only (bearish/red)
| `LowerBand` | double | Lower band only (bullish/green)

`UpperBand` and `LowerBand` values are provided to differentiate bullish vs bearish trends and to clearly demark trend reversal.  `Sar` is the contiguous combination of both upper and lower line data.

### Utilities

- [.Condense()]({{site.baseurl}}/utilities#condense)
- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()]({{site.baseurl}}/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and Helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Chaining

Results can be further processed on `Sar` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .GetVolatilityStop(..)
    .GetEma(..);
```

This indicator must be generated from `quotes` and **cannot** be generated from results of another chain-enabled indicator or method.
