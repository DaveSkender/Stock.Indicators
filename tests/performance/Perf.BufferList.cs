namespace Performance;

// BUFFER-STYLE INCREMENTING INDICATORS

[ShortRunJob]
public class BufferListIndicators
{
    private static readonly IReadOnlyList<Quote> quotes
       = Data.GetDefault();

    private static readonly QuoteHub<Quote> provider = new();

    private const int n = 14;

    [GlobalSetup]
    public void Setup() => provider.Add(quotes);

    [GlobalCleanup]
    public void Cleanup()
    {
        provider.EndTransmission();
        provider.Cache.Clear();
    }

    [Benchmark]
    public AdxList AdxBuffer()
        => new(n) { quotes };

    [Benchmark]
    public IReadOnlyList<AdxResult> AdxSeries()
        => quotes.ToAdx(n);

    [Benchmark]
    public AlmaList AlmaBuffer()
        => new(n, 0.85, 6) { quotes };

    [Benchmark]
    public IReadOnlyList<AlmaResult> AlmaSeries()
        => quotes.ToAlma(n, 0.85, 6);

    [Benchmark]
    public IReadOnlyList<AlmaResult> AlmaStream()
        => provider.ToAlma(n, 0.85, 6).Results;

    [Benchmark]
    public EmaList EmaBuffer()
        => new(n) { quotes };

    [Benchmark]
    public IReadOnlyList<EmaResult> EmaSeries()
        => quotes.ToEma(n);

    [Benchmark]
    public IReadOnlyList<EmaResult> EmaStream()
        => provider.ToEma(n).Results;
}
