---
title: Standard Deviation Channels
description: Standard Deviation Channels are price ranges based on an linear regression centerline and standard deviations band widths.
---

# Standard Deviation Channels

Standard Deviation Channels are prices ranges based on an linear regression centerline and standard deviations band widths.
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/368 "Community discussion about this indicator")

<ClientOnly>
  <IndicatorChart src="/data/StdDevChannels.json" :height="360" />
</ClientOnly>

```csharp
// C# usage syntax
IReadOnlyList<StdDevChannelsResult> results =
  quotes.ToStdDevChannels(lookbackPeriods, stdDeviations);
```

## Parameters

| param | type | description |
| ----- | ---- | ----------- |
| `lookbackPeriods` | int | Size (`N`) of the evaluation window.  Must be `null` or greater than 1 to calculate.  A `null` value will produce a full `quotes` evaluation window ([see below](#alternative-depiction-for-full-quotes-variant)).  Default is 20. |
| `stdDeviations` | double | Width of bands.  Standard deviations (`D`) from the regression line.  Must be greater than 0.  Default is 2. |

### Historical quotes requirements

You must have at least `N` periods of `quotes` to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](/guide#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<StdDevChannelsResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- Up to `N-1` periods will have `null` values since there's not enough data to calculate.

::: warning 🖌️ Repaint warning
Historical results are a function of the current period window position and will fluctuate over time.  Recommended for visualization; not recommended for backtesting.
:::

### `StdDevChannelsResult`

| property | type | description |
| -------- | ---- | ----------- |
| `Timestamp` | DateTime | Date from evaluated `TQuote` |
| `Centerline` | double | Linear regression line (center line) |
| `UpperChannel` | double | Upper line is `D` standard deviations above the center line |
| `LowerChannel` | double | Lower line is `D` standard deviations below the center line |
| `BreakPoint` | bool | Helper information.  Indicates first point in new window. |

### Utilities

- [.Condense()](/utilities/results#condense)
- [.Find(lookupDate)](/utilities/results#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()](/utilities/results#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)](/utilities/results#remove-warmup-periods)

See [Utilities and helpers](/utilities/results) for more information.

## Alternative depiction for full quotes variant

If you specify `null` for the `lookbackPeriods`, you will get a regression line over the entire provided `quotes`.

<ClientOnly>
  <IndicatorChart src="/data/StdDevChannels.json" :height="360" />
</ClientOnly>

## Chaining

This indicator may be generated from any chain-enabled indicator or method.

```csharp
// example
var results = quotesEval
    .Use(CandlePart.HL2)
    .ToStdDevChannels(..);
```

Results **cannot** be further chained with additional transforms.

## Streaming and real-time usage

**⚠️ Streaming not supported**: Due to the reverse-window algorithm that recalculates the entire dataset on each new data point, Standard Deviation Channels is only available as a batch Series implementation. The computational cost grows quadratically (O(n²)) as the dataset size increases, making it impractical for incremental streaming (StreamHub) or buffer (BufferList) scenarios.

**Recommendation**: Use the Series implementation (`ToStdDevChannels()`) with periodic batch recalculation. For real-time scenarios, consider recalculating at appropriate intervals (e.g., end of period, every N bars) rather than on every tick.
