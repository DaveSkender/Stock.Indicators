---
title: Balance of Power (BOP)
description: Created by Igor Levshin, the [Balance of Power](https://school.stockcharts.com/doku.php?id=technical_indicators:balance_of_power) (aka Balance of Market Power) is a momentum oscillator that depicts the strength of buying and selling pressure.
---

# Balance of Power (BOP)

Created by Igor Levshin, the [Balance of Power](https://school.stockcharts.com/doku.php?id=technical_indicators:balance_of_power) (aka Balance of Market Power) is a momentum oscillator that depicts the strength of buying and selling pressure.
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/302 "Community discussion about this indicator")

<ClientOnly>
  <IndicatorChart src="/data/Bop.json" :height="360" />
</ClientOnly>

```csharp
// C# usage syntax
IReadOnlyList<BopResult> results =
  quotes.ToBop(smoothPeriods);
```

## Parameters

| param | type | description |
| ----- | ---- | ----------- |
| `smoothPeriods` | int | Number of periods (`N`) for smoothing.  Must be greater than 0.  Default is 14. |

### Historical quotes requirements

You must have at least `N` periods of `quotes` to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](/guide#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<BopResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N-1` periods will have `null` values since there's not enough data to calculate.

### `BopResult`

| property | type | description |
| -------- | ---- | ----------- |
| `Timestamp` | DateTime | Date from evaluated `TQuote` |
| `Bop` | double | Balance of Power |

### Utilities

- [.Condense()](/utilities/results#condense)
- [.Find(lookupDate)](/utilities/results#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()](/utilities/results#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)](/utilities/results#remove-warmup-periods)

See [Utilities and helpers](/utilities/results) for more information.

## Chaining

Results can be further processed on `Bop` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .ToBop(..)
    .ToSlope(..);
```

This indicator must be generated from `quotes` and **cannot** be generated from results of another chain-enabled indicator or method.

## Streaming

Use the buffer-style `List<T>` when you need incremental calculations without a hub:

```csharp
BopList bopList = new(smoothPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  bopList.Add(quote);
}

// based on `ICollection<BopResult>`
IReadOnlyList<BopResult> results = bopList;
```

Subscribe to a `QuoteHub` for advanced streaming scenarios:

```csharp
QuoteHub quoteHub = new();
BopHub observer = quoteHub.ToBopHub(smoothPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  quoteHub.Add(quote);
}

IReadOnlyList<BopResult> results = observer.Results;
```
