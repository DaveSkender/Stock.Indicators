---
title: Bar parts and basic transforms
description: Basic bar transforms (e.g. HL2, OHL3, etc.) and isolation of individual price bar candle parts from a full OHLCV bar.
redirect-from:
  - /indicators/Use/
  - /indicators/BasicBar/
---

# Bar parts and basic transforms

Returns a reusable (chainable) basic bar transform (e.g. HL2, OHL3, etc.) by isolating a single component part value or calculated value from the full OHLCV bar candle parts.

```csharp
// C# usage syntax
IReadOnlyList<TimeValue> results =
  bars.Use(candlePart);

// alternate syntax
IReadOnlyList<TimeValue> results =
  bars.ToBarPart(candlePart);
```

## Parameters

| param | type | description |
| ----- | ---- | ----------- |
| `candlePart` | CandlePart | The [OHLCV](/guide/getting-started#historical-bars) element or simple price transform. |

### Historical price bars requirements

You must have at least 1 period of `bars`.

`bars` is a collection of generic `TBar` historical price bars.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](/guide/getting-started#historical-bars) for more information.

## Response

```csharp
IReadOnlyList<TimeValue>
```

- This method returns a time series of all available indicator values for the `bars` provided.
- It always returns the same number of elements as there are in the historical price bars.
- It does not return a single incremental indicator value.

### `TimeValue` type

| property | type | description |
| -------- | ---- | ----------- |
| `Timestamp` | DateTime | Date from evaluated `TBar` |
| `Value` | double | Price of `CandlePart` option |

### Utilities

- [.Find(lookupDate)](/utilities/results/find-by-date)

See [Utilities and helpers](/utilities/results/) for more information.

## Chaining

Results can be further processed on `Value` with additional chain-enabled indicators.

```csharp
// example
var results = bars
    .Use(CandlePart.OHLC4)
    .ToRsi(..);
```

This indicator must be generated from `bars` and **cannot** be generated from results of another chain-enabled indicator or method.

See [Chaining indicators](/guide/chaining) for more.

## Streaming

Subscribe to a `BarHub` for streaming scenarios:

```csharp
BarHub barHub = new();
BarPartHub observer = barHub.ToBarPartHub(CandlePart.HL2);

foreach (IBar bar in bars)  // simulating stream
{
  barHub.Add(bar);
}

IReadOnlyList<TimeValue> results = observer.Results;
```

See [Buffer lists](/guide/styles/buffer) and [Stream hubs](/guide/styles/stream) for full usage guides.

## Buffering

Use a `BarPartList` for incremental buffering scenarios:

```csharp
BarPartList buffer = new(CandlePart.Close);

foreach (IBar bar in bars)  // simulating stream
{
  buffer.Add(bar);
}

IReadOnlyList<TimeValue> results = buffer;
```
