---
title: Volume Simple Moving Average
permalink: /indicators/VolSma/
type: moving-average
layout: indicator
---

# {{ page.title }}

The Volume Simple Moving Average is the average volume over a lookback window.  This is helpful when you are trying to assess whether volume is above or below normal.
[[Discuss] :speech_balloon:]({{site.github.repository_url}}/discussions/230 "Community discussion about this indicator")

:warning: **Deprecation Warning!** `GetVolSma` is now redundant and will be removed from the library at the end of 2021.  It is replaced by [GetSma()](../Sma/#content) with a `CandlePart.Volume` specification.

![image]({{site.baseurl}}/assets/charts/VolSma.png)

```csharp
// legacy usage
IEnumerable<VolSmaResult> results =
  quotes.GetVolSma(lookbackPeriods);

// please convert to equivalent:
IEnumerable<SmaResult> results =
  quotes.GetSma(lookbackPeriods, CandlePart.Volume);
```

## Parameters

| name | type | notes
| -- |-- |--
| `lookbackPeriods` | int | Number of periods (`N`) in the moving average.  Must be greater than 0.

### Historical quotes requirements

You must have at least `N` periods of `quotes`.

`quotes` is an `IEnumerable<TQuote>` collection of historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<VolSmaResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N-1` periods will have `null` values for `VolSma` since there's not enough data to calculate.

### VolSmaResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Volume` | decimal | Volume
| `VolSma` | decimal | Simple moving average of `Volume` for `N` lookback periods

### Utilities

- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()]({{site.baseurl}}/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and Helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Example

```csharp
// fetch historical quotes from your feed (your method)
IEnumerable<Quote> quotes = GetHistoryFromFeed("MSFT");

// calculate 20-period SMA of Volume
IEnumerable<VolSmaResult> results = quotes.GetVolSma(20);
```
