---
title: Price Relative Strength (PRS)
permalink: /indicators/Prs/
type: price-characteristic
layout: indicator
---

# {{ page.title }}

[Price Relative Strength (PRS)](https://en.wikipedia.org/wiki/Relative_strength), also called Comparative Relative Strength, shows the ratio of two quote histories, based on Close price.  It is often used to compare against a market index or sector ETF.  When using the optional `lookbackPeriods`, this also returns relative percent change over the specified periods.  This is not the same as the more prevalent [Relative Strength Index (RSI)](../Rsi#content).
[[Discuss] :speech_balloon:]({{site.github.repository_url}}/discussions/243 "Community discussion about this indicator")

![image]({{site.baseurl}}/assets/charts/Prs.png)

```csharp
// usage
IEnumerable<PrsResult> results =
  historyBase.GetPrs(historyEval);  

// usage with optional lookback period and SMA of PRS (shown above)
IEnumerable<PrsResult> results =
  historyBase.GetPrs(historyEval, lookbackPeriods, smaPeriods);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `historyEval` | IEnumerable\<[TQuote]({{site.baseurl}}/guide/#historical-quotes)\> | Historical quotes for evaluation.  You must have the same number of periods as `historyBase`.
| `lookbackPeriods` | int | Optional.  Number of periods (`N`) to lookback to compute % difference.  Must be greater than 0 if specified or `null`.
| `smaPeriods` | int | Optional.  Number of periods (`S`) in the SMA lookback period for `Prs`.  Must be greater than 0.

### Historical quotes requirements

You must have at least `N` periods of `historyBase` to calculate `PrsPercent` if `lookbackPeriods` is specified; otherwise, you must specify at least `S+1` periods.  More than the minimum is typically specified.  For this indicator, the elements must match (e.g. the `n`th elements must be the same date).  An `Exception` will be thrown for mismatch dates.  Historical price quotes should have a consistent frequency (day, hour, minute, etc).

`historyBase` is an `IEnumerable<TQuote>` collection of historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<PrsResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The `N` periods will have `null` values for `PrsPercent` and the first `S-1` periods will have `null` values for `Sma` since there's not enough data to calculate.

### PrResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Prs` | double | Price Relative Strength compares `Eval` to `Base` histories
| `PrsSma` | double | Moving Average (SMA) of PRS over `S` periods
| `PrsPercent` | double | Percent change difference between `Eval` and `Base` over `N` periods

### Utilities

- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and Helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Example

```csharp
// fetch historical quotes from your feed (your method)
IEnumerable<Quote> historySPX = GetHistoryFromFeed("SPX");
IEnumerable<Quote> historyTSLA = GetHistoryFromFeed("TSLA");

// calculate 14-period PRS
IEnumerable<PrResult> results = historySPX.GetPrs(historyTSLA,14);
```
