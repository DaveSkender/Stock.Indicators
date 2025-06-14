---
title: Choppiness Index
description: Created by E.W. Dreiss, the Choppiness Index measures the trendiness or choppiness on a scale of 0 to 100, to depict steady trends versus conditions of choppiness.
permalink: /indicators/Chop/
image: /assets/charts/Chop.png
type: price-characteristic
layout: indicator
---

# {{ page.title }}

Created by E.W. Dreiss, the Choppiness Index measures the trendiness or choppiness on a scale of 0 to 100, to depict steady trends versus conditions of choppiness.  [[Discuss] &#128172;]({{site.github.repository_url}}/discussions/357 "Community discussion about this indicator")

![chart for {{page.title}}]({{site.baseurl}}{{page.image}})

```csharp
// C# usage syntax
IReadOnlyList<ChopResult> results =
  quotes.GetChop(lookbackPeriods);
```

## Parameters

**`lookbackPeriods`** _`int`_ - Number of periods (`N`) for the lookback evaluation.  Must be greater than 1.  Default is 14.

### Historical quotes requirements

You must have at least `N+1` periods of `quotes` to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<ChopResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N` periods will have `null` values since there's not enough data to calculate.

### ChopResult

**`Timestamp`** _`DateTime`_ - date from evaluated `TQuote`

**`Chop`** _`double`_ - Choppiness Index

### Utilities

- [.Condense()]({{site.baseurl}}/utilities#condense)
- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()]({{site.baseurl}}/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Chaining

Results can be further processed on `Chop` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .GetChop(..)
    .GetEma(..);
```

This indicator must be generated from `quotes` and **cannot** be generated from results of another chain-enabled indicator or method.
