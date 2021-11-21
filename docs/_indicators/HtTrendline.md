---
title: Hilbert Transform Instantaneous Trendline
permalink: /indicators/HtTrendline/
type: moving-average
layout: indicator
---

# {{ page.title }}

Created by John Ehlers, the Hilbert Transform Instantaneous Trendline is a 5-period trendline of high/low price that uses signal processing to reduce noise.
[[Discuss] :speech_balloon:]({{site.github.repository_url}}/discussions/363 "Community discussion about this indicator")

![image]({{site.baseurl}}/assets/charts/HtTrendline.png)

```csharp
// usage
IEnumerable<HtlResult> results =
  quotes.GetHtTrendline();
```

## Historical quotes requirements

Since this indicator has a warmup period, you must have at least `100` periods of `quotes`.

`quotes` is an `IEnumerable<TQuote>` collection of historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<HtlResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `6` periods will have `null` values for `SmoothPrice` since there's not enough data to calculate.

:hourglass: **Convergence Warning**: The first `100` periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods.

### HtlResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Trendline` | decimal | HT Trendline
| `SmoothPrice` | decimal | Weighted moving average of `(H+L)/2` price

### Utilities

- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()]({{site.baseurl}}/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and Helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Example

```csharp
// fetch historical quotes from your feed (your method)
IEnumerable<Quote> quotes = GetHistoryFromFeed("MSFT");

// calculate HT Trendline
IEnumerable<HtlResult> results = quotes.GetHtTrendline();
```
