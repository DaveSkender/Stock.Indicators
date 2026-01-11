---
title: Ultimate Oscillator
description: Created by Larry Williams, the Ultimate Oscillator uses several moving averages to weigh buying power against true range price to produce an oversold / overbought oscillator.
---

# Ultimate Oscillator

Created by Larry Williams, the [Ultimate Oscillator](https://en.wikipedia.org/wiki/Ultimate_oscillator) uses several moving averages to weigh buying power against true range price to produce an oversold / overbought oscillator.
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/231 "Community discussion about this indicator")

<ClientOnly>
  <IndicatorChart src="/data/Ultimate.json" :height="360" />
</ClientOnly>

```csharp
// C# usage syntax
IReadOnlyList<UltimateResult> results =
  quotes.ToUltimate(shortPeriods, middlePeriods, longPeriods);
```

## Parameters

| param | type | description |
| ----- | ---- | ----------- |
| `shortPeriods` | int | Number of periods (`S`) in the short lookback.  Must be greater than 0.  Default is 7. |
| `middlePeriods` | int | Number of periods (`M`) in the middle lookback.  Must be greater than `S`.  Default is 14. |
| `longPeriods` | int | Number of periods (`L`) in the long lookback.  Must be greater than `M`.  Default is 28. |

### Historical quotes requirements

You must have at least `L+1` periods of `quotes` to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](/guide#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<UltimateResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `L-1` periods will have `null` Ultimate values since there's not enough data to calculate.

### `UltimateResult`

| property | type | description |
| -------- | ---- | ----------- |
| `Timestamp` | DateTime | Date from evaluated `TQuote` |
| `Ultimate` | double | Ultimate Oscillator |

### Utilities

- [.Condense()](/utilities/results#condense)
- [.Find(lookupDate)](/utilities/results#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()](/utilities/results#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)](/utilities/results#remove-warmup-periods)

See [Utilities and helpers](/utilities/results) for more information.

## Chaining

Results can be further processed on `Ultimate` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .ToUltimate(..)
    .ToSlope(..);
```

This indicator must be generated from `quotes` and **cannot** be generated from results of another chain-enabled indicator or method.

## Streaming

Use the buffer-style `List<T>` when you need incremental calculations without a hub:

```csharp
UltimateList ultimateList = new(shortPeriods, middlePeriods, longPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  ultimateList.Add(quote);
}

// based on `ICollection<UltimateResult>`
IReadOnlyList<UltimateResult> results = ultimateList;
```

Subscribe to a `QuoteHub` for advanced streaming scenarios:

```csharp
QuoteHub quoteHub = new();
UltimateHub observer = quoteHub.ToUltimateHub(shortPeriods, middlePeriods, longPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  quoteHub.Add(quote);
}

IReadOnlyList<UltimateResult> results = observer.Results;
```
