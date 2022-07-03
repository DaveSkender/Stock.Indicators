---
title: Bollinger Bands&#174;
permalink: /indicators/BollingerBands/
type: price-channel
layout: indicator
---

# {{ page.title }}

Created by John Bollinger, [Bollinger Bands](https://en.wikipedia.org/wiki/Bollinger_Bands) depict volatility as standard deviation boundary lines from a moving average of price.  Bollinger Bands&#174; is a registered trademark of John A. Bollinger.
[[Discuss] :speech_balloon:]({{site.github.repository_url}}/discussions/267 "Community discussion about this indicator")

![image]({{site.baseurl}}/assets/charts/BollingerBands.png)

```csharp
// usage
IEnumerable<BollingerBandsResult> results =
  quotes.GetBollingerBands(lookbackPeriods, standardDeviations);
```

## Parameters

| name | type | notes
| -- |-- |--
| `lookbackPeriods` | int | Number of periods (`N`) for the center line moving average.  Must be greater than 1 to calculate; however we suggest a larger period for statistically appropriate sample size.  Default is 20.
| `standardDeviations` | double | Width of bands.  Standard deviations (`D`) from the moving average.  Must be greater than 0.  Default is 2.

### Historical quotes requirements

You must have at least `N` periods of `quotes` to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<BollingerBandsResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N-1` periods will have `null` values since there's not enough data to calculate.

### BollingerBandsResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Sma` | double | Simple moving average (SMA) of price (center line)
| `UpperBand` | double | Upper line is `D` standard deviations above the SMA
| `LowerBand` | double | Lower line is `D` standard deviations below the SMA
| `PercentB` | double | `%B` is the location within the bands.  `(Price-LowerBand)/(UpperBand-LowerBand)`
| `ZScore` | double | Z-Score of current price (number of standard deviations from mean)
| `Width` | double | Width as percent of SMA price.  `(UpperBand-LowerBand)/Sma`

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
    .GetBollingerBands(..);
```

Results can be further processed on `PercentB` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .GetBollingerBands(..)
    .GetRsi(..);
```
