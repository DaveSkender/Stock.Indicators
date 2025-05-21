# Endpoint Moving Average (EPMA)

 Endpoint Moving Average (EPMA), also known as Least Squares Moving Average (LSMA), plots the projected last point of a defined retrospective linear regression.



Endpoint Moving Average (EPMA), also known as Least Squares Moving Average (LSMA), plots the projected last point of a defined retrospective linear regression.
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/371 "Community discussion about this indicator")

![chart for Endpoint Moving Average (EPMA)]()

```csharp
// C# usage syntax
IReadOnlyList<EpmaResult> results =
  quotes.GetEpma(lookbackPeriods);
```

## Parameters

**`lookbackPeriods`** _`int`_ - Number of periods (`N`) in the moving average.  Must be greater than 0.

### Historical quotes requirements

You must have at least `N` periods of `quotes` to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](../guide.md#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<EpmaResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N-1` periods will have `null` values since there's not enough data to calculate.

### EpmaResult

**`Timestamp`** _`DateTime`_ - date from evaluated `TQuote`

**`Epma`** _`double`_ - Endpoint moving average

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
    .GetEpma(..);
```

Results can be further processed on `Epma` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .GetEpma(..)
    .GetRsi(..);
```
