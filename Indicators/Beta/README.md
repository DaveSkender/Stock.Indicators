# Beta Coefficient

Beta shows how strongly one stock responds to systemic volatility of the entire market.
[More info ...](https://en.wikipedia.org/wiki/Beta_(finance))

```csharp
// usage
IEnumerable<BetaResult> results = Indicator.GetBeta(historyMarket, historyEval, lookbackPeriod);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `historyMarket` | IEnumerable\<[Quote](/GUIDE.md#quote)\> | Historical [market] Quotes data should be at any consistent frequency (day, hour, minute, etc).  You must supply at least `N` periods of history.  This `market` history will be used to establish the baseline.
| `historyEval` | IEnumerable\<[Quote](/GUIDE.md#quote)\> | Historical [evaluation stock] Quotes data should be at any consistent frequency (day, hour, minute, etc).  You must have at least the same matching date elements of `historyMarket`.  Exception will be thrown if not matched.
| `lookbackPeriod` | int | Number of periods (`N`) in the lookback period.  Must be greater than 0 to calculate; however we suggest a larger period for statistically appropriate sample size.

## Response

```csharp
IEnumerable<BetaResult>
```

The first `N-1` periods will have `null` values since there's not enough data to calculate.  We always return the same number of elements as there are in the historical quotes.

### BetaResult

| name | type | notes
| -- |-- |--
| `Index` | int | Sequence of dates
| `Date` | DateTime | Date
| `Beta` | decimal | Beta coefficient based on `N` lookback periods

## Example

```csharp
// fetch historical quotes from your favorite feed, in Quote format
IEnumerable<Quote> historyTSLA = GetHistoryFromFeed("TSLA");
IEnumerable<Quote> historySPX = GetHistoryFromFeed("SPX");

// calculate 20-period Beta coefficient
IEnumerable<BetaResult> results = Indicator.GetBeta(historySPX,historyTSLA,20);

// use results as needed
DateTime evalDate = DateTime.Parse("12/31/2018");
BetaResult result = results.Where(x=>x.Date==evalDate).FirstOrDefault();
Console.WriteLine("Beta(SPX,TSLA,20) on {0} was {1}", result.Date, result.Beta);
```

```bash
Beta(SPX,TSLA,20) on 12/31/2018 was 1.676
```
