---
title: Money Flow Index (MFI)
description: Created by Quong and Soudack, the Money Flow Index is a price-volume oscillator that shows buying and selling momentum.
---

# Money Flow Index (MFI)

Created by Quong and Soudack, the [Money Flow Index](https://en.wikipedia.org/wiki/Money_flow_index) is a price-volume oscillator that shows buying and selling momentum.
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/247 "Community discussion about this indicator")

<ClientOnly>
  <StockIndicatorChart indicator="Mfi" withOverlay />
</ClientOnly>

```csharp
// C# usage syntax
IReadOnlyList<MfiResult> results =
  bars.ToMfi(lookbackPeriods);
```

## Parameters

| param | type | description |
| ----- | ---- | ----------- |
| `lookbackPeriods` | int | Number of periods (`N`) in the lookback period.  Must be greater than 1. Default is 14. |

### Historical bars requirements

You must have at least `N+1` historical bars to cover the warmup periods.

`bars` is a collection of generic `TBar` historical price bars.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](/guide/getting-started#historical-bars) for more information.

## Response

```csharp
IReadOnlyList<MfiResult>
```

- This method returns a time series of all available indicator values for the `bars` provided.
- It always returns the same number of elements as there are in the historical bars.
- It does not return a single incremental indicator value.
- The first `N` periods will have `null` MFI values since they cannot be calculated.

### `MfiResult`

| property | type | description |
| -------- | ---- | ----------- |
| `Timestamp` | DateTime | Date from evaluated `TBar` |
| `Mfi` | double | Money Flow Index |

### Utilities

- [.Condense()](/utilities/results/condense)
- [.Find(lookupDate)](/utilities/results/find-by-date)
- [.RemoveWarmupPeriods()](/utilities/results/remove-warmup-periods)
- [.RemoveWarmupPeriods(removePeriods)](/utilities/results/remove-warmup-periods)

See [Utilities and helpers](/utilities/results/) for more information.

## Streaming

Use the buffer-style `List<T>` when you need incremental calculations without a hub:

```csharp
MfiList mfiList = new(lookbackPeriods);

foreach (IBar bar in bars)  // simulating stream
{
  mfiList.Add(bar);
}

// based on `ICollection<MfiResult>`
IReadOnlyList<MfiResult> results = mfiList;
```

Subscribe to a `BarHub` for advanced streaming scenarios:

```csharp
BarHub barHub = new();
MfiHub observer = barHub.ToMfiHub(lookbackPeriods);

foreach (IBar bar in bars)  // simulating stream
{
  barHub.Add(bar);
}

IReadOnlyList<MfiResult> results = observer.Results;
```

See [Buffer lists](/guide/styles/buffer) and [Stream hubs](/guide/styles/stream) for full usage guides.

## Chaining

Results can be further processed on `Mfi` with additional chain-enabled indicators.

```csharp
// example
var results = bars
    .ToMfi(..)
    .ToRsi(..);
```

This indicator must be generated from `bars` and **cannot** be generated from results of another chain-enabled indicator or method.

See [Chaining indicators](/guide/chaining) for more.
