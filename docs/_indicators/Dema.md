---
title: Double Exponential Moving Average (DEMA)
permalink: /indicators/Dema/
type: moving-average
layout: indicator
redirect_from:
 - /indicators/DoubleEma/
---

# {{ page.title }}

Created by Patrick G. Mulloy, the [Double exponential moving average](https://en.wikipedia.org/wiki/Double_exponential_moving_average) is a faster smoothed EMA of the price over a lookback window.
[[Discuss] :speech_balloon:]({{site.github.repository_url}}/discussions/807 "Community discussion about this indicator")

![image]({{site.baseurl}}/assets/charts/Dema.png)

See also related [EMA]({{site.baseurl}}/indicators/Ema/#content) and [Triple EMA]({{site.baseurl}}/indicators/Tema/#content).

```csharp
// usage
IEnumerable<DemaResult> results =
  quotes.GetDema(lookbackPeriods);
```

## Parameters

| name | type | notes
| -- |-- |--
| `lookbackPeriods` | int | Number of periods (`N`) in the moving average.  Must be greater than 0.

### Historical quotes requirements

You must have at least `3×N` or `2×N+100` periods of `quotes`, whichever is more, to cover the convergence periods.  Since this uses a smoothing technique, we recommend you use at least `2×N+250` data points prior to the intended usage date for better precision.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<DemaResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N-1` periods will have `null` values since there's not enough data to calculate.

:hourglass: **Convergence Warning**: The first `2×N+100` periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods.

### DemaResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Dema` | double | Double exponential moving average

### Utilities

- [.Condense()]({{site.baseurl}}/utilities#condense)
- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()]({{site.baseurl}}/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and Helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Chaining

This indicator may be generated from any chain-enabled indicator or method.

```csharp
// example
var results = quotes
    .Use(CandlePart.HL2)
    .GetDema(..);
```

Results can be further processed on `Dema` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .GetDema(..)
    .GetRsi(..);
```
