---
title: Aroon
description: Created by Tushar Chande, Aroon is a oscillator view of how long ago the new high or low price occurred.
permalink: /indicators/Aroon/
image: /assets/charts/Aroon.png
type: price-trend
layout: indicator
---

# {{ page.title }}

Created by Tushar Chande, [Aroon](https://school.stockcharts.com/doku.php?id=technical_indicators:aroon) is a oscillator view of how long ago the new high or low price occurred.
[[Discuss] &#128172;]({{site.github.repository_url}}/discussions/266 "Community discussion about this indicator")

![chart for {{page.title}}]({{site.baseurl}}{{page.image}})

```csharp
// C# usage syntax
IEnumerable<AroonResult> results =
  quotes.GetAroon(lookbackPeriods);
```

## Parameters

**`lookbackPeriods`** _`int`_ - Number of periods (`N`) for the lookback evaluation.  Must be greater than 0.  Default is 25.

### Historical quotes requirements

You must have at least `N` periods of `quotes` to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<AroonResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N-1` periods will have `null` values for `Aroon` since there's not enough data to calculate.

### AroonResult

**`Date`** _`DateTime`_ - Date from evaluated `TQuote`

**`AroonUp`** _`double`_ - Based on last High price

**`AroonDown`** _`double`_ - Based on last Low price

**`Oscillator`** _`double`_ - AroonUp - AroonDown

### Utilities

- [.Condense()]({{site.baseurl}}/utilities#condense)
- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()]({{site.baseurl}}/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Chaining

Results can be further processed on `Oscillator` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .GetAroon(..)
    .GetSlope(..);
```

This indicator must be generated from `quotes` and **cannot** be generated from results of another chain-enabled indicator or method.
