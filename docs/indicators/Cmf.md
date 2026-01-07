---
title: Chaikin Money Flow (CMF)
description: Created by Marc Chaikin, Chaikin Money Flow is the simple moving average of the directional Money Flow Volume.
---

# Chaikin Money Flow (CMF)

Created by Marc Chaikin, [Chaikin Money Flow](https://en.wikipedia.org/wiki/Chaikin_Analytics#Chaikin_Money_Flow) is the simple moving average of the directional Money Flow Volume.
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/261 "Community discussion about this indicator")

<ClientOnly>
  <IndicatorChart src="/data/Cmf.json" :height="360" />
</ClientOnly>

```csharp
// C# usage syntax
IReadOnlyList<CmfResult> results =
  quotes.ToCmf(lookbackPeriods);
```

## Parameters

| param | type | description |
| ----- | ---- | ----------- |
| `lookbackPeriods` | int | Number of periods (`N`) in the moving average.  Must be greater than 0.  Default is 20. |

### Historical quotes requirements

You must have at least `N+1` periods of `quotes` to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](/guide#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<CmfResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N-1` periods will have `null` values since there's not enough data to calculate.

### `CmfResult`

| property | type | description |
| -------- | ---- | ----------- |
| `Timestamp` | DateTime | Date from evaluated `TQuote` |
| `MoneyFlowMultiplier` | double | Money Flow Multiplier |
| `MoneyFlowVolume` | double | Money Flow Volume |
| `Cmf` | double | Chaikin Money Flow = SMA of MFV |

::: warning
absolute values in MFV and CMF are somewhat meaningless.  Use with caution.
:::

### Utilities

- [.Condense()](/utilities/results#condense)
- [.Find(lookupDate)](/utilities/results#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()](/utilities/results#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)](/utilities/results#remove-warmup-periods)

See [Utilities and helpers](/utilities/results) for more information.

## Chaining

Results can be further processed on `Cmf` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .ToCmf(..)
    .ToSlope(..);
```

This indicator must be generated from `quotes` and **cannot** be generated from results of another chain-enabled indicator or method.

## Streaming

Use the buffer-style `List<T>` when you need incremental calculations without a hub:

```csharp
CmfList cmfList = new(lookbackPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  cmfList.Add(quote);
}

// based on `ICollection<CmfResult>`
IReadOnlyList<CmfResult> results = cmfList;
```

Subscribe to a `QuoteHub` for advanced streaming scenarios:

```csharp
QuoteHub quoteHub = new();
CmfHub observer = quoteHub.ToCmfHub(lookbackPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  quoteHub.Add(quote);
}

IReadOnlyList<CmfResult> results = observer.Results;
```
