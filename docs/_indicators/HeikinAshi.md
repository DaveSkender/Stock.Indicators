---
title: Heikin-Ashi
permalink: /indicators/HeikinAshi/
type: price-transform
layout: indicator
redirect_from:
 - /Indicators/HeikinAshi/
---

# {{ page.title }}

Created by Munehisa Homma, [Heikin-Ashi](https://en.wikipedia.org/wiki/Candlestick_chart#Heikin-Ashi_candlesticks) is a modified candlestick pattern that uses prior day for smoothing.
[[Discuss] :speech_balloon:]({{site.github.repository_url}}/discussions/254 "Community discussion about this indicator")

![image]({{site.baseurl}}/assets/charts/HeikinAshi.png)

```csharp
// usage
IEnumerable<HeikinAshiResult> results =
  quotes.GetHeikinAshi();  
```

## Historical quotes requirements

You must have at least two periods of `quotes`; however, more is typically provided since this is a chartable candlestick pattern.

`quotes` is an `IEnumerable<TQuote>` collection of historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<HeikinAshiResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first period will have `null` values since there's not enough data to calculate.

### HeikinAshiResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Open` | decimal | Modified open price
| `High` | decimal | Modified high price
| `Low` | decimal | Modified low price
| `Close` | decimal | Modified close price
| `Volume` | decimal | Volume (same as `quotes`)

### Utilities

- [.ConvertToQuotes()]({{site.baseurl}}/utilities#convert-to-quotes)
- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and Helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Example

```csharp
// fetch historical quotes from your feed (your method)
IEnumerable<Quote> quotes = GetHistoryFromFeed("MSFT");

// calculate
IEnumerable<HeikinAshiResult> results = quotes.GetHeikinAshi();
```
