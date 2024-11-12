---
title: Relative Strength Index (RSI)
description: Created by J. Welles Wilder, the Relative Strength Index is an oscillator that measures strength of the winning/losing price streak on a scale of 0 to 100, to depict overbought and oversold conditions.
permalink: /indicators/Rsi/
image: /assets/charts/Rsi.png
type: oscillator
layout: indicator
---

# {{ page.title }}

Created by J. Welles Wilder, the [Relative Strength Index](https://en.wikipedia.org/wiki/Relative_strength_index) is an oscillator that measures strength of the winning/losing streak over `N` lookback periods on a scale of 0 to 100, to depict overbought and oversold conditions.
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/224 "Community discussion about this indicator")

![chart for {{page.title}}]({{page.image}})

```csharp
// C# usage syntax
IReadOnlyList<RsiResult> results =
  quotes.GetRsi(lookbackPeriods);
```

## Parameters

**`lookbackPeriods`** _`int`_ - Number of periods (`N`) in the lookback period.  Must be greater than 0.  Default is 14.

### Historical quotes requirements

You must have at least `N+100` periods of `quotes` to cover the [warmup and convergence](https://github.com/DaveSkender/Stock.Indicators/discussions/688) periods.  Since this uses a smoothing technique, we recommend you use at least `10×N` data points prior to the intended usage date for better precision.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](pages/guide.md#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<RsiResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N-1` periods will have `null` values since there's not enough data to calculate.

>&#9886; **Convergence warning**: The first `10×N` periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods.

### RsiResult

**`Timestamp`** _`DateTime`_ - date from evaluated `TQuote`

**`Rsi`** _`double`_ - Relative Strength Index

### Utilities

- [.Condense()](pages/utilities.md#condense)
- [.Find(lookupDate)](pages/utilities.md#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()](pages/utilities.md#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)](pages/utilities.md#remove-warmup-periods)

See [Utilities and helpers](pages/utilities.md#utilities-for-indicator-results) for more information.

## Chaining

This indicator may be generated from any chain-enabled indicator or method.

```csharp
// example
var results = quotes
    .Use(CandlePart.HL2)
    .GetRsi(..);
```

Results can be further processed on `Rsi` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .GetRsi(..)
    .GetSlope(..);
```
