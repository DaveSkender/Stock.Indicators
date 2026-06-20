---
title: Bar utilities
description: Utilities for preparing and transforming historical price bars.
---

# Bar utilities

Utilities for preparing and transforming historical price bars before using them with indicators.

<div class="utility-cards">

## [Use alternate price](/utilities/bars/use-alternate-price)

Specify which price element to analyze (HL2, HLC3, OC2, etc.) instead of the standard Close price.

```csharp
var results = bars.Use(CandlePart.HL2).ToRsi(14);
```

[See more →](/utilities/bars/use-alternate-price)

## [Sort bars](/utilities/bars/sort-bars)

Sort any collection of bars into chronological order before using indicators.

```csharp
var sortedBars = bars.ToSortedList();
```

[See more →](/utilities/bars/sort-bars)

## [Resize bar history](/utilities/bars/resize-bar-history)

Aggregate intraday bars into larger timeframes.

```csharp
var hourlyBars = minuteBars.Aggregate(BarInterval.OneHour);
```

[See more →](/utilities/bars/resize-bar-history)

## [Extended candle properties](/utilities/bars/extended-candle-properties)

Convert bars into an extended format with calculated candle properties.

```csharp
var candles = bars.ToCandles();
```

[See more →](/utilities/bars/extended-candle-properties)

## [Validate bar history](/utilities/bars/validate-bar-history)

Advanced validation to check for duplicate dates and bad data.

```csharp
var validBars = bars.Validate();
```

[See more →](/utilities/bars/validate-bar-history)

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
  border-left: 4px solid var(--vp-c-brand-1);
  border-radius: 8px;
  transition: all 0.2s ease;
}

.utility-cards h2:hover {
  border-left-color: var(--vp-c-brand-2);
  transform: translateX(4px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

.utility-cards h2 a {
  text-decoration: none;
  color: var(--vp-c-brand-1);
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
