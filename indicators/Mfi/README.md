# Money Flow Index (MFI)

Created by Quong and Soudack, the [Money Flow Index](https://en.wikipedia.org/wiki/Money_flow_index) is a price-volume oscillator that shows buying and selling momentum.
[[Discuss] :speech_balloon:](https://github.com/DaveSkender/Stock.Indicators/discussions/247 "Community discussion about this indicator")

![image](chart.png)

```csharp
// usage
IEnumerable<MfiResult> results =
  quotes.GetMfi(lookbackPeriods);
```

## Parameters

| name | type | notes
| -- |-- |--
| `lookbackPeriods` | int | Number of periods (`N`) in the lookback period.  Must be greater than 1. Default is 14.

### Historical quotes requirements

You must have at least `N+1` historical quotes.

`quotes` is an `IEnumerable<TQuote>` collection of historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](../../docs/GUIDE.md) for more information.

## Response

```csharp
IEnumerable<MfiResult>
```

The first `N` periods will have `null` MFI values since they cannot be calculated.  We always return the same number of elements as there are in the historical quotes.

### MfiResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Mfi` | decimal | Money Flow Index

### Utilities

- [.Find()](../../docs/UTILITIES.md#find-indicator-result-by-date)
- [.PruneWarmupPeriods()](../../docs/UTILITIES.md#prune-warmup-periods)
- [.PruneWarmupPeriods(qty)](../../docs/UTILITIES.md#prune-warmup-periods)

See [Utilities and Helpers](../../docs/UTILITIES.md#content) for more information.

## Example

```csharp
// fetch historical quotes from your feed (your method)
IEnumerable<Quote> quotes = GetHistoryFromFeed("SPY");

// calculate
IEnumerable<MfiResult> results = quotes.GetMfi(14);

// use results as needed
MfiResult result = results.LastOrDefault();
Console.WriteLine("MFI on {0} was {1}", result.Date, result.Mfi);
```

```bash
MFI on 12/31/2018 was 39.95
```
