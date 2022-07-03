---
title: Average Directional Index (ADX)
permalink: /indicators/Adx/
type: price-trend
layout: indicator
---

# {{ page.title }}

Created by J. Welles Wilder, the [Average Directional Movement Index](https://en.wikipedia.org/wiki/Average_directional_movement_index) is a measure of price directional movement.  It includes upward and downward indicators, and is often used to measure strength of trend.
[[Discuss] :speech_balloon:]({{site.github.repository_url}}/discussions/270 "Community discussion about this indicator")

![image]({{site.baseurl}}/assets/charts/AdIndex.png)

```csharp
// usage
IEnumerable<AdxResult> results =
  quotes.GetAdx(lookbackPeriods);
```

## Parameters

| name | type | notes
| -- |-- |--
| `lookbackPeriods` | int | Number of periods (`N`) to consider.  Must be greater than 1.  Default is 14.

### Historical quotes requirements

You must have at least `2×N+100` periods of `quotes` to allow for smoothing convergence.  We generally recommend you use at least `2×N+250` data points prior to the intended usage date for better precision.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<AdxResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `2×N-1` periods will have `null` values for `Adx` since there's not enough data to calculate.

:hourglass: **Convergence Warning**: The first `2×N+100` periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods.

### AdxResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Pdi` | double | Plus Directional Index (+DI) for `N` lookback periods
| `Mdi` | double | Minus Directional Index (-DI) for `N` lookback periods
| `Adx` | double | Average Directional Index (ADX) for `N` lookback periods
| `Adxr` | double | Average Directional Index Rating (ADXR) for `N` lookback periods

### Utilities

- [.Condense()]({{site.baseurl}}/utilities#condense)
- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()]({{site.baseurl}}/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and Helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Chaining

Results can be further processed on `Adx` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .GetAdx(..)
    .GetRsi(..);
```

This indicator must be generated from `quotes` and **cannot** be generated from results of another chain-enabled indicator or method.
