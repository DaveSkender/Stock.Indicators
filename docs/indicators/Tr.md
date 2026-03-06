---
title: True Range (TR)
description: Created by J. Welles Wilder, True Range is a measure of volatility that captures gaps and limits between periods.  It is the foundation for Average True Range (ATR).
---

# True Range (TR)

Created by J. Welles Wilder, [True Range](https://en.wikipedia.org/wiki/Average_true_range) is a measure of volatility that captures gaps and limits between periods.  It is the building block for [Average True Range](/indicators/Atr).
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/269 "Community discussion about this indicator")

```csharp
// C# usage syntax
IReadOnlyList<TrResult> results =
  quotes.ToTr();
```

## Historical quotes requirements

You must have at least 2 periods of `quotes` to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](/guide/getting-started#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<TrResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first period will have a `null` value since there is no prior period close.

### `TrResult`

| property | type | description |
| -------- | ---- | ----------- |
| `Timestamp` | DateTime | Date from evaluated `TQuote` |
| `Tr` | double | True Range |

### Utilities

- [.Condense()](/utilities/results/condense)
- [.Find(lookupDate)](/utilities/results/find-by-date)
- [.RemoveWarmupPeriods()](/utilities/results/remove-warmup-periods)

See [Utilities and helpers](/utilities/results/) for more information.

## Chaining

Results can be further processed on `Tr` with additional chain-enabled indicators.

```csharp
// example: ATR using a custom moving average
var results = quotes
    .ToTr()
    .ToSmma(lookbackPeriods);
```

This indicator must be generated from `quotes` and **cannot** be generated from results of another chain-enabled indicator or method.

See [Chaining indicators](/guide/batch#chaining-indicators) for more.

## Streaming

Use the buffer-style `List<T>` when you need incremental calculations without a hub:

```csharp
TrList trList = new();

foreach (IQuote quote in quotes)  // simulating stream
{
  trList.Add(quote);
}

// based on `ICollection<TrResult>`
IReadOnlyList<TrResult> results = trList;
```

Subscribe to a `QuoteHub` for advanced streaming scenarios:

```csharp
QuoteHub quoteHub = new();
TrHub observer = quoteHub.ToTrHub();

foreach (IQuote quote in quotes)  // simulating stream
{
  quoteHub.Add(quote);
}

IReadOnlyList<TrResult> results = observer.Results;
```

See [Buffer lists](/guide/buffer) and [Stream hubs](/guide/stream) for full usage guides.
