# Price Relative Strength (PRS)

(this indicator is not yet available in the library)

[Price Relative Strength (PRS)](https://en.wikipedia.org/wiki/Relative_strength), also called Comparative Relative Strength, shows the ratio of two quote histories, based on change in Close price over a lookback period.  It is often used to compare against a market index or sector ETF.  This is not the same as the more prevalent [Relative Strength Index (RSI)](../Rsi/README.md).

![image](chart.png)

```csharp
// usage
IEnumerable<PrsResult> results = Indicator.GetPrs(historyBase, historyEval);  

// usage with optional SMA of PRS (shown above)
IEnumerable<PrsResult> results = Indicator.GetPrs(historyBase, historyEval, smaPeriod);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `historyBase` | IEnumerable\<[Quote](../../docs/GUIDE.md#quote)\> | This is usually market index data, but could be any baseline data that you might use for comparison.  You must supply at least one periods of `historyBase` to calculate, but more is typically used.
| `historyEval` | IEnumerable\<[Quote](../../docs/GUIDE.md#quote)\> | Historical quotes for evaluation.  You must supply the same number of periods as `historyBase`.
| `smaPeriod` | int | Optional.  Number of periods (`N`) in the SMA lookback period.  Must be greater than 0 to calculate.

Note: Historical Quotes data should be at any consistent frequency (day, hour, minute, etc).  For this indicator, the elements must match (e.g. the `n`th elements must be the same date).  An `Exception` will be thrown for mismatch dates.

## Response

```csharp
IEnumerable<PrsResult>
```

The first `N-1` periods will have `null` values for `Sma` since there's not enough data to calculate.  We always return the same number of elements as there are in the historical quotes.

### PrResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Prs` | decimal | Price Relative Strength compares `Eval` to `Base` histories
| `Sma` | decimal | Moving Average (SMA) of `Prs` over `N` periods

## Example

```csharp
// fetch historical quotes from your favorite feed, in Quote format
IEnumerable<Quote> historySPX = GetHistoryFromFeed("SPX");
IEnumerable<Quote> historyTSLA = GetHistoryFromFeed("TSLA");

// calculate 20-period Prs
IEnumerable<PrResult> results = Indicator.GetPrs(historySPX,historyTSLA,14);

// use results as needed
DateTime evalDate = DateTime.Parse("12/31/2018");
PrResult result = results.Where(x=>x.Date==evalDate).FirstOrDefault();
Console.WriteLine("PR(SPX,TSLA,20) on {0} was {1}", result.Date, result.PriceRatio);
```

```bash
PR(SPX,TSLA,20) on 12/31/2018 was 1.36
```
