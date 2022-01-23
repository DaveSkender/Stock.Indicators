---
title: Volume Weighted Moving Average (VWMA)
permalink: /indicators/Vwma/
type: moving-average
layout: indicator
---

# {{ page.title }}

Volume Weighted Moving Average is the volume adjusted average price over a lookback window.
[[Discuss] :speech_balloon:]({{site.github.repository_url}}/discussions/657 "Community discussion about this indicator")

![image]({{site.baseurl}}/assets/charts/Vwma.png)

```csharp
// legacy usage
IEnumerable<VwmaResult> results =
  quotes.GetVwma(lookbackPeriods);
```

## Parameters

| name | type | notes
| -- |-- |--
| `lookbackPeriods` | int | Number of periods (`N`) in the moving average.  Must be greater than 0.

### Historical quotes requirements

You must have at least `N` periods of `quotes` to cover the warmup periods.

`quotes` is an `IEnumerable<TQuote>` collection of historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<VwmaResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N-1` periods will have `null` values for `Vwma` since there's not enough data to calculate.

### VwmaResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Vwma` | decimal | Volume Weighted Moving Average for `N` lookback periods

### Utilities

- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()]({{site.baseurl}}/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and Helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Example

```csharp
// fetch historical quotes from your feed (your method)
IEnumerable<Quote> quotes = GetHistoryFromFeed("MSFT");

// calculate 10-period VWMA
IEnumerable<VwmaResult> results = quotes.GetVwma(10);
```
