---
title: Ulcer Index (UI)
description: Created by Peter Martin, the Ulcer Index is a measure of downside price volatility.  Often called the "heart attack" score, it measures the amount of pain seen from drawdowns in financial market prices and portfolio value.
permalink: /indicators/UlcerIndex/
image: /assets/charts/UlcerIndex.png
type: price-characteristic
layout: indicator
---

# {{ page.title }}

Created by Peter Martin, the [Ulcer Index](https://en.wikipedia.org/wiki/Ulcer_index) is a measure of downside price volatility over a lookback window.  Often called the "heart attack" score, it measures the amount of pain seen from drawdowns in financial market prices and portfolio value.
[[Discuss] &#128172;]({{site.github.repository_url}}/discussions/232 "Community discussion about this indicator")

![chart for {{page.title}}]({{site.baseurl}}{{page.image}})

```csharp
// C# usage syntax
IReadOnlyList<UlcerIndexResult> results =
  quotes.ToUlcerIndex(lookbackPeriods);
```

## Parameters

**`lookbackPeriods`** _`int`_ - Number of periods (`N`) for review.  Must be greater than 0.  Default is 14.

### Historical quotes requirements

You must have at least `N` periods of `quotes` to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<UlcerIndexResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N-1` periods will have `null` values since there's not enough data to calculate.

### UlcerIndexResult

**`Timestamp`** _`DateTime`_ - date from evaluated `TQuote`

**`UI`** _`double`_ - Ulcer Index

### Utilities

- [.Condense()]({{site.baseurl}}/utilities#condense)
- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()]({{site.baseurl}}/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Chaining

This indicator may be generated from any chain-enabled indicator or method.

```csharp
// example
var results = quotes
    .Use(CandlePart.HL2)
    .ToAlma(..);
```

Results can be further processed on `UI` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .ToAlma(..)
    .ToRsi(..);
```

## Streaming

Use the buffer-style `List<T>` when you need incremental calculations without a hub:

```csharp
UlcerIndexList ulcerIndexList = new(lookbackPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  ulcerIndexList.Add(quote);
}

// based on `ICollection<UlcerIndexResult>`
IReadOnlyList<UlcerIndexResult> results = ulcerIndexList;
```

Subscribe to a `QuoteHub` for advanced streaming scenarios:

```csharp
QuoteHub quoteHub = new();
UlcerIndexHub observer = quoteHub.ToUlcerIndexHub(lookbackPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  quoteHub.Add(quote);
}

IReadOnlyList<UlcerIndexResult> results = observer.Results;
```
