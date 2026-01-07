---
title: Price Relative Strength (PRS)
description: Price Relative Strength, also called Comparative Relative Strength, shows the ratio of two quote histories, based on price.  It is often used to compare against a market index or sector ETF.  When using the optional lookback window, this also returns relative percent change over the specified periods.  This is not the same as the more prevalent Relative Strength Index (RSI).
---

# Price Relative Strength (PRS)

[Price Relative Strength (PRS)](https://en.wikipedia.org/wiki/Relative_strength), also called Comparative Relative Strength, shows the ratio of two quote histories, based on price.  It is often used to compare against a market index or sector ETF.  When using the optional `lookbackPeriods`, this also returns relative percent change over the specified periods.  This is not the same as the more prevalent <a href="/indicators/Rsi/" rel="nofollow">Relative Strength Index (RSI)</a>.
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/243 "Community discussion about this indicator")

<ClientOnly>
  <IndicatorChart src="/data/Prs.json" :height="360" />
</ClientOnly>

```csharp
// C# usage syntax
IReadOnlyList<PrsResult> results =
  quotesEval.ToPrs(quotesBase);
```

## Parameters

| param | type | description |
| ----- | ---- | ----------- |
| `quotesBase` | IReadOnlyList\<TQuote\> | [Historical quotes](/guide#historical-quotes) used as the basis for comparison.  This is usually market index data.  You must have the same number of periods as `quotesEval`. |
| `lookbackPeriods` | int | Optional.  Number of periods (`N`) to lookback to compute % difference.  Must be greater than 0 if specified or `null`. |

### Historical quotes requirements

You must have at least `N` periods of `quotesEval` to calculate `PrsPercent` if `lookbackPeriods` is specified; otherwise, you must specify at least `S+1` periods.  More than the minimum is typically specified.  For this indicator, the elements must match (e.g. the `n`th elements must be the same date).  An `Exception` will be thrown for mismatch dates.  Historical price quotes should have a consistent frequency (day, hour, minute, etc).

`quotesEval` is an `IReadOnlyList\<TQuote\>` collection of historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](/guide#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<PrsResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The `N` periods will have `null` values for `PrsPercent` since there's not enough data to calculate.

### `PrsResult`

| property | type | description |
| -------- | ---- | ----------- |
| `Timestamp` | DateTime | Date from evaluated `TQuote` |
| `Prs` | double | Price Relative Strength compares `Eval` to `Base` histories |
| `PrsPercent` | double | Percent change difference between `Eval` and `Base` over `N` periods |

### Utilities

- [.Condense()](/utilities/results#condense)
- [.Find(lookupDate)](/utilities/results#find-indicator-result-by-date)
- [.RemoveWarmupPeriods(qty)](/utilities/results#remove-warmup-periods)

See [Utilities and helpers](/utilities/results) for more information.

## Chaining

This indicator may be generated from any chain-enabled indicator or method.

```csharp
// example
var results = quotesEval
    .Use(CandlePart.HL2)
    .ToPrs(quotesBase, ..);
```

::: warning
Both `quotesEval` and `quotesBase` arguments must contain the same number of elements and be the results of a chainable indicator or `.Use()` method.
:::

Results can be further processed on `Beta` with additional chain-enabled indicators.

```csharp
// example
var results = quotesEval
    .ToPrs(quotesBase, ..)
    .ToSlope(..);
```
