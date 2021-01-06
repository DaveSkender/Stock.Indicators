# Hull Moving Average (HMA)

Created by Alan Hull, the [Hull Moving Average](https://alanhull.com/hull-moving-average) is a modified weighted average of `Close` price over `N` lookback periods that reduces lag.
[[Discuss] :speech_balloon:](https://github.com/DaveSkender/Stock.Indicators/discussions/252 "Community discussion about this indicator")

![image](chart.png)

```csharp
// usage
IEnumerable<HmaResult> results = Indicator.GetHma(history, lookbackPeriod);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `history` | IEnumerable\<[TQuote](../../docs/GUIDE.md#historical-quotes)\> | Historical price quotes should have a consistent frequency (day, hour, minute, etc).
| `lookbackPeriod` | int | Number of periods (`N`) in the moving average.  Must be greater than 1.

### Minimum history requirements

You must supply at least `N` periods of `history`.

## Response

```csharp
IEnumerable<HmaResult>
```

The first `N-(integer of SQRT(N))-1` periods will have `null` values since there's not enough data to calculate.  We always return the same number of elements as there are in the historical quotes.

### HmaResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Hma` | decimal | Hull moving average for `N` lookback periods

## Example

```csharp
// fetch historical quotes from your favorite feed, in Quote format
IEnumerable<Quote> history = GetHistoryFromFeed("MSFT");

// calculate 20-period HMA
IEnumerable<HmaResult> results = Indicator.GetHma(history,20);

// use results as needed
HmaResult result = results.LastOrDefault();
Console.WriteLine("HMA on {0} was ${1}", result.Date, result.Hma);
```

```bash
HMA on 12/31/2018 was $235.70
```
