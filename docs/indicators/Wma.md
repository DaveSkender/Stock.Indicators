---
title: Weighted Moving Average (WMA)
description: Weighted Moving Average is the linear weighted average of financial market prices over a lookback window.  This also called Linear Weighted Moving Average (LWMA).
---



# {{ $frontmatter.title }}

[Weighted Moving Average](https://en.wikipedia.org/wiki/Moving_average#Weighted_moving_average) is the linear weighted average of price over a lookback window.  This also called Linear Weighted Moving Average (LWMA).
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/227 "Community discussion about this indicator")

<img src="/assets/charts/Wma.png" alt="chart for Weighted Moving Average (WMA)" />

```csharp
// C# usage syntax (with Close price)
IReadOnlyList<WmaResult> results =
  quotes.ToWma(lookbackPeriods);
```

## Parameters

**`lookbackPeriods`** _`int`_ - Number of periods (`N`) in the lookback window.  Must be greater than 0.

### Historical quotes requirements

You must have at least `N` periods of `quotes` to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](/guide/#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<WmaResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N-1` periods will have `null` values since there's not enough data to calculate.

### WmaResult

**`Timestamp`** _`DateTime`_ - date from evaluated `TQuote`

**`Wma`** _`double`_ - Weighted moving average

### Utilities

- [.Condense()](/utilities#condense)
- [.Find(lookupDate)](/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()](/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)](/utilities#remove-warmup-periods)

See [Utilities and helpers](/utilities#utilities-for-indicator-results) for more information.

## Chaining

This indicator may be generated from any chain-enabled indicator or method.

```csharp
// example
var results = quotes
    .Use(CandlePart.HL2)
    .ToWma(..);
```

Results can be further processed on `Wma` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .ToWma(..)
    .ToRsi(..);
```

## Streaming

Use the buffer-style `List<T>` when you need incremental calculations without a hub:

```csharp
WmaList wmaList = new(lookbackPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  quoteHub.Add(quote);
}

// based on `ICollection<WmaResult>`
IReadOnlyList<WmaResult> results = wmaList;
```

Subscribe to a `QuoteHub` for advanced streaming scenarios:

```csharp
QuoteHub<Quote> quoteHub = new();
WmaHub<Quote> observer = quoteHub.ToWma(lookbackPeriods);

foreach (Quote quote in quotes)  // simulating stream
{
  quoteHub.Add(quote);
}

IReadOnlyList<WmaResult> results = observer.Results;
```
