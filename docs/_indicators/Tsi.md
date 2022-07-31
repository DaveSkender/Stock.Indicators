---
title: True Strength Index (TSI)
permalink: /indicators/Tsi/
type: price-characteristic
layout: indicator
---

# {{ page.title }}

Created by William Blau, the [True Strength Index](https://en.wikipedia.org/wiki/True_strength_index) is a momentum oscillator that depicts trends in price changes.
[[Discuss] :speech_balloon:]({{site.github.repository_url}}/discussions/300 "Community discussion about this indicator")

![image]({{site.baseurl}}/assets/charts/Tsi.png)

```csharp
// usage
IEnumerable<TsiResult> results =
  quotes.GetTsi(lookbackPeriods, smoothPeriods, signalPeriods);
```

## Parameters

| name | type | notes
| -- |-- |--
| `lookbackPeriods` | int | Number of periods (`N`) for the first EMA.  Must be greater than 0.  Default is 25.
| `smoothPeriods` | int | Number of periods (`M`) for the second smoothing.  Must be greater than 0.  Default is 13.
| `signalPeriods` | int | Number of periods (`S`) in the TSI moving average.  Must be greater than or equal to 0.  Default is 7.

### Historical quotes requirements

You must have at least `N+M+100` periods of `quotes` to cover the convergence periods.  Since this uses a two EMA smoothing techniques, we recommend you use at least `N+M+250` data points prior to the intended usage date for better precision.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<TsiResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N+M-1` periods will have `null` values since there's not enough data to calculate.
- `Signal` will be `null` for all periods if `signalPeriods=0`.

:hourglass: **Convergence Warning**: The first `N+M+250` periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods.

### TsiResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Tsi` | double | True Strength Index
| `Signal` | double | Signal line (EMA of TSI)

### Utilities

- [.Condense()]({{site.baseurl}}/utilities#condense)
- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()]({{site.baseurl}}/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and Helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Chaining

This indicator may be generated from any chain-enabled indicator or method.

```csharp
// example
var results = quotes
    .Use(CandlePart.HL2)
    .GetTsi(..);
```

Results can be further processed on `Tsi` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .GetTsi(..)
    .GetSlope(..);
```
