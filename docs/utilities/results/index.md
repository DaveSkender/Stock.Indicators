---
title: Result utilities
description: Utilities for working with indicator results.
---

# Result utilities

Utilities for working with indicator results after calculation and analysis.

## Condense

Remove non-essential results so only meaningful data records are returned. [More info →](/utilities/results/condense)

```csharp
var signals = bars.ToMarubozu().Condense();
```

## Find by date

Simple lookup for indicator results by date. [More info →](/utilities/results/find-by-date)

```csharp
var result = results.Find(lookupDate);
```

## Remove warmup periods

Remove the recommended initial warmup periods from indicator results. [More info →](/utilities/results/remove-warmup-periods)

```csharp
var trimmed = results.RemoveWarmupPeriods();
```

## Sort results

Sort any collection of indicator results into chronological order. [More info →](/utilities/results/sort-results)

```csharp
var sorted = results.ToSortedList();
```
