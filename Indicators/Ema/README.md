# Exponential Moving Average (EMA)

Exponentially weighted moving average of the Close price over `N` periods.
[More info ...](https://school.stockcharts.com/doku.php?id=technical_indicators:moving_averages)

``` C#
// usage
IEnumerable<EmaResult> results = Indicator.GetEma(history, lookbackPeriod);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `history` | IEnumerable\<[Quote](/GUIDE.md#Quote)\> | Historical Quotes data should be at any consistent frequency (day, hour, minute, etc).  You must supply at least 2×`N` periods of `history`.  Since this uses a smoothing technique, we recommend you use at least 250 data points prior to the intended usage date for maximum precision.
| `lookbackPeriod` | int | Number of periods (`N`) in the moving average.

## Response

``` C#
IEnumerable<EmaResult>
```

The first `N-1` periods will have `null` values since there's not enough data to calculate.  We always return the same number of elements as there are in the historical quotes.

### EmaResult

| name | type | notes
| -- |-- |--
| `Index` | int | Sequence of dates
| `Date` | DateTime | Date
| `Ema` | decimal | Exponential moving average for `N` lookback period

## Example

``` C#
// fetch historical quotes from your favorite feed, in Quote format
IEnumerable<Quote> history = GetHistoryFromFeed("SPY");

// calculate 20-period EMA
IEnumerable<EmaResult> results = Indicator.GetEma(history,20);

// use results as needed
DateTime evalDate = DateTime.Parse("12/31/2018");
EmaResult result = results.Where(x=>x.Date==evalDate).FirstOrDefault();
Console.WriteLine("EMA on {0} was ${1}", result.Date, result.Value);
```

``` text
EMA on 12/31/2018 was $249.3519
```
