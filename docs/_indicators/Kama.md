---
title: Kaufman's Adaptive Moving Average (KAMA)
description: Created by Perry Kaufman, KAMA is an volatility adaptive (adjusted) moving average of price over configurable lookback periods.
permalink: /indicators/Kama/
image: /assets/charts/Kama.png
type: moving-average
layout: indicator
---

# {{ page.title }}

Created by Perry Kaufman, [KAMA](https://school.stockcharts.com/doku.php?id=technical_indicators:kaufman_s_adaptive_moving_average) is an volatility adaptive moving average of price over configurable lookback periods.
[[Discuss] &#128172;]({{site.github.repository_url}}/discussions/210 "Community discussion about this indicator")

![chart for {{page.title}}]({{site.baseurl}}{{page.image}})

```csharp
// C# usage syntax
IReadOnlyList<KamaResult> results =
  quotes.GetKama(erPeriods, fastPeriods, slowPeriods);
```

## Parameters

**`erPeriods`** _`int`_ - Number of Efficiency Ratio (volatility) periods (`E`).  Must be greater than 0.  Default is 10.

**`fastPeriods`** _`int`_ - Number of Fast EMA periods.  Must be greater than 0.  Default is 2.

**`slowPeriods`** _`int`_ - Number of Slow EMA periods.  Must be greater than `fastPeriods`.  Default is 30.

### Historical quotes requirements

You must have at least `6×E` or `E+100` periods of `quotes`, whichever is more, to cover the [warmup and convergence]({{site.github.repository_url}}/discussions/688) periods.  Since this uses a smoothing technique, we recommend you use at least `10×E` data points prior to the intended usage date for better precision.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<KamaResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `E-1` periods will have `null` values since there's not enough data to calculate.

>&#9886; **Convergence warning**: The first `10×E` periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods.

### KamaResult

**`Timestamp`** _`DateTime`_ - date from evaluated `TQuote`

**`ER`** _`double`_ - Efficiency Ratio is the fractal efficiency of price changes

**`Kama`** _`double`_ - Kaufman's adaptive moving average

More about Efficiency Ratio: ER fluctuates between 0 and 1, but these extremes are the exception, not the norm. ER would be 1 if prices moved up or down consistently over the `erPeriods` window. ER would be zero if prices are unchanged over the `erPeriods` window.

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
    .GetKama(..);
```

Results can be further processed on `Kama` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .GetKama(..)
    .GetRsi(..);
```
