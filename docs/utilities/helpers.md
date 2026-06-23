---
title: Additional utilities
description: Numerical methods and math helpers (NullMath, DeMath) for custom indicator development.
---

# Additional utilities

Numerical and math helpers for building [custom indicators](/guide/customization). These are the same internal tools the library's built-in indicators use.

## Numerical methods

The static `Numerical` class exposes slope and standard-deviation helpers.

### Slope

`Numerical.Slope(x, y)` returns the slope of the linear regression (best-fit) line through paired `x` and `y` values, using the least-squares method. The arrays must be the same length, or an `ArgumentException` is thrown.

```csharp
double[] x = [1, 2, 5, 4];
double[] y = [4, 7, 8, 1];

double slope = Numerical.Slope(x, y);  // slope of the best-fit line
```

### Standard deviation

`StdDev()` is an extension on `double[]` that returns the **population** standard deviation of the values (it divides by `n`, not `n−1`).

```csharp
double[] values = [1, 2, 3, 4];

double sd = values.StdDev();
```

## NullMath

Null-safe mathematical operations that handle null values gracefully. `System.Math` does not accept `null`, so these wrappers let you skip the repetitive defensive null-checking — they simply return `null` (or `NaN`) when the input is `null`.

| Method | Example | Result |
| ------ | ------- | ------ |
| `Abs(double?)` | `NullMath.Abs(-25.1)`<br>`NullMath.Abs(null)` | `25.1`<br>`null` |
| `Null2NaN(double?)` | `((double?)null).Null2NaN()` | `double.NaN` |
| `Null2NaN(decimal?)` | `((decimal?)1.5m).Null2NaN()` | `1.5` (as `double`) |
| `NaN2Null(double?)`<br>`NaN2Null(double)` | `double.NaN.NaN2Null()` | `null` |

```csharp
double? x = NullMath.Abs(-25.1);        // x → 25.1
double  n = ((double?)null).Null2NaN(); // n → NaN
double? v = double.NaN.NaN2Null();      // v → null
```

`Null2NaN` is useful when interfacing with systems — such as charting libraries — that expect `NaN` instead of `null`; `NaN2Null` does the reverse when ingesting external data. Both have overloads for the relevant `double`/`decimal` types.

## DeMath

`DeMath` provides deterministic math operations that produce identical, bit-for-bit results across .NET platforms and operating systems. It eliminates platform-specific floating-point drift that would otherwise cause indicator values to differ slightly between Windows, Linux, and macOS.

::: info Internal API
`DeMath` is an `internal` class and is not directly accessible from external code. It is documented here for contributors and to explain the library's cross-platform precision strategy.
:::

`System.Math` delegates some operations (`Log`, `Exp`, `Atan`, etc.) to native platform implementations that can produce slightly different results on different operating systems. For financial indicators — especially those that compound results over hundreds of periods — these tiny differences accumulate into observable divergence. `DeMath` reimplements them with deterministic algorithms so the same calculation yields the same result everywhere.

| Method | Description |
| ------ | ----------- |
| `Log(x)` | Natural logarithm |
| `Log10(x)` | Base-10 logarithm |
| `Exp(x)` | Exponential function (`e^x`) |
| `Atan(x)` | Arctangent |
| `Atan2(y, x)` | Two-argument arctangent (matches `Math.Atan2` quadrant conventions) |
| `Atanh(x)` | Inverse hyperbolic tangent |

If you are contributing a custom indicator that uses transcendental functions and cross-platform reproducibility matters, prefer `DeMath` over `System.Math` for those operations. See the [Customization guide](/guide/customization) for the full custom indicator pattern.

## See also

- [Bar utilities](/utilities/bars) — prepare and transform price bars
- [Result utilities](/utilities/results) — work with indicator results
- [Indicator catalog](/utilities/catalog) — discover indicator metadata programmatically
- [Slope indicator](/indicators/slope) and [Standard deviation indicator](/indicators/std-dev) — built-in equivalents
