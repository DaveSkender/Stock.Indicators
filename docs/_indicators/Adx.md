---
title: Average Directional Index (ADX)
description: Created by J. Welles Wilder, the Directional Movement Index (DMI) and Average Directional Movement Index (ADX) is a measure of price directional movement.  It includes upward and downward indicators, and is often used to measure strength of trend.
permalink: /indicators/Adx/
image: /assets/charts/AdIndex.png
type: price-trend
layout: indicator
---

# {{ page.title }}

Created by J. Welles Wilder, the Directional Movement Index (DMI) and [Average Directional Movement Index](https://en.wikipedia.org/wiki/Average_directional_movement_index) (ADX) is a measure of price directional movement.  It includes upward and downward indicators, and is often used to measure strength of trend.
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/270 "Community discussion about this indicator")

![chart for {{page.title}}]({{page.image}})

```csharp
// C# usage syntax
IReadOnlyList<AdxResult> results =
  quotes.GetAdx(lookbackPeriods);
```

## Parameters

**`lookbackPeriods`** _`int`_ - Number of periods (`N`) to consider.  Must be greater than 1.  Default is 14.

### Historical quotes requirements

You must have at least `2×N+100` periods of `quotes` to cover the [warmup and convergence](https://github.com/DaveSkender/Stock.Indicators/discussions/688) periods.  We generally recommend you use at least `2×N+250` data points prior to the intended usage date for better precision.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](pages/guide.md#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<AdxResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `2×N-1` periods will have `null` values for `Adx` since there's not enough data to calculate.

>&#9886; **Convergence warning**: The first `2×N+100` periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods.

### AdxResult

**`Timestamp`** _`DateTime`_ - date from evaluated `TQuote`

**`Pdi`** _`double`_ - Plus Directional Index (+DI)

**`Mdi`** _`double`_ - Minus Directional Index (-DI)

**`Adx`** _`double`_ - Average Directional Index (ADX)

**`Adxr`** _`double`_ - Average Directional Index Rating (ADXR)

### Utilities

- [.Condense()](pages/utilities.md#condense)
- [.Find(lookupDate)](pages/utilities.md#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()](pages/utilities.md#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)](pages/utilities.md#remove-warmup-periods)

See [Utilities and helpers](pages/utilities.md#utilities-for-indicator-results) for more information.

## Chaining

Results can be further processed on `Adx` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .GetAdx(..)
    .GetRsi(..);
```

This indicator must be generated from `quotes` and **cannot** be generated from results of another chain-enabled indicator or method.
