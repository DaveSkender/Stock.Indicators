# Bollinger Bands&#174;

Created by John Bollinger, [Bollinger Bands](https://en.wikipedia.org/wiki/Bollinger_Bands) depict volatility as standard deviation boundary lines from a moving average of Close price.  Bollinger Bands&#174; is a registered trademark of John A. Bollinger.
[[Discuss] :speech_balloon:](https://github.com/DaveSkender/Stock.Indicators/discussions/267 "Community discussion about this indicator")

![image](chart.png)

```csharp
// usage
IEnumerable<BollingerBandsResult> results =
  quotes.GetBollingerBands(lookbackPeriods, standardDeviations);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `lookbackPeriods` | int | Number of periods (`N`) for the center line moving average.  Must be greater than 1 to calculate; however we suggest a larger period for statistically appropriate sample size.  Default is 20.
| `standardDeviations` | int | Width of bands.  Standard deviations (`D`) from the moving average.  Must be greater than 0.  Default is 2.

### Historical quotes requirements

You must have at least `N` periods of `quotes`.

`quotes` is an `IEnumerable<TQuote>` collection of historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](../../docs/GUIDE.md#historical-quotes) for more information.

## Response

```csharp
IEnumerable<BollingerBandsResult>
```

The first `N-1` periods will have `null` values since there's not enough data to calculate.  We always return the same number of elements as there are in the historical quotes.

### BollingerBandsResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Sma` | decimal | Simple moving average (SMA) of Close price (center line)
| `UpperBand` | decimal | Upper line is `D` standard deviations above the SMA
| `LowerBand` | decimal | Lower line is `D` standard deviations below the SMA
| `PercentB` | decimal | `%B` is the location within the bands.  `(Price-LowerBand)/(UpperBand-LowerBand)`
| `ZScore` | decimal | Z-Score of current Close price (number of standard deviations from mean)
| `Width` | decimal | Width as percent of SMA price.  `(UpperBand-LowerBand)/Sma`

### Utilities

- [.Find(lookupDate)](../../docs/UTILITIES.md#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()](../../docs/UTILITIES.md#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)](../../docs/UTILITIES.md#remove-warmup-periods)

See [Utilities and Helpers](../../docs/UTILITIES.md#content) for more information.

## Example

```csharp
// fetch historical quotes from your feed (your method)
IEnumerable<Quote> quotes = GetHistoryFromFeed("SPY");

// calculate BollingerBands(12,26,9)
IEnumerable<BollingerBandsResult> results =
  quotes.GetBollingerBands(20,2);
```
