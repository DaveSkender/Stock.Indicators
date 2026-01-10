---
title: Chande Momentum Oscillator (CMO)
description: The Chande Momentum Oscillator is a momentum indicator depicting the weighted percent of higher prices in financial markets.
---

# Chande Momentum Oscillator (CMO)

Created by Tushar Chande, the [Chande Momentum Oscillator](https://www.investopedia.com/terms/c/chandemomentumoscillator.asp) is a weighted percent of higher prices over a lookback window.
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/892 "Community discussion about this indicator")

<ClientOnly>
  <IndicatorChart src="/data/Cmo.json" :height="360" />
</ClientOnly>

```csharp
// C# usage syntax
IReadOnlyList<CmoResult> results =
  quotes.ToCmo(lookbackPeriods);
```

## Parameters

| param | type | description |
| ----- | ---- | ----------- |
| `lookbackPeriods` | int | Number of periods (`N`) in the lookback window.  Must be greater than 0. |

### Historical quotes requirements

You must have at least `N+1` periods of `quotes` to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](/guide#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<CmoResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N` periods will have `null` values for CMO since there's not enough data to calculate.

### `CmoResult`

| property | type | description |
| -------- | ---- | ----------- |
| `Timestamp` | DateTime | Date from evaluated `TQuote` |
| `Cmo` | double | Chande Momentum Oscillator |

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
    .ToCmo(..);
```

Results can be further processed on `Cmo` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .ToCmo(..)
    .ToEma(..);
```

## Streaming

Use the buffer-style `List<T>` when you need incremental calculations without a hub:

```csharp
CmoList cmoList = new(lookbackPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  cmoList.Add(quote);
}

// based on `ICollection<CmoResult>`
IReadOnlyList<CmoResult> results = cmoList;
```

Subscribe to a `QuoteHub` for advanced streaming scenarios:

```csharp
QuoteHub quoteHub = new();
CmoHub observer = quoteHub.ToCmoHub(lookbackPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  quoteHub.Add(quote);
}

IReadOnlyList<CmoResult> results = observer.Results;
```

See the [guide](/guide) for more information.
