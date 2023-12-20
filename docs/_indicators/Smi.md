---
title: Stochastic Momentum Index (SMI)
description: Created by William Blau, the Stochastic Momentum Index (SMI) oscillator is a double-smoothed variant of the traditional Stochastic Oscillator, depicted on a scale from -100 to 100.
permalink: /indicators/Smi/
image: /assets/charts/Smi.png
type: oscillator
layout: indicator
---

# {{ page.title }}

Created by William Blau, the Stochastic Momentum Index (SMI) oscillator is a double-smoothed variant of the [Stochastic Oscillator]({{site.baseurl}}/indicators/Stoch/#content), depicted on a scale from -100 to 100.
[[Discuss] &#128172;]({{site.github.repository_url}}/discussions/625 "Community discussion about this indicator")

![chart for {{page.title}}]({{site.baseurl}}{{page.image}})

```csharp
// C# usage syntax (standard)
IEnumerable<SmiResult> results =
  quotes.GetSmi(lookbackPeriods, firstSmoothPeriods,
                 secondSmoothPeriods, signalPeriods);
```

## Parameters

**`lookbackPeriods`** _`int`_ - Lookback period (`N`) for the stochastic.  Must be greater than 0.  Default is 13.

**`firstSmoothPeriods`** _`int`_ - First smoothing factor lookback.  Must be greater than 0.  Default is 25.

**`secondSmoothPeriods`** _`int`_ - Second smoothing factor lookback.  Must be greater than 0.  Default is 2.

**`signalPeriods`** _`int`_ - EMA of SMI lookback periods.  Must be greater than 0. Default is 3.

### Historical quotes requirements

You must have at least `N+100` periods of `quotes` to cover the convergence periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<SmiResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N-1` periods will have `null` SMI values since there's not enough data to calculate.

>&#9886; **Convergence warning**: The first `N+100` periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods.

### SmiResult

**`Date`** _`DateTime`_ - Date from evaluated `TQuote`

**`Smi`** _`double`_ - Stochastic Momentum Index (SMI)

**`Signal`** _`double`_ - Signal line: an Exponential Moving Average (EMA) of SMI

### Utilities

- [.Condense()]({{site.baseurl}}/utilities#condense)
- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()]({{site.baseurl}}/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Chaining

Results can be further processed on `Smi` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .GetSmi(..)
    .GetSlope(..);
```

This indicator must be generated from `quotes` and **cannot** be generated from results of another chain-enabled indicator or method.
