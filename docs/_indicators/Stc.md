---
title: Schaff Trend Cycle
permalink: /indicators/Stc/
layout: default
---

# {{ page.title }}

Created by TBD, [Schaff Trend Cycle](https://www.investopedia.com/articles/forex/10/schaff-trend-cycle-indicator.asp) is TBD
[[Discuss] :speech_balloon:]({{site.github.repository_url}}/discussions/570 "Community discussion about this indicator")

![image]({{site.baseurl}}/assets/charts/Stc.png)

```csharp
// usage
IEnumerable<StcResult> results =
  quotes.GetStc(fastPeriods, slowPeriods, signalPeriods);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `fastPeriods` | int | Number of periods (`F`) for the faster moving average.  Must be greater than 0.  Default is 12.
| `slowPeriods` | int | Number of periods (`S`) for the slower moving average.  Must be greater than `fastPeriods`.  Default is 26.
| `signalPeriods` | int | Number of periods (`P`) for the moving average of STC.  Must be greater than or equal to 0.  Default is 9.

### Historical quotes requirements

You must have at least `2×(S+P)` or `S+P+100` worth of `quotes`, whichever is more.  Since this uses a smoothing technique, we recommend you use at least `S+P+250` data points prior to the intended usage date for better precision.

`quotes` is an `IEnumerable<TQuote>` collection of historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<StcResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `S-1` slow periods will have `null` values since there's not enough data to calculate.

:hourglass: **Convergence Warning**: The first `S+P+250` periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods.

### StcResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Stc` | decimal | The STC line is the difference between slow and fast moving averages
| `Signal` | decimal | Moving average of the `STC` line
| `Histogram` | decimal | Gap between of the `STC` and `Signal` line

### Utilities

- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()]({{site.baseurl}}/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and Helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Example

```csharp
// fetch historical quotes from your feed (your method)
IEnumerable<Quote> quotes = GetHistoryFromFeed("SPY");

// calculate STC(12,26,9)
IEnumerable<StcResult> results = quotes.GetStc(12,26,9);
```
