---
title: Vortex Indicator (VI)
description: Created by Etienne Botes and Douglas Siepman, the Vortex Indicator is a measure of price directional movement.  It includes positive and negative indicators, and is often used to identify trends and reversals.
permalink: /indicators/Vortex/
image: /assets/charts/Vortex.png
type: price-trend
layout: indicator
---

# {{ page.title }}

Created by Etienne Botes and Douglas Siepman, the [Vortex Indicator](https://en.wikipedia.org/wiki/Vortex_indicator) is a measure of price directional movement.  It includes positive and negative indicators, and is often used to identify trends and reversals.
[[Discuss] &#128172;]({{site.github.repository_url}}/discussions/339 "Community discussion about this indicator")

![chart for {{page.title}}]({{site.baseurl}}{{page.image}})

```csharp
// C# usage syntax
IEnumerable<VortexResult> results =
  quotes.GetVortex(lookbackPeriods);
```

## Parameters

**`lookbackPeriods`** _`int`_ - Number of periods (`N`) to consider.  Must be greater than 1 and is usually between 14 and 30.

### Historical quotes requirements

You must have at least `N+1` periods of `quotes` to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<VortexResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N` periods will have `null` values for VI since there's not enough data to calculate.

### VortexResult

**`Date`** _`DateTime`_ - Date from evaluated `TQuote`

**`Pvi`** _`double`_ - Positive Vortex Indicator (VI+)

**`Nvi`** _`double`_ - Negative Vortex Indicator (VI-)

### Utilities

- [.Condense()]({{site.baseurl}}/utilities#condense)
- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()]({{site.baseurl}}/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Chaining

This indicator is not chain-enabled and must be generated from `quotes`.  It **cannot** be used for further processing by other chain-enabled indicators.
