---
title: Williams %R
description: Created by Larry Williams, the Williams %R momentum oscillator compares current price with recent highs and lows and is presented on scale of -100 to 0.  It is exactly the same as the fast variant of Stochastic Oscillator, but with a different scaling.
---

# {{ $frontmatter.title }}

Created by Larry Williams, the [Williams %R](https://en.wikipedia.org/wiki/Williams_%25R) momentum oscillator compares current price with recent highs and lows and is presented on scale of -100 to 0.  It is exactly the same as the fast variant of [Stochastic Oscillator](/indicators/Stoch), but with a different scaling.
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/229 "Community discussion about this indicator")

<img src="/assets/charts/WilliamsR.png" alt="chart for Williams %R" />

```csharp
// C# usage syntax
IReadOnlyList<WilliamsResult> results =
  quotes.ToWilliamsR(lookbackPeriods);
```

## Parameters

**`lookbackPeriods`** _`int`_ - Number of periods (`N`) in the lookback period.  Must be greater than 0.  Default is 14.

### Historical quotes requirements

You must have at least `N` periods of `quotes` to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](/guide/#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<WilliamsResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N-1` periods will have `null` Oscillator values since there's not enough data to calculate.

### WilliamsResult

**`Timestamp`** _`DateTime`_ - date from evaluated `TQuote`

**`WilliamsR`** _`double`_ - Oscillator over prior `N` lookback periods

### Utilities

- [.Condense()](/utilities/#condense)
- [.Find(lookupDate)](/utilities/#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()](/utilities/#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)](/utilities/#remove-warmup-periods)

See [Utilities and helpers](/utilities/#utilities-for-indicator-results) for more information.

## Chaining

Results can be further processed on `WilliamsR` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .ToWilliamsR(..)
    .ToSlope(..);
```

This indicator must be generated from `quotes` and **cannot** be generated from results of another chain-enabled indicator or method.

## Streaming

Use the buffer-style `List<T>` when you need incremental calculations without a hub:

```csharp
WilliamsRList williamsRList = new(lookbackPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  williamsRList.Add(quote);
}

// based on `ICollection<WilliamsRResult>`
IReadOnlyList<WilliamsRResult> results = williamsRList;
```

Subscribe to a `QuoteHub` for advanced streaming scenarios:

```csharp
QuoteHub<Quote> quoteHub = new();
WilliamsRHub<Quote> observer = quoteHub.ToWilliamsR(lookbackPeriods);

foreach (Quote quote in quotes)  // simulating stream
{
  quoteHub.Add(quote);
}

IReadOnlyList<WilliamsRResult> results = observer.Results;
```
