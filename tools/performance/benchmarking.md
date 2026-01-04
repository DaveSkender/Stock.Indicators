# Performance Benchmarking Guide

This document describes the performance testing infrastructure for Stock Indicators for .NET, including how to run benchmarks, interpret results, and detect performance regressions.

For comprehensive performance analysis and results, see `PERFORMANCE_ANALYSIS.md`.

## Overview

The Stock Indicators library uses [BenchmarkDotNet](https://benchmarkdotnet.org/) for comprehensive performance testing. Benchmarks cover:

- **Series-style indicators** - Traditional batch processing (Perf.Series.cs)
- **Stream-style indicators** - Real-time streaming with QuoteHub (Perf.Stream.cs)
- **Buffer-style indicators** - Incremental processing with BufferList (Perf.Buffer.cs)
- **Style comparisons** - Performance comparison across different styles (Perf.StyleComparison.cs)
- **Utility functions** - Core library utilities (Perf.Utility.cs)

## Running benchmarks

### Run all benchmarks

```bash
cd tools/performance
dotnet run -c Release
```

This takes approximately 15-20 minutes and produces comprehensive results for all indicators.

### Run specific benchmark categories

```bash
# Stream indicators only
dotnet run -c Release --filter *Stream*

# Buffer indicators only
dotnet run -c Release --filter *Buffer*

# Series indicators only
dotnet run -c Release --filter *Series*

# Style comparison only
dotnet run -c Release --filter *StyleComparison*

# Specific indicator
dotnet run -c Release --filter *.ToEmaBatch
dotnet run -c Release --filter *.ToEmaList
dotnet run -c Release --filter *.ToEmaHub
```

### Run manual performance test with custom data size

Test a specific indicator with a custom number of randomly generated periods:

```bash
# Test SMA with 100,000 periods (direct calls, no catalog overhead)
PERF_TEST_KEYWORD=sma PERF_TEST_PERIODS=100000 dotnet run -c Release --filter "Performance.ManualTestDirect*"

# Test EMA with 500,000 periods (default)
PERF_TEST_KEYWORD=ema PERF_TEST_PERIODS=500000 dotnet run -c Release --filter "Performance.ManualTestDirect*"

# Test RSI with 1,000,000 periods
PERF_TEST_KEYWORD=rsi PERF_TEST_PERIODS=1000000 dotnet run -c Release --filter "Performance.ManualTestDirect*"
```

The direct manual test (`ManualTestDirect`):

- Uses precompiled delegates for common indicators (SMA, EMA, RSI, MACD, etc.)
- Zero catalog/reflection overhead - measures pure indicator performance
- Generates random quotes once before benchmarking (not counted in metrics)
- Runs all three indicator styles (Series, Buffer, Stream) with the same data
- Best for accurate performance measurements and scalability testing

For dynamic indicator discovery (with catalog/reflection overhead), use `Performance.ManualTest*` filter instead. This supports any catalog-registered indicator but includes method lookup and invocation overhead in the measurements.

**Note:** The manual test can also be triggered from GitHub Actions via the "Manual performance test" workflow with custom inputs.

### Run individual benchmarks

```bash
# Single method
dotnet run -c Release --filter *.ToEmaHub
```

### Run style comparison benchmarks

StyleComparison provides comprehensive comparison across all indicators:

```bash
# All indicators, all styles (grouped by indicator)
dotnet run -c Release -- --filter 'Performance.StyleComparison*'

# Specific indicator across all styles
dotnet run -c Release -- --filter '*StyleComparison.Ema*'
```

**Output format**: BenchmarkDotNet groups results by indicator category, showing Series (baseline), Buffer, and Stream implementations with automatic ratio columns. Ratio values show performance relative to Series (1.00 = same speed, 2.00 = 2x slower, 0.50 = 2x faster).

## Understanding results

### Result artifacts

BenchmarkDotNet generates multiple output formats in `BenchmarkDotNet.Artifacts/results/`:

- `*.md` - GitHub-flavored markdown tables
- `*.json` - Detailed JSON data for analysis

### Key metrics

- **Mean** - Average execution time (most important for typical usage)
- **Error** - Standard error of the mean
- **StdDev** - Standard deviation (variability indicator)
- **Ratio** - Performance relative to baseline (only in grouped benchmarks with baseline set)
- **RatioSD** - Standard deviation of the ratio (only in grouped benchmarks)
- **Allocated** - Total bytes allocated per operation
- **Gen0/Gen1/Gen2** - Garbage collection counts per 1,000 operations

**Ratio interpretation** (StyleComparison benchmarks):

- **1.00** - Same speed as Series baseline
- **2.00** - 2x slower than Series (takes twice as long)
- **0.50** - 2x faster than Series (takes half the time)
- Values closer to 1.00 indicate better performance relative to baseline

### Interpreting performance

Typical execution times (for 502 periods of historical data):

- **Fast indicators** (<30μs): SMA, EMA, WMA, RSI
- **Medium indicators** (30-60μs): MACD, Bollinger Bands, ATR
- **Complex indicators** (60-100μs): HMA, ADX, Stochastic
- **Advanced indicators** (100-200μs+): Ichimoku, Hurst, complex oscillators

### Style performance characteristics

**Series style** (batch processing):

- Best for: One-time calculations on complete datasets
- Optimized for: Throughput and memory efficiency
- Typical use: Historical analysis, backtesting

**Buffer style** (incremental):

- Best for: Growing datasets with frequent appends
- Optimized for: Balance between memory and performance
- Typical use: Accumulating historical data, hybrid scenarios

**Stream style** (real-time):

- Best for: Live data feeds, WebSocket integration
- Optimized for: Low latency per quote
- Typical use: Trading applications, live dashboards

## Performance regression detection

### Creating baselines

Establish performance baselines for comparison:

```bash
# After running benchmarks (from repo root)
cp tools/performance/BenchmarkDotNet.Artifacts/results/Performance.*-report-full.json tools/performance/baselines/baseline-v3.0.0.json
```

### Detecting regressions

Compare current results against baseline:

```bash
# Automatic detection with default 10% threshold (from repo root)
pwsh tools/performance/detect-regressions.ps1

# Custom threshold
pwsh tools/performance/detect-regressions.ps1 -ThresholdPercent 15

# Specific files
pwsh tools/performance/detect-regressions.ps1 -BaselineFile tools/performance/baselines/baseline-v3.0.0.json -CurrentFile tools/performance/BenchmarkDotNet.Artifacts/results/current.json
```

### Regression analysis

The script identifies:

- **Regressions** - Performance degradations exceeding threshold
- **Improvements** - Performance gains exceeding threshold
- **Stable** - Changes within threshold (not reported)

## CI/CD integration

### GitHub Actions workflows

The library includes two performance testing workflows:

**Automated testing** (`.github/workflows/test-performance.yml`):

1. Runs on pushes to main/version branches
2. Runs on PR changes to performance tests
3. Generates GitHub Summary with results
4. Uploads artifacts for historical tracking

**Manual testing** (`.github/workflows/test-performance-manual.yml`):

1. Manually triggered via GitHub Actions UI
2. Accepts two inputs:
   - **keyword**: Indicator name (case-insensitive, e.g., "sma", "ema", "rsi")
   - **periods**: Number of randomly generated quote periods (default: 500,000)
3. Runs all three styles (Series, Buffer, Stream) with the same data
4. Ideal for testing scalability and performance with custom dataset sizes
5. Generates GitHub Summary with results

To trigger the manual workflow:

1. Go to **Actions** → **Manual performance test**
2. Click **Run workflow**
3. Enter indicator keyword and desired number of periods
4. Click **Run workflow** button

### Adding regression gates (optional)

To fail builds on regressions, add to workflow:

```yaml
- name: Detect regressions
  working-directory: tools/performance
  run: |
    pwsh detect-regressions.ps1 -ThresholdPercent 10
```

## Performance optimization tips

### For indicator developers

1. **Minimize allocations** - Reuse buffers, avoid unnecessary collections
2. **Use spans** - Leverage `Span<T>` for zero-copy operations where possible
3. **Cache calculations** - Store intermediate results for reuse
4. **Profile before optimizing** - Use BenchmarkDotNet to identify bottlenecks
5. **Test incrementally** - Run specific benchmarks during development

### Common performance pitfalls

- Excessive LINQ usage in hot paths
- Boxing/unboxing of value types
- Unnecessary string allocations
- Redundant calculations in loops
- Poor cache locality

## Benchmark development

### Adding new benchmarks

Follow existing patterns in `Perf.*.cs` files:

```csharp
[Benchmark]
public object MyNewIndicator() => quotes.ToMyIndicator(14);
```

### Benchmark best practices

1. Use `[GlobalSetup]` for one-time initialization
2. Use `[GlobalCleanup]` for resource cleanup
3. Return results to prevent dead code elimination
4. Avoid I/O or external dependencies in benchmarks
5. Use realistic data sizes (502 periods matches standard)

### Style comparison benchmarks

When adding streaming/buffer support to existing indicators, add comparison benchmarks to `Perf.StyleComparison.cs`:

```csharp
[Benchmark]
public IReadOnlyList<MyResult> MySeries()
    => quotes.ToMyIndicator(14);

[Benchmark]
public MyList MyBuffer()
    => new(14) { quotes };

[Benchmark]
public IReadOnlyList<MyResult> MyStream()
    => quoteHub.ToMyIndicator(14).Results;
```

## Troubleshooting

### Long execution times

Benchmarks take 15-20 minutes for full suite. Use filters to test specific areas:

```bash
dotnet run -c Release --filter *Ema*
```

### Out of memory errors

Reduce parallelism or run specific categories separately:

```bash
dotnet run -c Release --filter *Series*
dotnet run -c Release --filter *Stream*
```

### Inconsistent results

- Ensure Release configuration (`-c Release`)
- Close unnecessary applications
- Run multiple times and compare trends
- Check system load (CPU/memory usage)

### Missing result files

Ensure benchmarks completed successfully:

```bash
ls -la BenchmarkDotNet.Artifacts/results/
```

## Performance monitoring

### Tracking trends

1. Run benchmarks regularly (e.g., before each release)
2. Save results with version tags
3. Compare trends over time
4. Document significant changes in release notes

### Historical data

Keep baseline files for major versions:

```text
baselines/
  baseline-v2.6.0.json
  baseline-v3.0.0.json
  baseline-latest.json (-> v3.0.0)
```

## Contributing

When contributing performance improvements:

1. Run baseline before changes
2. Implement optimization
3. Run benchmarks after changes
4. Document the improvement
5. Include before/after metrics in PR

## Resources

- [PERFORMANCE_ANALYSIS.md](PERFORMANCE_ANALYSIS.md) - Comprehensive performance analysis and results
- [BenchmarkDotNet Documentation](https://benchmarkdotnet.org/)
- [Performance Best Practices for .NET](https://learn.microsoft.com/dotnet/core/performance/)
- [Repository Performance Page](https://dotnet.stockindicators.dev/performance/)

---

Last updated: January 3, 2026
