---
title: Detrended Price Oscillator (DPO)
description: Detrended Price Oscillator depicts the difference between price and an offset simple moving average.  It is used to identify trend cycles and duration.
permalink: /indicators/Dpo/
image: /assets/charts/Dpo.png
type: oscillator
layout: indicator
---

# {{ page.title }}

[Detrended Price Oscillator](https://en.wikipedia.org/wiki/Detrended_price_oscillator) depicts the difference between price and an offset simple moving average.  It is used to identify trend cycles and duration.
[[Discuss] &#128172;]({{site.github.repository_url}}/discussions/551 "Community discussion about this indicator")

![chart for {{page.title}}]({{site.baseurl}}{{page.image}})

```csharp
// C# usage syntax
IReadOnlyList<DpoResult> results =
  quotes.ToDpo(lookbackPeriods);
```

## Parameters

**`lookbackPeriods`** _`int`_ - Number of periods (`N`) in the moving average.  Must be greater than 0.

### Historical quotes requirements

You must have at least `N` historical quotes to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<DpoResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N/2-2` and last `N/2+1` periods will be `null` since they cannot be calculated.

### DpoResult

**`Timestamp`** _`DateTime`_ - date from evaluated `TQuote`

**`Sma`** _`double`_ - Simple moving average offset by `N/2+1` periods

**`Dpo`** _`double`_ - Detrended Price Oscillator (DPO)

### Utilities

- [.Condense()]({{site.baseurl}}/utilities#condense)
- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Chaining

This indicator may be generated from any chain-enabled indicator or method.

```csharp
// example
var results = quotes
    .Use(CandlePart.HL2)
    .ToDpo(..);
```

Results can be further processed on `Dpo` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .ToDpo(..)
    .ToRsi(..);
```

## Streaming

Use the buffer-style `List<T>` when you need incremental calculations without a hub:

```csharp
DpoList dpoList = new(lookbackPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  dpoList.Add(quote);
}

// based on `ICollection<DpoResult>`
IReadOnlyList<DpoResult> results = dpoList;
```

Subscribe to a `QuoteHub` for advanced streaming scenarios:

```csharp
QuoteHub quoteHub = new();
DpoHub observer = quoteHub.ToDpoHub(lookbackPeriods);

foreach (IQuote quote in quotes)  // simulating stream  // simulating stream
{
  quoteHub.Add(quote);
}

IReadOnlyList<DpoResult> results = observer.Results;
```

**Note**: DPO has a lookahead requirement (offset = N/2+1 periods), which means results are calculated when sufficient future data becomes available. This introduces a delay in real-time scenarios but maintains mathematical accuracy with the series implementation.
