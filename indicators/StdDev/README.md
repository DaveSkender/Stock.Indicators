# Standard Deviation (volatility)

Rolling [Standard Deviation](https://en.wikipedia.org/wiki/Standard_deviation) of Close price over a lookback window.
[[Discuss] :speech_balloon:](https://github.com/DaveSkender/Stock.Indicators/discussions/239 "Community discussion about this indicator")

![image](chart.png)

```csharp
// usage
IEnumerable<StdDevResult> results = Indicator.GetStdDev(history, lookbackPeriod);  

// usage with optional SMA of STDEV (shown above)
IEnumerable<StdDevResult> results = Indicator.GetStdDev(history, lookbackPeriod, smaPeriod);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `history` | IEnumerable\<[TQuote](../../docs/GUIDE.md#historical-quotes)\> | Historical price quotes should have a consistent frequency (day, hour, minute, etc).
| `lookbackPeriod` | int | Number of periods (`N`) in the lookback period.  Must be greater than 1 to calculate; however we suggest a larger period for statistically appropriate sample size.
| `smaPeriod` | int | Optional.  Number of periods in the moving average of `StdDev`.  Must be greater than 0, if specified.

### Minimum history requirements

You must supply at least `N` periods of `history`.

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
| `StdDevSma` | decimal | Moving average (SMA) of STDDEV based on `smaPeriod` periods, if specified

## Example

```csharp
// fetch historical quotes from your favorite feed, in Quote format
IEnumerable<Quote> history = GetHistoryFromFeed("SPX");

// calculate 10-period Standard Deviation
IEnumerable<StdDevResult> results = Indicator.GetStdDev(history,10);

// use results as needed
StdDevResult result = results.LastOrDefault();
Console.WriteLine("StdDev(SPX,10) on {0} was ${1}", result.Date, result.StdDev);
```

```bash
StdDev(SPX,10) on 12/31/2018 was $5.47
```
