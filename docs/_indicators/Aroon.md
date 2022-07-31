---
title: Aroon
permalink: /indicators/Aroon/
type: price-trend
layout: indicator
---

# {{ page.title }}

Created by Tushar Chande, [Aroon](https://school.stockcharts.com/doku.php?id=technical_indicators:aroon) is a oscillator view of how long ago the new high or low price occurred over a lookback window.
[[Discuss] :speech_balloon:]({{site.github.repository_url}}/discussions/266 "Community discussion about this indicator")

![image]({{site.baseurl}}/assets/charts/Aroon.png)

```csharp
// usage
IEnumerable<AroonResult> results =
  quotes.GetAroon(lookbackPeriods);
```

## Parameters

| name | type | notes
| -- |-- |--
| `lookbackPeriods` | int | Number of periods (`N`) for the lookback evaluation.  Must be greater than 0.  Default is 25.

### Historical quotes requirements

You must have at least `N` periods of `quotes` to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<AroonResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N-1` periods will have `null` values for `Aroon` since there's not enough data to calculate.

### AroonResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `AroonUp` | double | Based on last High price
| `AroonDown` | double | Based on last Low price
| `Oscillator` | double | AroonUp - AroonDown

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
    .GetAroon(..)
    .GetSlope(..);
```

This indicator must be generated from `quotes` and **cannot** be generated from results of another chain-enabled indicator or method.
