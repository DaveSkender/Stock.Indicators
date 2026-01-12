---
title: Vortex Indicator (VI)
description: Created by Etienne Botes and Douglas Siepman, the Vortex Indicator is a measure of price directional movement.  It includes positive and negative indicators, and is often used to identify trends and reversals.
---

# Vortex Indicator (VI)

Created by Etienne Botes and Douglas Siepman, the [Vortex Indicator](https://en.wikipedia.org/wiki/Vortex_indicator) is a measure of price directional movement.  It includes positive and negative indicators, and is often used to identify trends and reversals.
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/339 "Community discussion about this indicator")

<ClientOnly>
  <IndicatorChart src="/data/Vortex.json" :height="360" />
</ClientOnly>

```csharp
// C# usage syntax
IReadOnlyList<VortexResult> results =
  quotes.ToVortex(lookbackPeriods);
```

## Parameters

| param | type | description |
| ----- | ---- | ----------- |
| `lookbackPeriods` | int | Number of periods (`N`) to consider.  Must be greater than 1 and is usually between 14 and 30. |

### Historical quotes requirements

You must have at least `N+1` periods of `quotes` to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](/guide#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<VortexResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N` periods will have `null` values for VI since there's not enough data to calculate.

### `VortexResult`

| property | type | description |
| -------- | ---- | ----------- |
| `Timestamp` | DateTime | Date from evaluated `TQuote` |
| `Pvi` | double | Positive Vortex Indicator (VI+) |
| `Nvi` | double | Negative Vortex Indicator (VI-) |

### Utilities

- [.Condense()](/utilities/results#condense)
- [.Find(lookupDate)](/utilities/results#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()](/utilities/results#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)](/utilities/results#remove-warmup-periods)

See [Utilities and helpers](/utilities/results) for more information.

## Chaining

This indicator is not chain-enabled and must be generated from `quotes`.  It **cannot** be used for further processing by other chain-enabled indicators.

## Streaming

Use the buffer-style `List<T>` when you need incremental calculations without a hub:

```csharp
VortexList vortexList = new(lookbackPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  vortexList.Add(quote);
}

// based on `ICollection<VortexResult>`
IReadOnlyList<VortexResult> results = vortexList;
```

Subscribe to a `QuoteHub` for advanced streaming scenarios:

```csharp
QuoteHub quoteHub = new();
VortexHub observer = quoteHub.ToVortexHub(lookbackPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  quoteHub.Add(quote);
}

IReadOnlyList<VortexResult> results = observer.Results;
```
