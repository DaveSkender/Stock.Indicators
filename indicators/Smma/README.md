# Smoothed Moving Average (SMMA)

[Smoothed Moving Average](https://en.wikipedia.org/wiki/Moving_average#Modified_moving_average) is the average of Close price over a lookback window using a smoothing method.
[[Discuss] :speech_balloon:](https://github.com/DaveSkender/Stock.Indicators/discussions/375 "Community discussion about this indicator")

![image](chart.png)

```csharp
// usage
IEnumerable<SmmaResult> results =
  quotes.GetSmma(lookbackPeriods);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `lookbackPeriods` | int | Number of periods (`N`) in the moving average.  Must be greater than 0.

### Historical quotes requirements

You must have at least `2×N` or `N+100` periods of `quotes`, whichever is more.  Since this uses a smoothing technique, we recommend you use at least `N+250` data points prior to the intended usage date for better precision.

`quotes` is an `IEnumerable<TQuote>` collection of historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](../../docs/GUIDE.md#historical-quotes) for more information.

## Response

```csharp
IEnumerable<SmmaResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N-1` periods will have `null` values since there's not enough data to calculate.

:hourglass: **Convergence Warning**: The first `N+100` periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods.

### SmmaResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Smma` | decimal | Smoothed moving average

### Utilities

- [.Find(lookupDate)](../../docs/UTILITIES.md#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()](../../docs/UTILITIES.md#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)](../../docs/UTILITIES.md#remove-warmup-periods)

See [Utilities and Helpers](../../docs/UTILITIES.md#content) for more information.

## Example

```csharp
// fetch historical quotes from your feed (your method)
IEnumerable<Quote> quotes = GetHistoryFromFeed("MSFT");

// calculate 20-period SMMA
IEnumerable<SmmaResult> results = quotes.GetSmma(20);
```
