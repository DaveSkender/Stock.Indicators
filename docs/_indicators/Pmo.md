---
title: Price Momentum Oscillator (PMO)
description: Created by Carl Swenlin, the DecisionPoint Price Momentum Oscillator is double-smoothed momentum indicator, based on Rate of Change (ROC).
permalink: /indicators/Pmo/
image: /assets/charts/Pmo.png
type: price-characteristic
layout: indicator
---

# {{ page.title }}

Created by Carl Swenlin, the DecisionPoint [Price Momentum Oscillator](https://school.stockcharts.com/doku.php?id=technical_indicators:dppmo) is double-smoothed momentum indicator based on Rate of Change (ROC).
[[Discuss] &#128172;]({{site.github.repository_url}}/discussions/244 "Community discussion about this indicator")

![chart for {{page.title}}]({{site.baseurl}}{{page.image}})

```csharp
// C# usage syntax
IReadOnlyList<PmoResult> results =
  quotes.ToPmo(timePeriods, smoothPeriods, signalPeriods);
```

## Parameters

**`timePeriods`** _`int`_ - Number of periods (`T`) for first ROC smoothing.  Must be greater than 1.  Default is 35.

**`smoothPeriods`** _`int`_ - Number of periods (`S`) for second PMO smoothing.  Must be greater than 0.  Default is 20.

**`signalPeriods`** _`int`_ - Number of periods (`G`) for Signal line EMA.  Must be greater than 0.  Default is 10.

### Historical quotes requirements

You must have at least `N` periods of `quotes`, where `N` is the greater of `T+S`,`2×T`, or `T+100` to cover the [warmup and convergence]({{site.github.repository_url}}/discussions/688) periods.  Since this uses multiple smoothing operations, we recommend you use at least `N+250` data points prior to the intended usage date for better precision.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<PmoResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `T+S-1` periods will have `null` values for PMO since there's not enough data to calculate.

>&#9886; **Convergence warning**: The first `T+S+250` periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods.

### PmoResult

**`Timestamp`** _`DateTime`_ - date from evaluated `TQuote`

**`Pmo`** _`double`_ - Price Momentum Oscillator

**`Signal`** _`double`_ - Signal line is EMA of PMO

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
    .ToPmo(..);
```

Results can be further processed on `Pmo` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .ToPmo(..)
    .ToRsi(..);
```

## Streaming

Use the buffer-style `List<T>` when you need incremental calculations without a hub:

```csharp
PmoList pmoList = new(timePeriods, smoothPeriods, signalPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  pmoList.Add(quote);
}

// based on `ICollection<PmoResult>`
IReadOnlyList<PmoResult> results = pmoList;
```

Subscribe to a `QuoteHub` for advanced streaming scenarios:

```csharp
QuoteHub quoteHub = new();
PmoHub observer = quoteHub.ToPmoHub(timePeriods, smoothPeriods, signalPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  quoteHub.Add(quote);
}

IReadOnlyList<PmoResult> results = observer.Results;
```
