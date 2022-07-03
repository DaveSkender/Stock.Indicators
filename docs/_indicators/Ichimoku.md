---
title: Ichimoku Cloud
description: Ichimoku Cloud (Ichimoku Kinkō Hyō)
permalink: /indicators/Ichimoku/
type: price-trend
layout: indicator
---

# {{ page.title }}

Created by Goichi Hosoda (細田悟一, Hosoda Goichi), [Ichimoku Cloud](https://en.wikipedia.org/wiki/Ichimoku_Kink%C5%8D_Hy%C5%8D), also known as Ichimoku Kinkō Hyō, is a collection of indicators that depict support and resistance, momentum, and trend direction.
[[Discuss] :speech_balloon:]({{site.github.repository_url}}/discussions/251 "Community discussion about this indicator")

![image]({{site.baseurl}}/assets/charts/Ichimoku.png)

```csharp
// usage
IEnumerable<IchimokuResult> results =
  quotes.GetIchimoku(tenkanPeriods, kijunPeriods, senkouBPeriods);

// usage with custom offset
IEnumerable<IchimokuResult> results =
  quotes.GetIchimoku(tenkanPeriods, kijunPeriods, senkouBPeriods, offsetPeriods);

// usage with different custom offsets
IEnumerable<IchimokuResult> results =
  quotes.GetIchimoku(tenkanPeriods, kijunPeriods, senkouBPeriods, senkouOffset, chikouOffset);
```

## Parameters

| name | type | notes
| -- |-- |--
| `tenkanPeriods` | int | Number of periods (`T`) in the Tenkan-sen midpoint evaluation.  Must be greater than 0.  Default is 9.
| `kijunPeriods` | int | Number of periods (`K`) in the shorter Kijun-sen midpoint evaluation.  Must be greater than 0.  Default is 26.
| `senkouBPeriods` | int | Number of periods (`S`) in the longer Senkou leading span B midpoint evaluation.  Must be greater than `K`.  Default is 52.
| `offsetPeriods` | int | Optional.  Number of periods to offset both `Senkou` and `Chikou` spans.  Must be non-negative.  Default is `kijunPeriods`.
| `senkouOffset` | int | Optional.  Number of periods to offset the `Senkou` span.  Must be non-negative.  Default is `kijunPeriods`.
| `chikouOffset` | int | Optional.  Number of periods to offset the `Chikou` span.  Must be non-negative.  Default is `kijunPeriods`.

See overloads usage above to determine which parameters are relevant for each.  If you are customizing offsets, all parameter arguments must be specified.

### Historical quotes requirements

You must have at least the greater of `T`,`K`, `S`, and offset periods for `quotes` to cover the warmup periods; though, given the leading and lagging nature, we recommend notably more.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<IchimokuResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `T-1`, `K-1`, and `S-1` periods will have various `null` values since there's not enough data to calculate.  Custom offset periods may also increase `null` results for warmup periods.

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

- [.Condense()]({{site.baseurl}}/utilities#condense)
- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and Helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Chaining

This indicator is not chain-enabled and must be generated from `quotes`.  It **cannot** be used for further processing by other chain-enabled indicators.
