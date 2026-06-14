---
title: Numerical methods
description: Mathematical analysis functions for slope calculation and standard deviation.
---

# Numerical methods

Mathematical analysis functions including slope calculation and standard deviation. These are the same internal tools used by the library's built-in indicators.

## Slope

Calculate the slope of a linear regression line from paired x and y values.

### Syntax

```csharp
double slope = Numerix.Slope(double[] xValues, double[] yValues);
```

### Parameters

| param | type | description |
| ----- | ---- | ----------- |
| `xValues` | double[] | Array of x-axis values (independent variable) |
| `yValues` | double[] | Array of y-axis values (dependent variable) |

### Returns

```csharp
double
```

The slope of the best-fit line through the data points.

### Usage

```csharp
double[] xValues = { 1, 2, 5, 4 };
double[] yValues = { 4, 7, 8, 1 };
double slope = Numerix.Slope(xValues, yValues);

Console.WriteLine($"Slope: {slope:F4}");
```

## Standard deviation

Calculate the standard deviation of a collection of values.

### Syntax

```csharp
double sd = values.StdDev();
```

### Parameters

| param | type | description |
| ----- | ---- | ----------- |
| `values` | IEnumerable\<double\> | An array or enumerable collection of double values |

### Returns

```csharp
double
```

The standard deviation of the values.

### Usage

```csharp
double[] values = { 1, 2, 3, 4 };
double sd = values.StdDev();

Console.WriteLine($"Standard Deviation: {sd:F4}");
```

## Performance considerations

::: tip Performance
Both `Slope()` and `StdDev()` are optimized for performance and are used internally by the library's built-in indicators. However, for large datasets or frequent calculations, consider caching results when possible.
:::

## Related utilities

- [Helper utilities overview](/utilities/helpers/)
- [Math helpers](/utilities/helpers/nullmath) - NullMath and DeMath utilities
- [Customization guide](/guide/customization) - Build custom indicators
- [Slope indicator](/indicators/Slope) - Built-in linear regression indicator
- [Standard Deviation indicator](/indicators/StdDev) - Built-in standard deviation indicator
