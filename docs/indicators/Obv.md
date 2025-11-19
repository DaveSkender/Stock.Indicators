---
title: On-Balance Volume (OBV)
description: Popularized by Joseph Granville, On-balance Volume is a rolling accumulation of volume based on Close price direction.
---



# {{ $frontmatter.title }}

Popularized by Joseph Granville, [On-balance Volume](https://en.wikipedia.org/wiki/On-balance_volume) is a rolling accumulation of volume based on Close price direction.
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/246 "Community discussion about this indicator")

<img src="/assets/charts/Obv.png" alt="chart for On-Balance Volume (OBV)" />

```csharp
// C# usage syntax
IReadOnlyList<ObvResult> results =
  quotes.ToObv();
```

## Historical quotes requirements

You must have at least two historical quotes to cover the warmup periods; however, since this is a trendline, more is recommended.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](/guide/#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<ObvResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first period OBV will have a `0` value since there's not enough data to calculate.

### ObvResult

**`Timestamp`** _`DateTime`_ - date from evaluated `TQuote`

**`Obv`** _`double`_ - On-balance Volume

> &#128681; **Warning**: absolute values in OBV are somewhat meaningless. Use with caution.

### Utilities

- [.Condense()](/utilities#condense)
- [.Find(lookupDate)](/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods(qty)](/utilities#remove-warmup-periods)

See [Utilities and helpers](/utilities#utilities-for-indicator-results) for more information.

## Chaining

Results can be further processed on `Obv` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .ToObv(..)
    .ToRsi(..);
```

This indicator must be generated from `quotes` and **cannot** be generated from results of another chain-enabled indicator or method.

## Streaming

Use the buffer-style `List<T>` when you need incremental calculations without a hub:

```csharp
ObvList obvList = new();

foreach (IQuote quote in quotes)  // simulating stream
{
  obvList.Add(quote);
}

// based on `ICollection<ObvResult>`
IReadOnlyList<ObvResult> results = obvList;
```

Subscribe to a `QuoteHub` for advanced streaming scenarios:

```csharp
QuoteHub<Quote> quoteHub = new();
ObvHub<Quote> observer = quoteHub.ToObv();

foreach (Quote quote in quotes)  // simulating stream
{
  quoteHub.Add(quote);
}

IReadOnlyList<ObvResult> results = observer.Results;
```
