---
title: Beta Coefficient
permalink: /indicators/Beta/
layout: default
---

# {{ page.title }}

[Beta](https://en.wikipedia.org/wiki/Beta_(finance)) shows how strongly one stock responds to systemic volatility of the entire market.
[[Discuss] :speech_balloon:](https://github.com/DaveSkender/Stock.Indicators/discussions/268 "Community discussion about this indicator")

![image]({{site.baseurl}}/assets/charts/Beta.png)

```csharp
// usage
IEnumerable<BetaResult> results =
  Indicator.GetBeta(historyMarket, historyEval, lookbackPeriods);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `historyMarket` | IEnumerable\<[TQuote]({{site.baseurl}}/guide#historical-quotes)\> | Historical [market] Quotes data should be at any consistent frequency (day, hour, minute, etc).  This `market` quotes will be used to establish the baseline.
| `historyEval` | IEnumerable\<[TQuote]({{site.baseurl}}/guide#historical-quotes)\> | Historical [evaluation stock] Quotes data should be at any consistent frequency (day, hour, minute, etc).
| `lookbackPeriods` | int | Number of periods (`N`) in the lookback period.  Must be greater than 0 to calculate; however we suggest a larger period for statistically appropriate sample size.

### Historical quotes requirements

You must have at least `N` periods of quotes.  You must have at least the same matching date elements of `historyMarket`.  Exception will be thrown if not matched.  Historical price quotes should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide#historical-quotes) for more information.

## Response

```csharp
IEnumerable<BetaResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N-1` periods will have `null` values since there's not enough data to calculate.

### BetaResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Beta` | decimal | Beta coefficient based on `N` lookback periods

### Utilities

- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()]({{site.baseurl}}/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and Helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Example

```csharp
// fetch historical quotes from your feed (your method)
IEnumerable<Quote> historyTSLA = GetHistoryFromFeed("TSLA");
IEnumerable<Quote> historySPX = GetHistoryFromFeed("SPX");

// calculate 20-period Beta coefficient
IEnumerable<BetaResult> results =
  Indicator.GetBeta(historySPX,historyTSLA,20);
```
