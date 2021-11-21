---
title: Beta Coefficient
description: Beta Coefficient with Beta+/Beta-
permalink: /indicators/Beta/
type: numerical-analysis
layout: indicator
---

# {{ page.title }}

[Beta](https://en.wikipedia.org/wiki/Beta_(finance)) shows how strongly one stock responds to systemic volatility of the entire market.  [Upside Beta](https://en.wikipedia.org/wiki/Upside_beta) (Beta+) and [Downside Beta](https://en.wikipedia.org/wiki/Downside_beta) (Beta-), [popularized by Harry M. Markowitz](https://www.jstor.org/stable/j.ctt1bh4c8h), are also included.
[[Discuss] :speech_balloon:]({{site.github.repository_url}}/discussions/268 "Community discussion about this indicator")

![image]({{site.baseurl}}/assets/charts/Beta.png)

```csharp
// usage
IEnumerable<BetaResult> results =
  Indicator.GetBeta(quotesMarket, quotesEval, lookbackPeriods, type);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `quotesMarket` | IEnumerable\<[TQuote]({{site.baseurl}}/guide/#historical-quotes)\> | Historical [market] Quotes data should be at any consistent frequency (day, hour, minute, etc).  This `market` quotes will be used to establish the baseline.
| `quotesEval` | IEnumerable\<[TQuote]({{site.baseurl}}/guide/#historical-quotes)\> | Historical [evaluation stock] Quotes data should be at any consistent frequency (day, hour, minute, etc).
| `lookbackPeriods` | int | Number of periods (`N`) in the lookback period.  Must be greater than 0 to calculate; however we suggest a larger period for statistically appropriate sample size and especially when using Beta +/-.
| `type` | BetaType | Type of Beta to calculate.  Default is `BetaType.Standard`. See [BetaType options](#betatype-options) below.

### Historical quotes requirements

You must have at least `N` periods of quotes.  You must have at least the same matching date elements of `quotesMarket`.  Exception will be thrown if not matched.  Historical price quotes should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

#### BetaType options

| type | description
|-- |--
| `Standard` | Standard Beta only.  Uses all historical quotes.
| `Up` | Upside Beta only.  Uses historical quotes from market up bars only.
| `Down` | Downside Beta only.  Uses historical quotes from market down bars only.
| `All` | Returns all of the above.  Use this option if you want `Ratio` and `Convexity` values returned.  Note: 3× slower to calculate.

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
| `BetaUp` | decimal | Beta+ (Up Beta)
| `BetaDown` | decimal | Beta- (Down Beta)
| `Ratio` | decimal | Beta ratio is `BetaUp/BetaDown`
| `Convexity` | decimal | Beta convexity is <code>(BetaUp-BetaDown)<sup>2</sup></code>

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

// calculate 20-period Beta coefficient
IEnumerable<BetaResult> results =
  Indicator.GetBeta(historySPX,historyTSLA,20);
```
