---
title: ConnorsRSI
description: Created by Laurence Connors, the ConnorsRSI is a composite oscillator that incorporates RSI, winning/losing streaks, and percentile gain metrics on scale of 0 to 100.
---

# ConnorsRSI

Created by Laurence Connors, the [ConnorsRSI](https://alvarezquanttrading.com/wp-content/uploads/2016/05/ConnorsRSIGuidebook.pdf) is a composite oscillator that incorporates RSI, winning/losing streaks, and percentile gain metrics on scale of 0 to 100.  See [analysis](https://alvarezquanttrading.com/blog/connorsrsi-analysis).
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/260 "Community discussion about this indicator")

<ClientOnly>
  <IndicatorChart src="/data/ConnorsRsi.json" :height="360" />
</ClientOnly>

```csharp
// C# usage syntax
IReadOnlyList<ConnorsRsiResult> results =
  quotes.ToConnorsRsi(rsiPeriods, streakPeriods, rankPeriods);
```

## Parameters

| param | type | description |
| ----- | ---- | ----------- |
| `rsiPeriods` | int | Lookback period (`R`) for the price RSI.  Must be greater than 1.  Default is 3. |
| `streakPeriods` | int | Lookback period (`S`) for the streak RSI.  Must be greater than 1.  Default is 2. |
| `rankPeriods` | int | Lookback period (`P`) for the Percentile Rank.  Must be greater than 1.  Default is 100. |

### Historical quotes requirements

`N` is the greater of `R+100`, `S`, and `P+2`.  You must have at least `N` periods of `quotes` to cover the [warmup and convergence](https://github.com/DaveSkender/Stock.Indicators/discussions/688) periods.  Since this uses a smoothing technique, we recommend you use at least `N+150` data points prior to the intended usage date for better precision.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](/guide#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<ConnorsRsiResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `MAX(R,S,P)-1` periods will have `null` values since there's not enough data to calculate.

::: warning ⚞ Convergence warning
The first `N` periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods.
:::

### `ConnorsRsiResult`

| property | type | description |
| -------- | ---- | ----------- |
| `Timestamp` | DateTime | Date from evaluated `TQuote` |
| `Rsi` | double | `RSI(R)` of the price. |
| `RsiStreak` | double | `RSI(S)` of the Streak. |
| `PercentRank` | double | Percentile rank of the period gain value. |
| `ConnorsRsi` | double | ConnorsRSI |

### Utilities

- [.Condense()](/utilities/results#condense)
- [.Find(lookupDate)](/utilities/results#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()](/utilities/results#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)](/utilities/results#remove-warmup-periods)

See [Utilities and helpers](/utilities/results) for more information.

## Chaining

This indicator may be generated from any chain-enabled indicator or method.

```csharp
// example
var results = quotes
    .Use(CandlePart.HL2)
    .ToConnorsRsi(..);
```

Results can be further processed on `ConnorsRsi` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .ToConnorsRsi(..)
    .ToSma(..);
```

## Streaming

Use the buffer-style `List<T>` when you need incremental calculations without a hub:

```csharp
ConnorsRsiList connorsRsiList = new(rsiPeriods, streakPeriods, rankPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  connorsRsiList.Add(quote);
}

// based on `ICollection<ConnorsRsiResult>`
IReadOnlyList<ConnorsRsiResult> results = connorsRsiList;
```

Subscribe to a `QuoteHub` for advanced streaming scenarios:

```csharp
QuoteHub quoteHub = new();
ConnorsRsiHub observer = quoteHub.ToConnorsRsiHub(rsiPeriods, streakPeriods, rankPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  quoteHub.Add(quote);
}

IReadOnlyList<ConnorsRsiResult> results = observer.Results;
```
