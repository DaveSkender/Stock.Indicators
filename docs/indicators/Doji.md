---
title: Doji
description: Doji is a single-bar candlestick pattern where open and close price are virtually identical, representing market indecision.
---

# Doji

[Doji](https://en.wikipedia.org/wiki/Doji) is a single-bar candlestick pattern where open and close price are virtually identical, representing market indecision.
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/734 "Community discussion about this indicator")

```csharp
// C# usage syntax
IReadOnlyList<CandleResult> results =
  quotes.ToDoji(maxPriceChangePercent);
```

## Parameters

| param | type | description |
| ----- | ---- | ----------- |
| `maxPriceChangePercent` | double | Optional.  Maximum absolute percent difference in open and close price.  Example: 0.3% would be entered as 0.3 (not 0.003).  Must be between 0 and 0.5 percent, if specified.  Default is 0.1 (0.1%). |

### Historical quotes requirements

You must have at least one historical quote; however, more is typically provided since this is a chartable candlestick pattern.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](/guide#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<CandleResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The candlestick pattern is indicated on dates where `Match` is `Match.Neutral`.
- `Price` is `Close` price; however, all OHLCV elements are included in `CandleProperties`.
- There is no intrinsic basis or confirmation signal provided for this pattern.

### Utilities

- [.Condense()](/utilities/results#condense)
- [.Find(lookupDate)](/utilities/results#find-indicator-result-by-date)
- [.RemoveWarmupPeriods(qty)](/utilities/results#remove-warmup-periods)

See [Utilities and helpers](/utilities/results) for more information.

## Streaming

Use the buffer-style `List<T>` when you need incremental calculations without a hub:

```csharp
DojiList dojiList = new(maxPriceChangePercent);

foreach (IQuote quote in quotes)  // simulating stream
{
  dojiList.Add(quote);
}

// based on `ICollection<DojiResult>`
IReadOnlyList<DojiResult> results = dojiList;
```

Subscribe to a `QuoteHub` for advanced streaming scenarios:

```csharp
QuoteHub quoteHub = new();
DojiHub observer = quoteHub.ToDojiHub(maxPriceChangePercent);

foreach (IQuote quote in quotes)  // simulating stream
{
  quoteHub.Add(quote);
}

IReadOnlyList<DojiResult> results = observer.Results;
```
