---
title: Choppiness Index
permalink: /indicators/Chop/
type: price-characteristic
layout: indicator
---

# {{ page.title }}

Created by E.W. Dreiss, the Choppiness Index measures the trendiness or choppiness on a scale of 0 to 100, to depict steady trends versus conditions of choppiness.  [[Discuss] :speech_balloon:]({{site.github.repository_url}}/discussions/357 "Community discussion about this indicator")

![image]({{site.baseurl}}/assets/charts/Chop.png)

```csharp
// usage
IEnumerable<ChopResult> results =
  quotes.GetChop(lookbackPeriods);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `lookbackPeriods` | int | Number of periods (`N`) for the lookback evaluation.  Must be greater than 1.  Default is 14.

### Historical quotes requirements

You must have at least `N+1` periods of `quotes`.

`quotes` is an `IEnumerable<TQuote>` collection of historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<ChopResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N` periods will have `null` values since there's not enough data to calculate.

### ChopResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Chop` | decimal | Choppiness Index

### Utilities

- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()]({{site.baseurl}}/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and Helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Example

```csharp
// fetch historical quotes from your feed (your method)
IEnumerable<Quote> quotes = GetHistoryFromFeed("SPY");

// calculate CHOP(14)
IEnumerable<ChopResult> results = quotes.GetChop(14);
```
