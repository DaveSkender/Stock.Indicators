# Ulcer Index (UI)

Created by Peter Martin, the [Ulcer Index](https://en.wikipedia.org/wiki/Ulcer_index) is a measure of downside Close price volatility over a lookback window.
[[Discuss] :speech_balloon:](https://github.com/DaveSkender/Stock.Indicators/discussions/232 "Community discussion about this indicator")

![image](chart.png)

```csharp
// usage
IEnumerable<UlcerIndexResult> results = Indicator.GetUlcerIndex(history, lookbackPeriod);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `history` | IEnumerable\<[TQuote](../../docs/GUIDE.md#historical-quotes)\> | Historical price quotes should have a consistent frequency (day, hour, minute, etc).
| `lookbackPeriod` | int | Number of periods (`N`) for review.  Must be greater than 0.  Default is 14.

### Minimum history requirements

You must supply at least `N` periods of `history`.

## Response

```csharp
IEnumerable<UlcerIndexResult>
```

The first `N-1` slow periods + signal period will have `null` values since there's not enough data to calculate.  We always return the same number of elements as there are in the historical quotes.

### UlcerIndexResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `UI` | decimal | Ulcer Index

## Example

```csharp
// fetch historical quotes from your favorite feed, in Quote format
IEnumerable<Quote> history = GetHistoryFromFeed("SPY");

// calculate UI(14)
IEnumerable<UlcerIndexResult> results = Indicator.GetUlcerIndex(history,14);

// use results as needed
UlcerIndexResult result = results.LastOrDefault();
Console.WriteLine("Ulcer Index on {0} was {1}", result.Date, result.UI);
```

```bash
Ulcer Index on 12/31/2018 was 5.73
```
