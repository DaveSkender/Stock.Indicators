---
title: Pivots
permalink: /indicators/Pivots/
type: price-pattern
layout: indicator
---

# {{ page.title }}

Pivots is an extended version of [Williams Fractal]({{site.baseurl}}/indicators/Fractal/#content) that includes identification of Higher High, Lower Low, Higher Low, and Lower Low trends between pivots in a lookback window.
[[Discuss] :speech_balloon:]({{site.github.repository_url}}/discussions/436 "Community discussion about this indicator")

![image]({{site.baseurl}}/assets/charts/Pivots.png)

```csharp
// usage
IEnumerable<PivotsResult> results =
  quotes.GetPivots(leftSpan, rightSpan, maxTrendPeriods, endType);
```

## Parameters

| name | type | notes
| -- |-- |--
| `leftSpan` | int | Left evaluation window span width (`L`).  Must be at least 2.  Default is 2.
| `rightSpan` | int | Right evaluation window span width (`R`).  Must be at least 2.  Default is 2.
| `maxTrendPeriods` | int | Number of periods (`N`) in evaluation window.  Must be greater than `leftSpan`.  Default is 20.
| `endType` | EndType | Determines whether `Close` or `High/Low` are used to find end points.  See [EndType options](#endtype-options) below.  Default is `EndType.HighLow`.

The total evaluation window size is `L+R+1`.

### Historical quotes requirements

You must have at least `L+R+1` periods of `quotes` to cover the warmup periods; however, more is typically provided since this is a chartable candlestick pattern.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

### EndType options

| type | description
|-- |--
| `EndType.Close` | Chevron point identified from `Close` price
| `EndType.HighLow` | Chevron point identified from `High` and `Low` price (default)

## Response

```csharp
IEnumerable<PivotsResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `L` and last `R` periods in `quotes` are unable to be calculated since there's not enough prior/following data.

:paintbrush: **Repaint Warning**: this price pattern looks forward and backward in the historical quotes so it will never identify a pivot in the last `R` periods of `quotes`.  Fractals are retroactively identified.

### PivotsResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `HighPoint` | decimal | Value indicates a **high** point; otherwise `null` is returned.
| `LowPoint` | decimal | Value indicates a **low** point; otherwise `null` is returned.
| `HighLine` | decimal | Drawn line between two high points in the `maxTrendPeriods`
| `LowLine` | decimal | Drawn line between two low points in the `maxTrendPeriods`
| `HighTrend` | PivotTrend | Enum that represents higher high or lower high.  See [PivotTrend values](#pivottrend-values) below.
| `LowTrend` | PivotTrend | Enum that represents higher low or lower low.  See [PivotTrend values](#pivottrend-values) below.

#### PivotTrend values

| type | description
|-- |--
| `PivotTrend.HH` | Higher high
| `PivotTrend.LH` | Lower high
| `PivotTrend.HL` | Higher low
| `PivotTrend.LL` | Lower low

### Utilities

- [.Condense()]({{site.baseurl}}/utilities#condense)
- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and Helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Chaining

This indicator is not chain-enabled and must be generated from `quotes`.  It **cannot** be used for further processing by other chain-enabled indicators.
