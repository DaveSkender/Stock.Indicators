---
title: Stochastic Oscillator
description: Created by George Lane, the Stochastic Oscillator, also known as KDJ Index, is a momentum oscillator that compares current financial market price with recent highs and lows and is presented on a scale of 0 to 100.  %J is also included for the KDJ Index extension.
permalink: /indicators/Stoch/
image: /assets/charts/Stoch.png
type: oscillator
layout: indicator
---

# {{ page.title }}

Created by George Lane, the [Stochastic Oscillator](https://en.wikipedia.org/wiki/Stochastic_oscillator), also known as KDJ Index, is a momentum oscillator that compares current price with recent highs and lows and is presented on a scale of 0 to 100.
[[Discuss] &#128172;]({{site.github.repository_url}}/discussions/237 "Community discussion about this indicator")

![chart for {{page.title}}]({{site.baseurl}}{{page.image}})

```csharp
// C# usage syntax (standard)
IEnumerable<StochResult> results =
  quotes.GetStoch(lookbackPeriods, signalPeriods, smoothPeriods);

// advanced customization
IEnumerable<StochResult> results =
  quotes.GetStoch(lookbackPeriods, signalPeriods, smoothPeriods,
                  kFactor, dFactor, movingAverageType);
```

## Parameters

**`lookbackPeriods`** _`int`_ - Lookback period (`N`) for the oscillator (%K).  Must be greater than 0.  Default is 14.

**`signalPeriods`** _`int`_ - Smoothing period for the signal (%D).  Must be greater than 0.  Default is 3.

**`smoothPeriods`** _`int`_ - Smoothing period (`S`) for the Oscillator (%K).  "Slow" stochastic uses 3, "Fast" stochastic uses 1.  Must be greater than 0.  Default is 3.

**`kFactor`** _`double`_ - Optional. Weight of %K in the %J calculation.  Must be greater than 0. Default is 3.

**`dFactor`** _`double`_ - Optional. Weight of %D in the %J calculation.  Must be greater than 0. Default is 2.

**`movingAverageType`** _`MaType`_ - Optional. Type of moving average (SMA or SMMA) used for smoothing.  See [MaType options](#matype-options) below.  Default is `MaType.SMA`.

### Historical quotes requirements

You must have at least `N+S` periods of `quotes` to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

### MaType options

These are the supported moving average types:

**`MaType.SMA`** - [Simple Moving Average]({{site.baseurl}}/indicators/Sma/#content) (default)

**`MaType.SMMA`** - [Smoothed Moving Average]({{site.baseurl}}/indicators/Smma/#content)

## Response

```csharp
IEnumerable<StochResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N+S-2` periods will have `null` Oscillator values since there's not enough data to calculate.

>&#9886; **Convergence warning**: The first `N+100` periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods when using `MaType.SMMA`.  Standard use of `MaType.SMA` does not have convergence-related precision errors.

### StochResult

**`Date`** _`DateTime`_ - Date from evaluated `TQuote`

**`Oscillator` or `K`** _`double`_ - %K Oscillator

**`Signal` or `D`** _`double`_ - %D Simple moving average of Oscillator

**`PercentJ` or `J`** _`double`_ - %J is the weighted divergence of %K and %D: `%J = kFactor × %K - dFactor × %D`

Note: aliases of `K`, `D`, and `J` are also provided.  They can be used interchangeably with the standard outputs.

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
    .GetStoch(..)
    .GetSlope(..);
```

This indicator must be generated from `quotes` and **cannot** be generated from results of another chain-enabled indicator or method.
