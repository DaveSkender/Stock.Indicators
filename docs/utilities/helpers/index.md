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
double slope = Numerix.Slope(xValues, yValues);
double sd = values.StdDev();
```

[See more →](/utilities/helpers/numerical-methods)

## [NullMath](/utilities/helpers/nullmath)

Null-safe mathematical operations that handle null values gracefully.

```csharp
decimal? abs = NullMath.Abs(value);
decimal? rounded = NullMath.Round(value, 2);
```

[See more →](/utilities/helpers/nullmath)

</div>
