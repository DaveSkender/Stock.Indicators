---
layout: docs
title: Helper utilities
description: Numerical analysis utilities for creating custom indicators.
nav:
  parent: utilities
  title: Helpers
---

# Helper utilities

Numerical analysis tools and utilities for creating custom indicators. These are the same internal tools used by the library's built-in indicators.

<div class="utility-cards">

## [Numerical methods](/utilities/helpers/numerical-methods)

Mathematical analysis functions including slope calculation and standard deviation.

```csharp
double slope = Numerical.Slope(xValues, yValues);
double sd = values.StdDev();
```

[See more →](/utilities/helpers/numerical-methods)

## [Math helpers](/utilities/helpers/nullmath)

`NullMath` for null-safe operations and `DeMath` for deterministic cross-platform precision math.

```csharp
double? abs = NullMath.Abs(value);
double? cleaned = value.NaN2Null();
```

[See more →](/utilities/helpers/nullmath)

</div>
