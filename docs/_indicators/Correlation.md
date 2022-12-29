---
title: Correlation Coefficient
description: Created by Karl Pearson, the Correlation Coefficient depicts the linear statistical correlation between two quote histories.  R-Squared (R&sup2;), Variance, and Covariance are also output.  This is also called the Pearson Correlation Coefficient or Coefficient of Determination.
permalink: /indicators/Correlation/
image: /assets/charts/Correlation.png
type: numerical-analysis
layout: indicator
---

# {{ page.title }}

Created by Karl Pearson, the [Correlation Coefficient](https://en.wikipedia.org/wiki/Correlation_coefficient) depicts the linear statistical correlation between two quote histories.  R-Squared (R&sup2;), Variance, and Covariance are also output.
[[Discuss] :speech_balloon:]({{site.github.repository_url}}/discussions/259 "Community discussion about this indicator")

![image]({{site.baseurl}}{{page.image}})

```csharp
// usage
IEnumerable<CorrResult> results =
  quotesA.GetCorrelation(quotesB, lookbackPeriods);
```

## Parameters

| name | type | notes
| -- |-- |--
| `quotesB` | IEnumerable\<[TQuote]({{site.baseurl}}/guide/#historical-quotes)\> | Historical quotes (B) must have at least the same matching date elements of `quotesA`.
| `lookbackPeriods` | int | Number of periods (`N`) in the lookback period.  Must be greater than 0 to calculate; however we suggest a larger period for statistically appropriate sample size.

### Historical quotes requirements

You must have at least `N` periods for both versions of `quotes` to cover the warmup periods.  Mismatch histories will produce a `InvalidQuotesException`.  Historical price quotes should have a consistent frequency (day, hour, minute, etc).

`quotesA` is an `IEnumerable<TQuote>` collection of historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<CorrResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N-1` periods will have `null` values since there's not enough data to calculate.

### CorrResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `VarianceA` | double | Variance of A
| `VarianceB` | double | Variance of B
| `Covariance` | double | Covariance of A+B
| `Correlation` | double | Correlation `R`
| `RSquared` | double | R-Squared (R&sup2;), aka Coefficient of Determination.  Simple linear regression models is used (square of Correlation).

### Utilities

- [.Condense()]({{site.baseurl}}/utilities#condense)
- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()]({{site.baseurl}}/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Chaining

This indicator may be generated from any chain-enabled indicator or method.

```csharp
// example
var results = quotes
    .Use(CandlePart.HL2)
    .GetCorrelation(quotesMarket.Use(CandlePart.HL2),20);
```

> :warning: **Warning!** Both `quotesA` and `quotesB` arguments must contain the same number of elements and be the results of a chainable indicator or `.Use()` method.

Results can be further processed on `Correlation` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .GetCorrelation(..)
    .GetSlope(..);
```
