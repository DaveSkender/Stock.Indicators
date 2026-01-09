---
title: Average True Range (ATR)
description: Created by J. Welles Wilder, True Range and Average True Range is a measure of volatility that captures gaps and limits between periods.
permalink: /indicators/Atr/
image: /assets/charts/Atr.png
type: price-characteristic
layout: indicator
---

# {{ page.title }}

Created by J. Welles Wilder, True Range and [Average True Range](https://en.wikipedia.org/wiki/Average_true_range) is a measure of volatility that captures gaps and limits between periods.
[[Discuss] &#128172;]({{site.github.repository_url}}/discussions/269 "Community discussion about this indicator")

![chart for {{page.title}}]({{site.baseurl}}{{page.image}})

```csharp
// C# usage syntax
IReadOnlyList<AtrResult> results =
  quotes.ToAtr(lookbackPeriods);

// ATR with custom moving average
IReadOnlyList<SmmaResult> results =
  quotes.ToTr().ToSmma(lookbackPeriods);

// raw True Range (TR) only
IReadOnlyList<TrResult> results =
  quote.ToTr();
```

## Parameters

**`lookbackPeriods`** _`int`_ - Number of periods (`N`) to consider.  Must be greater than 1.

### Historical quotes requirements

You must have at least `N+100` periods of `quotes` to cover the [warmup and convergence]({{site.github.repository_url}}/discussions/688) periods.  Since this uses a smoothing technique, we recommend you use at least `N+250` data points prior to the intended usage date for better precision.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<AtrResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N` periods will have `null` values for ATR since there's not enough data to calculate.

>&#9886; **Convergence warning**: The first `N+100` periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods.

### AtrResult

**`Timestamp`** _`DateTime`_ - date from evaluated `TQuote`

**`Tr`** _`double`_ - True Range for current period

**`Atr`** _`double`_ - Average True Range

**`Atrp`** _`double`_ - Average True Range Percent is `(ATR/Price)*100`.  This normalizes so it can be compared to other stocks.

### Utilities

- [.Condense()]({{site.baseurl}}/utilities#condense)
- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()]({{site.baseurl}}/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Chaining

Results can be further processed on `Atrp` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .ToAtr(..)
    .ToSlope(..);
```

This indicator must be generated from `quotes` and **cannot** be generated from results of another chain-enabled indicator or method.

## Streaming

Use the buffer-style `List<T>` when you need incremental calculations without a hub:

```csharp
AtrList atrList = new(lookbackPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  atrList.Add(quote);
}

// based on `ICollection<AtrResult>`
IReadOnlyList<AtrResult> results = atrList;
```

Subscribe to a `QuoteHub` for advanced streaming scenarios:

```csharp
QuoteHub quoteHub = new();
AtrHub observer = quoteHub.ToAtrHub(lookbackPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  quoteHub.Add(quote);
}

IReadOnlyList<AtrResult> results = observer.Results;
```
