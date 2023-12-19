---
title: Volume Weighted Moving Average (VWMA)
description: Volume Weighted Moving Average is the volume adjusted average price over a lookback window.
permalink: /indicators/Vwma/
image: /assets/charts/Vwma.png
type: moving-average
layout: indicator
---

# {{ page.title }}

Volume Weighted Moving Average is the volume adjusted average price over a lookback window.
[[Discuss] &#128172;]({{site.github.repository_url}}/discussions/657 "Community discussion about this indicator")

![chart for {{page.title}}]({{site.baseurl}}{{page.image}})

```csharp
// C# usage syntax
IEnumerable<VwmaResult> results =
  quotes.GetVwma(lookbackPeriods);
```

## Parameters

**`lookbackPeriods`** _`int`_ - Number of periods (`N`) in the moving average.  Must be greater than 0.

### Historical quotes requirements

You must have at least `N` periods of `quotes` to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<VwmaResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N-1` periods will have `null` values for `Vwma` since there's not enough data to calculate.

### VwmaResult

**`Date`** _`DateTime`_ - Date from evaluated `TQuote`

**`Vwma`** _`double`_ - Volume Weighted Moving Average

### Utilities

- [.Condense()]({{site.baseurl}}/utilities#condense)
- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()]({{site.baseurl}}/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Chaining

Results can be further processed on `Vwma` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .GetVwma(..)
    .GetRsi(..);
```

This indicator must be generated from `quotes` and **cannot** be generated from results of another chain-enabled indicator or method.
