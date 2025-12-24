# Performance Tuning: Deterministic Math (DeMath)

This document tracks performance analysis and optimization attempts for the `DeMath` class and affected indicators.

## Background

The `DeMath` class provides deterministic math functions (`Log`, `Log10`, `Atanh`, `Atan`, `Atan2`) to ensure consistent results across platforms. These functions are used by:

- **Fisher Transform**: Uses `DeMath.Atanh()` (calls Log twice internally)
- **MAMA**: Uses `DeMath.Atan()` (2 calls per calculation)
- **HT Trendline**: Uses `DeMath.Atan()` (1 call per calculation)
- **Hurst**: Uses `DeMath.Log10()` (calls Log internally)

## Baseline Performance

| Indicator | Style | Mean | StdDev |
| --------- | ----- | ---- | ------ |
| ToFisherTransform | Series | 70.65 µs | 0.28 µs |
| ToMama | Series | 180.6 µs | 1.13 µs |
| ToHtTrendline | Series | 119.7 µs | 1.50 µs |
| ToHurst | Series | 1,119 µs | 1.7 µs |
| FisherTransformHub | Stream | 187.1 µs | 5.25 µs |
| MamaHub | Stream | 324.9 µs | 1.44 µs |
| FisherTransformList | Buffer | 62.2 µs | 0.32 µs |

## Optimization Attempts

### Attempt 1: Loop Unrolling in Log()

**Approach**: Unroll the 20-iteration series loop in `Log()` to eliminate loop overhead.

**Result**: No measurable improvement. The JIT compiler already optimizes the tight loop effectively.

### Attempt 2: Pre-computed Reciprocals

**Approach**: Replace division by odd integers (`term / denominator`) with multiplication by pre-computed reciprocals (`term * reciprocal`).

**Result**: REJECTED - Produces different floating-point results due to different rounding in pre-computed vs runtime-computed reciprocals, breaking determinism.

### Attempt 3: Single Log Call in Atanh()

**Approach**: Replace two Log calls `0.5 * (Log(1+x) - Log(1-x))` with single Log call `0.5 * Log((1+x)/(1-x))`.

**Result**: REJECTED - Produces different floating-point results due to different intermediate rounding, breaking determinism.

### Attempt 4: Reduced CORDIC Iterations

**Approach**: Reduce AtanTable from 32 to 25 iterations (sufficient for double precision).

**Result**: REJECTED - Produces different results for some edge cases.

### Attempt 5: AggressiveInlining Hints

**Approach**: Add `[MethodImpl(MethodImplOptions.AggressiveInlining)]` to frequently called methods.

**Result**: Minimal impact (within measurement noise). The JIT already inlines small methods appropriately.

## Conclusions

The DeMath implementation is fundamentally constrained by the requirement for **bit-exact determinism**. The current implementation represents an optimal balance between:

1. **Determinism**: All calculations produce identical results across platforms
2. **Performance**: Uses efficient algorithms (mantissa extraction for Log, CORDIC for Atan)
3. **Accuracy**: Sufficient precision for double-precision floating-point

### Performance Overhead

The DeMath functions are inherently slower than native `Math` functions because:

- `Math.Log` uses hardware-optimized instructions (potentially SIMD/vectorized)
- `DeMath.Log` uses a software series expansion (20 iterations)
- `Math.Atan` uses hardware atan instruction
- `DeMath.Atan` uses CORDIC algorithm (32 iterations)

This overhead is the cost of determinism and cannot be eliminated without sacrificing cross-platform consistency.

### Recommendations

1. **Accept the performance characteristics**: The current implementation is optimal given the constraints.
2. **Consider caching**: If the same value is computed multiple times, cache the result.
3. **Profile specific use cases**: Focus optimization efforts on the indicator algorithms themselves, not the math primitives.

---
Last updated: December 24, 2025
