---
title: Money Flow Index (MFI)
description: Created by Quong and Soudack, the Money Flow Index is a price-volume oscillator that shows buying and selling momentum.
---

# {{ $frontmatter.title }}

Created by Quong and Soudack, the [Money Flow Index](https://en.wikipedia.org/wiki/Money_flow_index) is a price-volume oscillator that shows buying and selling momentum.
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/247 "Community discussion about this indicator")

<img src="/assets/charts/Mfi.png" alt="chart for Money Flow Index (MFI)" />

```csharp
// C# usage syntax
IReadOnlyList<MfiResult> results =
  quotes.ToMfi(lookbackPeriods);
```

## Parameters

**`lookbackPeriods`** _`int`_ - Number of periods (`N`) in the lookback period.  Must be greater than 1. Default is 14.

### Historical quotes requirements

You must have at least `N+1` historical quotes to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](/guide#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<MfiResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N` periods will have `null` MFI values since they cannot be calculated.

### MfiResult

**`Timestamp`** _`DateTime`_ - date from evaluated `TQuote`

**`Mfi`** _`decimal`_ - Money Flow Index

### Utilities

- [.Condense()](/utilities#condense)
- [.Find(lookupDate)](/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()](/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)](/utilities#remove-warmup-periods)

See [Utilities and helpers](/utilities#utilities-for-indicator-results) for more information.

## Streaming

Use the buffer-style `List<T>` when you need incremental calculations without a hub:

```csharp
MfiList mfiList = new(lookbackPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  mfiList.Add(quote);
}

// based on `ICollection<MfiResult>`
IReadOnlyList<MfiResult> results = mfiList;
```

Subscribe to a `QuoteHub` for advanced streaming scenarios:

```csharp
QuoteHub quoteHub = new();
MfiHub observer = quoteHub.ToMfiHub(lookbackPeriods);

foreach (Quote quote in quotes)  // simulating stream
{
  quoteHub.Add(quote);
}

IReadOnlyList<MfiResult> results = observer.Results;
```

## Chaining

Results can be further processed on `Mfi` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .ToMfi(..)
    .ToRsi(..);
```

This indicator must be generated from `quotes` and **cannot** be generated from results of another chain-enabled indicator or method.
