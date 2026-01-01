---
title: Hull Moving Average (HMA)
description: Created by Alan Hull, the Hull Moving Average is a modified weighted average of price that reduces lag.
permalink: /indicators/Hma/
image: /assets/charts/Hma.png
type: moving-average
layout: indicator
---

# {{ page.title }}

Created by Alan Hull, the [Hull Moving Average](https://alanhull.com/hull-moving-average) is a modified weighted average of price that reduces lag.
[[Discuss] &#128172;]({{site.github.repository_url}}/discussions/252 "Community discussion about this indicator")

![chart for {{page.title}}]({{site.baseurl}}{{page.image}})

```csharp
// C# usage syntax
IReadOnlyList<HmaResult> results =
  quotes.ToHma(lookbackPeriods);
```

## Parameters

**`lookbackPeriods`** _`int`_ - Number of periods (`N`) in the moving average.  Must be greater than 1.

### Historical quotes requirements

You must have at least `N+(integer of SQRT(N))-1` periods of `quotes` to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<HmaResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N+(integer of SQRT(N))-1` periods will have `null` values since there's not enough data to calculate.

### HmaResult

**`Timestamp`** _`DateTime`_ - date from evaluated `TQuote`

**`Hma`** _`double`_ - Hull moving average

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
    .ToHma(..);
```

Results can be further processed on `Hma` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .ToHma(..)
    .ToRsi(..);
```

## Streaming

Use the buffer-style `List<T>` when you need incremental calculations without a hub:

```csharp
HmaList hmaList = new(lookbackPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  hmaList.Add(quote);
}

// based on `ICollection<HmaResult>`
IReadOnlyList<HmaResult> results = hmaList;
```

Subscribe to a `QuoteHub` for advanced streaming scenarios:

```csharp
QuoteHub quoteHub = new();
HmaHub observer = quoteHub.ToHmaHub(lookbackPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  quoteHub.Add(quote);
}

IReadOnlyList<HmaResult> results = observer.Results;
```
