---
title: Remove warmup periods
description: Remove initial convergence periods from indicator results.
---

# Remove warmup periods

`results.RemoveWarmupPeriods()` removes the recommended initial warmup periods from indicator results. An alternative `.RemoveWarmupPeriods(removePeriods)` is also provided if you want to customize the pruning amount.

## Syntax

```csharp
// automatic removal (uses recommended amount)
IReadOnlyList<TResult> results = results.RemoveWarmupPeriods();

// custom removal amount
IReadOnlyList<TResult> results = results.RemoveWarmupPeriods(int removePeriods);
```

## Parameters

**removePeriods** _(optional)_ - The number of periods to remove from the beginning of the results. If not specified, uses the indicator's recommended warmup period.

## Returns

**IReadOnlyList\<TResult\>** - Results with the specified number of initial periods removed.

## Usage

### Automatic warmup removal

```csharp
// auto remove recommended warmup periods
IReadOnlyList<AdxResult> results =
  quotes.ToAdx(14).RemoveWarmupPeriods();
```

### Custom warmup removal

```csharp
// remove a specific quantity of periods
int n = 14;
IReadOnlyList<AdxResult> results =
  quotes.ToAdx(n).RemoveWarmupPeriods(n + 100);
```

## Why remove warmup periods?

Most indicators need time to "warm up" before producing reliable values. During this warmup period, the indicator is converging to its steady state, and the values may not be accurate.

### Example: Moving averages

A 20-period SMA needs 20 periods before it has enough data to calculate a full average:

```csharp
var smaResults = quotes.ToSma(20);
// First 19 results are null
// 20th result is the first valid SMA value

var trimmed = smaResults.RemoveWarmupPeriods();
// Now starts with the first valid value
```

## Common use cases

### Clean charting data

Remove warmup periods before charting to avoid misleading early values:

```csharp
var chartData = quotes
  .ToAdx(14)
  .RemoveWarmupPeriods();

// chart only reliable ADX values
PlotChart(chartData);
```

### Backtesting

Ensure trading signals are based only on fully converged indicators:

```csharp
var signals = quotes
  .ToRsi(14)
  .RemoveWarmupPeriods()
  .Where(r => r.Rsi > 70 || r.Rsi < 30);
```

### Performance comparison

Remove warmup periods to compare indicators fairly:

```csharp
var ema20 = quotes.ToEma(20).RemoveWarmupPeriods();
var sma20 = quotes.ToSma(20).RemoveWarmupPeriods();

// compare from same starting point
```

## Recommended warmup periods

See [individual indicator pages](/indicators) for information on recommended pruning quantities for each indicator.

Common warmup periods:

| Indicator | Recommended warmup |
|-----------|-------------------|
| SMA(n) | n periods |
| EMA(n) | 2×n periods |
| RSI(n) | n + 250 periods |
| ADX(n) | 2×n + 100 periods |
| MACD | 250 periods |

## Availability

::: info Limited availability
`.RemoveWarmupPeriods()` (without parameters) is not available on some indicators; however, you can still do a custom pruning by using the customizable `.RemoveWarmupPeriods(removePeriods)`.
:::

## Important warnings

::: warning Auto-pruning is unstable
Without a specified `removePeriods` value, this utility will reverse-engineer the pruning amount. When there are unusual results or when chaining indicators, there will be an erroneous increase in the amount of pruning.

If you want more certainty, use a specific number for `removePeriods`.

**Using this method on chained indicators without `removePeriods` is strongly discouraged.**
:::

### Chained indicators example

```csharp
// AVOID: auto-pruning on chained indicators
var chainedResults = quotes
  .ToEma(20)
  .ToRsi(14)
  .RemoveWarmupPeriods();  // ❌ May remove too much

// BETTER: specify the amount
var chainedResults = quotes
  .ToEma(20)
  .ToRsi(14)
  .RemoveWarmupPeriods(300);  // ✅ Predictable
```

## Manual pruning alternative

You can manually prune using LINQ if you need more control:

```csharp
// skip first 100 results manually
var trimmed = results.Skip(100).ToList();

// or skip until first non-null value
var trimmed = results
  .SkipWhile(r => r.Sma == null)
  .ToList();
```

## Related utilities

- [Result utilities overview](/utilities/results/)
- [Condense](/utilities/results/condense) - Remove non-essential results
- [Sort results](/utilities/results/sort-results) - Ensure chronological order
- [Individual indicators](/indicators) - See recommended warmup periods
