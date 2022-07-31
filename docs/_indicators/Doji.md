---
title: Doji
permalink: /indicators/Doji/
layout: indicator
type: candlestick-pattern
---

# {{ page.title }}

[Doji](https://en.wikipedia.org/wiki/Doji) is a single candlestick pattern where open and close price are virtually identical, representing market indecision.
[[Discuss] :speech_balloon:]({{site.github.repository_url}}/discussions/734 "Community discussion about this indicator")

![image]({{site.baseurl}}/assets/charts/Doji.png)

```csharp
// usage
IEnumerable<CandleResult> results =
  quotes.GetDoji(maxPriceChangePercent);
```

## Parameters

| name | type | notes
| -- |-- |--
| `maxPriceChangePercent` | double | Optional.  Maximum absolute percent difference in open and close price.  Example: 0.3% would be entered as 0.3 (not 0.003).  Must be between 0 and 0.5 percent, if specified.  Default is 0.1 (0.1%).

### Historical quotes requirements

You must have at least one historical quote; however, more is typically provided since this is a chartable candlestick pattern.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<CandleResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The candlestick pattern is indicated on dates where `Match` is `Match.Neutral`.
- `Price` is `Close` price; however, all OHLCV elements are included in `CandleProperties`.
- There is no intrinsic basis or confirmation signal provided for this pattern.

{% include candle-result.md %}

### Utilities

- [.Condense()]({{site.baseurl}}/utilities#condense)
- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and Helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.
