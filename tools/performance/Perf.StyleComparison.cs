namespace Performance;

// STYLE COMPARISON BENCHMARKS
// Compares performance between different indicator styles

[ShortRunJob]
public class StyleComparison
{
    private static readonly IReadOnlyList<Quote> quotes = Data.GetDefault();
    private const int n = 14;

    private readonly QuoteHub quoteHub = new();


    [GlobalSetup]
    public void Setup() => quoteHub.Add(quotes);

    [GlobalCleanup]
    public void Cleanup()
    {
        quoteHub.EndTransmission();
        quoteHub.Cache.Clear();
    }

    // TODO: this only needs to contain representative indicators of each style

    [Benchmark] public IReadOnlyList<AdlResult> AdlSeries() => quotes.ToAdl();
    [Benchmark] public IReadOnlyList<AdlResult> AdlBuffer() => quotes.ToAdlList();
    [Benchmark] public IReadOnlyList<AdlResult> AdlStream() => quoteHub.ToAdlHub().Results;

    [Benchmark] public IReadOnlyList<AdxResult> AdxSeries() => quotes.ToAdx(n);
    [Benchmark] public IReadOnlyList<AdxResult> AdxBuffer() => quotes.ToAdxList(n);
    [Benchmark] public IReadOnlyList<AdxResult> AdxStream() => quoteHub.ToAdxHub(n).Results;

    [Benchmark] public IReadOnlyList<AlmaResult> AlmaSeries() => quotes.ToAlma(n, 0.85, 6);
    [Benchmark] public IReadOnlyList<AlmaResult> AlmaBuffer() => quotes.ToAlmaList(n, 0.85, 6);
    [Benchmark] public IReadOnlyList<AlmaResult> AlmaStream() => quoteHub.ToAlmaHub(n, 0.85, 6).Results;

    [Benchmark] public IReadOnlyList<AtrResult> AtrSeries() => quotes.ToAtr(n);
    [Benchmark] public IReadOnlyList<AtrResult> AtrBuffer() => quotes.ToAtrList(n);
    [Benchmark] public IReadOnlyList<AtrResult> AtrStream() => quoteHub.ToAtrHub(n).Results;

    [Benchmark] public IReadOnlyList<EmaResult> EmaSeries() => quotes.ToEma(n);
    [Benchmark] public IReadOnlyList<EmaResult> EmaBuffer() => quotes.ToEmaList(n);
    [Benchmark] public IReadOnlyList<EmaResult> EmaStream() => quoteHub.ToEmaHub(n).Results;

    [Benchmark] public IReadOnlyList<HmaResult> HmaSeries() => quotes.ToHma(n);
    [Benchmark] public IReadOnlyList<HmaResult> HmaBuffer() => quotes.ToHmaList(n);
    [Benchmark] public IReadOnlyList<HmaResult> HmaStream() => quoteHub.ToHmaHub(n).Results;

    [Benchmark] public IReadOnlyList<SmaResult> SmaSeries() => quotes.ToSma(n);
    [Benchmark] public IReadOnlyList<SmaResult> SmaBuffer() => quotes.ToSmaList(n);
    [Benchmark] public IReadOnlyList<SmaResult> SmaStream() => quoteHub.ToSmaHub(n).Results;

    [Benchmark] public IReadOnlyList<WmaResult> WmaSeries() => quotes.ToWma(n);
    [Benchmark] public IReadOnlyList<WmaResult> WmaBuffer() => quotes.ToWmaList(n);
    [Benchmark] public IReadOnlyList<WmaResult> WmaStream() => quoteHub.ToWmaHub(n).Results;

    [Benchmark] public IReadOnlyList<TrResult> TrSeries() => quotes.ToTr();
    [Benchmark] public IReadOnlyList<TrResult> TrBuffer() => quotes.ToTrList();
    [Benchmark] public IReadOnlyList<TrResult> TrStream() => quoteHub.ToTrHub().Results;
}
