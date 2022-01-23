---
title: Ultimate Oscillator
permalink: /indicators/Ultimate/
type: oscillator
layout: indicator
---

# {{ page.title }}

Created by Larry Williams, the [Ultimate Oscillator](https://en.wikipedia.org/wiki/Ultimate_oscillator) uses several lookback periods to weigh buying power against true range price to produce on oversold / overbought oscillator.
[[Discuss] :speech_balloon:]({{site.github.repository_url}}/discussions/231 "Community discussion about this indicator")

![image]({{site.baseurl}}/assets/charts/Ultimate.png)

```csharp
// usage
IEnumerable<UltimateResult> results =
  quotes.GetUltimate(shortPeriods, middlePeriods, longPeriods);
```

## Parameters

| name | type | notes
| -- |-- |--
| `shortPeriods` | int | Number of periods (`S`) in the short lookback.  Must be greater than 0.  Default is 7.
| `middlePeriods` | int | Number of periods (`M`) in the middle lookback.  Must be greater than `S`.  Default is 14.
| `longPeriods` | int | Number of periods (`L`) in the long lookback.  Must be greater than `M`.  Default is 28.

### Historical quotes requirements

You must have at least `L+1` periods of `quotes` to cover the warmup periods.

`quotes` is an `IEnumerable<TQuote>` collection of historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<UltimateResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `L-1` periods will have `null` Ultimate values since there's not enough data to calculate.

### UltimateResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Ultimate` | decimal | Simple moving average for `N` lookback periods

### Utilities

- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()]({{site.baseurl}}/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and Helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Example

```csharp
// fetch historical quotes from your feed (your method)
IEnumerable<Quote> quotes = GetHistoryFromFeed("MSFT");

// calculate 20-period Ultimate
IEnumerable<UltimateResult> results = quotes.GetUltimate(7,14,28);
```
