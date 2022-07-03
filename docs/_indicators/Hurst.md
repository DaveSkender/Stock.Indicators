---
title: Hurst Exponent
description: Hurst Exponent with Rescaled Range Analysis
permalink: /indicators/Hurst/
type: price-characteristic
layout: indicator
---

# {{ page.title }}

The [Hurst Exponent](https://en.wikipedia.org/wiki/Hurst_exponent) is a [random-walk](https://en.wikipedia.org/wiki/Random_walk) path analysis that measures trending and mean-reverting tendencies of incremental return values.  When `H` is greater than 0.5 it depicts trending.  When `H` is less than 0.5 it is is more likely to revert to the mean.  When `H` is around 0.5 it represents a random walk.
[[Discuss] :speech_balloon:]({{site.github.repository_url}}/discussions/477 "Community discussion about this indicator")

![image]({{site.baseurl}}/assets/charts/Hurst.png)

```csharp
// usage
IEnumerable<HurstResult> results =
  quotes.GetHurst(lookbackPeriods);
```

## Parameters

| name | type | notes
| -- |-- |--
| `lookbackPeriods` | int | Number of periods (`N`) in the Hurst Analysis.  Must be greater than 100.  Default is 100.

### Historical quotes requirements

You must have at least `N+1` periods of `quotes` to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<HurstResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N` periods will have `null` values since there's not enough data to calculate.

### HurstResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `HurstExponent` | double | Hurst Exponent (`H`)

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
    .Use(CandlePart.HLC3)
    .GetHurst(..);
```

Results can be further processed on `HurstExponent` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .GetHurst(..)
    .GetSlope(..);
```
