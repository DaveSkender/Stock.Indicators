# Benchmark patterns

## File organization

| File | Purpose |
| ---- | ------- |
| `Perf.Series.cs` | Series-style benchmarks |
| `Perf.Stream.cs` | Stream-style benchmarks |
| `Perf.Buffer.cs` | Buffer-style benchmarks |
| `Perf.StyleComparison.cs` | Cross-style comparisons |
| `Perf.Utility.cs` | Utility function benchmarks |

## Benchmark class structure

```csharp
[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net80)]
public class SeriesIndicators
{
    private static readonly IReadOnlyList<Quote> quotes = Data.GetDefault();

    [Benchmark]
    public void ToEma() => quotes.ToEma(14);

    [Benchmark]
    public void ToSma() => quotes.ToSma(20);

    [Benchmark]
    public void ToRsi() => quotes.ToRsi(14);
}
```

## Stream benchmark with hub setup

```csharp
[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net80)]
public class StreamIndicators
{
    private static readonly IReadOnlyList<Quote> quotes = Data.GetDefault();
    private static readonly QuoteHub quoteHub = new();

    [GlobalSetup]
    public void Setup()
    {
        foreach (Quote quote in quotes)
            quoteHub.Add(quote);
    }

    [Benchmark]
    public object EmaHub() => quoteHub.ToEmaHub(14).Results;

    [Benchmark]
    public object SmaHub() => quoteHub.ToSmaHub(20).Results;
}
```

## Buffer benchmark with collection initializer

```csharp
[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net80)]
public class BufferIndicators
{
    private static readonly IReadOnlyList<Quote> quotes = Data.GetDefault();

    [Benchmark]
    public EmaList EmaList() => new(14) { quotes };

    [Benchmark]
    public SmaList SmaList() => new(20) { quotes };
}
```

## Style comparison benchmark

```csharp
[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net80)]
public class EmaStyleComparison
{
    private const int LookbackPeriods = 14;
    private static readonly IReadOnlyList<Quote> quotes = Data.GetDefault();
    private static readonly QuoteHub quoteHub = new();

    [GlobalSetup]
    public void Setup()
    {
        foreach (Quote quote in quotes)
            quoteHub.Add(quote);
    }

    [Benchmark(Baseline = true)]
    public IReadOnlyList<EmaResult> Series() => quotes.ToEma(LookbackPeriods);

    [Benchmark]
    public IReadOnlyList<EmaResult> Buffer() => new EmaList(LookbackPeriods) { quotes };

    [Benchmark]
    public IReadOnlyList<EmaResult> Stream() => quoteHub.ToEmaHub(LookbackPeriods).Results;
}
```

## Running specific benchmarks

```bash
# Single indicator
dotnet run -c Release --filter *.ToEma

# All EMA benchmarks
dotnet run -c Release --filter *Ema*

# Style category
dotnet run -c Release --filter *Stream*

# Multiple indicators
dotnet run -c Release --filter *.ToEma --filter *.ToSma
```

## Interpreting results

```text
|     Method |     Mean |   Error |  StdDev |   Median | Allocated |
|----------- |---------:|--------:|--------:|---------:|----------:|
|     Series |  25.3 μs | 0.50 μs | 0.47 μs |  25.1 μs |   12.8 KB |
|     Buffer |  28.7 μs | 0.57 μs | 0.64 μs |  28.5 μs |   14.2 KB |
|     Stream |  32.1 μs | 0.63 μs | 0.70 μs |  31.9 μs |   16.1 KB |
```

**Ratio analysis**:

- Buffer/Series: 28.7/25.3 = 1.13x (within 1.2x target ✅)
- Stream/Series: 32.1/25.3 = 1.27x (within 1.5x target ✅)

## Memory profiling

Add `[MemoryDiagnoser]` to see allocations:

- **Allocated**: Total bytes allocated per operation
- Target: Minimize allocations in hot paths
- Watch for: Large object heap allocations (> 85KB)

## Regression detection workflow

1. Run baseline benchmarks after stable release
2. Save results to `baselines/` directory
3. Run benchmarks after changes
4. Compare with `detect-regressions.ps1`
5. Investigate any > 10% regressions

---
Last updated: December 31, 2025
