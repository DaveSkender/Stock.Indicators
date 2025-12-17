# QuoteX Research: Internal Long Storage Experiment

## Overview

This document summarizes the experimental implementation of `QuoteX`, a Quote variant using internal `long` storage for OHLCV properties and DateTime, as an alternative to the standard `decimal`-based `Quote` class.

## Implementation Details

### QuoteX Class Structure

- **Location**: `src/_common/Quotes/Quote.Models.cs`
- **Storage**: Internal `long` fields for all numeric data
- **Conversion**: Uses `decimal.ToOACurrency()` / `decimal.FromOACurrency()` for price/volume
- **DateTime**: Stored as `long` (Ticks) with separate `DateTimeKind` field
- **Public Interface**: Identical to `Quote` - implements `IQuote` with decimal properties
- **Internal Accessors**: Provides `*Long` properties for accessing raw long values

### MacdX Implementation

- **Location**: `src/m-r/Macd/Macd.StaticSeriesX.cs`
- **Approach**: Experimental MACD calculation using QuoteX
- **Optimization**: Uses long arithmetic in SMA summation helper
- **Output**: Returns standard `MacdResult` for compatibility

### Benchmarks

Two benchmark suites were created to measure performance:

1. **QuoteSizeComparison** (`tools/performance/Perf.QuoteSizeComparison.cs`)
   - Measures creation time and memory allocation
   - Compares Quote, QuoteD, and QuoteX

2. **MacdQuoteComparison** (`tools/performance/Perf.MacdQuoteComparison.cs`)
   - Measures MACD calculation performance
   - Compares all three quote variants with identical parameters

## Benchmark Results

### Test Environment

- **Platform**: Linux Ubuntu 24.04.3 LTS (Noble Numbat)
- **CPU**: AMD EPYC 7763 2.45GHz, 1 CPU, 4 logical and 2 physical cores
- **Runtime**: .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3
- **Data Size**: 502 quotes (typical test data)

### Quote Creation Performance

| Method           | Mean      | Allocated Memory | Relative Performance |
|------------------|-----------|------------------|----------------------|
| CreateQuoteList  |  9.21 µs  | 56,280 bytes     | Baseline (1.00x)     |
| CreateQuoteDList | 14.47 µs  | 36,256 bytes     | 1.57x slower         |
| CreateQuoteXList | 17.13 µs  | 40,312 bytes     | 1.86x slower         |

### MACD Calculation Performance

| Method         | Mean     | Allocated Memory | Relative Performance |
|----------------|----------|------------------|----------------------|
| MacdWithQuote  | 16.09 µs | 56,280 bytes     | Baseline (1.00x)     |
| MacdWithQuoteX | 18.66 µs | 56,280 bytes     | 1.16x slower         |
| MacdWithQuoteD | 46.16 µs | 112,632 bytes    | 2.87x slower         |

## Analysis

### Memory Footprint

**Theoretical per-object sizes** (excluding object header and padding):

- `Quote`: ~88 bytes (DateTime: 8 + 5 × decimal: 80)
- `QuoteD`: ~48 bytes (DateTime: 8 + 5 × double: 40)
- `QuoteX`: ~52 bytes (long: 8 + DateTimeKind: 4 + 5 × long: 40)

**Actual allocation measurements** (502 quotes):

- `Quote`: 56,280 bytes (~112 bytes per quote with overhead)
- `QuoteD`: 36,256 bytes (~72 bytes per quote with overhead)
- `QuoteX`: 40,312 bytes (~80 bytes per quote with overhead)

**Key Finding**: QuoteX achieves ~28% memory reduction compared to Quote, but QuoteD is even more efficient at ~36% reduction.

### Performance Characteristics

**Creation Overhead**:

- QuoteX is 86% slower to create than Quote
- The `ToOACurrency()` / `FromOACurrency()` conversion adds significant overhead
- Each property access requires decimal conversion from long

**Calculation Performance**:

- QuoteX MACD calculation is 16% slower than Quote
- Limited benefit from long arithmetic due to frequent conversions
- The experimental SMA helper using long summation doesn't offset conversion costs

**QuoteD Performance**:

- QuoteD creation is 57% slower (includes decimal to double conversion)
- MACD with QuoteD is 187% slower due to additional Quote conversion layer
- Not a fair comparison as it includes double conversion overhead

## Conclusions

### Pros of QuoteX

- ✅ Achieves 28% memory reduction per quote object
- ✅ Maintains identical public API to Quote (seamless integration)
- ✅ Type-safe internal long storage
- ✅ Suitable for scenarios where memory is critical

### Cons of QuoteX

- ❌ 86% slower creation time
- ❌ 16% slower calculations
- ❌ Increased code complexity
- ❌ Conversion overhead on every property access
- ❌ Limited benefit from long arithmetic in practice

### Verdict

**Not recommended for general use.** The memory savings (28%) do not justify the performance penalties:

- Creation: 86% slower
- Calculations: 16% slower

The standard `Quote` class using `decimal` provides the best balance of:

- Performance (fastest creation and calculations)
- Precision (appropriate for financial data)
- Simplicity (no conversion overhead)

### When QuoteX Might Be Considered

QuoteX could be viable only in very specific scenarios:

1. Extremely memory-constrained environments with millions of quotes in memory
2. Applications that load quotes once and perform minimal calculations
3. Scenarios where memory pressure causes more performance impact than CPU overhead

For 99% of use cases, stick with the standard `Quote` class.

## References

- Standard Quote: `src/_common/Quotes/Quote.Models.cs`
- QuoteX Implementation: `src/_common/Quotes/Quote.Models.cs`
- QuoteX Converters: `src/_common/Quotes/Quote.Converters.cs`
- MacdX Implementation: `src/m-r/Macd/Macd.StaticSeriesX.cs`
- Size Benchmarks: `tools/performance/Perf.QuoteSizeComparison.cs`
- MACD Benchmarks: `tools/performance/Perf.MacdQuoteComparison.cs`
- Benchmark Results: `tools/performance/BenchmarkDotNet.Artifacts/results/`

---
Created: December 17, 2024  
Author: GitHub Copilot (research experiment)
