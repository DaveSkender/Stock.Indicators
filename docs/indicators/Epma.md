---
title: Endpoint Moving Average (EPMA)
description: Endpoint Moving Average (EPMA), also known as Least Squares Moving Average (LSMA), plots the projected last point of a defined retrospective linear regression.
---

# Endpoint Moving Average (EPMA)

Endpoint Moving Average (EPMA), also known as Least Squares Moving Average (LSMA), plots the projected last point of a defined retrospective linear regression.
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/371 "Community discussion about this indicator")

<ClientOnly>
  <IndicatorChart src="/data/Epma.json" :height="360" />
</ClientOnly>

```csharp
// C# usage syntax
IReadOnlyList<EpmaResult> results =
  quotes.ToEpma(lookbackPeriods);
```

## Parameters

| param | type | description |
| ----- | ---- | ----------- |
| `lookbackPeriods` | int | Number of periods (`N`) in the moving average.  Must be greater than 0. |

### Historical quotes requirements

You must have at least `N` periods of `quotes` to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](/guide#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<EpmaResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N-1` periods will have `null` values since there's not enough data to calculate.

### `EpmaResult`

| property | type | description |
| -------- | ---- | ----------- |
| `Timestamp` | DateTime | Date from evaluated `TQuote` |
| `Epma` | double | Endpoint moving average |

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
    .ToEpma(..);
```

Results can be further processed on `Epma` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .ToEpma(..)
    .ToRsi(..);
```

## Streaming

Use the buffer-style `List<T>` when you need incremental calculations without a hub:

```csharp
EpmaList epmaList = new(lookbackPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  epmaList.Add(quote);
}

// based on `ICollection<EpmaResult>`
IReadOnlyList<EpmaResult> results = epmaList;
```

Subscribe to a `QuoteHub` for advanced streaming scenarios:

```csharp
QuoteHub quoteHub = new();
EpmaHub observer = quoteHub.ToEpmaHub(lookbackPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  quoteHub.Add(quote);
}

IReadOnlyList<EpmaResult> results = observer.Results;
```
