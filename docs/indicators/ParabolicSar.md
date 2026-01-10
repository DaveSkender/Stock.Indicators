---
title: Parabolic SAR
description: Created by J. Welles Wilder, Parabolic SAR (stop and reverse) is a price-time based indicator used to determine trend direction and reversals.  It can be used to identify trend direction, reversals, and stop-loss signals.
---

# Parabolic SAR

Created by J. Welles Wilder, [Parabolic SAR](https://en.wikipedia.org/wiki/Parabolic_SAR) (stop and reverse) is a price-time based indicator used to determine trend direction and reversals.
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/245 "Community discussion about this indicator")

<ClientOnly>
  <IndicatorChart src="/data/ParabolicSar.json" :height="360" />
</ClientOnly>

```csharp
// C# usage syntax (standard)
IReadOnlyList<ParabolicSarResult> results =
  quotes.ToParabolicSar(accelerationStep, maxAccelerationFactor);

// alternate usage with custom initial Factor
IReadOnlyList<ParabolicSarResult> results =
  quotes.ToParabolicSar(accelerationStep, maxAccelerationFactor, initialFactor);
```

## Parameters

| param | type | description |
| ----- | ---- | ----------- |
| `accelerationStep` | double | Incremental step size for the Acceleration Factor.  Must be greater than 0.  Default is 0.02 |
| `maxAccelerationFactor` | double | Maximum factor limit.  Must be greater than `accelerationStep`.  Default is 0.2 |
| `initialFactor` | double | Optional.  Initial Acceleration Factor.  Must be greater than 0 and not larger than `maxAccelerationFactor`.  Default is `accelerationStep`. |

### Historical quotes requirements

You must have at least two historical quotes to cover the warmup periods; however, we recommend at least 100 data points.  Initial Parabolic SAR values prior to the first reversal are not accurate and are excluded from the results.  Therefore, provide sufficient quotes to capture prior trend reversals, before your intended usage period.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](/guide#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<ParabolicSarResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first trend will have `null` values since it is not accurate and based on an initial guess.

### `ParabolicSarResult`

| property | type | description |
| -------- | ---- | ----------- |
| `Timestamp` | DateTime | Date from evaluated `TQuote` |
| `Sar` | double | Stop and Reverse value |
| `IsReversal` | bool | Indicates a trend reversal |

### Utilities

- [.Condense()](/utilities/results#condense)
- [.Find(lookupDate)](/utilities/results#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()](/utilities/results#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)](/utilities/results#remove-warmup-periods)

See [Utilities and helpers](/utilities/results) for more information.

## Chaining

Results can be further processed on `Sar` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .ToParabolicSar(..)
    .ToEma(..);
```

This indicator must be generated from `quotes` and **cannot** be generated from results of another chain-enabled indicator or method.

## Streaming

Use the buffer-style `List<T>` when you need incremental calculations without a hub:

```csharp
ParabolicSarList psarList = new(accelerationStep, maxAccelerationFactor);

foreach (IQuote quote in quotes)  // simulating stream
{
  psarList.Add(quote);
}

// based on `ICollection<ParabolicSarResult>`
IReadOnlyList<ParabolicSarResult> results = psarList;
```

Subscribe to a `QuoteHub` for advanced streaming scenarios:

```csharp
QuoteHub quoteHub = new();
ParabolicSarHub observer = quoteHub.ToParabolicSarHub(accelerationStep, maxAccelerationFactor);

foreach (IQuote quote in quotes)  // simulating stream
{
  quoteHub.Add(quote);
}

IReadOnlyList<ParabolicSarResult> results = observer.Results;
```
