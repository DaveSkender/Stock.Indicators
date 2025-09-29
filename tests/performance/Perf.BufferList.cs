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
    public EmaList EmaBuffer()
        => new(n) { quotes };

    [Benchmark]
    public IReadOnlyList<EmaResult> EmaSeries()
        => quotes.ToEma(n);

    [Benchmark]
    public IReadOnlyList<EmaResult> EmaStream()
        => provider.ToEma(n).Results;

    [Benchmark]
    public HmaList HmaBuffer()
        => new(n) { quotes };

    [Benchmark]
    public IReadOnlyList<HmaResult> HmaSeries()
        => quotes.ToHma(n);

    [Benchmark]
    public IReadOnlyList<HmaResult> HmaStream()
        => provider.ToHma(n).Results;

    [Benchmark]
    public SmaList SmaBuffer()
        => new(n) { quotes };

    [Benchmark]
    public IReadOnlyList<SmaResult> SmaSeries()
        => quotes.ToSma(n);

    [Benchmark]
    public IReadOnlyList<SmaResult> SmaStream()
        => provider.ToSma(n).Results;

    [Benchmark]
    public WmaList WmaBuffer()
        => new(n) { quotes };

    [Benchmark]
    public IReadOnlyList<WmaResult> WmaSeries()
        => quotes.ToWma(n);

    [Benchmark]
    public IReadOnlyList<WmaResult> WmaStream()
        => provider.ToWma(n).Results;

    [Benchmark]
    public TrList TrBuffer()
        => new() { quotes };

    [Benchmark]
    public IReadOnlyList<TrResult> TrSeries()
        => quotes.ToTr();

    [Benchmark]
    public IReadOnlyList<TrResult> TrStream()
        => provider.ToTr().Results;
}
