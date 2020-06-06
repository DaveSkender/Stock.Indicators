# Stochastic StochRSI

Stochastic interpretation of the Relative Strength Index.
[More info ...](https://school.stockcharts.com/doku.php?id=technical_indicators:stochrsi)

```csharp
// usage
IEnumerable<StochRsiResult> results = Indicator.GetStochRsi(history, lookbackPeriod);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `history` | IEnumerable\<[Quote](/GUIDE.md#Quote)\> | Historical Quotes data should be at any consistent frequency (day, hour, minute, etc).  You must supply at least 2×`N` periods of `history`.  Since this uses a smoothing technique in the underlying RSI value, we recommend you use at least 250 data points prior to the intended usage date for maximum precision.
| `lookbackPeriod` | int | Number of periods (`N`) in the lookback period.

## Response

```csharp
IEnumerable<StochRsiResult>
```

The first `2×N-1` periods will have `null` values since there's not enough data to calculate.  We always return the same number of elements as there are in the historical quotes.

### StochRsiResult

| name | type | notes
| -- |-- |--
| `Index` | int | Sequence of dates
| `Date` | DateTime | Date
| `StochRsi` | float | StochRSI over prior `N` lookback periods
| `IsIncreasing` | bool | Direction since last period (e.g. up or down).  Is `null` for no change.

## Example

```csharp
// fetch historical quotes from your favorite feed, in Quote format
IEnumerable<Quote> history = GetHistoryFromFeed("SPY");

// calculate StochRSI(14)
IEnumerable<StochRsiResult> results = Indicator.GetStochRsi(history,14);

// use results as needed
DateTime evalDate = DateTime.Parse("12/31/2018");
StochRsiResult result = results.Where(x=>x.Date==evalDate).FirstOrDefault();
Console.WriteLine("StochRSI on {0} was {1}", result.Date, result.StochRsi);
```

```bash
StochRSI on 12/31/2018 was 0.975
```
