---
title: Pivots
description: Pivots is an extended customizable version of Williams Fractal that includes identification of Higher High, Lower Low, Higher Low, and Lower Low trends between pivots in a lookback window.
permalink: /indicators/Pivots/
image: /assets/charts/pivots.webp
type: price-pattern
layout: indicator
---

# {{ page.title }}

Pivots is an extended customizable version of <a href="{{site.baseurl}}/indicators/Fractal/#content" rel="nofollow">Williams Fractal</a> that includes identification of Higher High, Lower Low, Higher Low, and Lower Low trends between pivots in a lookback window.
[[Discuss] &#128172;]({{site.github.repository_url}}/discussions/436 "Community discussion about this indicator")

![chart for {{page.title}}]({{site.baseurl}}{{page.image}})

```csharp
// C# usage syntax
IEnumerable<PivotsResult> results =
  quotes.GetPivots(leftSpan, rightSpan, maxTrendPeriods, endType);
```

## Parameters

**`leftSpan`** _`int`_ - Left evaluation window span width (`L`).  Must be at least 2.  Default is 2.

**`rightSpan`** _`int`_ - Right evaluation window span width (`R`).  Must be at least 2.  Default is 2.

**`maxTrendPeriods`** _`int`_ - Number of periods (`N`) in evaluation window.  Must be greater than `leftSpan`.  Default is 20.

**`endType`** _`EndType`_ - Determines whether `Close` or `High/Low` are used to find end points.  See [EndType options](#endtype-options) below.  Default is `EndType.HighLow`.

The total evaluation window size is `L+R+1`.

### Historical quotes requirements

You must have at least `L+R+1` periods of `quotes` to cover the warmup periods; however, more is typically provided since this is a chartable candlestick pattern.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

### EndType options

**`EndType.Close`** - Chevron point identified from `Close` price

**`EndType.HighLow`** - Chevron point identified from `High` and `Low` price (default)

## Response

```csharp
IEnumerable<PivotsResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `L` and last `R` periods in `quotes` are unable to be calculated since there's not enough prior/following data.

> &#128073; **Repaint warning**: this price pattern looks forward and backward in the historical quotes so it will never identify a pivot in the last `R` periods of `quotes`.  Fractals are retroactively identified.

### PivotsResult

**`Date`** _`DateTime`_ - Date from evaluated `TQuote`

**`HighPoint`** _`decimal`_ - Value indicates a **high** point; otherwise `null` is returned.

**`LowPoint`** _`decimal`_ - Value indicates a **low** point; otherwise `null` is returned.

**`HighLine`** _`decimal`_ - Drawn line between two high points in the `maxTrendPeriods`

**`LowLine`** _`decimal`_ - Drawn line between two low points in the `maxTrendPeriods`

**`HighTrend`** _`PivotTrend`_ - Enum that represents higher high or lower high.  See [PivotTrend values](#pivottrend-values) below.

**`LowTrend`** _`PivotTrend`_ - Enum that represents higher low or lower low.  See [PivotTrend values](#pivottrend-values) below.

#### PivotTrend values

**`PivotTrend.HH`** - Higher high

**`PivotTrend.LH`** - Lower high

**`PivotTrend.HL`** - Higher low

**`PivotTrend.LL`** - Lower low

### Utilities

- [.Condense()]({{site.baseurl}}/utilities#condense)
- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Chaining

This indicator is not chain-enabled and must be generated from `quotes`.  It **cannot** be used for further processing by other chain-enabled indicators.
