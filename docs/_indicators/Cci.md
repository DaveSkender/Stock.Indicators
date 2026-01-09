---
title: Commodity Channel Index (CCI)
description: Created by Donald Lambert, the Commodity Channel Index is an oscillator depicting deviation from typical price range, often used to identify cyclical trends.
permalink: /indicators/Cci/
image: /assets/charts/Cci.png
type: oscillator
layout: indicator
---

# {{ page.title }}

Created by Donald Lambert, the [Commodity Channel Index](https://en.wikipedia.org/wiki/Commodity_channel_index) is an oscillator depicting deviation from typical price range, often used to identify cyclical trends.
[[Discuss] &#128172;]({{site.github.repository_url}}/discussions/265 "Community discussion about this indicator")

![chart for {{page.title}}]({{site.baseurl}}{{page.image}})

```csharp
// C# usage syntax
IReadOnlyList<CciResult> results =
  quotes.ToCci(lookbackPeriods);
```

## Parameters

**`lookbackPeriods`** _`int`_ - Number of periods (`N`) in the moving average.  Must be greater than 0.  Default is 20.

### Historical quotes requirements

You must have at least `N+1` periods of `quotes` to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<CciResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N-1` periods will have `null` values since there's not enough data to calculate.

### CciResult

**`Timestamp`** _`DateTime`_ - date from evaluated `TQuote`

**`Cci`** _`double`_ - Commodity Channel Index

### Utilities

- [.Condense()]({{site.baseurl}}/utilities#condense)
- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()]({{site.baseurl}}/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Chaining

Results can be further processed on `Cci` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .ToCci(..)
    .ToSlope(..);
```

This indicator must be generated from `quotes` and **cannot** be generated from results of another chain-enabled indicator or method.

## Streaming

Use the buffer-style `List<T>` when you need incremental calculations without a hub:

```csharp
CciList cciList = new(lookbackPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  cciList.Add(quote);
}

// based on `ICollection<CciResult>`
IReadOnlyList<CciResult> results = cciList;
```

Subscribe to a `QuoteHub` for advanced streaming scenarios:

```csharp
QuoteHub quoteHub = new();
CciHub observer = quoteHub.ToCciHub(lookbackPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  quoteHub.Add(quote);
}

IReadOnlyList<CciResult> results = observer.Results;
```
