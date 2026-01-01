---
title: Endpoint Moving Average (EPMA)
description: Endpoint Moving Average (EPMA), also known as Least Squares Moving Average (LSMA), plots the projected last point of a defined retrospective linear regression.
permalink: /indicators/Epma/
image: /assets/charts/Epma.png
type: moving-average
layout: indicator
---

# {{ page.title }}

Endpoint Moving Average (EPMA), also known as Least Squares Moving Average (LSMA), plots the projected last point of a defined retrospective linear regression.
[[Discuss] &#128172;]({{site.github.repository_url}}/discussions/371 "Community discussion about this indicator")

![chart for {{page.title}}]({{site.baseurl}}{{page.image}})

```csharp
// C# usage syntax
IReadOnlyList<EpmaResult> results =
  quotes.ToEpma(lookbackPeriods);
```

## Parameters

**`lookbackPeriods`** _`int`_ - Number of periods (`N`) in the moving average.  Must be greater than 0.

### Historical quotes requirements

You must have at least `N` periods of `quotes` to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<EpmaResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N-1` periods will have `null` values since there's not enough data to calculate.

### EpmaResult

**`Timestamp`** _`DateTime`_ - date from evaluated `TQuote`

**`Epma`** _`double`_ - Endpoint moving average

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
    .ToEpma(..);
```

Results can be further processed on `Epma` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .ToEpma(..)
    .ToRsi(..);
```

## Streaming

Use the buffer-style `List<T>` when you need incremental calculations without a hub:

```csharp
EpmaList epmaList = new(lookbackPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  epmaList.Add(quote);
}

// based on `ICollection<EpmaResult>`
IReadOnlyList<EpmaResult> results = epmaList;
```

Subscribe to a `QuoteHub` for advanced streaming scenarios:

```csharp
QuoteHub quoteHub = new();
EpmaHub observer = quoteHub.ToEpmaHub(lookbackPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  quoteHub.Add(quote);
}

IReadOnlyList<EpmaResult> results = observer.Results;
```
