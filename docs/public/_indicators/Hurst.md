---
title: Hurst Exponent
description: Hurst Exponent (H) with Rescaled Range Analysis is a random-walk path analysis that measures trending and mean-reverting tendencies of incremental return values.  When H is greater than 0.5 it depicts trending.  When H is less than 0.5 it is is more likely to revert to the mean.  When H is around 0.5 it represents a random walk.
permalink: /indicators/Hurst/
image: /assets/charts/Hurst.png
type: price-characteristic
layout: indicator
---

# Hurst Exponent

The [Hurst Exponent](https://en.wikipedia.org/wiki/Hurst_exponent) (`H`) is part of a Rescaled Range Analysis, a [random-walk](https://en.wikipedia.org/wiki/Random_walk) path analysis that measures trending and mean-reverting tendencies of incremental return values.  When `H` is greater than 0.5 it depicts trending.  When `H` is less than 0.5 it is is more likely to revert to the mean.  When `H` is around 0.5 it represents a random walk.
[[Discuss] 💬](https://github.com/DaveSkender/Stock.Indicators/discussions/477 "Community discussion about this indicator")

![chart for Hurst Exponent](/assets/charts/Hurst.png)

```csharp
// C# usage syntax
IReadOnlyList<HurstResult> results =
  quotes.GetHurst(lookbackPeriods);
```

## Parameters

**`lookbackPeriods`** _`int`_ - Number of periods (`N`) in the Hurst Analysis.  Must be greater than 20.  Default is 100.

### Historical quotes requirements

You must have at least `N+1` periods of `quotes` to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](/guide/#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<HurstResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N` periods will have `null` values since there's not enough data to calculate.

### HurstResult

**`Timestamp`** _`DateTime`_ - date from evaluated `TQuote`

**`HurstExponent`** _`double`_ - Hurst Exponent (`H`)

### Utilities

- [.Condense()](/utilities#condense)
- [.Find(lookupDate)](/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()](/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)](/utilities#remove-warmup-periods)

See [Utilities and helpers](/utilities#utilities-for-indicator-results) for more information.

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
