---
title: Kaufman's Adaptive Moving Average (KAMA)
permalink: /indicators/Kama/
type: moving-average
layout: indicator
---

# {{ page.title }}

Created by Perry Kaufman, [KAMA](https://school.stockcharts.com/doku.php?id=technical_indicators:kaufman_s_adaptive_moving_average) is an volatility adaptive moving average of Close price over configurable lookback periods.
[[Discuss] :speech_balloon:]({{site.github.repository_url}}/discussions/210 "Community discussion about this indicator")

![image]({{site.baseurl}}/assets/charts/Kama.png)

```csharp
// usage
IEnumerable<KamaResult> results =
  quotes.GetKama(erPeriods, fastPeriods, slowPeriods);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `erPeriods` | int | Number of Efficiency Ratio (volatility) periods (`E`).  Must be greater than 0.  Default is 10.
| `fastPeriods` | int | Number of Fast EMA periods.  Must be greater than 0.  Default is 2.
| `slowPeriods` | int | Number of Slow EMA periods.  Must be greater than `fastPeriods`.  Default is 30.

### Historical quotes requirements

You must have at least `6×E` or `E+100` periods of `quotes`, whichever is more.  Since this uses a smoothing technique, we recommend you use at least `10×E` data points prior to the intended usage date for better precision.

`quotes` is an `IEnumerable<TQuote>` collection of historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<KamaResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N-1` periods will have `null` values since there's not enough data to calculate.

:hourglass: **Convergence Warning**: The first `10×E` periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods.

### KamaResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `ER`   | decimal | Efficiency Ratio is the fractal efficiency of price changes
| `Kama` | decimal | Kaufman's adaptive moving average

More about Efficiency Ratio: ER fluctuates between 0 and 1, but these extremes are the exception, not the norm. ER would be 1 if prices moved up or down consistently over the `erPeriods` periods. ER would be zero if prices are unchanged over the `erPeriods` periods.

### Utilities

- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()]({{site.baseurl}}/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and Helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Example

```csharp
// fetch historical quotes from your feed (your method)
IEnumerable<Quote> quotes = GetHistoryFromFeed("MSFT");

// calculate KAMA(10,2,30)
IEnumerable<KamaResult> results = quotes.GetKama(10,2,30);
```
