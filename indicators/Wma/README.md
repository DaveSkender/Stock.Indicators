# [Linear] Weighted Moving Average (WMA)

Weighted moving average is the linear weighted average of `Close` price over `N` lookback periods.  This also called Linear Weighted Moving Average (LWMA).
[More info ...](https://www.fidelity.com/learning-center/trading-investing/technical-analysis/technical-indicator-guide/wma)

```csharp
// usage
IEnumerable<WmaResult> results = Indicator.GetWma(history, lookbackPeriod);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `history` | IEnumerable\<[Quote](../../docs/GUIDE.md#quote)\> | Historical Quotes data should be at any consistent frequency (day, hour, minute, etc).  You must supply at least `N` periods of `history`.
| `lookbackPeriod` | int | Number of periods (`N`) in the moving average.  Must be greater than 0.

## Response

```csharp
IEnumerable<WmaResult>
```

The first `N-1` periods will have `null` values since there's not enough data to calculate.  We always return the same number of elements as there are in the historical quotes.

### WmaResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Wma` | decimal | Weighted moving average for `N` lookback periods

## Example

```csharp
// fetch historical quotes from your favorite feed, in Quote format
IEnumerable<Quote> history = GetHistoryFromFeed("MSFT");

// calculate 20-period WMA
IEnumerable<WmaResult> results = Indicator.GetWma(history,20);

// use results as needed
DateTime evalDate = DateTime.Parse("12/31/2018");
WmaResult result = results.Where(x=>x.Date==evalDate).FirstOrDefault();
Console.WriteLine("WMA on {0} was ${1}", result.Date, result.Wma);
```

```bash
WMA on 12/31/2018 was $235.53
```
