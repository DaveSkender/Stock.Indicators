---
title: Helper utilities
description: Numerical analysis utilities for creating custom indicators.
---

# Helper utilities

This library also includes several tools that we use internally to calculate indicator algorithms. These can be useful if you are creating your own [custom indicators](/customization).

## Numerical methods

<!-- markdownlint-disable MD060 -->
| method             | example usage |
|--------------------|---------------|
| Slope              | `double[] xValues = { 1, 2, 5, 4 };`<br>`double[] yValues = { 4, 7, 8, 1 };`<br>`double slope = Numerix.Slope(xValues, yValues);` |
| Standard deviation | `double[] values = { 1, 2, 3, 4 };`<br>`double sd = values.StdDev();` |
<!-- markdownlint-enable MD060 -->

## NullMath

Most `NullMath` methods work exactly like the `System.Math` library in C#, except these return `null` if a `null` is provided. The `System.Math` library infamously does not allow `null` values, so you'd always need to apply defensive code. This class does that defensive `null` handling for you.

<!-- markdownlint-disable MD060 -->
| method   | example usage |
|----------|---------------|
| Abs      | `var abs = NullMath.Abs(-25)` » `25`<br>`var abs = NullMath.Abs(null)` » `null` |
| Round    | `var rnd = NullMath.Round(1.234, 1)` » `1.2`<br>`var rnd = NullMath.Round(null, 1)` » `null` |
| Null2NaN | `var val = null;`<br>`var n2n = val.Null2NaN()` » `[NaN]` |
| NaN2Null | `var val = double.NaN;`<br>`var n2n = val.NaN2Null()` » `null` |
<!-- markdownlint-enable MD060 -->

---
Last updated: January 7, 2026
