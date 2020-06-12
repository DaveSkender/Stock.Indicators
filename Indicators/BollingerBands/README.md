# Bollinger Bands

Bollinger Bands indicate volatility and displays standard deviation boundary lines from moving average of Close price.
[More info ...](https://school.stockcharts.com/doku.php?id=technical_indicators:bollinger_bands)

```csharp
// usage
IEnumerable<BollingerBandsResult> results = Indicator.GetBollingerBands(history, lookbackPeriod, standardDeviation);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `history` | IEnumerable\<[Quote](/GUIDE.md#Quote)\> | Historical Quotes data should be at any consistent frequency (day, hour, minute, etc).  You must supply at least `N` periods of `history`.
| `lookbackPeriod` | int | Number of periods (`N`) for the center line moving average.  Must be greater than 1 to calculate; however we suggest a larger period for statistically appropriate sample size.  Default is 20.
| `standardDeviation` | int | Width of bands.  Standard deviations (`D`) from the moving average.  Must be greater than 0.  Default is 2.

## Response

```csharp
IEnumerable<BollingerBandsResult>
```

The first `N-1` slow periods + signal period will have `null` values since there's not enough data to calculate.  We always return the same number of elements as there are in the historical quotes.

### BollingerBandsResult

| name | type | notes
| -- |-- |--
| `Index` | int | Sequence of dates
| `Date` | DateTime | Date
| `Sma` | decimal | Simple moving average (SMA) of Close price (center line)
| `UpperBand` | decimal | Upper line is `D` standard deviations above the SMA
| `LowerBand` | decimal | Lower line is `D` standard deviations below the SMA
| `ZScore` | decimal | Z-Score of current Close price (number of standard deviations from mean)
| `Width` | decimal | Width as percent of SMA price.  `(UpperBand-LowerBand)/Sma`
| `IsDiverging` | bool | Upper and Lower bands are diverging

## Example

```csharp
// fetch historical quotes from your favorite feed, in Quote format
IEnumerable<Quote> history = GetHistoryFromFeed("SPY");

// calculate BollingerBands(12,26,9)
IEnumerable<BollingerBandsResult> results = Indicator.GetBollingerBands(history,20,2);

// use results as needed
DateTime evalDate = DateTime.Parse("12/31/2018");
BollingerBandsResult result = results.Where(x=>x.Date==evalDate).FirstOrDefault();
Console.WriteLine("Upper Bollinger Band on {0} was ${1}", result.Date, result.UpperBand);
```

```bash
Upper Bollinger Band on 12/31/2018 was $273.7
```
