# Ulcer Index (UI)

 Created by Peter Martin, the Ulcer Index is a measure of downside price volatility.  Often called the "heart attack" score, it measures the amount of pain seen from drawdowns in financial market prices and portfolio value.



Created by Peter Martin, the [Ulcer Index](https://en.wikipedia.org/wiki/Ulcer_index) is a measure of downside price volatility over a lookback window.  Often called the "heart attack" score, it measures the amount of pain seen from drawdowns in financial market prices and portfolio value.
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/232 "Community discussion about this indicator")

![chart for Ulcer Index (UI)]()

```csharp
// C# usage syntax
IReadOnlyList<UlcerIndexResult> results =
  quotes.GetUlcerIndex(lookbackPeriods);
```

## Parameters

**`lookbackPeriods`** _`int`_ - Number of periods (`N`) for review.  Must be greater than 0.  Default is 14.

### Historical quotes requirements

You must have at least `N` periods of `quotes` to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](../guide.md#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<UlcerIndexResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N-1` periods will have `null` values since there's not enough data to calculate.

### UlcerIndexResult

**`Timestamp`** _`DateTime`_ - date from evaluated `TQuote`

**`UI`** _`double`_ - Ulcer Index

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
    .GetAlma(..);
```

Results can be further processed on `UI` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .GetAlma(..)
    .GetRsi(..);
```
