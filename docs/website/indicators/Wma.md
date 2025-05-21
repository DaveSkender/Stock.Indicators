# Weighted Moving Average (WMA)

 Weighted Moving Average is the linear weighted average of financial market prices over a lookback window.  This also called Linear Weighted Moving Average (LWMA).



[Weighted Moving Average](https://en.wikipedia.org/wiki/Moving_average#Weighted_moving_average) is the linear weighted average of price over a lookback window.  This also called Linear Weighted Moving Average (LWMA).
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/227 "Community discussion about this indicator")

![chart for Weighted Moving Average (WMA)]()

```csharp
// C# usage syntax (with Close price)
IEnumerable<WmaResult> results =
  quotes.GetWma(lookbackPeriods);
```

## Parameters

**`lookbackPeriods`** _`int`_ - Number of periods (`N`) in the lookback window.  Must be greater than 0.

### Historical quotes requirements

You must have at least `N` periods of `quotes` to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](../guide.md#historical-quotes) for more information.

## Response

```csharp
IEnumerable<WmaResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N-1` periods will have `null` values since there's not enough data to calculate.

### WmaResult

**`Date`** _`DateTime`_ - Date from evaluated `TQuote`

**`Wma`** _`double`_ - Weighted moving average

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
    .GetWma(..);
```

Results can be further processed on `Wma` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .GetWma(..)
    .GetRsi(..);
```
