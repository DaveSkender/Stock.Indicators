---
title: Awesome Oscillator (AO)
description: Created by Bill Williams, the Awesome Oscillator (AO), also known as Super AO, is a measure of the gap between a fast and slow period modified moving average.
---

# Awesome Oscillator (AO)

Created by Bill Williams, the Awesome Oscillator (aka Super AO) is a measure of the gap between a fast and slow period modified moving average.
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/282 "Community discussion about this indicator")

<ClientOnly>
  <IndicatorChart src="/data/Awesome.json" :height="360" />
</ClientOnly>

```csharp
// C# usage syntax
IReadOnlyList<AwesomeResult> results =
  quotes.ToAwesome(fastPeriods, slowPeriods);
```

## Parameters

| param | type | description |
| ----- | ---- | ----------- |
| `fastPeriods` | int | Number of periods (`F`) for the faster moving average.  Must be greater than 0.  Default is 5. |
| `slowPeriods` | int | Number of periods (`S`) for the slower moving average.  Must be greater than `fastPeriods`.  Default is 34. |

### Historical quotes requirements

You must have at least `S` periods of `quotes` to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](/guide#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<AwesomeResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first period `S-1` periods will have `null` values since there's not enough data to calculate.

### `AwesomeResult`

| property | type | description |
| -------- | ---- | ----------- |
| `Timestamp` | DateTime | Date from evaluated `TQuote` |
| `Oscillator` | double | Awesome Oscillator |
| `Normalized` | double | `100 × Oscillator ÷ (median price)` |

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
    .ToAwesome(..);
```

Results can be further processed on `Oscillator` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .ToAwesome(..)
    .ToRsi(..);
```

## Streaming

Use the buffer-style `List<T>` when you need incremental calculations without a hub:

```csharp
AwesomeList awesomeList = new(fastPeriods, slowPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  awesomeList.Add(quote);
}

// based on `ICollection<AwesomeResult>`
IReadOnlyList<AwesomeResult> results = awesomeList;
```

Subscribe to a `QuoteHub` for advanced streaming scenarios:

```csharp
QuoteHub quoteHub = new();
AwesomeHub observer = quoteHub.ToAwesomeHub(fastPeriods, slowPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  quoteHub.Add(quote);
}

IReadOnlyList<AwesomeResult> results = observer.Results;
```
