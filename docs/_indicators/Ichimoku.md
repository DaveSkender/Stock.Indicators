---
title: Ichimoku Cloud
permalink: /indicators/Ichimoku/
layout: default
---

# {{ page.title }}

Created by Goichi Hosoda (細田悟一, Hosoda Goichi), [Ichimoku Cloud](https://en.wikipedia.org/wiki/Ichimoku_Kink%C5%8D_Hy%C5%8D), also known as Ichimoku Kinkō Hyō, is a collection of indicators that depict support and resistance, momentum, and trend direction.
[[Discuss] :speech_balloon:]({{site.github.repository_url}}/discussions/251 "Community discussion about this indicator")

![image]({{site.baseurl}}/assets/charts/Ichimoku.png)

```csharp
// usage
IEnumerable<IchimokuResult> results =
  quotes.GetIchimoku(signalPeriods, shortSpanPeriods, longSpanPeriods);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `signalPeriods` | int | Number of periods (`N`) in the Tenkan-sen midpoint evaluation.  Must be greater than 0.  Default is 9.
| `shortSpanPeriods` | int | Number of periods (`S`) in the shorter Kijun-sen midpoint evaluation.  It also sets the Chikou span lag/shift.  Must be greater than 0.  Default is 26.
| `longSpanPeriods` | int | Number of periods (`L`) in the longer Senkou leading span B midpoint evaluation.  Must be greater than `S`.  Default is 52.

### Historical quotes requirements

You must have at least the greater of `N`,`S`, or `L` periods of `quotes`; though, given the leading and lagging nature, we recommend notably more.

`quotes` is an `IEnumerable<TQuote>` collection of historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<IchimokuResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N-1`, `S-1`, and `L-1` periods will have various `null` values since there's not enough data to calculate.

### IchimokuResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `TenkanSen` | decimal | Conversion / signal line
| `KijunSen` | decimal | Base line
| `SenkouSpanA` | decimal | Leading span A
| `SenkouSpanB` | decimal | Leading span B
| `ChikouSpan` | decimal | Lagging span

### Utilities

- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and Helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Example

```csharp
// fetch historical quotes from your feed (your method)
IEnumerable<Quote> quotes = GetHistoryFromFeed("MSFT");

// calculate ICHIMOKU(9,26,52)
IEnumerable<IchimokuResult> results
  = quotes.GetIchimoku(9,26,52);
```
