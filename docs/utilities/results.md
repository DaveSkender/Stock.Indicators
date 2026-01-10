---
title: Result utilities
description: Utilities for working with indicator results.
---

# Result utilities

Utilities for working with indicator results after calculation.

See [individual indicator pages](/indicators) for information on recommended pruning quantities.

## Condense

`results.Condense()` removes non-essential results so it only returns meaningful data records. For example, when used on [Candlestick Patterns](/indicators/Doji), it only returns records where a signal is generated.

```csharp
// example: only show Marubozu signals
IReadOnlyList<CandleResult> results
  = quotes.ToMarubozu(..).Condense();
```

::: warning
In all cases, `.Condense()` removes non-essential results and less than the input `quotes`.
:::

## Find indicator result by date

`results.Find(lookupDate)` is a simple lookup for your indicator results collection. Just specify the date you want returned.

```csharp
// calculate indicator series
IReadOnlyList<SmaResult> results = quotes.ToSma(20);

// find result on a specific date
DateTime lookupDate = [..];
SmaResult result = results.Find(lookupDate);

// throws 'InvalidOperationException' when not found
```

## Remove warmup periods

`results.RemoveWarmupPeriods()` removes the recommended initial warmup periods from indicator results. An alternative `.RemoveWarmupPeriods(removePeriods)` is also provided if you want to customize the pruning amount.

```csharp
// auto remove recommended warmup periods
IReadOnlyList<AdxResult> results =
  quotes.ToAdx(14).RemoveWarmupPeriods();

// remove a specific quantity of periods
int n = 14;
IReadOnlyList<AdxResult> results =
  quotes.ToAdx(n).RemoveWarmupPeriods(n + 100);
```

See [individual indicator pages](/indicators) for information on recommended pruning quantities.

::: info Limited availability
`.RemoveWarmupPeriods()` is not available on some indicators; however, you can still do a custom pruning by using the customizable `.RemoveWarmupPeriods(removePeriods)`.
:::

::: warning Auto-pruning is unstable
Without a specified `removePeriods` value, this utility will reverse-engineer the pruning amount. When there are unusual results or when chaining indicators, there will be an erroneous increase in the amount of pruning. If you want more certainty, use a specific number for `removePeriods`. Using this method on chained indicators without `removePeriods` is strongly discouraged.
:::

## Sort results

`results.ToSortedList()` sorts any collection of indicator results and returns it as an `IReadOnlyList` sorted by ascending `Timestamp`. Results from the library indicators are already sorted, so you'd only potentially need this if you're creating [custom indicators](/customization).

---
Last updated: January 7, 2026
