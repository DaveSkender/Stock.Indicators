---
title: Double Exponential Moving Average (DEMA)
permalink: /indicators/DoubleEma/
type: moving-average
layout: indicator
---

# {{ page.title }}

[Double exponential moving average](https://en.wikipedia.org/wiki/Double_exponential_moving_average) of the Close price over a lookback window.
[[Discuss] :speech_balloon:]({{site.github.repository_url}}/discussions/256 "Community discussion about this indicator")

![image]({{site.baseurl}}/assets/charts/DoubleEma.png)

DEMA is shown as the dashed line above.  [EMA](../Ema#content) (solid line) and [Triple EMA](../TripleEma#content) (dotted line) are also shown here for comparison.

```csharp
// usage
IEnumerable<DemaResult> results =
  quotes.GetDoubleEma(lookbackPeriods);
```

## Parameters

| name | type | notes
| -- |-- |--
| `lookbackPeriods` | int | Number of periods (`N`) in the moving average.  Must be greater than 0.

### Historical quotes requirements

You must have at least `3×N` or `2×N+100` periods of `quotes`, whichever is more.  Since this uses a smoothing technique, we recommend you use at least `2×N+250` data points prior to the intended usage date for better precision.

`quotes` is an `IEnumerable<TQuote>` collection of historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<DemaResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `2×N-1` periods will have `null` values since there's not enough data to calculate.

:hourglass: **Convergence Warning**: The first `2×N+100` periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods.

### DemaResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Dema` | decimal | Double exponential moving average

### Utilities

- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()]({{site.baseurl}}/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and Helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Example

```csharp
// fetch historical quotes from your feed (your method)
IEnumerable<Quote> quotes = GetHistoryFromFeed("SPY");

// calculate 20-period DEMA
IEnumerable<DemaResult> results = quotes.GetDoubleEma(20);
```
