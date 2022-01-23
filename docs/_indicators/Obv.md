---
title: On-Balance Volume (OBV)
permalink: /indicators/Obv/
type: volume-based
layout: indicator
---

# {{ page.title }}

Popularized by Joseph Granville, [On-balance Volume](https://en.wikipedia.org/wiki/On-balance_volume) is a rolling accumulation of volume based on Close price direction.
[[Discuss] :speech_balloon:]({{site.github.repository_url}}/discussions/246 "Community discussion about this indicator")

![image]({{site.baseurl}}/assets/charts/Obv.png)

```csharp
// usage
IEnumerable<ObvResult> results =
  quotes.GetObv();

// usage with optional overlay SMA of OBV (shown above)
IEnumerable<ObvResult> results =
  quotes.GetObv(smaPeriods);
```

## Parameters

| name | type | notes
| -- |-- |--
| `smaPeriods` | int | Optional.  Number of periods (`N`) in the moving average of OBV.  Must be greater than 0, if specified.

### Historical quotes requirements

You must have at least two historical quotes to cover the warmup periods; however, since this is a trendline, more is recommended.

`quotes` is an `IEnumerable<TQuote>` collection of historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<ObvResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first period OBV will have `0` value since there's not enough data to calculate.

### ObvResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Obv` | double | On-balance Volume
| `ObvSma` | double | Moving average (SMA) of OBV based on `smaPeriods` periods, if specified

:warning: **Warning**: absolute values in OBV are somewhat meaningless, so use with caution.

### Utilities

- [.ConvertToQuotes()]({{site.baseurl}}/utilities#convert-to-quotes)
- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and Helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Example

```csharp
// fetch historical quotes from your feed (your method)
IEnumerable<Quote> quotes = GetHistoryFromFeed("SPY");

// calculate
IEnumerable<ObvResult> results = quotes.GetObv();
```
