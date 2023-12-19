---
title: Pivot Points
description: Pivot Points depict classic support and resistance levels, based on prior calendar windows.  You can specify window size (e.g. month, week, day, etc) and any of the traditional Floor Trading, Camarilla, Demark, Fibonacci, and Woodie variants.
permalink: /indicators/PivotPoints/
image: /assets/charts/PivotPoints.png
type: price-channel
layout: indicator
---

# {{ page.title }}

[Pivot Points](https://en.wikipedia.org/wiki/Pivot_point_(technical_analysis)) depict support and resistance levels, based on prior calendar windows.  You can specify window size (e.g. month, week, day, etc) and any of the traditional Floor Trading, Camarilla, Demark, Fibonacci, and Woodie variants.

[[Discuss] &#128172;]({{site.github.repository_url}}/discussions/274 "Community discussion about this indicator")

![chart for {{page.title}}]({{site.baseurl}}{{page.image}})

```csharp
// C# usage syntax
IEnumerable<PivotPointsResult> results =
  quotes.GetPivotPoints(windowSize, pointType);
```

## Parameters

**`windowSize`** _`PeriodSize`_ - Size of the lookback window

**`pointType`** _`PivotPointType`_ - Type of Pivot Point.  Default is `PivotPointType.Standard`

### Historical quotes requirements

You must have at least `2` windows of `quotes` to cover the warmup periods.  For example, if you specify a `Week` window size, you need at least 14 calendar days of `quotes`.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

### PeriodSize options (for windowSize)

**`PeriodSize.Month`** - Use the prior month's data to calculate current month's Pivot Points

**`PeriodSize.Week`** - [..] weekly

**`PeriodSize.Day`** - [..] daily.  Commonly used for intraday data.

**`PeriodSize.OneHour`** - [..] hourly

### PivotPointType options

**`PivotPointType.Standard`** - Floor Trading (default)

**`PivotPointType.Camarilla`** - Camarilla

**`PivotPointType.Demark`** - Demark

**`PivotPointType.Fibonacci`** - Fibonacci

**`PivotPointType.Woodie`** - Woodie

## Response

```csharp
IEnumerable<PivotPointsResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first window will have `null` values since there's not enough data to calculate.

> &#128681; **Warning**: The second window may be inaccurate if the first window contains incomplete data.  For example, this can occur if you specify a `Month` window size and only provide 45 calendar days (1.5 months) of `quotes`.
>
> &#128073; **Repaint warning**: the last window will be repainted if it does not contain a full window of data.

### PivotPointsResult

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
