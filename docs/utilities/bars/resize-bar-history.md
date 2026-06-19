---
title: Resize bar history
description: Aggregate intraday bars into larger timeframes.
---

# Resize bar history

Aggregate intraday bars into larger timeframes using `bars.Aggregate()`. Convert minute bars to hourly, hourly to daily, or use any custom TimeSpan period.

## Syntax

```csharp
// using BarInterval enum
IReadOnlyList<Bar> aggregatedBars = bars.Aggregate(BarInterval newSize);

// using TimeSpan
IReadOnlyList<Bar> aggregatedBars = bars.Aggregate(TimeSpan timeSpan);
```

## Parameters

**newSize** - A `BarInterval` enum value representing the target period size. See [BarInterval options](#barinterval-options) below.

**timeSpan** - Any `TimeSpan` value greater than `TimeSpan.Zero` for custom aggregation periods.

## Returns

**IReadOnlyList\<Bar\>** - Aggregated bars at the specified period size.

## Usage

### Standard periods

```csharp
// aggregate 1-minute bars into 15-minute bars
IReadOnlyList<Bar> fifteenMinBars =
  minuteBarBars.Aggregate(BarInterval.FifteenMinutes);

// aggregate hourly bars into daily bars
IReadOnlyList<Bar> dayBarBars =
  hourlyBarBars.Aggregate(BarInterval.Day);
```

### Custom periods

```csharp
// aggregate into 2-hour bars using TimeSpan
IReadOnlyList<Bar> twoHourBars =
  minuteBarBars.Aggregate(TimeSpan.FromHours(2));

// aggregate into 45-minute bars
IReadOnlyList<Bar> customBars =
  minuteBarBars.Aggregate(TimeSpan.FromMinutes(45));
```

## BarInterval options

- `BarInterval.Month`
- `BarInterval.Week`
- `BarInterval.Day`
- `BarInterval.FourHours`
- `BarInterval.TwoHours`
- `BarInterval.OneHour`
- `BarInterval.ThirtyMinutes`
- `BarInterval.FifteenMinutes`
- `BarInterval.FiveMinutes`
- `BarInterval.ThreeMinutes`
- `BarInterval.TwoMinutes`
- `BarInterval.OneMinute`

## How aggregation works

When aggregating bars, the utility:

- **Open** - Uses the first bar's open price in the period
- **High** - Uses the highest high price in the period
- **Low** - Uses the lowest low price in the period
- **Close** - Uses the last bar's close price in the period
- **Volume** - Sums all volume in the period
- **Timestamp** - Uses the period's starting timestamp

## Important considerations

::: warning 🚩 Partially populated periods
Partially populated period windows at the beginning, end, and market open/close points in `bars` can be misleading when aggregated.

**Example**: If you are aggregating intraday minute bars into 15-minute bars and there is a single 4:00pm minute bar at the end, the resulting 4:00pm 15-minute bar will only have one minute of data in it whereas the previous 3:45pm bar will have all 15 minutes of bars aggregated (3:45-3:59pm).
:::

::: tip ✨ Best practices

- Filter out partial periods at market open/close if they could skew your analysis
- Be aware of time zone handling when aggregating across day boundaries
- Consider using only complete periods for backtesting
:::

## Common use cases

### Multi-timeframe analysis

Analyze the same security on different timeframes:

```csharp
// 15-minute trend
var shortTermTrend = minuteBars
  .Aggregate(BarInterval.FifteenMinutes)
  .ToEma(20);

// 4-hour trend
var longTermTrend = minuteBars
  .Aggregate(BarInterval.FourHours)
  .ToEma(20);
```

### Reducing data noise

Aggregate to larger timeframes to smooth out intraday volatility:

```csharp
// reduce 1-minute noise by using 5-minute bars
var smoothedBars = tickData
  .Aggregate(BarInterval.FiveMinutes);
```

## Streaming aggregation

For live feeds, use the streaming aggregator hubs to convert small bars or raw ticks into larger period bars in real time.

```csharp
// bar → bar aggregation (e.g. 1-minute bars to 5-minute bars)
BarHub barHub = new();
BarAggregatorHub fiveMinHub = barHub.ToBarAggregatorHub(
    BarInterval.FiveMinutes);

// tick → bar aggregation (raw trades to 1-minute OHLCV)
TradeTickHub tickHub = new();
TradeTickAggregatorHub oneMinHub = tickHub.ToTradeTickAggregatorHub(
    BarInterval.OneMinute);
```

Both hubs accept either a `BarInterval` enum or a custom `TimeSpan`. `BarInterval.Month` is not supported in streaming mode — use the `TimeSpan` overload for month-or-longer periods.

### Gap-fill behavior

Both `ToBarAggregatorHub` and `ToTradeTickAggregatorHub` accept an optional `fillGaps` flag (default `false`):

- `fillGaps: false` (default) — silent buckets are simply omitted from the output stream. If no upstream bar/tick lands inside a bucket, no bar is emitted for that period.
- `fillGaps: true` — synthetic zero-volume bars are emitted to bridge any silent buckets between the last active bar and the next active bucket. The synthetic bar's `Open`, `High`, `Low`, and `Close` all carry forward the prior bar's `Close`.

Consumers that need a different gap policy (e.g. interpolation, holding the prior `High`/`Low`, or a configurable null marker) should pre-process the upstream stream before subscribing the aggregator. A `GapFillMode` enum that exposes interpolation and other policies natively is on the v3.1 roadmap.

```csharp
// emit a synthetic bar for every silent 5-minute window
BarAggregatorHub gappedHub = barHub
    .ToBarAggregatorHub(BarInterval.FiveMinutes, fillGaps: true);
```

## Related utilities

- [Bar utilities overview](/utilities/bars/)
- [Validate bar history](/utilities/bars/validate-bar-history) - Ensure data quality
- [Sort bars](/utilities/bars/sort-bars) - Ensure chronological order
