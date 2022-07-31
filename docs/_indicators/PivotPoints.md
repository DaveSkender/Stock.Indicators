---
title: Pivot Points
permalink: /indicators/PivotPoints/
type: price-channel
layout: indicator
---

# {{ page.title }}

[Pivot Points](https://en.wikipedia.org/wiki/Pivot_point_(technical_analysis)) depict support and resistance levels, based on the prior lookback window.  You can specify window size (e.g. month, week, day, etc).
See also the alternative [Rolling Pivot Points]({{site.baseurl}}/indicators/RollingPivots/#content) variant for a modern update that uses a rolling window.
[[Discuss] :speech_balloon:]({{site.github.repository_url}}/discussions/274 "Community discussion about this indicator")

![image]({{site.baseurl}}/assets/charts/PivotPoints.png)

```csharp
// usage
IEnumerable<PivotPointsResult> results =
  quotes.GetPivotPoints(windowSize, pointType);
```

## Parameters

| name | type | notes
| -- |-- |--
| `windowSize` | PeriodSize | Size of the lookback window
| `pointType` | PivotPointType | Type of Pivot Point.  Default is `PivotPointType.Standard`

### Historical quotes requirements

You must have at least `2` windows of `quotes` to cover the warmup periods.  For example, if you specify a `Week` window size, you need at least 14 calendar days of `quotes`.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

### PeriodSize options (for windowSize)

| type | description
|-- |--
| `PeriodSize.Month` | Use the prior month's data to calculate current month's Pivot Points
| `PeriodSize.Week` | [..] weekly
| `PeriodSize.Day` | [..] daily.  Commonly used for intraday data.
| `PeriodSize.OneHour` | [..] hourly

### PivotPointType options

| type | description
|-- |--
| `PivotPointType.Standard` | Floor Trading (default)
| `PivotPointType.Camarilla` | Camarilla
| `PivotPointType.Demark` | Demark
| `PivotPointType.Fibonacci` | Fibonacci
| `PivotPointType.Woodie` | Woodie

## Response

```csharp
IEnumerable<PivotPointsResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first window will have `null` values since there's not enough data to calculate.

:warning: **Warning**: The second window may be inaccurate if the first window contains incomplete data.  For example, this can occur if you specify a `Month` window size and only provide 45 calendar days (1.5 months) of `quotes`.

:paintbrush: **Repaint Warning**: the last window will be repainted if it does not contain a full window of data.

### PivotPointsResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `R3` | decimal | Resistance level 3
| `R2` | decimal | Resistance level 2
| `R1` | decimal | Resistance level 1
| `PP` | decimal | Pivot Point
| `S1` | decimal | Support level 1
| `S2` | decimal | Support level 2
| `S3` | decimal | Support level 3

### Utilities

- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()]({{site.baseurl}}/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and Helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Chaining

This indicator is not chain-enabled and must be generated from `quotes`.  It **cannot** be used for further processing by other chain-enabled indicators.
