# Awesome Oscillator (AO)

Created by Bill Williams, the Awesome Oscillator (aka Super AO) is a measure of the gap between a fast and slow period modified moving average.
[[Discuss] :speech_balloon:](https://github.com/DaveSkender/Stock.Indicators/discussions/282 "Community discussion about this indicator")

![image](chart.png)

```csharp
// usage
IEnumerable<AwesomeResult> results =
  quotes.GetAwesome(fastPeriods, slowPeriods);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `fastPeriods` | int | Number of periods (`F`) for the faster moving average.  Must be greater than 0.  Default is 5.
| `slowPeriods` | int | Number of periods (`S`) for the slower moving average.  Must be greater than `fastPeriods`.  Default is 34.

### Historical quotes requirements

You must have at least `S` periods of `quotes`.

`quotes` is an `IEnumerable<TQuote>` collection of historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](../../docs/GUIDE.md) for more information.

## Response

```csharp
IEnumerable<AwesomeResult>
```

The first period `S-1` periods will have `null` values since there's not enough data to calculate.  We always return the same number of elements as there are in the historical quotes.

### AwesomeResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Oscillator` | decimal | Awesome Oscillator
| `Normalized` | decimal | `100 × Oscillator ÷ (median price)`

### Utilities

- [.Find()](../../docs/UTILITIES.md#find-indicator-result-by-date)
- [.PruneWarmupPeriods()](../../docs/UTILITIES.md#prune-warmup-periods)
- [.PruneWarmupPeriods(qty)](../../docs/UTILITIES.md#prune-warmup-periods)

See [Utilities and Helpers](../../docs/UTILITIES.md#content) for more information.

## Example

```csharp
// fetch historical quotes from your feed (your method)
IEnumerable<Quote> quotes = GetHistoryFromFeed("MSFT");

// calculate
IEnumerable<AwesomeResult> results = quotes.GetAwesome(5,34);

// use results as needed
AwesomeResult r = results.LastOrDefault();
Console.WriteLine("AO on {0} was {1}", r.Date, r.Oscillator);
```

```bash
AO on 12/31/2018 was -17.77
```
