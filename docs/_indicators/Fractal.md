---
title: Williams Fractal
permalink: /indicators/Fractal/
type: price-pattern
layout: indicator
---

# {{ page.title }}

Created by Larry Williams, [Fractal](https://www.investopedia.com/terms/f/fractal.asp) is a retrospective price pattern that identifies a central high or low point.
[[Discuss] :speech_balloon:]({{site.github.repository_url}}/discussions/255 "Community discussion about this indicator")

![image]({{site.baseurl}}/assets/charts/Fractal.png)

```csharp
// usage
IEnumerable<FractalResult> results =
  quotes.GetFractal(windowSpan);
```

## Parameters

| name | type | notes
| -- |-- |--
| `windowSpan` | int | Evaluation window span width (`S`).  Must be at least 2.  Default is 2.
| `endType` | EndType | Determines whether `Close` or `High/Low` are used to find end points.  See [EndType options](#endtype-options) below.  Default is `EndType.HighLow`.

The total evaluation window size is `2×S+1`, representing `±S` from the evaluation date.

### Historical quotes requirements

You must have at least `2×S+1` periods of `quotes` to cover the warmup periods; however, more is typically provided since this is a chartable candlestick pattern.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

### EndType options

| type | description
|-- |--
| `EndType.Close` | Chevron point identified from `Close` price
| `EndType.HighLow` | Chevron point identified from `High` and `Low` price (default)

## Response

```csharp
IEnumerable<FractalResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first and last `S` periods in `quotes` are unable to be calculated since there's not enough prior/following data.

:paintbrush: **Repaint Warning**: this price pattern looks forward and backward in the historical quotes so it will never identify a `fractal` in the last `S` periods of `quotes`.  Fractals are retroactively identified.

### FractalResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `FractalBear` | decimal | Value indicates a **high** point; otherwise `null` is returned.
| `FractalBull` | decimal | Value indicates a **low** point; otherwise `null` is returned.

### Utilities

- [.Condense()]({{site.baseurl}}/utilities#condense)
- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and Helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Chaining

This indicator is not chain-enabled and must be generated from `quotes`.  It **cannot** be used for further processing by other chain-enabled indicators.
