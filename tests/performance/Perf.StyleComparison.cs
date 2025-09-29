namespace Performance;

// STYLE COMPARISON BENCHMARKS
// Compares performance between different indicator styles

[ShortRunJob]
public class StyleComparison
{
    private static readonly IReadOnlyList<Quote> quotes
       = Data.GetDefault();

    private readonly QuoteHub<Quote> provider = new();

    private const int n = 14;

    [GlobalSetup]
    public void Setup() => provider.Add(quotes);

    [GlobalCleanup]
    public void Cleanup()
    {
        provider.EndTransmission();
        provider.Cache.Clear();
    }

    // EMA comparison across styles
    [Benchmark(Baseline = true)]
    public IReadOnlyList<EmaResult> EmaSeries()
        => quotes.ToEma(n);

    [Benchmark]
    public EmaList EmaBuffer()
        => new(n) { quotes };

    [Benchmark]
    public IReadOnlyList<EmaResult> EmaStream()
        => provider.ToEma(n).Results;

    // ADX comparison (Series vs Buffer only)
    [Benchmark]
    public IReadOnlyList<AdxResult> AdxSeries()
        => quotes.ToAdx(n);

    [Benchmark]
    public AdxList AdxBuffer()
        => new(n) { quotes };

    // SMA comparison (Series vs Stream only)  
    [Benchmark]
    public IReadOnlyList<SmaResult> SmaSeries()
        => quotes.ToSma(n);

    [Benchmark]
    public IReadOnlyList<SmaResult> SmaStream()
        => provider.ToSma(n).Results;
}