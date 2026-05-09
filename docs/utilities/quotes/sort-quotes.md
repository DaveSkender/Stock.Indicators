---
title: Sort quotes
description: Sort quotes into chronological order.
---

# Sort quotes

`quotes.ToSortedList()` sorts any collection of `TQuote` or `ISeries` and returns it as an `IReadOnlyList` sorted by ascending `Timestamp`. Quotes should be in chronological order before using library indicators; use this utility to sort them if needed.

## Syntax

```csharp
IReadOnlyList<TQuote> sortedQuotes = quotes.ToSortedList();
```

## Returns

**IReadOnlyList\<TQuote\>** - A read-only list of quotes sorted by ascending timestamp.

## Usage

```csharp
// sort unsorted quotes
IReadOnlyList<Quote> sortedQuotes = quotes.ToSortedList();

// use inline with indicators
var results = quotes
  .ToSortedList()
  .ToRsi(14);
```

## When to use

### Imported data

When importing quotes from external sources that may not be in chronological order:

```csharp
// import from API or database
IEnumerable<Quote> importedQuotes = GetQuotesFromApi();

// ensure chronological order
IReadOnlyList<Quote> sortedQuotes = importedQuotes.ToSortedList();

// now safe to use with indicators
var smaResults = sortedQuotes.ToSma(20);
```

### Combined datasets

When merging quotes from multiple sources:

```csharp
var combinedQuotes = source1Quotes
  .Concat(source2Quotes)
  .ToSortedList();
```

## Performance considerations

::: tip Performance
`.ToSortedList()` uses an efficient sorting algorithm, but sorting large datasets can be expensive. If your data source guarantees chronological order, you can skip this step.
:::

::: info Pre-sorted data
Most data providers return quotes in chronological order. Use `.ToSortedList()` only when you cannot guarantee the sort order from your data source.
:::

## Related utilities

- [Quote utilities overview](/utilities/quotes/)
- [Validate quote history](/utilities/quotes/validate-quote-history) - Detect duplicates and bad data
- [Sort results](/utilities/results/sort-results) - Sort indicator results
