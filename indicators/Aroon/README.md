# Aroon

Created by Tushar Chande, [Aroon](https://school.stockcharts.com/doku.php?id=technical_indicators:aroon) is a simple oscillator view of how long the new high or low price occured over a lookback window.
[[Discuss] :speech_balloon:](https://github.com/DaveSkender/Stock.Indicators/discussions/266 "Community discussion about this indicator")

![image](chart.png)

```csharp
// usage
IEnumerable<AroonResult> results =
  history.GetAroon(lookbackPeriod);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `lookbackPeriod` | int | Number of periods (`N`) for the lookback evaluation.  Must be greater than 0.  Default is 25.

### Historical quotes requirements

You must have at least `N` periods of `history`.

`history` is an `IEnumerable<TQuote>` collection of historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](../../docs/GUIDE.md) for more information.

## Response

```csharp
IEnumerable<AroonResult>
```

The first `N-1` periods will have `null` Aroon values since there's not enough data to calculate.  We always return the same number of elements as there are in the historical quotes.

### AroonResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `AroonUp` | decimal | Based on last High price
| `AroonDown` | decimal | Based on last Low price
| `Oscillator` | decimal | AroonUp - AroonDown

## Example

```csharp
// fetch historical quotes from your feed (your method)
IEnumerable<Quote> history = GetHistoryFromFeed("SPY");

// calculate Aroon(25)
IEnumerable<AroonResult> results = history.GetAroon(25);

// use results as needed
AroonResult result = results.LastOrDefault();
Console.WriteLine("Aroon-Up(25) on {0} was {1}", result.Date, result.AroonUp);
```

```bash
Aroon-Up(25) on 12/31/2018 was 28.0
```
