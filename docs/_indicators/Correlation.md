---
title: Correlation Coefficient
description: Correlation Coefficient and R-Squared (Coefficient of Determination)
permalink: /indicators/Correlation/
type: numerical-analysis
layout: indicator
---

# {{ page.title }}

[Correlation Coefficient](https://en.wikipedia.org/wiki/Correlation_coefficient) between two quote histories, based on Close price.  R-Squared (R&sup2;), Variance, and Covariance are also output.
[[Discuss] :speech_balloon:]({{site.github.repository_url}}/discussions/259 "Community discussion about this indicator")

![image]({{site.baseurl}}/assets/charts/Correlation.png)

```csharp
// usage
IEnumerable<CorrResult> results =
  quotesA.GetCorr(quotesB, lookbackPeriods);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `quotesB` | IEnumerable\<[TQuote]({{site.baseurl}}/guide/#historical-quotes)\> | Historical quotes (B) must have at least the same matching date elements of `quotesA`.
| `lookbackPeriods` | int | Number of periods (`N`) in the lookback period.  Must be greater than 0 to calculate; however we suggest a larger period for statistically appropriate sample size.

### Historical quotes requirements

You must have at least `N` periods for both versions of `quotes`.  Mismatch histories will produce a `BadQuotesException`.  Historical price quotes should have a consistent frequency (day, hour, minute, etc).

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
| `VarianceA` | decimal | Variance of A based on `N` lookback periods
| `VarianceB` | decimal | Variance of B based on `N` lookback periods
| `Covariance` | decimal | Covariance of A+B based on `N` lookback periods
| `Correlation` | decimal | Correlation `R` based on `N` lookback periods
| `RSquared` | decimal | R-Squared (R&sup2;), aka Coefficient of Determination.  Simple linear regression models is used (square of Correlation).

### Utilities

- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()]({{site.baseurl}}/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and Helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Example

```csharp
// fetch historical quotes from your feed (your method)
IEnumerable<Quote> historySPX = GetHistoryFromFeed("SPX");
IEnumerable<Quote> historyTSLA = GetHistoryFromFeed("TSLA");

// calculate 20-period Correlation
IEnumerable<CorrResult> results 
  = historySPX.GetCorr(historyTSLA,20);
```
