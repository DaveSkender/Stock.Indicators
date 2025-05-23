# Volume Weighted Moving Average (VWMA)

 Volume Weighted Moving Average is the volume adjusted average price over a lookback window.



Volume Weighted Moving Average is the volume adjusted average price over a lookback window.
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/657 "Community discussion about this indicator")

![chart for Vwma](../assets/charts/Vwma.png)

```csharp
// C# usage syntax
IReadOnlyList<VwmaResult> results =
  quotes.GetVwma(lookbackPeriods);
```

## Parameters

**`lookbackPeriods`** _`int`_ - Number of periods (`N`) in the moving average.  Must be greater than 0.

### Historical quotes requirements

You must have at least `N` periods of `quotes` to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](../guide.md#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<VwmaResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N-1` periods will have `null` values for `Vwma` since there's not enough data to calculate.

### VwmaResult

**`Timestamp`** _`DateTime`_ - date from evaluated `TQuote`

**`Vwma`** _`double`_ - Volume Weighted Moving Average

### Utilities

- [.Condense()](../utilities.md#sort-quotes)
- [.Find(lookupDate)](../utilities.md#find-indicator-result)
- [.RemoveWarmupPeriods()](../utilities.md#get-or-exclude-nulls)
- [.RemoveWarmupPeriods(qty)](../utilities.md#get-or-exclude-nulls)

See [Utilities and helpers](../utilities.md#utilities-for-indicator-results) for more information.

## Chaining

Results can be further processed on `Vwma` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .GetVwma(..)
    .GetRsi(..);
```

This indicator must be generated from `quotes` and **cannot** be generated from results of another chain-enabled indicator or method.
