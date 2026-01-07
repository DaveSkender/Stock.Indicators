---
title: Zig Zag
description: Zig Zag is a financial market price chart overlay that simplifies the up and down movements and transitions based on a percent change smoothing threshold.
---

# Zig Zag

[Zig Zag](https://school.stockcharts.com/doku.php?id=technical_indicators:zigzag) is a price chart overlay that simplifies the up and down movements and transitions based on a percent change smoothing threshold.
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/226 "Community discussion about this indicator")

<ClientOnly>
  <IndicatorChart src="/data/ZigZag.json" :height="360" />
</ClientOnly>

```csharp
// C# usage syntax
IReadOnlyList<ZigZagResult> results =
  quotes.ToZigZag(endType, percentChange);
```

## Parameters

| param | type | description |
| ----- | ---- | ----------- |
| `endType` | EndType | Determines whether `Close` or `High/Low` are used to measure percent change.  See [EndType options](#endtype-options) below.  Default is `EndType.Close`. |
| `percentChange` | decimal | Percent change required to establish a line endpoint.  Example: 3.5% would be entered as 3.5 (not 0.035).  Must be greater than 0.  Typical values range from 3 to 10.  Default is 5. |

### Historical quotes requirements

You must have at least two periods of `quotes` to cover the warmup periods, but notably more is needed to be useful.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](/guide#historical-quotes) for more information.

### EndType options

**`EndType.Close`** - Percent change measured from `Close` price (default)

**`EndType.HighLow`** - Percent change measured from `High` and `Low` price

## Response

```csharp
IReadOnlyList<ZigZagResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- If you do not supply enough points to cover the percent change, there will be no Zig Zag points or lines.
- The first line segment starts after the first confirmed point; ZigZag values before the first confirmed point will be `null`.
- The last line segment is an approximation as the direction is indeterminate.

::: warning
depending on the specified `endType`, the indicator cannot be initialized if the first `Quote` in `quotes` has a `High`,`Low`, or `Close` value of 0 (zero).
:::

::: warning 🖌️ Repaint warning
The last line segment will always be redrawn back to the last known pivot.  Do not attempt to calculate incremental values since previous values may change based on newer quotes.
:::

### `ZigZagResult`

| property | type | description |
| -------- | ---- | ----------- |
| `Timestamp` | DateTime | Date from evaluated `TQuote` |
| `ZigZag` | decimal | Zig Zag line for `percentChange` |
| `PointType` | string | Zig Zag endpoint type (`H` for high point, `L` for low point) |
| `RetraceHigh` | decimal | Retrace line for high points |
| `RetraceLow` | decimal | Retrace line for low points |

### Utilities

- [.Condense()](/utilities/results#condense)
- [.Find(lookupDate)](/utilities/results#find-indicator-result-by-date)
- [.RemoveWarmupPeriods(qty)](/utilities/results#remove-warmup-periods)

See [Utilities and helpers](/utilities/results) for more information.

## Chaining

Results can be further processed on `ZigZag` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .ToZigZag(..)
    .ToSlope(..);
```

This indicator must be generated from `quotes` and **cannot** be generated from results of another chain-enabled indicator or method.
