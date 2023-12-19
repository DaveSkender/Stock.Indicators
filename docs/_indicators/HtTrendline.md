---
title: Hilbert Transform Instantaneous Trendline
description: Created by John Ehlers, the Hilbert Transform Instantaneous Trendline is a 5-period trendline of high/low price that that uses classic electrical radio-frequency signal processing algorithms reduce noise.
permalink: /indicators/HtTrendline/
image: /assets/charts/HtTrendline.png
type: moving-average
layout: indicator
---

# {{ page.title }}

Created by John Ehlers, the Hilbert Transform Instantaneous Trendline is a 5-period trendline of high/low price that that uses classic electrical radio-frequency signal processing algorithms reduce noise.  Dominant Cycle Periods information is also provided.
[[Discuss] &#128172;]({{site.github.repository_url}}/discussions/363 "Community discussion about this indicator")

![chart for {{page.title}}]({{site.baseurl}}{{page.image}})

```csharp
// C# usage syntax
IEnumerable<HtlResult> results =
  quotes.GetHtTrendline();
```

## Historical quotes requirements

You must have at least `100` periods of `quotes` to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<HtlResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `6` periods will have `null` values for `SmoothPrice` since there's not enough data to calculate.
- The first `7` periods will have `null` values for `DcPeriods` since there is not enough data to calculate; and are generally unreliable for the first ~25 periods.

>&#9886; **Convergence warning**: The first `100` periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods.

### HtlResult

**`Date`** _`DateTime`_ - Date from evaluated `TQuote`

**`DcPeriods`** _`int`_ - Dominant cycle periods (smoothed)

**`Trendline`** _`double`_ - HT Trendline

**`SmoothPrice`** _`double`_ - Weighted moving average of `(H+L)/2` price

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
    .Use(CandlePart.HLC3)
    .GetHtTrendline(..);
```

Results can be further processed on `Trendline` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .GetHtTrendline(..)
    .GetRsi(..);
```
