# Hull Moving Average (HMA)

Created by Alan Hull, the [Hull Moving Average](https://alanhull.com/hull-moving-average) is a modified weighted average of `Close` price over `N` lookback periods that reduces lag.
[[Discuss] :speech_balloon:](https://github.com/DaveSkender/Stock.Indicators/discussions/252 "Community discussion about this indicator")

![image](chart.png)

```csharp
// usage
IEnumerable<HmaResult> results =
  history.GetHma(lookbackPeriods);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `lookbackPeriods` | int | Number of periods (`N`) in the moving average.  Must be greater than 1.

### Historical quotes requirements

You must have at least `N` periods of `history`.

`history` is an `IEnumerable<TQuote>` collection of historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](../../docs/GUIDE.md) for more information.

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

### Utilities

- [.Find()](../../docs/UTILITIES.md#find-indicator-result-by-date)
- [.PruneWarmupPeriods()](../../docs/UTILITIES.md#prune-warmup-periods)
- [.PruneWarmupPeriods(qty)](../../docs/UTILITIES.md#prune-warmup-periods)

See [Utilities and Helpers](../../docs/UTILITIES.md#content) for more information.

## Example

```csharp
// fetch historical quotes from your feed (your method)
IEnumerable<Quote> history = GetHistoryFromFeed("MSFT");

// calculate 20-period HMA
IEnumerable<HmaResult> results = history.GetHma(20);

// use results as needed
HmaResult result = results.LastOrDefault();
Console.WriteLine("HMA on {0} was ${1}", result.Date, result.Hma);
```

```bash
HMA on 12/31/2018 was $235.70
```
