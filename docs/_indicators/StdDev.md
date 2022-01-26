---
title: Standard Deviation (volatility)
description: Standard Deviation, Historical Volatility (HV)
permalink: /indicators/StdDev/
type: numerical-analysis
layout: indicator
---

# {{ page.title }}

[Standard Deviation](https://en.wikipedia.org/wiki/Standard_deviation) of Close price over a rolling lookback window.  Also known as Historical Volatility (HV).
[[Discuss] :speech_balloon:]({{site.github.repository_url}}/discussions/239 "Community discussion about this indicator")

![image]({{site.baseurl}}/assets/charts/StdDev.png)

```csharp
// usage
IEnumerable<StdDevResult> results =
  quotes.GetStdDev(lookbackPeriods);

// usage with optional SMA of STDEV (shown above)
IEnumerable<StdDevResult> results =
  quotes.GetStdDev(lookbackPeriods, smaPeriods);
```

## Parameters

| name | type | notes
| -- |-- |--
| `lookbackPeriods` | int | Number of periods (`N`) in the lookback period.  Must be greater than 1 to calculate; however we suggest a larger period for statistically appropriate sample size.
| `smaPeriods` | int | Optional.  Number of periods in the moving average of `StdDev`.  Must be greater than 0, if specified.

### Historical quotes requirements

You must have at least `N` periods of `quotes` to cover the warmup periods.

`quotes` is an `IEnumerable<TQuote>` collection of historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<StdDevResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N-1` periods will have `null` values since there's not enough data to calculate.

### StdDevResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `StdDev` | double | Standard Deviation of Close price over `N` lookback periods
| `Mean` | double | Mean value of Close price over `N` lookback periods
| `ZScore` | double | Z-Score of current Close price (number of standard deviations from mean)
| `StdDevSma` | double | Moving average (SMA) of STDDEV based on `smaPeriods` periods, if specified

### Utilities

- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()]({{site.baseurl}}/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and Helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Example

```csharp
// fetch historical quotes from your feed (your method)
IEnumerable<Quote> quotes = GetHistoryFromFeed("SPX");

// calculate 10-period Standard Deviation
IEnumerable<StdDevResult> results = quotes.GetStdDev(10);
```
