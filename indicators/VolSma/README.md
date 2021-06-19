# Volume Simple Moving Average

The Volume Simple Moving Average is the average volume over a lookback window.  This is helpful when you are trying to assess whether volume is above or below normal.
[[Discuss] :speech_balloon:](https://github.com/DaveSkender/Stock.Indicators/discussions/230 "Community discussion about this indicator")

![image](chart.png)

```csharp
// usage
IEnumerable<VolSmaResult> results =
  history.GetVolSma(lookbackPeriod);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `lookbackPeriod` | int | Number of periods (`N`) in the moving average.  Must be greater than 0.

### Historical quotes requirements

You must have at least `N` periods of `history`.

`history` is an `IEnumerable<TQuote>` collection of historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](../../docs/GUIDE.md) for more information.

## Response

```csharp
IEnumerable<VolSmaResult>
```

The first `N-1` periods will have `null` values for `VolSma` since there's not enough data to calculate.  We always return the same number of elements as there are in the historical quotes.

### VolSmaResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Volume` | decimal | Volume
| `VolSma` | decimal | Simple moving average of `Volume` for `N` lookback periods

## Example

```csharp
// fetch historical quotes from your feed (your method)
IEnumerable<Quote> history = GetHistoryFromFeed("MSFT");

// calculate 20-period SMA of Volume
IEnumerable<VolSmaResult> results = history.GetVolSma(20);

// use results as needed
VolSmaResult result = results.LastOrDefault();
Console.WriteLine("Average Volume on {0} was {1}", result.Date, result.VolSma);
```

```bash
Average Volume on 12/31/2018 was 163695200
```
