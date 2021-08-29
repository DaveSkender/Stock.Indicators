# Parabolic SAR (stop and reverse)

Created by J. Welles Wilder, [Parabolic SAR](https://en.wikipedia.org/wiki/Parabolic_SAR) (stop and reverse) is a price-time based indicator used to determine trend direction and reversals.
[[Discuss] :speech_balloon:](https://github.com/DaveSkender/Stock.Indicators/discussions/245 "Community discussion about this indicator")

![image](chart.png)

```csharp
// usage
IEnumerable<ParabolicSarResult> results =
  quotes.GetParabolicSar(accelerationStep, maxAccelerationFactor);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `accelerationStep` | decimal | Incremental step size.  Must be greater than 0.  Default is 0.02
| `maxAccelerationFactor` | decimal | Maximimum step limit.  Must be greater than `accelerationStep`.  Default is 0.2

### Historical quotes requirements

At least two quotes records are required to calculate; however, we recommend at least 100 data points.  Initial Parabolic SAR values prior to the first reversal are not accurate and are excluded from the results.  Therefore, provide sufficient quotes to capture prior trend reversals, before your intended usage period.

`quotes` is an `IEnumerable<TQuote>` collection of historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](../../docs/GUIDE.md#historical-quotes) for more information.

## Response

```csharp
IEnumerable<ParabolicSarResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first trend will have `null` values since it is not accurate and based on an initial guess.

### ParabolicSarResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Sar` | decimal | Stop and Reverse value
| `IsReversal` | bool | Indicates a trend reversal

### Utilities

- [.Find(lookupDate)](../../docs/UTILITIES.md#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()](../../docs/UTILITIES.md#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)](../../docs/UTILITIES.md#remove-warmup-periods)

See [Utilities and Helpers](../../docs/UTILITIES.md#utilities-for-indicator-results) for more information.

## Example

```csharp
// fetch historical quotes from your feed (your method)
IEnumerable<Quote> quotes = GetHistoryFromFeed("SPY");

// calculate ParabolicSar(0.02,0.2)
IEnumerable<ParabolicSarResult> results
  = quotes.GetParabolicSar(0.02,0.2);
```
