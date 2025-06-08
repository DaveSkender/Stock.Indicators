namespace Performance;

// BUFFER-STYLE INCREMENTING INDICATORS

[ShortRunJob]
public class BufferLists
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
    public EmaList EmaBuffer()
        => new(n) { quotes };

    [Benchmark]
    public IReadOnlyList<EmaResult> EmaSeries()
        => quotes.ToEma(n);

    [Benchmark]
    public IReadOnlyList<EmaResult> EmaStream()
        => provider.ToEma(n).Results;
}
