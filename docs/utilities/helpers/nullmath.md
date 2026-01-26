---
title: NullMath
description: Null-safe mathematical operations.
---

# NullMath

Null-safe mathematical operations that handle null values gracefully. Most `NullMath` methods work exactly like the `System.Math` library in C#, except these return `null` if a `null` is provided.

## Why NullMath?

The `System.Math` library does not allow `null` values, requiring defensive null-checking throughout your code. `NullMath` provides that defensive handling automatically.

```csharp
// Without NullMath - requires null checking
decimal? value = GetNullableValue();
decimal? result = value.HasValue ? Math.Abs(value.Value) : null;

// With NullMath - handles nulls automatically
decimal? value = GetNullableValue();
decimal? result = NullMath.Abs(value);
```

## Available methods

<!-- markdownlint-disable MD060 -->
| Method | Example usage | Description |
|--------|---------------|-------------|
| Abs | `var abs = NullMath.Abs(-25)` → `25`<br>`var abs = NullMath.Abs(null)` → `null` | Absolute value |
| Round | `var rnd = NullMath.Round(1.234, 1)` → `1.2`<br>`var rnd = NullMath.Round(null, 1)` → `null` | Round to decimal places |
| Null2NaN | `var val = null;`<br>`var n2n = val.Null2NaN()` → `[NaN]` | Convert null to NaN |
| NaN2Null | `var val = double.NaN;`<br>`var n2n = val.NaN2Null()` → `null` | Convert NaN to null |
<!-- markdownlint-enable MD060 -->

## Abs

Returns the absolute value of a nullable decimal, or null if the input is null.

### Syntax

```csharp
decimal? abs = NullMath.Abs(decimal? value);
```

### Usage

```csharp
decimal? positive = NullMath.Abs(-25);    // 25
decimal? negative = NullMath.Abs(null);   // null
decimal? alreadyPos = NullMath.Abs(10);   // 10
```

### Common use case

Calculate absolute differences in custom indicators:

```csharp
public decimal? CalculateAbsChange(decimal? current, decimal? previous)
{
  if (current == null || previous == null)
    return null;
    
  return NullMath.Abs(current - previous);
}
```

## Round

Rounds a nullable decimal to a specified number of decimal places, or returns null if the input is null.

### Syntax

```csharp
decimal? rounded = NullMath.Round(decimal? value, int decimals);
```

### Parameters

**value** - The nullable decimal value to round

**decimals** - The number of decimal places

### Usage

```csharp
decimal? rounded = NullMath.Round(1.234, 1);   // 1.2
decimal? nulled = NullMath.Round(null, 1);     // null
decimal? whole = NullMath.Round(1.567, 0);     // 2
```

### Common use case

Format indicator output for display:

```csharp
public string FormatIndicatorValue(decimal? value)
{
  var rounded = NullMath.Round(value, 2);
  return rounded?.ToString("F2") ?? "N/A";
}
```

## Null2NaN

Converts a nullable double to NaN (Not a Number) if it is null. Useful when interfacing with systems that expect NaN instead of null.

### Syntax

```csharp
double nan = value.Null2NaN();
```

### Usage

```csharp
double? value = null;
double result = value.Null2NaN();  // double.NaN

double? value2 = 1.5;
double result2 = value2.Null2NaN();  // 1.5
```

### Common use case

Prepare data for charting libraries that use NaN for missing values:

```csharp
public double[] PrepareChartData(IEnumerable<decimal?> values)
{
  return values.Select(v => ((double?)v).Null2NaN()).ToArray();
}
```

## NaN2Null

Converts NaN (Not a Number) to null. Useful when receiving data from external sources that use NaN for missing values.

### Syntax

```csharp
double? nullValue = value.NaN2Null();
```

### Usage

```csharp
double value = double.NaN;
double? result = value.NaN2Null();  // null

double value2 = 1.5;
double? result2 = value2.NaN2Null();  // 1.5
```

### Common use case

Convert external data to library-compatible format:

```csharp
public List<Quote> ImportFromExternalSource(ExternalData[] data)
{
  return data.Select(d => new Quote {
    Timestamp = d.Date,
    Open = d.Open.NaN2Null(),
    High = d.High.NaN2Null(),
    Low = d.Low.NaN2Null(),
    Close = d.Close.NaN2Null(),
    Volume = d.Volume.NaN2Null()
  }).ToList();
}
```

## Custom indicator example

Using NullMath in a custom indicator to avoid null-checking boilerplate:

```csharp
public static IReadOnlyList<MyResult> ToMyIndicator(
  this IEnumerable<IQuote> quotes, int period)
{
  var quotesList = quotes.ToList();
  var results = new List<MyResult>();
  
  for (int i = 0; i < quotesList.Count; i++)
  {
    decimal? previous = i > 0 ? results[i - 1].Value : null;
    decimal? current = quotesList[i].Close;
    
    // NullMath handles nulls automatically
    var change = NullMath.Abs(current - previous);
    var rounded = NullMath.Round(change, 2);
    
    results.Add(new MyResult {
      Timestamp = quotesList[i].Timestamp,
      Value = current,
      AbsChange = rounded
    });
  }
  
  return results;
}
```

## Performance considerations

::: tip Performance
`NullMath` methods are lightweight wrappers with minimal overhead. The null-checking cost is negligible compared to the cost of explicit null checks throughout your code.
:::

## Related utilities

- [Helper utilities overview](/utilities/helpers/)
- [Numerical methods](/utilities/helpers/numerical-methods) - Slope and standard deviation
- [Customization guide](/customization) - Build custom indicators
