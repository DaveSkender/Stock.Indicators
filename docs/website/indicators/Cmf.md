# Chaikin Money Flow (CMF)

 Created by Marc Chaikin, Chaikin Money Flow is the simple moving average of the directional Money Flow Volume.



Created by Marc Chaikin, [Chaikin Money Flow](https://en.wikipedia.org/wiki/Chaikin_Analytics#Chaikin_Money_Flow) is the simple moving average of the directional Money Flow Volume.
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/261 "Community discussion about this indicator")

![chart for Chaikin Money Flow (CMF)]()

```csharp
// C# usage syntax
IEnumerable<CmfResult> results =
  quotes.GetCmf(lookbackPeriods);
```

## Parameters

**`lookbackPeriods`** _`int`_ - Number of periods (`N`) in the moving average.  Must be greater than 0.  Default is 20.

### Historical quotes requirements

You must have at least `N+1` periods of `quotes` to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](../guide.md#historical-quotes) for more information.

## Response

```csharp
IEnumerable<CmfResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N-1` periods will have `null` values since there's not enough data to calculate.

### CmfResult

**`Date`** _`DateTime`_ - Date from evaluated `TQuote`

**`MoneyFlowMultiplier`** _`double`_ - Money Flow Multiplier

**`MoneyFlowVolume`** _`double`_ - Money Flow Volume

**`Cmf`** _`double`_ - Chaikin Money Flow = SMA of MFV

> &#128681; **Warning**: absolute values in MFV and CMF are somewhat meaningless.  Use with caution.

### Utilities

- [.Condense()](../utilities.md#condense)
- [.Find(lookupDate)](../utilities.md#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()](../utilities.md#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)](../utilities.md#remove-warmup-periods)

See [Utilities and helpers](../utilities.md#utilities-for-indicator-results) for more information.

## Chaining

Results can be further processed on `Cmf` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .GetCmf(..)
    .GetSlope(..);
```

This indicator must be generated from `quotes` and **cannot** be generated from results of another chain-enabled indicator or method.
