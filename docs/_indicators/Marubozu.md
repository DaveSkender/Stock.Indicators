---
title: Marubozu
description: Marubozu is a single-bar candlestick pattern that has no wicks, representing consistent directional movement.
permalink: /indicators/Marubozu/
image: /assets/charts/Marubozu.png
layout: indicator
type: candlestick-pattern
---

# {{ page.title }}

[Marubozu](https://en.wikipedia.org/wiki/Marubozu) is a single-bar candlestick pattern that has no wicks, representing consistent directional movement.
[[Discuss] &#128172;]({{site.github.repository_url}}/discussions/512 "Community discussion about this indicator")

![chart for {{page.title}}]({{site.baseurl}}{{page.image}})

```csharp
// C# usage syntax
IReadOnlyList<CandleResult> results =
  quotes.ToMarubozu(minBodyPercent);
```

## Parameters

**`minBodyPercent`** _`double`_ - Optional.  Minimum body size as a percent of total candle size.  Example: 85% would be entered as 85 (not 0.85).  Must be between 80 and 100, if specified.  Default is 95 (95%).

### Historical quotes requirements

You must have at least one historical quote; however, more is typically provided since this is a chartable candlestick pattern.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<CandleResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The candlestick pattern is indicated on dates where `Match` is `Match.BullSignal` or `Match.BearSignal`.
- `Price` is `Close` price; however, all OHLCV elements are included in `CandleProperties`.
- There is no intrinsic basis or confirmation signal provided for this pattern.

{% include candle-result.md %}

### Utilities

- [.Condense()]({{site.baseurl}}/utilities#condense)
- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Streaming

Use the buffer-style `List<T>` when you need incremental calculations without a hub:

```csharp
MarubozuList marubozuList = new(minBodyPercent);

foreach (IQuote quote in quotes)  // simulating stream
{
  marubozuList.Add(quote);
}

// based on `ICollection<MarubozuResult>`
IReadOnlyList<MarubozuResult> results = marubozuList;
```

Subscribe to a `QuoteHub` for advanced streaming scenarios:

```csharp
QuoteHub quoteHub = new();
MarubozuHub observer = quoteHub.ToMarubozuHub(minBodyPercent);

foreach (IQuote quote in quotes)  // simulating stream
{
  quoteHub.Add(quote);
}

IReadOnlyList<MarubozuResult> results = observer.Results;
```
