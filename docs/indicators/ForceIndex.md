---
title: Force Index
description: Created by Alexander Elder, the Force Index depicts volume-based buying and selling pressure based on the change in price.
---

# Force Index

Created by Alexander Elder, the [Force Index](https://en.wikipedia.org/wiki/Force_index) depicts volume-based buying and selling pressure based on the change in price.
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/382 "Community discussion about this indicator")

<ClientOnly>
  <IndicatorChart src="/data/ForceIndex.json" :height="360" />
</ClientOnly>

```csharp
// C# usage syntax
IReadOnlyList<ForceIndexResult> results =
  quotes.ToForceIndex(lookbackPeriods);
```

## Parameters

| param | type | description |
| ----- | ---- | ----------- |
| `lookbackPeriods` | int | Lookback window (`N`) for the EMA of Force Index.  Must be greater than 0 and is commonly 2 or 13 (shorter/longer view).  Default is 2. |

### Historical quotes requirements

You must have at least `N+100` for `2×N` periods of `quotes`, whichever is more, to cover the [warmup and convergence](https://github.com/DaveSkender/Stock.Indicators/discussions/688) periods.  Since this uses a smoothing technique for EMA, we recommend you use at least `N+250` data points prior to the intended usage date for better precision.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](/guide#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<ForceIndexResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N` periods will be `null` since they cannot be calculated.

::: warning ⚞ Convergence warning
The first `N+100` periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods.
:::

### `ForceIndexResult`

| property | type | description |
| -------- | ---- | ----------- |
| `Timestamp` | DateTime | Date from evaluated `TQuote` |
| `ForceIndex` | double | Force Index |

### Utilities

- [.Condense()](/utilities/results#condense)
- [.Find(lookupDate)](/utilities/results#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()](/utilities/results#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)](/utilities/results#remove-warmup-periods)

See [Utilities and helpers](/utilities/results) for more information.

## Chaining

Results can be further processed on `ForceIndex` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .ToForceIndex(..)
    .ToEma(..);
```

This indicator must be generated from `quotes` and **cannot** be generated from results of another chain-enabled indicator or method.

## Streaming

Use the buffer-style `List<T>` when you need incremental calculations without a hub:

```csharp
ForceIndexList forceIndexList = new(lookbackPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  forceIndexList.Add(quote);
}

// based on `ICollection<ForceIndexResult>`
IReadOnlyList<ForceIndexResult> results = forceIndexList;
```

Subscribe to a `QuoteHub` for advanced streaming scenarios:

```csharp
QuoteHub quoteHub = new();
ForceIndexHub observer = quoteHub.ToForceIndexHub(lookbackPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  quoteHub.Add(quote);
}

IReadOnlyList<ForceIndexResult> results = observer.Results;
```
