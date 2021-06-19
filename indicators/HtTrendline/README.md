# Hilbert Transform Instantaneous Trendline

Created by John Ehlers, the Hilbert Transform Instantaneous Trendline is a 5-period trendline of high/low price that uses signal processing to reduce noise.
[[Discuss] :speech_balloon:](https://github.com/DaveSkender/Stock.Indicators/discussions/363 "Community discussion about this indicator")

![image](chart.png)

```csharp
// usage
IEnumerable<HtlResult> results =
  history.GetHtTrendline();
```

## Historical quotes requirements

Since this indicator has a warmup period, you must have at least `100` periods of `history`.

`history` is an `IEnumerable<TQuote>` collection of historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](../../docs/GUIDE.md) for more information.

## Response

```csharp
IEnumerable<HtlResult>
```

The first `6` periods will have `null` values for `SmoothPrice` since there's not enough data to calculate.  We always return the same number of elements as there are in the historical quotes.

:warning: **Warning**: The first `100` periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods.

### HtlResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Trendline` | decimal | HT Trendline
| `SmoothPrice` | decimal | Weighted moving average of `(H+L)/2` price

## Example

```csharp
// fetch historical quotes from your feed (your method)
IEnumerable<Quote> history = GetHistoryFromFeed("MSFT");

// calculate HT Trendline
IEnumerable<HtlResult> results = history.GetHtTrendline();

// use results as needed
HtlResult result = results.LastOrDefault();
Console.WriteLine("HTL on {0} was ${1}", result.Date, result.Trendline);
```

```bash
HTL on 12/31/2018 was $242.34
```
