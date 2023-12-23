---
title: Zig Zag
description: Zig Zag is a financial market price chart overlay that simplifies the up and down movements and transitions based on a percent change smoothing threshold.
permalink: /indicators/ZigZag/
image: /assets/charts/ZigZag.png
type: price-transform
layout: indicator
---

# {{ page.title }}

[Zig Zag](https://school.stockcharts.com/doku.php?id=technical_indicators:zigzag) is a price chart overlay that simplifies the up and down movements and transitions based on a percent change smoothing threshold.
[[Discuss] &#128172;]({{site.github.repository_url}}/discussions/226 "Community discussion about this indicator")

![chart for {{page.title}}]({{site.baseurl}}{{page.image}})

```csharp
// C# usage syntax
IEnumerable<ZigZagResult> results =
  quotes.GetZigZag(endType, percentChange);
```

## Parameters

**`endType`** _`EndType`_ - Determines whether `Close` or `High/Low` are used to measure percent change.  See [EndType options](#endtype-options) below.  Default is `EndType.Close`.

**`percentChange`** _`decimal`_ - Percent change required to establish a line endpoint.  Example: 3.5% would be entered as 3.5 (not 0.035).  Must be greater than 0.  Typical values range from 3 to 10.  Default is 5.

### Historical quotes requirements

You must have at least two periods of `quotes` to cover the warmup periods, but notably more is needed to be useful.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

### EndType options

**`EndType.Close`** - Percent change measured from `Close` price (default)

**`EndType.HighLow`** - Percent change measured from `High` and `Low` price

## Response

```csharp
IEnumerable<ZigZagResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- If you do not supply enough points to cover the percent change, there will be no Zig Zag points or lines.
- The first line segment starts after the first confirmed point; ZigZag values before the first confirmed point will be `null`.
- The last line segment is an approximation as the direction is indeterminate.

> &#128681; **Warning**: depending on the specified `endType`, the indicator cannot be initialized if the first `Quote` in `quotes` has a `High`,`Low`, or `Close` value of 0 (zero).
>
> &#128073; **Repaint warning**: the last line segment will always be redrawn back to the last known pivot.  Do not attempt to calculate incremental values since previous values may change based on newer quotes.

### ZigZagResult

**`Date`** _`DateTime`_ - Date from evaluated `TQuote`

**`ZigZag`** _`decimal`_ - Zig Zag line for `percentChange`

**`PointType`** _`string`_ - Zig Zag endpoint type (`H` for high point, `L` for low point)

**`RetraceHigh`** _`decimal`_ - Retrace line for high points

**`RetraceLow`** _`decimal`_ - Retrace line for low points

### Utilities

- [.Condense()]({{site.baseurl}}/utilities#condense)
- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Chaining

Results can be further processed on `ZigZag` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .GetZigZag(..)
    .GetSlope(..);
```

This indicator must be generated from `quotes` and **cannot** be generated from results of another chain-enabled indicator or method.
