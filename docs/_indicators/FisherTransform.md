---
title: Ehlers Fisher Transform
description: Created by John Ehlers, the Fisher Transform converts financial market prices into a Gaussian normal distribution.
permalink: /indicators/FisherTransform/
image: /assets/charts/FisherTransform.png
type: price-transform
layout: indicator
---

# {{ page.title }}

Created by John Ehlers, the [Fisher Transform](https://www.investopedia.com/terms/f/fisher-transform.asp) converts prices into a Gaussian normal distribution.
[[Discuss] &#128172;]({{site.github.repository_url}}/discussions/409 "Community discussion about this indicator")

![chart for {{page.title}}]({{site.baseurl}}{{page.image}})

```csharp
// C# usage syntax
IReadOnlyList<FisherTransformResult> results =
  quotes.ToFisherTransform(lookbackPeriods);
```

## Parameters

**`lookbackPeriods`** _`int`_ - Number of periods (`N`) in the lookback window.  Must be greater than 0.  Default is 10.

### Historical quotes requirements

You must have at least `N` periods of `quotes` to cover the [warmup and convergence]({{site.github.repository_url}}/discussions/688) periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<FisherTransformResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.

>&#9886; **Convergence warning**: The first `N+15` warmup periods will have unusable decreasing magnitude, convergence-related precision errors that can be as high as ~25% deviation in earlier indicator values.

### FisherTransformResult

**`Timestamp`** _`DateTime`_ - date from evaluated `TQuote`

**`Fisher`** _`double`_ - Fisher Transform

**`Trigger`** _`double`_ - FT offset by one period

### Utilities

- [.Condense()]({{site.baseurl}}/utilities#condense)
- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

For pruning of warmup periods, we recommend using the following guidelines:

```csharp
quotes.ToFisherTransform(lookbackPeriods)
  .RemoveWarmupPeriods(lookbackPeriods+15);
```

See [Utilities and helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Chaining

This indicator may be generated from any chain-enabled indicator or method.

```csharp
// example
var results = quotes
    .Use(CandlePart.HL2)
    .ToFisherTransform(..);
```

Results can be further processed on `Alma` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .ToFisherTransform(..)
    .ToRsi(..);
```

## Streaming

Use the buffer-style `List<T>` when you need incremental calculations without a hub:

```csharp
FisherTransformList fisherList = new(lookbackPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  fisherList.Add(quote);
}

// based on `ICollection<FisherTransformResult>`
IReadOnlyList<FisherTransformResult> results = fisherList;
```

Subscribe to a `QuoteHub` for advanced streaming scenarios:

```csharp
QuoteHub quoteHub = new();
FisherTransformHub observer = quoteHub.ToFisherTransformHub(lookbackPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  quoteHub.Add(quote);
}

IReadOnlyList<FisherTransformResult> results = observer.Results;
```
