---
title: Rolling Pivot Points
description: Rolling Pivot Points is a modern update to traditional fixed calendar window Pivot Points.  It depicts support and resistance levels, based on a defined rolling window and offset.
permalink: /indicators/RollingPivots/
image: /assets/charts/RollingPivots.png
type: price-channel
layout: indicator
---

# {{ page.title }}

Created by Dave Skender, Rolling Pivot Points is a modern update to traditional fixed calendar window <a href="{{site.baseurl}}/indicators/PivotPoints/#content" rel="nofollow">Pivot Points</a>.  It depicts support and resistance levels, based on a defined _rolling_ window and offset.
[[Discuss] &#128172;]({{site.github.repository_url}}/discussions/274 "Community discussion about this indicator")

![chart for {{page.title}}]({{site.baseurl}}{{page.image}})

```csharp
// C# usage syntax
IEnumerable<RollingPivotsResult> results =
  quotes.GetRollingPivots(windowPeriods, offsetPeriods, pointType);
```

## Parameters

**`windowPeriods`** _`int`_ - Number of periods (`W`) in the evaluation window.  Must be greater than 0 to calculate; but is typically specified in the 5-20 range.

**`offsetPeriods`** _`int`_ - Number of periods (`F`) to offset the window from the current period.  Must be greater than or equal to 0 and is typically less than or equal to `W`.

**`pointType`** _`PivotPointType`_ - Type of Pivot Point.  Default is `PivotPointType.Standard`

For example, a window of 8 with an offset of 4 would evaluate quotes like: `W W W W W W W W F F  F F C`, where `W` is the window included in the Pivot Point calculation, and `F` is the distance from the current evaluation position `C`.  A `quotes` with daily bars using `W/F` values of `20/10` would most closely match the `month` variant of the traditional [Pivot Points]({{site.baseurl}}/indicators/PivotPoints/#content) indicator.

### Historical quotes requirements

You must have at least `W+F` periods of `quotes` to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

### PivotPointType options

**`PivotPointType.Standard`** - Floor Trading (default)

**`PivotPointType.Camarilla`** - Camarilla

**`PivotPointType.Demark`** - Demark

**`PivotPointType.Fibonacci`** - Fibonacci

**`PivotPointType.Woodie`** - Woodie

## Response

```csharp
IEnumerable<RollingPivotsResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `W+F-1` periods will have `null` values since there's not enough data to calculate.

### RollingPivotsResult

**`Date`** _`DateTime`_ - Date from evaluated `TQuote`

**`R3`** _`decimal`_ - Resistance level 3

**`R2`** _`decimal`_ - Resistance level 2

**`R1`** _`decimal`_ - Resistance level 1

**`PP`** _`decimal`_ - Pivot Point

**`S1`** _`decimal`_ - Support level 1

**`S2`** _`decimal`_ - Support level 2

**`S3`** _`decimal`_ - Support level 3

### Utilities

- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()]({{site.baseurl}}/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Chaining

This indicator is not chain-enabled and must be generated from `quotes`.  It **cannot** be used for further processing by other chain-enabled indicators.
