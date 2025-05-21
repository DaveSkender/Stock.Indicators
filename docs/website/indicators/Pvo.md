# Percentage Volume Oscillator (PVO)

 The Percentage Volume Oscillator is a simple oscillator view of the rate of change between two converging / diverging exponential moving averages of Volume.  It is presented similarly to MACD.



The [Percentage Volume Oscillator](https://school.stockcharts.com/doku.php?id=technical_indicators:percentage_volume_oscillator_pvo) is a simple oscillator view of the rate of change between two converging / diverging exponential moving averages of Volume.
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/305 "Community discussion about this indicator")

![chart for Percentage Volume Oscillator (PVO)]()

```csharp
// C# usage syntax
IReadOnlyList<PvoResult> results =
  quotes.GetPvo(fastPeriods, slowPeriods, signalPeriods);
```

## Parameters

**`fastPeriods`** _`int`_ - Number of periods (`F`) for the faster moving average.  Must be greater than 0.  Default is 12.

**`slowPeriods`** _`int`_ - Number of periods (`S`) for the slower moving average.  Must be greater than `fastPeriods`.  Default is 26.

**`signalPeriods`** _`int`_ - Number of periods (`P`) for the moving average of PVO.  Must be greater than or equal to 0.  Default is 9.

### Historical quotes requirements

You must have at least `2×(S+P)` or `S+P+100` worth of `quotes`, whichever is more, to cover the [warmup and convergence](https://github.com/DaveSkender/Stock.Indicators/discussions/688) periods.  Since this uses a smoothing technique, we recommend you use at least `S+P+250` data points prior to the intended usage date for better precision.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](../guide.md#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<PvoResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `S-1` slow periods will have `null` values since there's not enough data to calculate.

>&#9886; **Convergence warning**: The first `S+P+250` periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods.

### PvoResult

**`Timestamp`** _`DateTime`_ - date from evaluated `TQuote`

**`Pvo`** _`double`_ - Normalized difference between two Volume moving averages

**`Signal`** _`double`_ - Moving average of the `Pvo` line

**`Histogram`** _`double`_ - Gap between of the `Pvo` and `Signal` line

### Utilities

- [.Condense()](../utilities.md#condense)
- [.Find(lookupDate)](../utilities.md#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()](../utilities.md#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)](../utilities.md#remove-warmup-periods)

See [Utilities and helpers](../utilities.md#utilities-for-indicator-results) for more information.

## Chaining

Results can be further processed on `Pvo` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .GetPvo(..)
    .GetSlope(..);
```

This indicator must be generated from `quotes` and **cannot** be generated from results of another chain-enabled indicator or method.
