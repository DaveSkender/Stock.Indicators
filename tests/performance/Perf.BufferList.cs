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
    public IReadOnlyList<AdxResult> AdxStream()
        => provider.ToAdx(n).Results;

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

    [Benchmark]
    public AdlList AdlBuffer()
        => new() { quotes };

    [Benchmark]
    public IReadOnlyList<AdlResult> AdlSeries()
        => quotes.ToAdl();

    [Benchmark]
    public IReadOnlyList<AdlResult> AdlStream()
        => provider.ToAdl().Results;

    [Benchmark]
    public AtrList AtrBuffer()
        => new(n) { quotes };

    [Benchmark]
    public IReadOnlyList<AtrResult> AtrSeries()
        => quotes.ToAtr(n);

    [Benchmark]
    public IReadOnlyList<AtrResult> AtrStream()
        => provider.ToAtr(n).Results;
}
