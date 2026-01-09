---
title: On-Balance Volume (OBV)
description: Popularized by Joseph Granville, On-balance Volume is a rolling accumulation of volume based on Close price direction.
permalink: /indicators/Obv/
image: /assets/charts/Obv.png
type: volume-based
layout: indicator
---

# {{ page.title }}

Popularized by Joseph Granville, [On-balance Volume](https://en.wikipedia.org/wiki/On-balance_volume) is a rolling accumulation of volume based on Close price direction.
[[Discuss] &#128172;]({{site.github.repository_url}}/discussions/246 "Community discussion about this indicator")

![chart for {{page.title}}]({{site.baseurl}}{{page.image}})

```csharp
// C# usage syntax
IReadOnlyList<ObvResult> results =
  quotes.ToObv();
```

## Historical quotes requirements

You must have at least two historical quotes to cover the warmup periods; however, since this is a trendline, more is recommended.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

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

- [.Condense()]({{site.baseurl}}/utilities#condense)
- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

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
QuoteHub quoteHub = new();
ObvHub observer = quoteHub.ToObvHub();

foreach (IQuote quote in quotes)  // simulating stream
{
  quoteHub.Add(quote);
}

IReadOnlyList<ObvResult> results = observer.Results;
```
