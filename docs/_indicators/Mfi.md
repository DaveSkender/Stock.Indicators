---
title: Money Flow Index (MFI)
permalink: /indicators/Mfi/
type: volume-based
layout: indicator
---

# {{ page.title }}

Created by Quong and Soudack, the [Money Flow Index](https://en.wikipedia.org/wiki/Money_flow_index) is a price-volume oscillator that shows buying and selling momentum.
[[Discuss] :speech_balloon:]({{site.github.repository_url}}/discussions/247 "Community discussion about this indicator")

![image]({{site.baseurl}}/assets/charts/Mfi.png)

```csharp
// usage
IEnumerable<MfiResult> results =
  quotes.GetMfi(lookbackPeriods);
```

## Parameters

| name | type | notes
| -- |-- |--
| `lookbackPeriods` | int | Number of periods (`N`) in the lookback period.  Must be greater than 1. Default is 14.

### Historical quotes requirements

You must have at least `N+1` historical quotes to cover the warmup periods.

`quotes` is an `IEnumerable<TQuote>` collection of historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<MfiResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N` periods will have `null` MFI values since they cannot be calculated.

### MfiResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Mfi` | decimal | Money Flow Index

### Utilities

- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()]({{site.baseurl}}/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and Helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Example

```csharp
// fetch historical quotes from your feed (your method)
IEnumerable<Quote> quotes = GetHistoryFromFeed("SPY");

// calculate
IEnumerable<MfiResult> results = quotes.GetMfi(14);
```
