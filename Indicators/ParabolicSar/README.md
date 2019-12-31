# Parabolic SAR

Parabolic SAR is a price-time based indicator.
[More info ...](https://school.stockcharts.com/doku.php?id=technical_indicators:parabolic_sar)

``` C#
// usage
IEnumerable<ParabolicSarResult> results = Indicator.GetParabolicSar(history, accelerationStep, maxAccelerationFactor);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `history` | IEnumerable\<[Quote](../GUIDE.md#Quote)\> | Historical Quotes data should be at any consistent frequency (day, hour, minute, etc).  Provide sufficient history to capture prior trend reversals, before your usage period.
| `accelerationStep` | decimal | Incremental step size
| `maxAccelerationFactor` | decimal | Maximimum step limit

NOTE: Initial Parabolic SAR values before the first reversal are not accurate.

## Response

``` C#
IEnumerable<ParabolicSarResult>
```

The first period will have `null` values since there's not enough data to calculate.  We always return the same number of elements as there are in the historical quotes.

### ParabolicSarResult

| name | type | notes
| -- |-- |--
| `Index` | int | Sequence of dates
| `Date` | DateTime | Date
| `Sar` | decimal | Stop and Reverse value
| `IsReversal` | bool | Indicates a trend reversal
| `IsBullish` | bool | Rising (true), Falling (false)

## Example

``` C#
// fetch historical quotes from your favorite feed, in Quote format
IEnumerable<Quote> history = GetHistoryFromFeed("SPY");

// calculate ParabolicSar(0.02,0.2)
IEnumerable<ParabolicSarResult> results = Indicator.GetParabolicSar(history,0.02,0.2);

// use results as needed
DateTime evalDate = DateTime.Parse("12/31/2018");
ParabolicSarResult result = results.Where(x=>x.Date==evalDate).FirstOrDefault();
Console.WriteLine("SAR on {0} was ${1}", result.Date, result.Sar);
```

``` text
SAR on 12/31/2018 was $229.76
```
