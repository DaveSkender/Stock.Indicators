---
title: Sort results
description: Sort indicator results by timestamp.
---

# Sort results

`results.ToSortedList()` sorts any collection of indicator results and returns it as an `IReadOnlyList` sorted by ascending `Timestamp`. Results from the library indicators are already sorted, so you'd only potentially need this if you're creating [custom indicators](/guide/customization).

## Syntax

```csharp
IReadOnlyList<TResult> sortedResults = results.ToSortedList();
```

## Returns

**IReadOnlyList\<TResult\>** - A read-only list of results sorted by ascending timestamp.

## Usage

```csharp
// sort custom indicator results
IReadOnlyList<MyCustomResult> sortedResults =
  customResults.ToSortedList();
```

## When to use

::: info Built-in indicators already sorted
All built-in library indicators return results in chronological order. You typically only need `.ToSortedList()` for custom indicator implementations.
:::

### Custom indicators

When building custom indicators, ensure results are sorted:

```csharp
public IReadOnlyList<MyResult> ToMyCustomIndicator(
  this IEnumerable<IBar> bars)
{
  // perform custom calculations
  var results = CalculateMyIndicator(bars);
  
  // ensure chronological order before returning
  return results.ToSortedList();
}
```

### Combined result sets

When merging results from multiple sources or calculations:

```csharp
var combined = results1
  .Concat(results2)
  .ToSortedList();
```

### After manual manipulation

When you've manually modified or filtered results and need to restore order:

```csharp
var manipulated = results
  .Where(r => r.Sma > 50)
  .Select(r => new SmaResult { 
    Timestamp = r.Timestamp.AddDays(1), 
    Sma = r.Sma 
  })
  .ToSortedList();  // re-sort after timestamp modification
```

## Performance considerations

::: tip Performance
`.ToSortedList()` uses an efficient sorting algorithm, but sorting large result sets can be expensive. Only use when necessary (typically for custom indicators or when you've manipulated timestamps).
:::

## Comparison with bar sorting

This utility is similar to [Sort bars](/utilities/bars/sort-bars), but works on indicator results instead of raw bars:

| Utility | Input type | Output type | Use case |
|---------|-----------|-------------|----------|
| `.ToSortedList()` (bars) | `IEnumerable<IBar>` | `IReadOnlyList<TBar>` | Sort historical bars |
| `.ToSortedList()` (results) | `IEnumerable<IResult>` | `IReadOnlyList<TResult>` | Sort indicator results |

## Custom indicator example

Complete example of using `.ToSortedList()` in a custom indicator:

```csharp
public static IReadOnlyList<CustomResult> ToCustom(
  this IEnumerable<IBar> bars, int period)
{
  // convert to list for indexing
  var barsList = bars.ToList();
  var results = new List<CustomResult>();
  
  // calculate indicator (may produce unsorted results)
  for (int i = 0; i < barsList.Count; i++)
  {
    var result = new CustomResult
    {
      Timestamp = barsList[i].Timestamp,
      Value = CalculateValue(barsList, i, period)
    };
    results.Add(result);
  }
  
  // ensure results are sorted before returning
  return results.ToSortedList();
}
```

## Alternatives

For simple chronological ordering, you can also use LINQ:

```csharp
// LINQ alternative
var sorted = results
  .OrderBy(r => r.Timestamp)
  .ToList();

// ToSortedList() is more concise
var sorted = results.ToSortedList();
```

Both approaches work, but `.ToSortedList()` is the library's convention and returns the correct `IReadOnlyList` type.

## Related utilities

- [Result utilities overview](/utilities/results/)
- [Sort bars](/utilities/bars/sort-bars) - Sort bar data
- [Customization guide](/guide/customization) - Build custom indicators
