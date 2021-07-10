# Standard Deviation (volatility)

[Standard Deviation](https://en.wikipedia.org/wiki/Standard_deviation) of Close price over a rolling lookback window.  Also known as Historical Volatility (HV).
[[Discuss] :speech_balloon:](https://github.com/DaveSkender/Stock.Indicators/discussions/239 "Community discussion about this indicator")

![image](chart.png)

```csharp
// usage
IEnumerable<StdDevResult> results =
  quotes.GetStdDev(lookbackPeriods);  

// usage with optional SMA of STDEV (shown above)
IEnumerable<StdDevResult> results =
  quotes.GetStdDev(lookbackPeriods, smaPeriods);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `lookbackPeriods` | int | Number of periods (`N`) in the lookback period.  Must be greater than 1 to calculate; however we suggest a larger period for statistically appropriate sample size.
| `smaPeriods` | int | Optional.  Number of periods in the moving average of `StdDev`.  Must be greater than 0, if specified.

### Historical quotes requirements

You must have at least `N` periods of `quotes`.

`quotes` is an `IEnumerable<TQuote>` collection of historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](../../docs/GUIDE.md) for more information.

## Response

```csharp
IEnumerable<StdDevResult>
```

The first `N-1` periods will have `null` values since there's not enough data to calculate.  We always return the same number of elements as there are in the historical quotes.

### StdDevResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `StdDev` | decimal | Standard Deviation of Close price over `N` lookback periods
| `Mean` | decimal | Mean value of Close price over `N` lookback periods
| `ZScore` | decimal | Z-Score of current Close price (number of standard deviations from mean)
| `StdDevSma` | decimal | Moving average (SMA) of STDDEV based on `smaPeriods` periods, if specified

### Utilities

- [.Find()](../../docs/UTILITIES.md#find-indicator-result-by-date)
- [.PruneWarmupPeriods()](../../docs/UTILITIES.md#prune-warmup-periods)
- [.PruneWarmupPeriods(qty)](../../docs/UTILITIES.md#prune-warmup-periods)

See [Utilities and Helpers](../../docs/UTILITIES.md#content) for more information.

## Example

```csharp
// fetch historical quotes from your feed (your method)
IEnumerable<Quote> quotes = GetHistoryFromFeed("SPX");

// calculate 10-period Standard Deviation
IEnumerable<StdDevResult> results = quotes.GetStdDev(10);

// use results as needed
StdDevResult result = results.LastOrDefault();
Console.WriteLine("StdDev(SPX,10) on {0} was ${1}", result.Date, result.StdDev);
```

```bash
StdDev(SPX,10) on 12/31/2018 was $5.47
```
