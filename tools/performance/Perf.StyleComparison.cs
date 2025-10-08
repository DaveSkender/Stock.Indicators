namespace Performance;

// STYLE COMPARISON BENCHMARKS
// Compares performance between different indicator styles

[ShortRunJob]
public class StyleComparison
{
    private static readonly IReadOnlyList<Quote> quotes
       = Data.GetDefault();

    private readonly QuoteHub<Quote> quoteHub = new();

    private const int n = 14;

    [GlobalSetup]
    public void Setup() => quoteHub.Add(quotes);

    [GlobalCleanup]
    public void Cleanup()
    {
        quoteHub.EndTransmission();
        quoteHub.Cache.Clear();
    }

    [Benchmark]
    public AdlList AdlBuffer()
        => new() { quotes };

    [Benchmark]
    public IReadOnlyList<AdlResult> AdlSeries()
        => quotes.ToAdl();

    [Benchmark]
    public IReadOnlyList<AdlResult> AdlStream()
        => quoteHub.ToAdlHub().Results;

    [Benchmark]
    public IReadOnlyList<AdxResult> AdxSeries()
        => quotes.ToAdx(n);

    [Benchmark]
    public AdxList AdxBuffer()
        => new(n) { quotes };

    [Benchmark]
    public IReadOnlyList<AdxResult> AdxStream()
        => quoteHub.ToAdxHub(n).Results;

    [Benchmark]
    public AlmaList AlmaBuffer()
        => new(n, 0.85, 6) { quotes };

    [Benchmark]
    public IReadOnlyList<AlmaResult> AlmaSeries()
        => quotes.ToAlma(n, 0.85, 6);

    [Benchmark]
    public IReadOnlyList<AlmaResult> AlmaStream()
        => quoteHub.ToAlmaHub(n, 0.85, 6).Results;

    [Benchmark]
    public AtrList AtrBuffer()
        => new(n) { quotes };

    [Benchmark]
    public IReadOnlyList<AtrResult> AtrSeries()
        => quotes.ToAtr(n);

    [Benchmark]
    public IReadOnlyList<AtrResult> AtrStream()
        => quoteHub.ToAtrHub(n).Results;

    [Benchmark]
    public IReadOnlyList<EmaResult> EmaStream()
        => quoteHub.ToEmaHub(n).Results;

    [Benchmark]
    public HmaList HmaBuffer()
        => new(n) { quotes };

    [Benchmark]
    public IReadOnlyList<HmaResult> HmaSeries()
        => quotes.ToHma(n);

    [Benchmark]
    public IReadOnlyList<HmaResult> HmaStream()
        => quoteHub.ToHmaHub(n).Results;

    [Benchmark]
    public SmaList SmaBuffer()
        => new(n) { quotes };

    [Benchmark]
    public IReadOnlyList<SmaResult> SmaSeries()
        => quotes.ToSma(n);

    [Benchmark]
    public IReadOnlyList<SmaResult> SmaStream()
        => quoteHub.ToSma(n).Results;

    [Benchmark]
    public WmaList WmaBuffer()
        => new(n) { quotes };

    [Benchmark]
    public IReadOnlyList<WmaResult> WmaSeries()
        => quotes.ToWma(n);

    [Benchmark]
    public IReadOnlyList<WmaResult> WmaStream()
        => quoteHub.ToWmaHub(n).Results;

    [Benchmark]
    public TrList TrBuffer()
        => new() { quotes };

    [Benchmark]
    public IReadOnlyList<TrResult> TrSeries()
        => quotes.ToTr();

    [Benchmark]
    public IReadOnlyList<TrResult> TrStream()
        => quoteHub.ToTrHub().Results;
}
