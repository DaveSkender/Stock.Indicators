# Arnaud Legoux Moving Average (ALMA)

Created by Arnaud Legoux and Dimitrios Kouzis-Loukas, [ALMA](https://github.com/DaveSkender/Stock.Indicators/files/5654531/ALMA-Arnaud-Legoux-Moving-Average.pdf) is a Gaussian distribution weighted moving average of Close price over a lookback window.
[[Discuss] :speech_balloon:](https://github.com/DaveSkender/Stock.Indicators/discussions/209 "Community discussion about this indicator")

![image](chart.png)

```csharp
// usage
IEnumerable<AlmaResult> results =
  quotes.GetAlma(lookbackPeriods, offset, sigma);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `lookbackPeriods` | int | Number of periods (`N`) in the moving average.  Must be greater than 1, but is typically in the 5-20 range.  Default is 9.
| `offset` | double | Adjusts smoothness versus responsiveness on a scale from 0 to 1; where 1 is max responsiveness.  Default is 0.85.
| `sigma` | double | Defines the width of the Gaussian [normal distribution](https://en.wikipedia.org/wiki/Normal_distribution).  Must be greater than 0.  Default is 6.

### Historical quotes requirements

You must have at least `N` periods of `quotes`.

`quotes` is an `IEnumerable<TQuote>` collection of historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](../../docs/GUIDE.md#historical-quotes) for more information.

## Response

```csharp
IEnumerable<AlmaResult>
```

The first `N-1` periods will have `null` values since there's not enough data to calculate.  We always return the same number of elements as there are in the historical quotes.

### AlmaResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Alma` | decimal | Arnaud Legoux Moving Average

### Utilities

- [.Find(lookupDate)](../../docs/UTILITIES.md#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()](../../docs/UTILITIES.md#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)](../../docs/UTILITIES.md#remove-warmup-periods)

See [Utilities and Helpers](../../docs/UTILITIES.md#content) for more information.

## Example

```csharp
// fetch historical quotes from your feed (your method)
IEnumerable<Quote> quotes = GetHistoryFromFeed("MSFT");

// calculate Alma(10,0.5,6)
IEnumerable<AlmaResult> results = quotes.GetAlma(10,0.5,6);
```
