---
title: Hurst Exponent
description: Hurst Exponent (H) with Rescaled Range Analysis is a random-walk path analysis that measures trending and mean-reverting tendencies of incremental return values.  When H is greater than 0.5 it depicts trending.  When H is less than 0.5 it is is more likely to revert to the mean.  When H is around 0.5 it represents a random walk.
permalink: /indicators/Hurst/
image: /assets/charts/Hurst.png
type: price-characteristic
layout: indicator
---

# {{ page.title }}

The [Hurst Exponent](https://en.wikipedia.org/wiki/Hurst_exponent) (`H`) is part of a Rescaled Range Analysis, a [random-walk](https://en.wikipedia.org/wiki/Random_walk) path analysis that measures trending and mean-reverting tendencies of incremental return values.  When `H` is greater than 0.5 it depicts trending.  When `H` is less than 0.5 it is is more likely to revert to the mean.  When `H` is around 0.5 it represents a random walk.
[[Discuss] &#128172;]({{site.github.repository_url}}/discussions/477 "Community discussion about this indicator")

![chart for {{page.title}}]({{site.baseurl}}{{page.image}})

```csharp
// C# usage syntax
IReadOnlyList<HurstResult> results =
  quotes.ToHurst(lookbackPeriods);
```

## Parameters

**`lookbackPeriods`** _`int`_ - Number of periods (`N`) in the Hurst Analysis.  Must be greater than 20.  Default is 100.

### Historical quotes requirements

You must have at least `N+1` periods of `quotes` to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<HurstResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N` periods will have `null` values since there's not enough data to calculate.

### HurstResult

**`Timestamp`** _`DateTime`_ - date from evaluated `TQuote`

**`HurstExponent`** _`double`_ - Hurst Exponent (`H`)

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
    .Use(CandlePart.HLC3)
    .ToHurst(..);
```

Results can be further processed on `HurstExponent` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .ToHurst(..)
    .ToSlope(..);
```

## Streaming

Use the buffer-style `List<T>` when you need incremental calculations without a hub:

```csharp
HurstList hurstList = new(lookbackPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  hurstList.Add(quote);
}

// based on `ICollection<HurstResult>`
IReadOnlyList<HurstResult> results = hurstList;
```

Subscribe to a `QuoteHub` for advanced streaming scenarios:

```csharp
QuoteHub quoteHub = new();
HurstHub observer = quoteHub.ToHurstHub(lookbackPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  quoteHub.Add(quote);
}

IReadOnlyList<HurstResult> results = observer.Results;
```
