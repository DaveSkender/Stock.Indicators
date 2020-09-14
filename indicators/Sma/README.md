# Simple Moving Average (SMA)

Simple moving average is the average of Close price of `N` lookback periods.
[More info ...](https://school.stockcharts.com/doku.php?id=technical_indicators:moving_averages)

![image](chart.png)

```csharp
// usage
IEnumerable<SmaResult> results = Indicator.GetSma(history, lookbackPeriod);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `history` | IEnumerable\<[Quote](../../docs/GUIDE.md#quote)\> | Historical Quotes data should be at any consistent frequency (day, hour, minute, etc).  You must supply at least `N` periods of `history`.
| `lookbackPeriod` | int | Number of periods (`N`) in the moving average.  Must be greater than 0.
| `extended` | bool | A `true` will include values for MAD, MSE, and MAPE.  Default is `false`.

## Response

```csharp
IEnumerable<SmaResult>
```

The first `N-1` periods will have `null` values since there's not enough data to calculate.  We always return the same number of elements as there are in the historical quotes.

### SmaResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Sma` | decimal | Simple moving average for `N` lookback periods
| `Mad` | decimal | Mean absolute deviation
| `Mse` | decimal | Mean square error
| `Mape` | decimal | Mean absolute percentage error

MAD, MSE, and MAPE values are only included if you set `extended` to `true`

## Example

```csharp
// fetch historical quotes from your favorite feed, in Quote format
IEnumerable<Quote> history = GetHistoryFromFeed("MSFT");

// calculate 20-period SMA
IEnumerable<SmaResult> results = Indicator.GetSma(history,20);

// use results as needed
DateTime evalDate = DateTime.Parse("12/31/2018");
SmaResult result = results.Where(x=>x.Date==evalDate).FirstOrDefault();
Console.WriteLine("SMA on {0} was ${1}", result.Date, result.Sma);
```

```bash
SMA on 12/31/2018 was $251.86
```
