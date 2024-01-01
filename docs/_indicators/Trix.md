---
title: Triple EMA Oscillator (TRIX)
description: Created by Jack Hutson, TRIX is a rolling rate of change for a 3 EMA smoothing of the price over a lookback window.  TRIX is often confused with Triple EMA (TEMA).
permalink: /indicators/Trix/
image: /assets/charts/Trix.png
type: oscillator
layout: indicator
---

# {{ page.title }}

Created by Jack Hutson, [TRIX](https://en.wikipedia.org/wiki/Trix_(technical_analysis)) is the rate of change for a 3 EMA smoothing of the price over a lookback window.  TRIX is often confused with [TEMA]({{site.baseurl}}/indicators/Tema/#content).
[[Discuss] &#128172;]({{site.github.repository_url}}/discussions/234 "Community discussion about this indicator")

![chart for {{page.title}}]({{site.baseurl}}{{page.image}})

```csharp
// C# usage syntax for Trix
IEnumerable<TrixResult> results =
  quotes.GetTrix(lookbackPeriods);

// usage for Trix with Signal Line (shown above)
IEnumerable<TrixResult> results =
  quotes.GetTrix(lookbackPeriods, signalPeriods);
```

## Parameters

**`lookbackPeriods`** _`int`_ - Number of periods (`N`) in each of the the exponential moving averages.  Must be greater than 0.

**`signalPeriods`** _`int`_ - Optional.  Number of periods in the moving average of TRIX.  Must be greater than 0, if specified.

### Historical quotes requirements

You must have at least `4×N` or `3×N+100` periods of `quotes`, whichever is more, to cover the convergence periods.  Since this uses a smoothing technique, we recommend you use at least `3×N+250` data points prior to the intended usage date for better precision.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<TrixResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `3×N-3` periods will have `null` values since there's not enough data to calculate.

>&#9886; **Convergence warning**: The first `3×N+250` periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods.

### TrixResult

**`Date`** _`DateTime`_ - Date from evaluated `TQuote`

**`Ema3`** _`decimal`_ - 3 EMAs of the price

**`Trix`** _`decimal`_ - Rate of Change of 3 EMAs

**`Signal`** _`decimal`_ - SMA of `Trix` based on `signalPeriods` periods, if specified

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
    .GetTrix(..);
```

Results can be further processed on `Trix` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .GetTrix(..)
    .GetRsi(..);
```
