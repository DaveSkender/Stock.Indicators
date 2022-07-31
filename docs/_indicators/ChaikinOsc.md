---
title: Chaikin Oscillator
permalink: /indicators/ChaikinOsc/
type: volume-based
layout: indicator
---

# {{ page.title }}

Created by Marc Chaikin, the [Chaikin Oscillator](https://en.wikipedia.org/wiki/Chaikin_Analytics#Chaikin_Oscillator) is the difference between fast and slow Exponential Moving Averages (EMA) of the [Accumulation/Distribution Line]({{site.baseurl}}/indicators/Adl/#content) (ADL).
[[Discuss] :speech_balloon:]({{site.github.repository_url}}/discussions/264 "Community discussion about this indicator")

![image]({{site.baseurl}}/assets/charts/ChaikinOsc.png)

```csharp
// usage
IEnumerable<ChaikinOscResult> results =
  quotes.GetChaikinOsc(fastPeriods, slowPeriods);
```

## Parameters

| name | type | notes
| -- |-- |--
| `fastPeriods` | int | Number of periods (`F`) in the ADL fast EMA.  Must be greater than 0 and smaller than `S`.  Default is 3.
| `slowPeriods` | int | Number of periods (`S`) in the ADL slow EMA.  Must be greater `F`.  Default is 10.

### Historical quotes requirements

You must have at least `2×S` or `S+100` periods of `quotes`, whichever is more,  to cover the convergence periods.  Since this uses a smoothing technique, we recommend you use at least `S+250` data points prior to the intended usage date for better precision.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<ChaikinOscResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `S-1` periods will have `null` values for `Oscillator` since there's not enough data to calculate.

:hourglass: **Convergence Warning**: The first `S+100` periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods.

### ChaikinOscResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `MoneyFlowMultiplier` | double | Money Flow Multiplier
| `MoneyFlowVolume` | double | Money Flow Volume
| `Adl` | double | Accumulation Distribution Line (ADL)
| `Oscillator` | double | Chaikin Oscillator

:warning: **Warning**: absolute values in MFV, ADL, and Oscillator are somewhat meaningless, so use with caution.

### Utilities

- [.Condense()]({{site.baseurl}}/utilities#condense)
- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()]({{site.baseurl}}/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and Helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Chaining

Results can be further processed on `Oscillator` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .GetChaikinOsc(..)
    .GetSlope(..);
```

This indicator must be generated from `quotes` and **cannot** be generated from results of another chain-enabled indicator or method.
