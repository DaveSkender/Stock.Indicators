---
title: ATR Trailing Stop
description: Created by Welles Wilder, the ATR Trailing Stop indicator attempts to determine the primary trend of financial market prices by using Average True Range (ATR) band thresholds.  It can indicate a buy/sell signal or a trailing stop when the trend changes.
permalink: /indicators/AtrStop/
image: /assets/charts/AtrStop.png
type: price-trend
layout: indicator
---

# {{ page.title }}

Created by Welles Wilder, the ATR Trailing Stop indicator attempts to determine the primary trend of Close prices by using [Average True Range (ATR)]({{site.baseurl}}/indicators/Atr/#content) band thresholds.  It can indicate a buy/sell signal or a trailing stop when the trend changes.
[[Discuss] &#128172;]({{site.github.repository_url}}/discussions/724 "Community discussion about this indicator")

![chart for {{page.title}}]({{site.baseurl}}{{page.image}})

```csharp
// C# usage syntax
IEnumerable<AtrStopResult> results =
  quotes.GetAtrStop(lookbackPeriods, multiplier, endType);
```

## Parameters

**`lookbackPeriods`** _`int`_ - Number of periods (`N`) for the ATR evaluation.  Must be greater than 1.  Default is 21.

**`multiplier`** _`double`_ - Multiplier sets the ATR band width.  Must be greater than 0 and is usually set around 2 to 3.  Default is 3.

**`endType`** _`EndType`_ - Determines whether `Close` or `High/Low` is used as basis for stop offset.  See [EndType options](#endtype-options) below.  Default is `EndType.Close`.

### Historical quotes requirements

You must have at least `N+100` periods of `quotes` to cover the convergence periods.  Since this uses a smoothing technique, we recommend you use at least `N+250` periods prior to the intended usage date for optimal precision.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

### EndType options

**`EndType.Close`** - Stop offset from `Close` price (default)

**`EndType.HighLow`** - Stop offset from `High` or `Low` price

## Response

```csharp
IEnumerable<AtrStopResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N` periods will have `null` AtrStop values since there's not enough data to calculate.

>&#9886; **Convergence warning**: the line segment before the first reversal and the first `N+100` periods are unreliable due to an initial guess of trend direction and precision convergence for the underlying ATR values.

### AtrStopResult

**`Date`** _`DateTime`_ - Date from evaluated `TQuote`

**`AtrStop`** _`decimal`_ - ATR Trailing Stop line contains both Upper and Lower segments

**`BuyStop`** _`decimal`_ - Upper band only (green)

**`SellStop`** _`decimal`_ - Lower band only (red)

`BuyStop` and `SellStop` values are provided to differentiate buy vs sell stop lines and to clearly demark trend reversal.  `AtrStop` is the contiguous combination of both upper and lower line data.

### Utilities

- [.Condense()]({{site.baseurl}}/utilities#condense)
- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()]({{site.baseurl}}/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Chaining

This indicator is not chain-enabled and must be generated from `quotes`.  It **cannot** be used for further processing by other chain-enabled indicators.
