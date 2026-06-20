---
title: Helper utilities
description: Numerical analysis utilities for creating custom indicators.
---

# Helper utilities

Numerical analysis tools and utilities for creating custom indicators. These are the same internal tools used by the library's built-in indicators.

## Numerical methods

Mathematical analysis functions including slope calculation and standard deviation. [More info →](/utilities/helpers/numerical-methods)

```csharp
double slope = Numerical.Slope(xValues, yValues);
double sd = values.StdDev();
```

## Math helpers

`NullMath` for null-safe operations and `DeMath` for deterministic cross-platform precision math. [More info →](/utilities/helpers/nullmath)

```csharp
double? abs = NullMath.Abs(value);
double? cleaned = value.NaN2Null();
```
