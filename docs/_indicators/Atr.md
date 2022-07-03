---
title: Average True Range (ATR)
permalink: /indicators/Atr/
type: price-characteristic
layout: indicator
---

# {{ page.title }}

Created by J. Welles Wilder, [Average True Range](https://en.wikipedia.org/wiki/Average_true_range) is a measure of volatility that captures gaps and limits between periods.
[[Discuss] :speech_balloon:]({{site.github.repository_url}}/discussions/269 "Community discussion about this indicator")

![image]({{site.baseurl}}/assets/charts/Atr.png)

```csharp
// usage
IEnumerable<AtrResult> results =
  quotes.GetAtr(lookbackPeriods);
```

## Parameters

| name | type | notes
| -- |-- |--
| `lookbackPeriods` | int | Number of periods (`N`) to consider.  Must be greater than 1.

### Historical quotes requirements

You must have at least `N+100` periods of `quotes` to cover the convergence periods.  Since this uses a smoothing technique, we recommend you use at least `N+250` data points prior to the intended usage date for better precision.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<AtrResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N-1` periods will have `null` values for ATR since there's not enough data to calculate.

:hourglass: **Convergence Warning**: The first `N+100` periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods.

### AtrResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Tr` | double | True Range for current period
| `Atr` | double | Average True Range for `N` lookback periods
| `Atrp` | double | Average True Range Percent is `(ATR/Price)*100`.  This normalizes so it can be compared to other stocks.

### Utilities

- [.Condense()]({{site.baseurl}}/utilities#condense)
- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()]({{site.baseurl}}/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and Helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Chaining

Results can be further processed on `Atrp` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .GetAtr(..)
    .GetSlope(..);
```

This indicator must be generated from `quotes` and **cannot** be generated from results of another chain-enabled indicator or method.
