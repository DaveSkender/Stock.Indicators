---
title: Correlation Coefficient
description: Created by Karl Pearson, the Correlation Coefficient depicts the linear statistical correlation between two quote histories.  R-Squared (R&sup2;), Variance, and Covariance are also output.  This is also called the Pearson Correlation Coefficient or Coefficient of Determination.
---

# Correlation Coefficient

Created by Karl Pearson, the [Correlation Coefficient](https://en.wikipedia.org/wiki/Correlation_coefficient) depicts the linear statistical correlation between two quote histories.  R-Squared (R&sup2;), Variance, and Covariance are also output.
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/259 "Community discussion about this indicator")

<ClientOnly>
  <IndicatorChart src="/data/Correlation.json" :height="360" />
</ClientOnly>

```csharp
// C# usage syntax
IReadOnlyList<CorrResult> results =
  quotesA.ToCorrelation(quotesB, lookbackPeriods);
```

## Parameters

| param | type | description |
| ----- | ---- | ----------- |
| `quotesB` | IReadOnlyList\<TQuote\> | [Historical quotes](/guide#historical-quotes) (B) must have at least the same matching date elements of `quotesA`. |
| `lookbackPeriods` | int | Number of periods (`N`) in the lookback period.  Must be greater than 0 to calculate; however we suggest a larger period for statistically appropriate sample size. |

### Historical quotes requirements

You must have at least `N` periods for both versions of `quotes` to cover the warmup periods.  Mismatch histories will produce a `InvalidQuotesException`.  Historical price quotes should have a consistent frequency (day, hour, minute, etc).

`quotesA` is an `IReadOnlyList\<TQuote\>` collection of historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](/guide#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<CorrResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N-1` periods will have `null` values since there's not enough data to calculate.

### `CorrResult`

| property | type | description |
| -------- | ---- | ----------- |
| `Timestamp` | DateTime | Date from evaluated `TQuote` |
| `VarianceA` | double | Variance of A |
| `VarianceB` | double | Variance of B |
| `Covariance` | double | Covariance of A+B |
| `Correlation` | double | Correlation `R` |
| `RSquared` | double | R-Squared (R&sup2;), aka Coefficient of Determination.  Simple linear regression models is used (square of Correlation). |

### Utilities

- [.Condense()](/utilities/results#condense)
- [.Find(lookupDate)](/utilities/results#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()](/utilities/results#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)](/utilities/results#remove-warmup-periods)

See [Utilities and helpers](/utilities/results) for more information.

## Chaining

This indicator may be generated from any chain-enabled indicator or method.

```csharp
// example
var results = quotes
    .Use(CandlePart.HL2)
    .ToCorrelation(quotesMarket.Use(CandlePart.HL2),20);
```

::: warning
Both `quotesA` and `quotesB` arguments must contain the same number of elements and be the results of a chainable indicator or `.Use()` method.
:::

Results can be further processed on `Correlation` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .ToCorrelation(..)
    .ToSlope(..);
```
