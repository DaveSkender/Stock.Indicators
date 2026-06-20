---
title: Sort bars
description: Sort bars into chronological order.
---

# Sort bars

`bars.ToSortedList()` sorts any collection of `TBar` or `ISeries` and returns it as an `IReadOnlyList` sorted by ascending `Timestamp`. Bars should be in chronological order before using library indicators; use this utility to sort them if needed.

## Syntax

```csharp
IReadOnlyList<TBar> sortedBars = bars.ToSortedList();
```

## Returns

**`IReadOnlyList<TBar>`** - A read-only list of bars sorted by ascending timestamp.

## Usage

```csharp
// sort unsorted bars
IReadOnlyList<Bar> sortedBars = bars.ToSortedList();

// use inline with indicators
var results = bars
  .ToSortedList()
  .ToRsi(14);
```

## Common use cases

### Imported data

When importing bars from external sources that may not be in chronological order:

```csharp
// import from API or database
IEnumerable<Bar> importedBars = GetBarsFromApi();

// ensure chronological order
IReadOnlyList<Bar> sortedBars = importedBars.ToSortedList();

// now safe to use with indicators
var smaResults = sortedBars.ToSma(20);
```

### Combined datasets

When merging bars from multiple sources:

```csharp
var combinedBars = source1Bars
  .Concat(source2Bars)
  .ToSortedList();
```

## Performance considerations

::: tip Performance
`.ToSortedList()` uses an efficient sorting algorithm, but sorting large datasets can be expensive. If your data source guarantees chronological order, you can skip this step.
:::

::: info Pre-sorted data
Most data providers return bars in chronological order. Use `.ToSortedList()` only when you cannot guarantee the sort order from your data source.
:::

## Related utilities

- [Bar utilities overview](/utilities/bars/)
- [Validate bar history](/utilities/bars/validate-bar-history) - Detect duplicates and bad data
- [Sort results](/utilities/results/sort-results) - Sort indicator results
