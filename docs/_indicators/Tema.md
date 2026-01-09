---
title: Triple Exponential Moving Average (TEMA)
description: Created by Patrick G. Mulloy, the Triple Exponential Moving Average is a faster multi-smoothed moving average. TEMA is often confused with the alternative TRIX oscillator.
permalink: /indicators/Tema/
image: /assets/charts/Tema.png
type: moving-average
layout: indicator
redirect_from:
 - /indicators/TripleEma/
---

# {{ page.title }}

Created by Patrick G. Mulloy, the [Triple exponential moving average](https://en.wikipedia.org/wiki/Triple_exponential_moving_average) is a faster multi-smoothed EMA of the price over a lookback window.
[[Discuss] &#128172;]({{site.github.repository_url}}/discussions/808 "Community discussion about this indicator")

![chart for {{page.title}}]({{site.baseurl}}{{page.image}})

```csharp
// C# usage syntax
IReadOnlyList<TemaResult> results =
  quotes.ToTema(lookbackPeriods);
```

## Parameters

**`lookbackPeriods`** _`int`_ - Number of periods (`N`) in the moving average.  Must be greater than 0.

### Historical quotes requirements

You must have at least `N` periods of `quotes` to produce any TEMA values.  However, due to the nature of the smoothing technique, we recommend you use at least `3×N+250` data points prior to the intended usage date for better precision.  See [warmup and convergence]({{site.github.repository_url}}/discussions/688) guidance for more information.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<TemaResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N-1` periods will have `null` values since there's not enough data to calculate.  Also note that we are using the proper [weighted variant](https://en.wikipedia.org/wiki/Triple_exponential_moving_average) for TEMA.  If you prefer the unweighted raw 3 EMAs value, please use the `Ema3` output from the [TRIX]({{site.baseurl}}/indicators/Trix#content) oscillator instead.

**Example for TEMA(20)**:

```text
Period 1-19:  null values (incalculable)
Period 20:    first TEMA value (may have convergence issues)
Period 160+:  fully converged, reliable values
```

>&#9432; **Incalculable periods**: The first `N-1` periods will have `null` values since there's not enough data to calculate.
>
>&#9886; **Convergence warning**: The first `3×N+100` periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods.  Use the `.RemoveWarmupPeriods()` method to remove these potentially unreliable values.

### TemaResult

**`Timestamp`** _`DateTime`_ - date from evaluated `TQuote`

**`Tema`** _`double`_ - Triple exponential moving average

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
    .ToTema(..);
```

Results can be further processed on `Tema` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .ToTema(..)
    .ToRsi(..);
```

## Streaming

Use the buffer-style `List<T>` when you need incremental calculations without a hub:

```csharp
TemaList temaList = new(lookbackPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  temaList.Add(quote);
}

// based on `ICollection<TemaResult>`
IReadOnlyList<TemaResult> results = temaList;
```

Subscribe to a `QuoteHub` for advanced streaming scenarios:

```csharp
QuoteHub quoteHub = new();
TemaHub observer = quoteHub.ToTemaHub(lookbackPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  quoteHub.Add(quote);
}

IReadOnlyList<TemaResult> results = observer.Results;
```
