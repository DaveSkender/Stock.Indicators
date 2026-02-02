---
title: Find result by date
description: Lookup indicator results for a specific date.
---

# Find result by date

`results.Find(lookupDate)` is a simple lookup for your indicator results collection. Just specify the date you want returned.

## Syntax

```csharp
TResult result = results.Find(DateTime lookupDate);
```

## Parameters

**lookupDate** - The `DateTime` to search for in the results collection.

## Returns

**TResult** - The indicator result for the specified date.

## Throws

**InvalidOperationException** - Thrown when the specified date is not found in the results collection.

## Usage

```csharp
// calculate indicator series
IReadOnlyList<SmaResult> results = quotes.ToSma(20);

// find result on a specific date
DateTime lookupDate = DateTime.Parse("2024-01-15");
SmaResult result = results.Find(lookupDate);

Console.WriteLine($"SMA on {lookupDate:d}: {result.Sma}");
```

## Common use cases

### Single date lookup

Get the indicator value for a specific trading day:

```csharp
var rsiResults = quotes.ToRsi(14);
var todayRsi = rsiResults.Find(DateTime.Today);

Console.WriteLine($"Today's RSI: {todayRsi.Rsi}");
```

### Comparison across dates

Compare indicator values between different dates:

```csharp
var emaResults = quotes.ToEma(20);

var jan1 = emaResults.Find(DateTime.Parse("2024-01-01"));
var feb1 = emaResults.Find(DateTime.Parse("2024-02-01"));

decimal change = feb1.Ema.Value - jan1.Ema.Value;
Console.WriteLine($"EMA changed by {change:F2}");
```

### Event-driven lookups

Look up indicator values based on external events:

```csharp
// get earnings announcement dates
var earningsDates = GetEarningsCalendar();

foreach (var date in earningsDates)
{
  try
  {
    var result = rsiResults.Find(date);
    Console.WriteLine($"RSI at earnings ({date:d}): {result.Rsi}");
  }
  catch (InvalidOperationException)
  {
    Console.WriteLine($"No data for {date:d}");
  }
}
```

## Error handling

### Date not found

The method throws `InvalidOperationException` when the date doesn't exist:

```csharp
try
{
  var result = results.Find(DateTime.Parse("2099-01-01"));
}
catch (InvalidOperationException)
{
  Console.WriteLine("Date not found in results");
}
```

### Safe lookup with TryFind pattern

For safe lookups, use a try-catch or check if the date exists first:

```csharp
DateTime searchDate = DateTime.Parse("2024-01-15");

// Option 1: Try-catch
try
{
  var result = results.Find(searchDate);
  ProcessResult(result);
}
catch (InvalidOperationException)
{
  Console.WriteLine("Date not in range");
}

// Option 2: Check existence first
if (results.Any(r => r.Timestamp.Date == searchDate.Date))
{
  var result = results.Find(searchDate);
  ProcessResult(result);
}
```

## Performance considerations

::: tip Performance
`.Find()` performs a linear search through the results collection. For frequent lookups on large datasets, consider:

- Converting results to a `Dictionary<DateTime, TResult>` for O(1) lookups
- Caching frequently accessed results
- Using LINQ `.Where()` for range queries instead of multiple `.Find()` calls
:::

## Time precision

The comparison uses exact `DateTime` matching:

```csharp
// These are different timestamps
DateTime date1 = DateTime.Parse("2024-01-15 00:00:00");
DateTime date2 = DateTime.Parse("2024-01-15 09:30:00");

// Only one will match if your data has specific times
var result = results.Find(date1); // may not match date2's data
```

::: tip Date-only comparison
For date-only comparisons (ignoring time), compare using `.Date`:

```csharp
var targetDate = DateTime.Parse("2024-01-15").Date;
var result = results.FirstOrDefault(r => r.Timestamp.Date == targetDate);
```

:::

## Alternatives

For more complex queries, consider LINQ:

```csharp
// Find results in a date range
var rangeResults = results
  .Where(r => r.Timestamp >= startDate && r.Timestamp <= endDate);

// Find results meeting criteria
var highRsi = results
  .Where(r => r.Rsi > 70)
  .OrderByDescending(r => r.Rsi);
```

## Related utilities

- [Result utilities overview](/utilities/results/)
- [Condense](/utilities/results/condense) - Filter to meaningful results
- [Remove warmup periods](/utilities/results/remove-warmup-periods) - Remove initial periods
