# ROC with Bands

 Rate of Change with Bands, created by Vitali Apirine, is a volatility banded variant of the basic Rate of Change (ROC) indicator.



Rate of Change (ROC) with Bands, created by Vitali Apirine, is a volatility banded variant of [Rate of Change (ROC)](Roc.md#content).
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/242 "Community discussion about this indicator")

![chart for ROC with Bands]()

```csharp
// C# usage syntax
IEnumerable<RocWbResult> results =
  quotes.GetRocWb(lookbackPeriods, emaPeriods, stdDevPeriods);
```

## Parameters

**`lookbackPeriods`** _`int`_ - Number of periods (`N`) to go back.  Must be greater than 0.  Typical values range from 10-20.

**`emaPeriods`** _`int`_ - Number of periods for the ROC EMA line.  Must be greater than 0.  Standard is 3.

**`stdDevPeriods`** _`int`_ - Number of periods the standard deviation for upper/lower band lines.  Must be greater than 0 and not more than `lookbackPeriods`.  Standard is to use same value as `lookbackPeriods`.

### Historical quotes requirements

You must have at least `N+1` periods of `quotes` to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](../guide.md#historical-quotes) for more information.

## Response

```csharp
IEnumerable<RocWbResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N` periods will have `null` values for ROC since there's not enough data to calculate.

### RocWbResult

**`Date`** _`DateTime`_ - Date from evaluated `TQuote`

**`Roc`** _`double`_ - Rate of Change over `N` lookback periods (%, not decimal)

**`RocEma`** _`double`_ - Exponential moving average (EMA) of `Roc`

**`UpperBand`** _`double`_ - Upper band of ROC (overbought indicator)

**`LowerBand`** _`double`_ - Lower band of ROC (oversold indicator)

### Utilities

- [.Condense()](../utilities.md#condense)
- [.Find(lookupDate)](../utilities.md#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()](../utilities.md#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)](../utilities.md#remove-warmup-periods)

See [Utilities and helpers](../utilities.md#utilities-for-indicator-results) for more information.

## Chaining

This indicator may be generated from any chain-enabled indicator or method.

```csharp
// example
var results = quotes
    .Use(CandlePart.HL2)
    .GetRocWb(..);
```

Results can be further processed on `Roc` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .GetRocWb(..)
    .GetEma(..);
```
