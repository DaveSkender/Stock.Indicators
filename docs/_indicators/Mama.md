---
title: MESA Adaptive Moving Average (MAMA)
permalink: /indicators/Mama/
type: moving-average
layout: indicator
---

# {{ page.title }}

Created by John Ehlers, the [MAMA](http://mesasoftware.com/papers/MAMA.pdf) indicator is a 5-period adaptive moving average of high/low price.
[[Discuss] :speech_balloon:]({{site.github.repository_url}}/discussions/211 "Community discussion about this indicator")

![image]({{site.baseurl}}/assets/charts/Mama.png)

```csharp
// usage
IEnumerable<MamaResult> results =
  quotes.GetMama(fastLimit, slowLimit);
```

## Parameters

| name | type | notes
| -- |-- |--
| `fastLimit` | double | Fast limit threshold.  Must be greater than `slowLimit` and less than 1.  Default is 0.5.
| `slowLimit` | double | Slow limit threshold.  Must be greater than 0.  Default is 0.05.

### Historical quotes requirements

You must have at least `50` periods of `quotes` to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<MamaResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `5` periods will have `null` values for `Mama` since there's not enough data to calculate.

:hourglass: **Convergence Warning**: The first `50` periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods.

### MamaResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Mama` | decimal | MESA adaptive moving average (MAMA)
| `Fama` | decimal | Following adaptive moving average (FAMA)

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
    .GetMama(..);
```

Results can be further processed on `Mama` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .GetMama(..)
    .GetRsi(..);
```
