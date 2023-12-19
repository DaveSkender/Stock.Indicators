---
title: Williams %R
description: Created by Larry Williams, the Williams %R momentum oscillator compares current price with recent highs and lows and is presented on scale of -100 to 0.  It is exactly the same as the fast variant of Stochastic Oscillator, but with a different scaling.
permalink: /indicators/WilliamsR/
image: /assets/charts/WilliamsR.png
type: oscillator
layout: indicator
---

# {{ page.title }}

Created by Larry Williams, the [Williams %R](https://en.wikipedia.org/wiki/Williams_%25R) momentum oscillator compares current price with recent highs and lows and is presented on scale of -100 to 0.  It is exactly the same as the fast variant of [Stochastic Oscillator]({{site.baseurl}}/indicators/Stoch/#content), but with a different scaling.
[[Discuss] &#128172;]({{site.github.repository_url}}/discussions/229 "Community discussion about this indicator")

![chart for {{page.title}}]({{site.baseurl}}{{page.image}})

```csharp
// C# usage syntax
IEnumerable<WilliamsResult> results =
  quotes.GetWilliamsR(lookbackPeriods);
```

## Parameters

**`lookbackPeriods`** _`int`_ - Number of periods (`N`) in the lookback period.  Must be greater than 0.  Default is 14.

### Historical quotes requirements

You must have at least `N` periods of `quotes` to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<WilliamsResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N-1` periods will have `null` Oscillator values since there's not enough data to calculate.

### WilliamsResult

**`Date`** _`DateTime`_ - Date from evaluated `TQuote`

**`WilliamsR`** _`double`_ - Oscillator over prior `N` lookback periods

### Utilities

- [.Condense()]({{site.baseurl}}/utilities#condense)
- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()]({{site.baseurl}}/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Chaining

Results can be further processed on `WilliamsR` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .GetWilliamsR(..)
    .GetSlope(..);
```

This indicator must be generated from `quotes` and **cannot** be generated from results of another chain-enabled indicator or method.
