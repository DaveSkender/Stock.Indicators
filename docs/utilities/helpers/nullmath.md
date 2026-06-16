---
title: Math helpers
description: Null-safe math operations (NullMath) and deterministic precision math (DeMath) for custom indicator development.
---

# Math helpers

The library provides two math helper classes for use in custom indicator development: `NullMath` for null-safe operations, and `DeMath` for deterministic precision math.

## NullMath

Null-safe mathematical operations that handle null values gracefully. Most `NullMath` methods work exactly like the `System.Math` library in C#, except these return `null` if a `null` is provided.

## Why NullMath?

The `System.Math` library does not allow `null` values, requiring defensive null-checking throughout your code. `NullMath` provides that defensive handling automatically.

```csharp
// Without NullMath - requires null checking
double? value = GetNullableValue();
double? result = value.HasValue ? Math.Abs(value.Value) : null;

// With NullMath - handles nulls automatically
double? value = GetNullableValue();
double? result = NullMath.Abs(value);
```

## Available methods

<!-- markdownlint-disable MD060 -->
| Method | Example usage | Description |
|--------|---------------|-------------|
| Abs | `var abs = NullMath.Abs(-25d)` → `25`<br>`var abs = NullMath.Abs(null)` → `null` | Absolute value |
| Null2NaN | `var val = null;`<br>`var n2n = val.Null2NaN()` → `[NaN]` | Convert null to NaN |
| NaN2Null | `var val = double.NaN;`<br>`var n2n = val.NaN2Null()` → `null` | Convert NaN to null |
<!-- markdownlint-enable MD060 -->

## Abs

Returns the absolute value of a nullable decimal, or null if the input is null.

### Syntax

```csharp
double? abs = NullMath.Abs(double? value);
```

### Usage

```csharp
double? positive = NullMath.Abs(-25);    // 25
double? negative = NullMath.Abs(null);   // null
double? alreadyPos = NullMath.Abs(10);   // 10
```

### Common use case

Calculate absolute differences in custom indicators:

```csharp
public double? CalculateAbsChange(double? current, double? previous)
{
  if (current == null || previous == null)
    return null;
    
  return NullMath.Abs(current - previous);
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
    double? previous = i > 0 ? results[i - 1].Value : null;
    double? current = (double?)quotesList[i].Close;
    
    // NullMath handles nulls automatically
    var change = NullMath.Abs(current - previous);
    
    results.Add(new MyResult {
      Timestamp = quotesList[i].Timestamp,
      Value = current,
      AbsChange = change
    });
  }
  
  return results;
}
```

## Performance considerations

::: tip Performance
`NullMath` methods are lightweight wrappers with minimal overhead. The null-checking cost is negligible compared to the cost of explicit null checks throughout your code.
:::

---

## DeMath

`DeMath` provides deterministic math operations that produce identical results across .NET platforms and operating systems. It is used internally throughout the library to eliminate platform-specific floating-point drift that would otherwise cause indicator values to differ slightly between Windows, Linux, and macOS.

::: info Internal API
`DeMath` is an `internal` class and is not directly accessible from external code. It is documented here for contributors and to explain the library's cross-platform precision strategy.
:::

### Why DeMath?

The standard `System.Math` library delegates some operations (`Log`, `Exp`, `Atan`, etc.) to native platform implementations that can produce slightly different results on different operating systems. For financial indicators — especially those that compound results over hundreds of periods — these tiny differences accumulate into observable divergence.

`DeMath` reimplements these operations using IEEE 754-deterministic algorithms, ensuring that any indicator calculated on Windows produces the exact same bit-for-bit result on Linux or macOS.

### Available methods

| Method | Description |
| ------ | ----------- |
| `DeMath.Log(x)` | Natural logarithm — deterministic cross-platform implementation |
| `DeMath.Log10(x)` | Base-10 logarithm |
| `DeMath.Exp(x)` | Exponential function |
| `DeMath.Atan(x)` | Arctangent |
| `DeMath.Atanh(x)` | Inverse hyperbolic tangent |

### Contributing custom indicators

If you are contributing a custom indicator that uses transcendental functions (`log`, `exp`, `atan`, etc.) and cross-platform reproducibility matters for your indicator, prefer `DeMath` over `System.Math` for those operations. See the [Customization guide](/guide/customization) for the full custom indicator pattern.

## Related utilities

- [Helper utilities overview](/utilities/helpers/)
- [Numerical methods](/utilities/helpers/numerical-methods) - Slope and standard deviation
- [Customization guide](/guide/customization) - Build custom indicators
