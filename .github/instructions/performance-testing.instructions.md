---
applyTo: "tools/performance/**/*.cs,tools/performance/**/*.ps1,tools/performance/**/*.md"
description: "Performance testing, benchmarking, and regression detection guidelines"
---

# Performance testing and benchmarking guidelines

These instructions apply to performance testing, benchmarking, and regression detection for the Stock Indicators library.

## Overview

The library uses [BenchmarkDotNet](https://benchmarkdotnet.org/) for comprehensive performance testing. Performance tests are located in `tools/performance/` and cover:

- **Series-style indicators** - Traditional batch processing
- **Stream-style indicators** - Real-time streaming with QuoteHub
- **Buffer-style indicators** - Incremental processing with BufferList
- **Style comparisons** - Performance comparison across different styles
- **Utility functions** - Core library utilities

## File naming conventions

Performance test files follow these patterns:

- `Perf.Series.cs` - Series-style indicator benchmarks
- `Perf.Stream.cs` - Stream-style indicator benchmarks
- `Perf.Buffer.cs` - Buffer-style indicator benchmarks
- `Perf.StyleComparison.cs` - Cross-style performance comparisons
- `Perf.Utility.cs` - Utility function benchmarks

## Running benchmarks

### Basic commands

```bash
cd tools/performance

# Run all benchmarks (~15-20 minutes)
dotnet run -c Release

# Run specific category
dotnet run -c Release --filter *Stream*
dotnet run -c Release --filter *Buffer*
dotnet run -c Release --filter *Series*

# Run specific indicator
dotnet run -c Release --filter *.EmaHub
```

### Important notes

- **Always build in Release mode** - Debug mode results are not representative
- **Close other applications** - Reduce system load for consistent results
- **Run multiple times** - Compare trends to identify outliers
- **Use filters** - Test specific areas during development

## Adding new benchmarks

### Pattern for series indicators

```csharp
[Benchmark]
public object ToMyIndicator() => quotes.ToMyIndicator(14);
```

### Pattern for stream indicators

```csharp
[Benchmark]
public object MyIndicatorHub() => quoteHub.ToMyIndicator(14).Results;
```

### Pattern for buffer indicators

```csharp
[Benchmark]
public MyIndicatorList MyIndicatorList()
    => new(14) { quotes };
```

### Pattern for style comparisons

When adding streaming/buffer support to existing indicators:

```csharp
[Benchmark]
public IReadOnlyList<MyResult> MyIndicatorSeries()
    => quotes.ToMyIndicator(14);

[Benchmark]
public MyIndicatorList MyIndicatorList()
    => new(14) { quotes };

[Benchmark]
public IReadOnlyList<MyResult> MyIndicatorStream()
    => quoteHub.ToMyIndicator(14).Results;
```

## Benchmark configuration

The `BenchmarkConfig.cs` file configures:

- **Exporters**: GitHub markdown and JSON for analysis
- **Columns**: Mean, Error, StdDev
- **Logger**: Console output

Do not modify this configuration without consulting the maintainers.

## Performance regression detection

### Creating baselines

After running benchmarks, create a baseline for future comparison:

```bash
# Copy JSON results to baselines directory
cp BenchmarkDotNet.Artifacts/results/Performance.*-report-full.json \
   baselines/baseline-v3.0.0.json
```

### Detecting regressions

Use the PowerShell script to compare results:

```bash
# Auto-detect latest baseline and results
pwsh detect-regressions.ps1

# Custom threshold (default is 10%)
pwsh detect-regressions.ps1 -ThresholdPercent 15

# Specific files
pwsh detect-regressions.ps1 \
  -BaselineFile baselines/baseline-v3.0.0.json \
  -CurrentFile BenchmarkDotNet.Artifacts/results/current.json
```

### Regression analysis

The script identifies:

- **Regressions** - Performance degradations exceeding threshold
- **Improvements** - Performance gains exceeding threshold
- **Stable** - Changes within threshold (not reported)

Exit codes:

- `0` - No regressions detected
- `1` - Regressions found (can fail CI/CD builds)

## Performance expectations

Typical execution times for 502 periods of historical data:

- **Fast indicators** (<30μs): SMA, EMA, WMA, RSI
- **Medium indicators** (30-60μs): MACD, Bollinger Bands, ATR
- **Complex indicators** (60-100μs): HMA, ADX, Stochastic
- **Advanced indicators** (100-200μs+): Ichimoku, Hurst

### Style performance characteristics

**Series style**:

- Best for: One-time calculations on complete datasets
- Optimized for: Throughput and memory efficiency
- Use case: Historical analysis, backtesting

**Stream style**:

- Best for: Live data feeds, WebSocket integration
- Optimized for: Low latency per quote
- Use case: Trading applications, live dashboards

**Buffer style**:

- Best for: Growing datasets with frequent appends
- Optimized for: Balance between memory and performance
- Use case: Accumulating historical data, hybrid scenarios

## Best practices

### During development

1. **Profile before optimizing** - Use benchmarks to identify bottlenecks
2. **Test incrementally** - Run specific benchmarks during development
3. **Compare styles** - Add style comparison benchmarks when implementing new styles
4. **Document changes** - Note significant performance improvements in PR descriptions

### Optimization guidelines

- Minimize allocations in hot paths
- Avoid excessive LINQ operations
- Use `Span<T>` for zero-copy operations where appropriate
- Cache calculations when possible
- Test with realistic data sizes (502 periods is standard)

### Common pitfalls

- Excessive LINQ usage in hot paths
- Boxing/unboxing of value types
- Unnecessary string allocations
- Redundant calculations in loops
- Poor cache locality

## CI/CD integration

The GitHub Actions workflow (`test-performance.yml`):

1. Runs on pushes to main/version branches
2. Runs on PR changes to performance tests
3. Builds in Release configuration
4. Executes full benchmark suite
5. Uploads artifacts
6. Publishes GitHub Actions summaries

### Optional regression gates

To fail builds on regressions, add to workflow:

```yaml
- name: Detect regressions
  working-directory: tools/performance
  run: pwsh detect-regressions.ps1 -ThresholdPercent 10
```

## Troubleshooting

### Long execution times

- Use filters to test specific areas: `--filter *Ema*`
- Run categories separately instead of full suite

### Inconsistent results

- Ensure Release configuration: `-c Release`
- Close unnecessary applications
- Run multiple times and compare trends
- Check system load (CPU/memory usage)

### Missing result files

Check that benchmarks completed successfully:

```bash
ls -la BenchmarkDotNet.Artifacts/results/
```

## Additional resources

- [BenchmarkDotNet Documentation](https://benchmarkdotnet.org/)
- [Performance Best Practices for .NET](https://docs.microsoft.com/en-us/dotnet/core/performance/)
- [Repository Performance Page](https://dotnet.stockindicators.dev/performance/)
- [Comprehensive Guide](benchmarking.md) - Detailed benchmarking documentation

---

Last updated: October 8, 2025
