---
title: Hull Moving Average (HMA)
permalink: /indicators/Hma/
type: moving-average
layout: indicator
---

# {{ page.title }}

Created by Alan Hull, the [Hull Moving Average](https://alanhull.com/hull-moving-average) is a modified weighted average of price that reduces lag.
[[Discuss] :speech_balloon:]({{site.github.repository_url}}/discussions/252 "Community discussion about this indicator")

![image]({{site.baseurl}}/assets/charts/Hma.png)

```csharp
// usage
IEnumerable<HmaResult> results =
  quotes.GetHma(lookbackPeriods);
```

## Parameters

| name | type | notes
| -- |-- |--
| `lookbackPeriods` | int | Number of periods (`N`) in the moving average.  Must be greater than 1.

### Historical quotes requirements

You must have at least `N+(integer of SQRT(N))-1` periods of `quotes` to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<HmaResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N+(integer of SQRT(N))-1` periods will have `null` values since there's not enough data to calculate.

### HmaResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Hma` | double | Hull moving average for `N` lookback periods

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
    .GetHma(..);
```

Results can be further processed on `Hma` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .GetHma(..)
    .GetRsi(..);
```
