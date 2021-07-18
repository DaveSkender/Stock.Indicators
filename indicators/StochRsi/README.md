# Stochastic RSI

Created by by Tushar Chande and Stanley Kroll, [Stochastic RSI](https://school.stockcharts.com/doku.php?id=technical_indicators:stochrsi) is a Stochastic interpretation of the Relative Strength Index.  It is different from, and often confused with the more traditional [Stochastic Oscillator](../Stochastic/README.md).
[[Discuss] :speech_balloon:](https://github.com/DaveSkender/Stock.Indicators/discussions/236 "Community discussion about this indicator")

![image](chart.png)

```csharp
// usage
IEnumerable<StochRsiResult> results =
  quotes.GetStochRsi(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods);
```

## Parameters

| name | type | notes
| -- |-- |--
| `rsiPeriods` | int | Number of periods (`R`) in the lookback period.  Must be greater than 0.  Standard is 14.
| `stochPeriods` | int | Number of periods (`S`) in the lookback period.  Must be greater than 0.  Typically the same value as `rsiPeriods`.
| `signalPeriods` | int | Number of periods (`G`) in the signal line (SMA of the StochRSI).  Must be greater than 0.  Typically 3-5.
| `smoothPeriods` | int | Smoothing periods (`M`) for the Stochastic.  Must be greater than 0.  Default is 1 (Fast variant).

The original Stochasic RSI formula uses a the Fast variant of the Stochastic calculation (`smoothPeriods=1`).  For a standard period of 14, the original formula would be `GetStochRSI(quotes,14,14,3,1)`; though, the "3" here is just for the Signal, which is not present in the original formula, but useful for additional smoothing of the Stochastic RSI.

### Historical quotes requirements

You must have at least `N` periods of `quotes`, where `N` is the greater of `R+S+M` and `R+100`.  Since this uses a smoothing technique in the underlying RSI value, we recommend you use at least `10×R` periods prior to the intended usage date for better precision.

`quotes` is an `IEnumerable<TQuote>` collection of historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](../../docs/GUIDE.md#historical-quotes) for more information.

## Response

```csharp
IEnumerable<StochRsiResult>
```

The first `R+S+M` periods will have `null` values for `StochRsi` since there's not enough data to calculate.  We always return the same number of elements as there are in the historical quotes.

:warning: **Warning**: The first `10×R` periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods.
We recommend pruning at least `R+S+M+100` initial values.

### StochRsiResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `StochRsi` | decimal | %K Oscillator = Stochastic RSI = Stoch(`S`,`G`,`M`) of RSI(`R`) of Close price
| `Signal` | decimal | %D Signal Line = Simple moving average of %K based on `G` periods

### Utilities

- [.Find(lookupDate)](../../docs/UTILITIES.md#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()](../../docs/UTILITIES.md#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)](../../docs/UTILITIES.md#remove-warmup-periods)

See [Utilities and Helpers](../../docs/UTILITIES.md#content) for more information.

## Example

```csharp
// fetch historical quotes from your feed (your method)
IEnumerable<Quote> quotes = GetHistoryFromFeed("SPY");

// calculate StochRSI(14)
IEnumerable<StochRsiResult> results = quotes.GetStochRsi(14,14,1,1);

// use results as needed
StochRsiResult result = results.LastOrDefault();
Console.WriteLine("StochRSI on {0} was {1}", result.Date, result.StochRsi);
```

```bash
StochRSI on 12/31/2018 was 0.975
```
