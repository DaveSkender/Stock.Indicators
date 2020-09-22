# Slope and Linear Regression

[Slope of the best fit line](https://school.stockcharts.com/doku.php?id=technical_indicators:slope), determined by Linear Regression on Close price.  It can be used to help identify trend strength and direction.  R-Squared (R&sup2;) is also output as a fitness measure of the linear regression; this is not the same as the more prevalent [Correlation (R&sup2;)](../Correlation/README.md).  A best-fit `Line` is also returned for the last lookback segment.

<!-- ![image](chart.png) -->

```csharp
// usage
IEnumerable<SlopeResult> results = Indicator.GetSlope(history, lookbackPeriod);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `history` | IEnumerable\<[Quote](../../docs/GUIDE.md#quote)\> | Historical Quotes data should be at any consistent frequency (day, hour, minute, etc).  You must supply at least `N` periods.
| `lookbackPeriod` | int | Number of periods (`N`) for the linear regression.  Must be greater than 0.

## Response

```csharp
IEnumerable<SlopeResult>
```

The first `N-1` periods will have `null` values for `Slope` since there's not enough data to calculate.  We always return the same number of elements as there are in the historical quotes.

### SlopeResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Slope` | decimal | Slope `b` of the best-fit line of Close price
| `Intercept` | decimal | Y-Intercept `m` of the best-fit line
| `StdDev` | double | Standard Deviation of Close price over `N` lookback periods
| `RSquared` | double | R-Squared (R&sup2;), aka Coefficient of Determination
| `Line` | decimal | Best-fit line `y` segment for the last 'N' periods (`y=mx+b` where x is period)

## Example

```csharp
// fetch historical quotes from your favorite feed, in Quote format
IEnumerable<Quote> historySPX = GetHistoryFromFeed("SPX");

// calculate 20-period Slope
IEnumerable<SlopeResult> results = Indicator.GetSlope(history,20);

// use results as needed
DateTime evalDate = DateTime.Parse("12/31/2018");
SlopeResult result = results.Where(x=>x.Date==evalDate).FirstOrDefault();
Console.WriteLine("Slope(20) on {0} was {1}", result.Date, result.Slope);
```

```bash
Slope(20) on 12/31/2018 was 0.85
```
