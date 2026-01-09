---
title: Simple Moving Average (SMA)
description: Simple moving average.  Extended to include mean absolute deviation, mean square error, and mean absolute percentage error
permalink: /indicators/Sma/
image: /assets/charts/Sma.png
type: moving-average
layout: indicator
---

# {{ page.title }}

[Simple Moving Average](https://en.wikipedia.org/wiki/Moving_average#Simple_moving_average) is the average price over a lookback window.  An [extended analysis](#analysis) option includes mean absolute deviation (MAD), mean square error (MSE), and mean absolute percentage error (MAPE).
[[Discuss] &#128172;]({{site.github.repository_url}}/discussions/240 "Community discussion about this indicator")

![chart for {{page.title}}]({{site.baseurl}}{{page.image}})

```csharp
// C# usage syntax (with Close price)
IReadOnlyList<SmaResult> results =
  quotes.ToSma(lookbackPeriods);
```

## Parameters

**`lookbackPeriods`** _`int`_ - Number of periods (`N`) in the lookback window.  Must be greater than 0.

### Historical quotes requirements

You must have at least `N` periods of `quotes` to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<SmaResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N-1` periods will have `null` values since there's not enough data to calculate.

### SmaResult

**`Timestamp`** _`DateTime`_ - date from evaluated `TQuote`

**`Sma`** _`double`_ - Simple moving average

### Utilities

- [.Condense()]({{site.baseurl}}/utilities#condense)
- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()]({{site.baseurl}}/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Analysis

This indicator has an extended version with more analysis.

```csharp
// C# usage syntax
IEnumberable<SmaAnalysis> analysis =
  results.ToSmaAnalysis();
```

### SmaAnalysis

**`Timestamp`** _`DateTime`_ - date from evaluated `TQuote`

**`Sma`** _`decimal`_ - Simple moving average

**`Mad`** _`double`_ - Mean absolute deviation

**`Mse`** _`double`_ - Mean square error

**`Mape`** _`double`_ - Mean absolute percentage error

## Chaining

This indicator may be generated from any chain-enabled indicator or method.

```csharp
// example
var results = quotes
    .Use(CandlePart.Volume)
    .ToSma(..);
```

Results can be further processed on `Sma` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .ToSma(..)
    .ToRsi(..);
```

## Streaming

Use the buffer-style `List<T>` when you need incremental calculations without a hub:

```csharp
SmaList smaList = new(lookbackPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  smaList.Add(quote);
}

// based on `ICollection<SmaResult>`
IReadOnlyList<SmaResult> results = smaList;
```

Subscribe to a `QuoteHub` for advanced streaming scenarios:

```csharp
QuoteHub quoteHub = new();
SmaHub observer = quoteHub.ToSmaHub(lookbackPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  quoteHub.Add(quote);
}

IReadOnlyList<SmaResult> results = observer.Results;
```
