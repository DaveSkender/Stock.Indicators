---
title: Moving Average Convergence/Divergence (MACD)
permalink: /indicators/Macd/
type: price-trend
layout: indicator
---

# {{ page.title }}

Created by Gerald Appel, [MACD](https://en.wikipedia.org/wiki/MACD) is a simple oscillator view of two converging/diverging exponential moving averages.
[[Discuss] :speech_balloon:]({{site.github.repository_url}}/discussions/248 "Community discussion about this indicator")

![image]({{site.baseurl}}/assets/charts/Macd.png)

```csharp
// usage
IEnumerable<MacdResult> results =
  quotes.GetMacd(fastPeriods, slowPeriods, signalPeriods);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `fastPeriods` | int | Number of periods (`F`) for the faster moving average.  Must be greater than 0.  Default is 12.
| `slowPeriods` | int | Number of periods (`S`) for the slower moving average.  Must be greater than `fastPeriods`.  Default is 26.
| `signalPeriods` | int | Number of periods (`P`) for the moving average of MACD.  Must be greater than or equal to 0.  Default is 9.

### Historical quotes requirements

You must have at least `2×(S+P)` or `S+P+100` worth of `quotes`, whichever is more.  Since this uses a smoothing technique, we recommend you use at least `S+P+250` data points prior to the intended usage date for better precision.

`quotes` is an `IEnumerable<TQuote>` collection of historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<MacdResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `S-1` slow periods will have `null` values since there's not enough data to calculate.

:hourglass: **Convergence Warning**: The first `S+P+250` periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods.

### MacdResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Macd` | decimal | The MACD line is the difference between slow and fast moving averages (`MACD = FastEma - SlowEma`)
| `Signal` | decimal | Moving average of the `MACD` line
| `Histogram` | decimal | Gap between of the `MACD` and `Signal` line
| `FastEma` | decimal | Fast Exponential Moving Average
| `SlowEma` | decimal | Slow Exponential Moving Average

### Utilities

- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()]({{site.baseurl}}/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and Helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Example

```csharp
// fetch historical quotes from your feed (your method)
IEnumerable<Quote> quotes = GetHistoryFromFeed("SPY");

// calculate MACD(12,26,9)
IEnumerable<MacdResult> results = quotes.GetMacd(12,26,9);
```
