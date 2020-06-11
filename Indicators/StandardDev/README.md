# Standard Deviation (Volatility)

Standard Deviation of Close price over a lookback period.
[More info ...](https://school.stockcharts.com/doku.php?id=technical_indicators:standard_deviation_volatility)

```csharp
// usage
IEnumerable<StdDevResult> results = Indicator.GetStdDev(history, lookbackPeriod);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `history` | IEnumerable\<[Quote](/GUIDE.md#Quote)\> | Historical Quotes data should be at any consistent frequency (day, hour, minute, etc).  You must supply at least `N` periods of `history`.
| `lookbackPeriod` | int | Number of periods (`N`) in the lookback period.  Must be greater than 1 to calculate; however we suggest a larger period for statistically appropriate sample size.

## Response

```csharp
IEnumerable<StdDevResult>
```

The first `N-1` periods will have `null` values for `StdDev` since there's not enough data to calculate.  `Avg` values will be `null` for `2×N` periods since it requires `N` periods of `StdDev` results to calculate.  We always return the same number of elements as there are in the historical quotes.

### StdDevResult

| name | type | notes
| -- |-- |--
| `Index` | int | Sequence of dates
| `Date` | DateTime | Date
| `StdDev` | decimal | Standard Deviation of Close price based on `N` lookback periods
| `ZScore` | decimal | Z-Score of current Close price (number of standard deviations from mean)
| `StdDevChange` | decimal | Standard Deviation of Percent Change in Close price based on `N` lookback periods
| `StdDevPercent` | decimal | `Percent = StdDev / Average Close price` over `N` lookback periods ***
| `AvgStdDev` | decimal | Average of standard deviation of Close price over `N` lookback periods  ***
| `AvgStdDevChange` | decimal | Average standard deviation of Percent Change in Close price based on `N` lookback periods ***

*** Note: Percentage and Avg values are opinionated since there is no standard lookback periods for average range.  We use `N` periods to keep things simple.

## Example

```csharp
// fetch historical quotes from your favorite feed, in Quote format
IEnumerable<Quote> history = GetHistoryFromFeed("SPX");

// calculate 10-period Standard Deviation
IEnumerable<StdDevResult> results = Indicator.GetStdDev(history,10);

// use results as needed
DateTime evalDate = DateTime.Parse("12/31/2018");
StdDevResult result = results.Where(x=>x.Date==evalDate).FirstOrDefault();
Console.WriteLine("StdDev(SPX,10) on {0} was ${1}", result.Date, result.StdDev);
```

```bash
StdDev(SPX,10) on 12/31/2018 was $5.4738
```
