# Choppiness Index

Created by E.W. Dreiss, the Choppiness Index measures the trendiness or choppiness on a scale of 0 to 100, to depict steady trends versus conditions of choppiness.  [[Discuss] :speech_balloon:](https://github.com/DaveSkender/Stock.Indicators/discussions/357 "Community discussion about this indicator")

![image](chart.png)

```csharp
// usage
IEnumerable<ChopResult> results = Indicator.GetChop(history, lookbackPeriod);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `history` | IEnumerable\<[TQuote](../../docs/GUIDE.md#historical-quotes)\> | Historical price quotes should have a consistent frequency (day, hour, minute, etc).
| `lookbackPeriod` | int | Number of periods (`N`) for the lookback evaluation.  Must be greater than 1.  Default is 14.

### Minimum history requirements

You must supply at least `N+1` periods of `history`.

## Response

```csharp
IEnumerable<ChopResult>
```

The first `N` periods will have `null` values since there's not enough data to calculate.  We always return the same number of elements as there are in the historical quotes.

### ChopResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Chop` | decimal | Choppiness Index

## Example

```csharp
// fetch historical quotes from your favorite feed, in Quote format
IEnumerable<Quote> history = GetHistoryFromFeed("SPY");

// calculate CHOP(14)
IEnumerable<ChopResult> results = Indicator.GetChop(history,14);

// use results as needed
ChopResult result = results.LastOrDefault();
Console.WriteLine("CHOP(14) on {0} was {1}", result.Date, result.Chop);
```

```bash
CHOP(14) on 12/31/2018 was 38.65 
```
