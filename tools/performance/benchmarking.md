# Performance Benchmarking Guide

This document describes the performance testing infrastructure for Stock Indicators for .NET, including how to run benchmarks, interpret results, and detect performance regressions.

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
dotnet run -c Release --filter *.ToEma
```

### Run individual benchmarks

```bash
# Single method
dotnet run -c Release --filter *.EmaHub
```

## Understanding results

### Result artifacts

BenchmarkDotNet generates multiple output formats in `BenchmarkDotNet.Artifacts/results/`:

- `*.md` - GitHub-flavored markdown tables
- `*.json` - Detailed JSON data for analysis

### Key metrics

- **Mean** - Average execution time (most important for typical usage)
- **Error** - Standard error of the mean
- **StdDev** - Standard deviation (variability indicator)

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

**Stream style** (real-time):

- Best for: Live data feeds, WebSocket integration
- Optimized for: Low latency per quote
- Typical use: Trading applications, live dashboards

**Buffer style** (incremental):

- Best for: Growing datasets with frequent appends
- Optimized for: Balance between memory and performance
- Typical use: Accumulating historical data, hybrid scenarios

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

### GitHub Actions workflow

The `.github/workflows/test-performance.yml` workflow:

1. Runs on pushes to main/version branches
2. Runs on PR changes to performance tests
3. Generates GitHub Summary with results
4. Uploads artifacts for historical tracking

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

- [BenchmarkDotNet Documentation](https://benchmarkdotnet.org/)
- [Performance Best Practices for .NET](https://learn.microsoft.com/dotnet/core/performance/)
- [Repository Performance Page](https://dotnet.stockindicators.dev/performance/)

---

Last updated: October 1, 2025
