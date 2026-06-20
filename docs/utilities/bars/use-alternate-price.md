---
title: Use alternate price
description: Specify which price element to analyze instead of the standard Close price.
---

# Use alternate price

`bars.Use()` can be used before most indicator calls to specify which price element to analyze. It cannot be used for indicators that require the full OHLCV bar profile.

```csharp
IReadOnlyList<TimeValue> barParts = bars.Use(CandlePart candlePart);
```

## Parameters

**`candlePart`** - The price element to use for calculations

<!--@include: ../../shared/candlepart-options.md-->

## Common use cases

### Median price (HL2)

Use the median between high and low for smoother price action analysis:

```csharp
var smaResults = bars
  .Use(CandlePart.HL2)
  .ToSma(20);
```

### Typical price (HLC3)

Use the typical price for volume-weighted analyses:

```csharp
var emaResults = bars
  .Use(CandlePart.HLC3)
  .ToEma(50);
```

## Limitations

::: warning 🚩 Incompatible indicators
Some indicators require the full OHLCV bar profile and cannot be used with `.Use()`. These include:

- Indicators that explicitly use multiple price points (ATR, Stochastic, etc.)
- Volume-based indicators (OBV, CMF, etc.) when not using `CandlePart.Volume`
- Candlestick pattern indicators

Attempting to use `.Use()` with incompatible indicators may produce unexpected results.
:::

## Streaming

Both incremental and live streaming variants of the bar-part selector are available and produce `TimeValue` results (the same `IReusable` type the Series form emits).

### BufferList

Use `ToBarPartList()` when you need incremental, single-threaded selection without a hub:

```csharp
BarPartList parts = bars.ToBarPartList(CandlePart.HL2);

// or, build incrementally
BarPartList parts = new(CandlePart.HL2);

foreach (IBar bar in bars)
{
  parts.Add(bar);
}

IReadOnlyList<TimeValue> results = parts;
```

### StreamHub

Subscribe to a `BarHub` to chain the bar-part selector into a live pipeline:

```csharp
BarHub barHub = new();
BarPartHub partHub = barHub.ToBarPartHub(CandlePart.HLC3);

// chain downstream indicators off the part selector
EmaHub emaHub = partHub.ToEmaHub(20);

foreach (IBar bar in bars)  // simulating stream
{
  barHub.Add(bar);
}

IReadOnlyList<TimeValue> selected = partHub.Results;
IReadOnlyList<EmaResult> emaResults = emaHub.Results;
```

See [Buffer lists](/guide/styles/buffer) and [Stream hubs](/guide/styles/stream) for full usage guides.

## Related utilities

- [Bar utilities overview](/utilities/bars/)
- [Validate bar history](/utilities/bars/validate-bar-history) - Ensure data quality
- [Sort bars](/utilities/bars/sort-bars) - Ensure chronological order
