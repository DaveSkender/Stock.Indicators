---
title: Heikin-Ashi
description: Created by Munehisa Homma, [Heikin-Ashi](https://en.wikipedia.org/wiki/Candlestick_chart#Heikin-Ashi_candlesticks) is a modified candlestick pattern that transforms prices based on prior period prices for smoothing.
---

# Heikin-Ashi

Created by Munehisa Homma, [Heikin-Ashi](https://en.wikipedia.org/wiki/Candlestick_chart#Heikin-Ashi_candlesticks) is a modified candlestick pattern that transforms prices based on prior period prices for smoothing.
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/254 "Community discussion about this indicator")

```csharp
// C# usage syntax
IReadOnlyList<HeikinAshiResult> results =
  bars.ToHeikinAshi();
```

## Historical bars requirements

You must have at least two periods of `bars` to cover the warmup periods; however, more is typically provided since this is a chartable candlestick pattern.

`bars` is a collection of generic `TBar` historical price bars.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](/guide/getting-started#historical-bars) for more information.

## Response

```csharp
IReadOnlyList<HeikinAshiResult>
```

- This method returns a time series of all available indicator values for the `bars` provided.
- It always returns the same number of elements as there are in the historical bars.
- It does not return a single incremental indicator value.
- `HeikinAshiResult` is based on `IBar`, so it can be used as a direct replacement for `bars`.

### `HeikinAshiResult`

| property | type | description |
| -------- | ---- | ----------- |
| `Timestamp` | DateTime | Date from evaluated `TBar` |
| `Open` | decimal | Modified open price |
| `High` | decimal | Modified high price |
| `Low` | decimal | Modified low price |
| `Close` | decimal | Modified close price |
| `Volume` | decimal | Volume (same as `bars`) |

### Utilities

- [.Find(lookupDate)](/utilities/results/find-by-date)
- [.RemoveWarmupPeriods(removePeriods)](/utilities/results/remove-warmup-periods)
- .ToBars() to convert to a `Bar` collection.  Example:

  ```csharp
  IReadOnlyList<Bar> results = bars
    .ToHeikinAshi()
    .ToBars();
  ```

See [Utilities and helpers](/utilities/results/) for more information.

## Chaining

Results are based in `IBar` and can be further used in any indicator.

```csharp
// example
var results = bars
    .ToHeikinAshi(..)
    .ToRsi(..);
```

This indicator must be generated from `bars` and **cannot** be generated from results of another chain-enabled indicator or method.

See [Chaining indicators](/guide/chaining) for more.

## Streaming

Use the buffer-style `List<T>` when you need incremental calculations without a hub:

```csharp
HeikinAshiList heikinAshiList = new();

foreach (IBar bar in bars)  // simulating stream
{
  heikinAshiList.Add(bar);
}

// based on `ICollection<HeikinAshiResult>`
IReadOnlyList<HeikinAshiResult> results = heikinAshiList;
```

Subscribe to a `BarHub` for advanced streaming scenarios:

```csharp
BarHub barHub = new();
HeikinAshiHub observer = barHub.ToHeikinAshiHub();

foreach (IBar bar in bars)  // simulating stream
{
  barHub.Add(bar);
}

IReadOnlyList<HeikinAshiResult> results = observer.Results;
```

See [Buffer lists](/guide/styles/buffer) and [Stream hubs](/guide/styles/stream) for full usage guides.
