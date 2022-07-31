---
title: Price Momentum Oscillator (PMO)
permalink: /indicators/Pmo/
type: price-characteristic
layout: indicator
---

# {{ page.title }}

Created by Carl Swenlin, the DecisionPoint [Price Momentum Oscillator](https://school.stockcharts.com/doku.php?id=technical_indicators:dppmo) is double-smoothed ROC based momentum indicator.
[[Discuss] :speech_balloon:]({{site.github.repository_url}}/discussions/244 "Community discussion about this indicator")

![image]({{site.baseurl}}/assets/charts/Pmo.png)

```csharp
// usage
IEnumerable<PmoResult> results =
  quotes.GetPmo(timePeriods, smoothPeriods, signalPeriods);
```

## Parameters

| name | type | notes
| -- |-- |--
| `timePeriods` | int | Number of periods (`T`) for ROC EMA smoothing.  Must be greater than 1.  Default is 35.
| `smoothPeriods` | int | Number of periods (`S`) for PMO EMA smoothing.  Must be greater than 0.  Default is 20.
| `signalPeriods` | int | Number of periods (`G`) for Signal line EMA.  Must be greater than 0.  Default is 10.

### Historical quotes requirements

You must have at least `N` periods of `quotes`, where `N` is the greater of `T+S`,`2×T`, or `T+100` to cover the convergence periods.  Since this uses multiple smoothing operations, we recommend you use at least `N+250` data points prior to the intended usage date for better precision.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<PmoResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `T+S-1` periods will have `null` values for PMO since there's not enough data to calculate.

:hourglass: **Convergence Warning**: The first `T+S+250` periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods.

### PmoResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Pmo` | double | Price Momentum Oscillator
| `Signal` | double | Signal line is EMA of PMO

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
    .GetPmo(..);
```

Results can be further processed on `Pmo` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .GetPmo(..)
    .GetRsi(..);
```
