---
title: Result utilities
description: Utilities for working with indicator results.
---

# Result utilities

Utilities for working with indicator results after calculation.

<div class="utility-cards">

## [Condense](/utilities/results/condense)

Remove non-essential results so only meaningful data records are returned.

```csharp
var signals = quotes.ToMarubozu().Condense();
```

[See more →](/utilities/results/condense)

## [Find by date](/utilities/results/find-by-date)

Simple lookup for indicator results by date.

```csharp
var result = results.Find(lookupDate);
```

[See more →](/utilities/results/find-by-date)

## [Remove warmup periods](/utilities/results/remove-warmup-periods)

Remove the recommended initial warmup periods from indicator results.

```csharp
var trimmed = results.RemoveWarmupPeriods();
```

[See more →](/utilities/results/remove-warmup-periods)

## [Sort results](/utilities/results/sort-results)

Sort any collection of indicator results by ascending timestamp.

```csharp
var sorted = results.ToSortedList();
```

[See more →](/utilities/results/sort-results)

</div>

<style scoped>
.utility-cards {
  display: grid;
  gap: 1.5rem;
  margin: 2rem 0;
}

.utility-cards h2 {
  margin: 0;
  font-size: 1.35rem;
  padding: 1.25rem;
  background: var(--vp-c-bg-soft);
  border: 1px solid var(--vp-c-divider);
  border-left: 4px solid var(--vp-c-brand);
  border-radius: 8px;
  transition: all 0.2s ease;
}

.utility-cards h2:hover {
  border-left-color: var(--vp-c-brand-dark);
  transform: translateX(4px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

.utility-cards h2 a {
  text-decoration: none;
  color: var(--vp-c-brand);
}

.utility-cards p {
  margin: 0.75rem 0 0 0;
  padding: 0 1.25rem 1.25rem 1.25rem;
  color: var(--vp-c-text-2);
  line-height: 1.6;
}

.utility-cards p strong {
  display: block;
  margin-top: 0.5rem;
  color: var(--vp-c-text-1);
  font-size: 0.9rem;
}
</style>
