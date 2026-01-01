---
title: Chande Momentum Oscillator (CMO)
description: The Chande Momentum Oscillator is a momentum indicator depicting the weighted percent of higher prices in financial markets.
permalink: /indicators/Cmo/
image: /assets/charts/Cmo.png
type: oscillator
layout: indicator
---

# {{ page.title }}

Created by Tushar Chande, the [Chande Momentum Oscillator](https://www.investopedia.com/terms/c/chandemomentumoscillator.asp) is a weighted percent of higher prices over a lookback window.
[[Discuss] &#128172;]({{site.github.repository_url}}/discussions/892 "Community discussion about this indicator")

![chart for {{page.title}}]({{site.baseurl}}{{page.image}})

```csharp
// C# usage syntax
IReadOnlyList<CmoResult> results =
  quotes.ToCmo(lookbackPeriods);
```

## Parameters

**`lookbackPeriods`** _`int`_ - Number of periods (`N`) in the lookback window.  Must be greater than 0.

### Historical quotes requirements

You must have at least `N+1` periods of `quotes` to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<CmoResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N` periods will have `null` values for CMO since there's not enough data to calculate.

### CmoResult

**`Timestamp`** _`DateTime`_ - date from evaluated `TQuote`

**`Cmo`** _`double`_ - Chande Momentum Oscillator

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
    .ToCmo(..);
```

Results can be further processed on `Cmo` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .ToCmo(..)
    .ToEma(..);
```

## Streaming

Use the buffer-style `List<T>` when you need incremental calculations without a hub:

```csharp
CmoList cmoList = new(lookbackPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  cmoList.Add(quote);
}

// based on `ICollection<CmoResult>`
IReadOnlyList<CmoResult> results = cmoList;
```

Subscribe to a `QuoteHub` for advanced streaming scenarios:

```csharp
QuoteHub quoteHub = new();
CmoHub observer = quoteHub.ToCmoHub(lookbackPeriods);

foreach (IQuote quote in quotes)  // simulating stream  // simulating stream
{
  quoteHub.Add(quote);
}

IReadOnlyList<CmoResult> results = observer.Results;
```

See the [guide]({{site.baseurl}}/guide/) for more information.
