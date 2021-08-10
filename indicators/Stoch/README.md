# Stochastic Oscillator

Created by George Lane, the [Stochastic Oscillator](https://en.wikipedia.org/wiki/Stochastic_oscillator) is a momentum indicator that looks back `N` periods to produce a scale of 0 to 100.  %J is also included for the KDJ Index extension.
[[Discuss] :speech_balloon:](https://github.com/DaveSkender/Stock.Indicators/discussions/237 "Community discussion about this indicator")

![image](chart.png)

```csharp
// usage
IEnumerable<StochResult> results =
  quotes.GetStoch(lookbackPeriods, signalPeriods, smoothPeriods);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `lookbackPeriods` | int | Lookback period (`N`) for the oscillator (%K).  Must be greater than 0.  Default is 14.
| `signalPeriods` | int | Smoothing period for the signal (%D).  Must be greater than 0.  Default is 3.
| `smoothPeriods` | int | Smoothing period (`S`) for the Oscillator (%K).  "Slow" stochastic uses 3, "Fast" stochastic uses 1.  Must be greater than 0.  Default is 3.

### Historical quotes requirements

You must have at least `N+S` periods of `quotes`.

`quotes` is an `IEnumerable<TQuote>` collection of historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](../../docs/GUIDE.md#historical-quotes) for more information.

## Response

```csharp
IEnumerable<StochResult>
```

The first `N+S-2` periods will have `null` Oscillator values since there's not enough data to calculate.  We always return the same number of elements as there are in the historical quotes.

### StochResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Oscillator` | decimal | %K Oscillator over prior `N` lookback periods
| `Signal` | decimal | %D Simple moving average of Oscillator
| `PercentJ` | decimal | %J is the weighted divergence of %K and %D: `%J=3×%K-2×%D`

### Utilities

- [.Find(lookupDate)](../../docs/UTILITIES.md#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()](../../docs/UTILITIES.md#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)](../../docs/UTILITIES.md#remove-warmup-periods)

See [Utilities and Helpers](../../docs/UTILITIES.md#content) for more information.

## Example

```csharp
// fetch historical quotes from your feed (your method)
IEnumerable<Quote> quotes = GetHistoryFromFeed("SPY");

// calculate STO %K(14),%D(3) (slow)
IEnumerable<StochResult> results = quotes.GetStoch(14,3,3);
```
