---
title: ConnorsRSI
permalink: /indicators/ConnorsRsi/
type: oscillator
layout: indicator
---

# {{ page.title }}

Created by Laurence Connors, the [ConnorsRSI](https://alvarezquanttrading.com/wp-content/uploads/2016/05/ConnorsRSIGuidebook.pdf) is a composite oscillator that incorporates RSI, winning/losing streaks, and percentile gain metrics on scale of 0 to 100.  See [analysis](https://alvarezquanttrading.com/blog/connorsrsi-analysis).
[[Discuss] :speech_balloon:]({{site.github.repository_url}}/discussions/260 "Community discussion about this indicator")

![image]({{site.baseurl}}/assets/charts/ConnorsRsi.png)

```csharp
// usage
IEnumerable<ConnorsRsiResult> results =
  quotes.GetConnorsRsi(rsiPeriods, streakPeriods, rankPeriods);
```

## Parameters

| name | type | notes
| -- |-- |--
| `rsiPeriods` | int | Lookback period (`R`) for the price RSI.  Must be greater than 1.  Default is 3.
| `streakPeriods` | int | Lookback period (`S`) for the streak RSI.  Must be greater than 1.  Default is 2.
| `rankPeriods` | int | Lookback period (`P`) for the Percentile Rank.  Must be greater than 1.  Default is 100.

### Historical quotes requirements

`N` is the greater of `R+100`, `S`, and `P+2`.  You must have at least `N` periods of `quotes` to cover the convergence periods.  Since this uses a smoothing technique, we recommend you use at least `N+150` data points prior to the intended usage date for better precision.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<ConnorsRsiResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `MAX(R,S,P)-1` periods will have `null` values since there's not enough data to calculate.

:hourglass: **Convergence Warning**: The first `N` periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods.

### ConnorsRsiResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Rsi` | double | RSI(`R`) of the price.
| `RsiStreak` | double | RSI(`S`) of the Streak.
| `PercentRank` | double | Percentile rank of the period gain value.
| `ConnorsRsi` | double | ConnorsRSI

### Utilities

- [.Condense()]({{site.baseurl}}/utilities#condense)
- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()]({{site.baseurl}}/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and Helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Chaining

This indicator may be generated from any chain-enabled indicator or method.

```csharp
// example
var results = quotes
    .Use(CandlePart.HL2)
    .GetConnorsRsi(..);
```

Results can be further processed on `ConnorsRsi` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .GetConnorsRsi(..)
    .GetSma(..);
```
