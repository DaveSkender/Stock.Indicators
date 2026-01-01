---
title: Rate of Change (ROC)
description: Rate of Change, also known as Momentum Oscillator, is the percent change of price over a lookback window.  Momentum is the raw price change equivalent.
permalink: /indicators/Roc/
image: /assets/charts/Roc.png
type: price-characteristic
layout: indicator
---

# {{ page.title }}

[Rate of Change](https://en.wikipedia.org/wiki/Momentum_(technical_analysis)), also known as Momentum Oscillator, is the percent change of price over a lookback window.  Momentum is the raw price change equivalent.  A [Rate of Change with Bands]({{site.baseurl}}/indicators/RocWb/#content) variant, created by Vitali Apirine, is also available.
[[Discuss] &#128172;]({{site.github.repository_url}}/discussions/242 "Community discussion about this indicator")

![chart for {{page.title}}]({{site.baseurl}}{{page.image}})

```csharp
// C# usage syntax
IReadOnlyList<RocResult> results =
  quotes.ToRoc(lookbackPeriods);
```

## Parameters

**`lookbackPeriods`** _`int`_ - Number of periods (`N`) to go back.  Must be greater than 0.

### Historical quotes requirements

You must have at least `N+1` periods of `quotes` to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<RocResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N` periods will have `null` values for ROC since there's not enough data to calculate.

### RocResult

**`Timestamp`** _`DateTime`_ - date from evaluated `TQuote`

**`Momentum`** _`double`_ - Raw change in price over `N` periods

**`Roc`** _`double`_ - Percent change in price (%, not decimal)

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
    .ToRoc(..);
```

Results can be further processed on `Roc` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .ToRoc(..)
    .ToEma(..);
```

## Streaming

Use the buffer-style `List<T>` when you need incremental calculations without a hub:

```csharp
RocList rocList = new(lookbackPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  rocList.Add(quote);
}

// based on `ICollection<RocResult>`
IReadOnlyList<RocResult> results = rocList;
```

Subscribe to a `QuoteHub` for advanced streaming scenarios:

```csharp
QuoteHub quoteHub = new();
RocHub observer = quoteHub.ToRocHub(lookbackPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  quoteHub.Add(quote);
}

IReadOnlyList<RocResult> results = observer.Results;
```
