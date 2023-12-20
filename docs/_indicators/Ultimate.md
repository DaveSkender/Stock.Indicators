---
title: Ultimate Oscillator
description: Created by Larry Williams, the Ultimate Oscillator uses several moving averages to weigh buying power against true range price to produce on oversold / overbought oscillator.
permalink: /indicators/Ultimate/
image: /assets/charts/Ultimate.png
type: oscillator
layout: indicator
---

# {{ page.title }}

Created by Larry Williams, the [Ultimate Oscillator](https://en.wikipedia.org/wiki/Ultimate_oscillator) uses several moving averages to weigh buying power against true range price to produce on oversold / overbought oscillator.
[[Discuss] &#128172;]({{site.github.repository_url}}/discussions/231 "Community discussion about this indicator")

![chart for {{page.title}}]({{site.baseurl}}{{page.image}})

```csharp
// C# usage syntax
IEnumerable<UltimateResult> results =
  quotes.GetUltimate(shortPeriods, middlePeriods, longPeriods);
```

## Parameters

**`shortPeriods`** _`int`_ - Number of periods (`S`) in the short lookback.  Must be greater than 0.  Default is 7.

**`middlePeriods`** _`int`_ - Number of periods (`M`) in the middle lookback.  Must be greater than `S`.  Default is 14.

**`longPeriods`** _`int`_ - Number of periods (`L`) in the long lookback.  Must be greater than `M`.  Default is 28.

### Historical quotes requirements

You must have at least `L+1` periods of `quotes` to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<UltimateResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `L-1` periods will have `null` Ultimate values since there's not enough data to calculate.

### UltimateResult

**`Date`** _`DateTime`_ - Date from evaluated `TQuote`

**`Ultimate`** _`double`_ - Ultimate Oscillator

### Utilities

- [.Condense()]({{site.baseurl}}/utilities#condense)
- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()]({{site.baseurl}}/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Chaining

Results can be further processed on `Ultimate` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .GetUltimate(..)
    .GetSlope(..);
```

This indicator must be generated from `quotes` and **cannot** be generated from results of another chain-enabled indicator or method.
