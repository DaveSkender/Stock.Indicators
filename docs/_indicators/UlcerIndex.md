---
title: Ulcer Index (UI)
description: Created by Peter Martin, the Ulcer Index is a measure of downside price volatility.  Often called the "heart attack" score, it measures the amount of pain seen from drawdowns in financial market prices and portfolio value.
permalink: /indicators/UlcerIndex/
image: /assets/charts/UlcerIndex.png
type: price-characteristic
layout: indicator
---

# {{ page.title }}

Created by Peter Martin, the [Ulcer Index](https://en.wikipedia.org/wiki/Ulcer_index) is a measure of downside price volatility over a lookback window.  Often called the "heart attack" score, it measures the amount of pain seen from drawdowns in financial market prices and portfolio value.
[[Discuss] &#128172;]({{site.github.repository_url}}/discussions/232 "Community discussion about this indicator")

![chart for {{page.title}}]({{site.baseurl}}{{page.image}})

```csharp
// C# usage syntax
IEnumerable<UlcerIndexResult> results =
  quotes.GetUlcerIndex(lookbackPeriods);
```

## Parameters

**`lookbackPeriods`** _`int`_ - Number of periods (`N`) for review.  Must be greater than 0.  Default is 14.

### Historical quotes requirements

You must have at least `N` periods of `quotes` to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<UlcerIndexResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N-1` periods will have `null` values since there's not enough data to calculate.

### UlcerIndexResult

**`Date`** _`DateTime`_ - Date from evaluated `TQuote`

**`UI`** _`double`_ - Ulcer Index

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
    .GetAlma(..);
```

Results can be further processed on `UI` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .GetAlma(..)
    .GetRsi(..);
```
