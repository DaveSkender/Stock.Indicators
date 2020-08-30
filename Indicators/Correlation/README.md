# Correlation Coefficient

Correlation between two quote histories, based on Close price.  R-Squared (R&sup2;), Variance, and covariance are also output.
[More info ...](https://school.stockcharts.com/doku.php?id=technical_indicators:correlation_coeffici)

```csharp
// usage
IEnumerable<CorrResult> results = Indicator.GetCorr(historyA, historyB, lookbackPeriod);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `historyA` | IEnumerable\<[Quote](../../GUIDE.md#quote)\> | Historical Quotes data should be at any consistent frequency (day, hour, minute, etc).  You must supply at least `N` periods of `history`.  The `A` history will be used to establish result length, so use the shorter history here.
| `historyB` | IEnumerable\<[Quote](../../GUIDE.md#quote)\> | Historical Quotes data should be at any consistent frequency (day, hour, minute, etc).  You must have at least the same matching date elements of `historyA`.  Exception will be thrown if not matched.
| `lookbackPeriod` | int | Number of periods (`N`) in the lookback period.  Must be greater than 0 to calculate; however we suggest a larger period for statistically appropriate sample size.

## Response

```csharp
IEnumerable<CorrResult>
```

The first `N-1` periods will have `null` values since there's not enough data to calculate.  We always return the same number of elements as there are in the historical quotes.

### CorrResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `VarianceA` | decimal | Variance of A based on `N` lookback periods
| `VarianceB` | decimal | Variance of B based on `N` lookback periods
| `Covariance` | decimal | Covariance of A+B based on `N` lookback periods
| `Correlation` | decimal | Correlation `R` based on `N` lookback periods
| `RSquared` | decimal | R-Squared (R&sup2;), aka Coefficient of Determination.  Simple linear regression models is used (square of Correlation).

## Example

```csharp
// fetch historical quotes from your favorite feed, in Quote format
IEnumerable<Quote> historyTSLA = GetHistoryFromFeed("TSLA");
IEnumerable<Quote> historySPX = GetHistoryFromFeed("SPX");

// calculate 20-period Correlation
IEnumerable<CorrResult> results = Indicator.GetCorr(historySPX,historyTSLA,20);

// use results as needed
DateTime evalDate = DateTime.Parse("12/31/2018");
CorrResult result = results.Where(x=>x.Date==evalDate).FirstOrDefault();
Console.WriteLine("CORR(SPX,TSLA,20) on {0} was {1}", result.Date, result.Corr);
```

```bash
CORR(SPX,TSLA,20) on 12/31/2018 was 0.85
```
