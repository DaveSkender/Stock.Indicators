---
title: Klinger Volume Oscillator
permalink: /indicators/Kvo/
type: volume-based
layout: indicator
---

# {{ page.title }}

Created by Stephen Klinger, the [Klinger Volume Oscillator](https://www.investopedia.com/terms/k/klingeroscillator.asp) depicts volume-based trend reversal and divergence between short and long-term money flow.
[[Discuss] :speech_balloon:]({{site.github.repository_url}}/discussions/446 "Community discussion about this indicator")

![image]({{site.baseurl}}/assets/charts/Kvo.png)

```csharp
// usage
IEnumerable<KvoResult> results =
  quotes.GetKvo(shortPeriods, longPeriods, signalPeriods);
```

## Parameters

| name | type | notes
| -- |-- |--
| `fastPeriods` | int | Number of lookback periods (`F`) for the short-term EMA.  Must be greater than 2.  Default is 34.
| `slowPeriods` | int | Number of lookback periods (`L`) for the long-term EMA.  Must be greater than `F`.  Default is 55.
| `signalPeriods` | int | Number of lookback periods for the signal line.  Must be greater than 0.  Default is 13.

### Historical quotes requirements

You must have at least `L+100` periods of `quotes` to cover the warmup periods.  Since this uses a smoothing technique, we recommend you use at least `L+150` data points prior to the intended usage date for better precision.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<KvoResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `L+1` periods will have `null` values since there's not enough data to calculate.

:hourglass: **Convergence Warning**: The first `L+150` periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods.

### KvoResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Oscillator` | double | Klinger Oscillator
| `Signal` | double | EMA of Klinger Oscillator (signal line)

### Utilities

- [.Condense()]({{site.baseurl}}/utilities#condense)
- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()]({{site.baseurl}}/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and Helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Chaining

Results can be further processed on `Kvo` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .GetKvo(..)
    .GetSlope(..);
```

This indicator must be generated from `quotes` and **cannot** be generated from results of another chain-enabled indicator or method.
