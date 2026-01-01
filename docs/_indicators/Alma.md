---
title: Arnaud Legoux Moving Average (ALMA)
description: Created by Arnaud Legoux and Dimitrios Kouzis-Loukas, ALMA is a normal Gaussian distribution weighted moving average of price.
permalink: /indicators/Alma/
image: /assets/charts/Alma.png
type: moving-average
layout: indicator
---

# {{ page.title }}

Created by Arnaud Legoux and Dimitrios Kouzis-Loukas, [ALMA]({{site.github.repository_url}}/files/5654531/ALMA-Arnaud-Legoux-Moving-Average.pdf) is a normal Gaussian distribution weighted moving average of price.
[[Discuss] &#128172;]({{site.github.repository_url}}/discussions/209 "Community discussion about this indicator")

![chart for {{page.title}}]({{site.baseurl}}{{page.image}})

```csharp
// C# usage syntax
IReadOnlyList<AlmaResult> results =
  quotes.ToAlma(lookbackPeriods, offset, sigma);
```

## Parameters

**`lookbackPeriods`** _`int`_ - Number of periods (`N`) in the moving average.  Must be greater than 1, but is typically in the 5-20 range.  Default is 9.

**`offset`** _`double`_ - Adjusts smoothness versus responsiveness on a scale from 0 to 1; where 1 is max responsiveness.  Default is 0.85.

**`sigma`** _`double`_ - Defines the width of the Gaussian [normal distribution](https://en.wikipedia.org/wiki/Normal_distribution).  Must be greater than 0.  Default is 6.

### Historical quotes requirements

You must have at least `N` periods of `quotes` to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<AlmaResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N-1` periods will have `null` values since there's not enough data to calculate.

### AlmaResult

**`Timestamp`** _`DateTime`_ - date from evaluated `TQuote`

**`Alma`** _`double`_ - Arnaud Legoux Moving Average

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

Results can be further processed on `Alma` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .ToAlma(..)
    .ToRsi(..);
```

## Streaming

Use the buffer-style `List<T>` when you need incremental calculations without a hub:

```csharp
AlmaList almaList = new(lookbackPeriods, offset, sigma);

foreach (IQuote quote in quotes)  // simulating stream
{
  almaList.Add(quote);
}

// based on `ICollection<AlmaResult>`
IReadOnlyList<AlmaResult> results = almaList;
```

Subscribe to a `QuoteHub` for advanced streaming scenarios:

```csharp
QuoteHub quoteHub = new();
AlmaHub observer = quoteHub.ToAlmaHub(lookbackPeriods, offset, sigma);

foreach (IQuote quote in quotes)  // simulating stream
{
  quoteHub.Add(quote);
}

IReadOnlyList<AlmaResult> results = observer.Results;
```
