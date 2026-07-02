---
name: performance-testing
description: Benchmark indicator performance with BenchmarkDotNet. Use for Series/Buffer/Stream benchmarks, regression detection, and optimization patterns. Target 1.5x Series for StreamHub, 1.2x for BufferList.
---

# Performance testing

## Running benchmarks

```bash
cd tools/performance

# Run all benchmarks (can take several hours)
dotnet run -c Release

# Run specific category
dotnet run -c Release -- --filter "*StreamIndicators*"
dotnet run -c Release -- --filter "*BufferIndicators*"
dotnet run -c Release -- --filter "*SeriesIndicators*"

# Run specific indicator
dotnet run -c Release -- --filter "*.EmaHub"

# Fast spot-check with direct harness (large-N friendly)
PERF_TEST_KEYWORD=ema PERF_TEST_PERIODS=500000 dotnet run -c Release -- --filter "Performance.ManualTestDirect*"

# Exercise pruning path (Cap < Periods)
PERF_TEST_KEYWORD=adl PERF_TEST_PERIODS=500000 PERF_TEST_CAP=100000 dotnet run -c Release -- --filter "Performance.ManualTestDirect*"
```

## Adding benchmarks

### Series pattern

```csharp
[Benchmark]
public void ToMyIndicator() => bars.ToMyIndicator(14);
```

### Stream pattern

```csharp
[Benchmark]
public object MyIndicatorHub() => barHub.ToMyIndicatorHub(14).Results;
```

### Buffer pattern

```csharp
[Benchmark]
public MyIndicatorList MyIndicatorList() => new(14) { bars };
```

### Style comparison

```csharp
[Benchmark]
public IReadOnlyList<MyResult> MyIndicatorSeries() => bars.ToMyIndicator(14);

[Benchmark]
public IReadOnlyList<MyResult> MyIndicatorBuffer() => bars.ToMyIndicatorList(14);

[Benchmark]
public IReadOnlyList<MyResult> MyIndicatorStream() => barHub.ToMyIndicator(14).Results;
```

## Performance targets

**Note**: These are optimization goals for future v3.1+ effort. Current implementations vary by indicator family.

| Style | Target vs Series | Use Case |
| ----- | ---------------- | -------- |
| Series | Baseline | Batch processing |
| BufferList | ≤ 1.2x | Incremental data |
| StreamHub | ≤ 1.5x | Real-time feeds |

## Expected execution times (502 periods)

**Note**: These are optimization targets. Actual execution times vary by indicator complexity and current implementation.

| Complexity | Time | Examples |
| ---------- | ---- | -------- |
| Fast | < 30μs | SMA, EMA, WMA, RSI |
| Medium | 30-60μs | MACD, Bollinger Bands, ATR |
| Complex | 60-100μs | HMA, ADX, Stochastic |
| Advanced | 100-200μs+ | Ichimoku, Hurst |

## Regression detection

```bash
# Auto-detect baseline and results
pwsh detect-regressions.ps1

# Custom threshold (default 10%)
pwsh detect-regressions.ps1 -ThresholdPercent 15
```

Exit codes:

- `0` - No regressions
- `1` - Regressions found

## Creating baselines

```bash
# baselines/ keeps machine + human baseline artifacts per benchmark class
cp BenchmarkDotNet.Artifacts/results/Performance.*-report-full.json baselines/
cp BenchmarkDotNet.Artifacts/results/Performance.*-report-github.md baselines/
```

## Required optimization patterns

- Minimize allocations in hot paths
- Avoid LINQ in performance-critical loops
- Use `Span<T>` for zero-copy operations
- Cache calculations when possible
- Test with realistic data sizes (502 periods)

## Prohibited patterns

- Excessive LINQ in hot paths
- Boxing/unboxing of value types
- Unnecessary string allocations
- Redundant calculations in loops
- Poor cache locality

See [references/benchmark-patterns.md](references/benchmark-patterns.md) for detailed patterns.
