---
title: Slope and Linear Regression
permalink: /indicators/Slope/
type: numerical-analysis
layout: indicator
---

# {{ page.title }}

[Slope of the best fit line](https://school.stockcharts.com/doku.php?id=technical_indicators:slope) is determined by an [ordinary least-squares simple linear regression](https://en.wikipedia.org/wiki/Simple_linear_regression) on Close price.  It can be used to help identify trend strength and direction.  Standard Deviation, R&sup2;, and a best-fit `Line` (for last lookback segment) are also output.  See also [Standard Deviation Channels](../StdDevChannels#content) for an alternative depiction.
[[Discuss] :speech_balloon:]({{site.github.repository_url}}/discussions/241 "Community discussion about this indicator")

![image]({{site.baseurl}}/assets/charts/Slope.png)

```csharp
// usage
IEnumerable<SlopeResult> results =
  quotes.GetSlope(lookbackPeriods);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `lookbackPeriods` | int | Number of periods (`N`) for the linear regression.  Must be greater than 0.

### Historical quotes requirements

You must have at least `N` periods of `quotes`.

`quotes` is an `IEnumerable<TQuote>` collection of historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<SlopeResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N-1` periods will have `null` values for `Slope` since there's not enough data to calculate.

:paintbrush: **Repaint Warning**: the `Line` will be continuously repainted since it is based on the last quote and lookback period.

### SlopeResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Slope` | decimal | Slope `m` of the best-fit line of Close price
| `Intercept` | decimal | Y-Intercept `b` of the best-fit line
| `StdDev` | double | Standard Deviation of Close price over `N` lookback periods
| `RSquared` | double | R-Squared (R&sup2;), aka Coefficient of Determination
| `Line` | decimal | Best-fit line `y` over the last 'N' periods (i.e. `y=mx+b` using last period values)

### Utilities

- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()]({{site.baseurl}}/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and Helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Example

```csharp
// fetch historical quotes from your feed (your method)
IEnumerable<Quote> historySPX = GetHistoryFromFeed("SPX");

// calculate 20-period Slope
IEnumerable<SlopeResult> results = quotes.GetSlope(20);
```
