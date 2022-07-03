---
title: Force Index
permalink: /indicators/ForceIndex/
type: volume-based
layout: indicator
---

# {{ page.title }}

Created by Alexander Elder, the [Force Index](https://en.wikipedia.org/wiki/Force_index) depicts volume-based buying and selling pressure.
[[Discuss] :speech_balloon:]({{site.github.repository_url}}/discussions/382 "Community discussion about this indicator")

![image]({{site.baseurl}}/assets/charts/ForceIndex.png)

```csharp
// usage
IEnumerable<ForceIndexResult> results =
  quotes.GetForceIndex(lookbackPeriods);
```

## Parameters

| name | type | notes
| -- |-- |--
| `lookbackPeriods` | int | Lookback window (`N`) for the EMA of Force Index.  Must be greater than 0 and is commonly 2 or 13 (shorter/longer view).  Default is 2.

### Historical quotes requirements

You must have at least `N+100` for `2×N` periods of `quotes`, whichever is more, to cover the convergence periods.  Since this uses a smoothing technique for EMA, we recommend you use at least `N+250` data points prior to the intended usage date for better precision.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<ForceIndexResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N` periods for will be `null` since they cannot be calculated.

:hourglass: **Convergence Warning**: The first `N+100` periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods.

### ForceIndexResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `ForceIndex` | double | Force Index

### Utilities

- [.Condense()]({{site.baseurl}}/utilities#condense)
- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()]({{site.baseurl}}/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and Helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Chaining

Results can be further processed on `ForceIndex` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .GetForceIndex(..)
    .GetEma(..);
```

This indicator must be generated from `quotes` and **cannot** be generated from results of another chain-enabled indicator or method.
