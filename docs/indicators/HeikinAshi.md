---
title: Heikin-Ashi
description: Created by Munehisa Homma, [Heikin-Ashi](https://en.wikipedia.org/wiki/Candlestick_chart#Heikin-Ashi_candlesticks) is a modified candlestick pattern that transforms prices based on prior period prices for smoothing.
---

# {{ $frontmatter.title }}

Created by Munehisa Homma, [Heikin-Ashi](https://en.wikipedia.org/wiki/Candlestick_chart#Heikin-Ashi_candlesticks) is a modified candlestick pattern based on prior period prices for smoothing.
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/254 "Community discussion about this indicator")

<img src="/assets/charts/HeikinAshi.png" alt="chart for Heikin-Ashi" />

```csharp
// C# usage syntax
IReadOnlyList<HeikinAshiResult> results =
  quotes.ToHeikinAshi();
```

## Historical quotes requirements

You must have at least two periods of `quotes` to cover the warmup periods; however, more is typically provided since this is a chartable candlestick pattern.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](/guide#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<HeikinAshiResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- `HeikinAshiResult` is based on `IQuote`, so it can be used as a direct replacement for `quotes`.

### HeikinAshiResult

**`Timestamp`** _`DateTime`_ - date from evaluated `TQuote`

**`Open`** _`decimal`_ - Modified open price

**`High`** _`decimal`_ - Modified high price

**`Low`** _`decimal`_ - Modified low price

**`Close`** _`decimal`_ - Modified close price

**`Volume`** _`decimal`_ - Volume (same as `quotes`)

### Utilities

- [.Find(lookupDate)](/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods(qty)](/utilities#remove-warmup-periods)
- .ToQuotes() to convert to a `Quote` collection.  Example:

  ```csharp
  IReadOnlyList<Quote> results = quotes
    .ToHeikinAshi()
    .ToQuotes();
  ```

See [Utilities and helpers](/utilities#utilities-for-indicator-results) for more information.

## Chaining

Results are based in `IQuote` and can be further used in any indicator.

```csharp
// example
var results = quotes
    .ToHeikinAshi(..)
    .ToRsi(..);
```

This indicator must be generated from `quotes` and **cannot** be generated from results of another chain-enabled indicator or method.

## Streaming

Use the buffer-style `List<T>` when you need incremental calculations without a hub:

```csharp
HeikinAshiList heikinAshiList = new();

foreach (IQuote quote in quotes)  // simulating stream
{
  heikinAshiList.Add(quote);
}

// based on `ICollection<HeikinAshiResult>`
IReadOnlyList<HeikinAshiResult> results = heikinAshiList;
```

Subscribe to a `QuoteHub` for advanced streaming scenarios:

```csharp
QuoteHub quoteHub = new();
HeikinAshiHub observer = quoteHub.ToHeikinAshiHub();

foreach (IQuote quote in quotes)  // simulating stream
{
  quoteHub.Add(quote);
}

IReadOnlyList<HeikinAshiResult> results = observer.Results;
```
