# [Linear] Weighted Moving Average (WMA)

[Weighted Moving Average](https://en.wikipedia.org/wiki/Moving_average#Weighted_moving_average) is the linear weighted average of `Close` price over `N` lookback periods.  This also called Linear Weighted Moving Average (LWMA).
[[Discuss] :speech_balloon:](https://github.com/DaveSkender/Stock.Indicators/discussions/227 "Community discussion about this indicator")

![image](chart.png)

```csharp
// usage
IEnumerable<WmaResult> results = Indicator.GetWma(history, lookbackPeriod);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `history` | IEnumerable\<[TQuote](../../docs/GUIDE.md#historical-quotes)\> | Historical price quotes should have a consistent frequency (day, hour, minute, etc).
| `lookbackPeriod` | int | Number of periods (`N`) in the moving average.  Must be greater than 0.

### Minimum history requirements

You must supply at least `N` periods of `history`.

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
WmaResult result = results.LastOrDefault();
Console.WriteLine("WMA on {0} was ${1}", result.Date, result.Wma);
```

```bash
WMA on 12/31/2018 was $235.53
```
