---
title: Quote utilities
description: Utilities for preparing and transforming historical price quotes.
---

# Quote utilities

Utilities for preparing and transforming historical price quotes before using them with indicators.

<div class="utility-cards">

## [Use alternate price](/utilities/quotes/use-alternate-price)

Specify which price element to analyze (HL2, HLC3, OC2, etc.) instead of the standard Close price.

```csharp
var results = quotes.Use(CandlePart.HL2).ToRsi(14);
```

[See more →](/utilities/quotes/use-alternate-price)

## [Sort quotes](/utilities/quotes/sort-quotes)

Sort any collection of quotes into chronological order before using indicators.

```csharp
var sortedQuotes = quotes.ToSortedList();
```

[See more →](/utilities/quotes/sort-quotes)

## [Resize quote history](/utilities/quotes/resize-quote-history)

Aggregate intraday bars into larger timeframes.

```csharp
var hourlyQuotes = minuteQuotes.Aggregate(PeriodSize.OneHour);
```

[See more →](/utilities/quotes/resize-quote-history)

## [Extended candle properties](/utilities/quotes/extended-candle-properties)

Convert quotes into an extended format with calculated candle properties.

```csharp
var candles = quotes.ToCandles();
```

[See more →](/utilities/quotes/extended-candle-properties)

## [Validate quote history](/utilities/quotes/validate-quote-history)

Advanced validation to check for duplicate dates and bad data.

```csharp
var validQuotes = quotes.Validate();
```

[See more →](/utilities/quotes/validate-quote-history)

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
