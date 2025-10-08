namespace Performance;

// STREAM-STYLE INDICATORS

[ShortRunJob]
public class StreamIndicators
{
    private static readonly IReadOnlyList<Quote> quotes
        = Data.GetDefault();

    private readonly QuoteHub<Quote> quoteHub = new();  // prepopulated

    /* SETUP/CLEANUP - runs before and after each.
     *
     * This Setup implies that each benchmark
     * will start with a prepopulated observable
     * QuoteHub provider.
     *
     * We do this because we want to measure
     * the performance of observer methods
     * without the overhead of the provider. */

    [GlobalSetup]
    public void Setup() => quoteHub.Add(quotes);

    [GlobalCleanup]
    public void Cleanup()
    {
        quoteHub.EndTransmission();
        quoteHub.Cache.Clear();
    }

    // BENCHMARKS

    [Benchmark]
    public object AdlHub() => quoteHub.ToAdlHub().Results;

    [Benchmark]
    public object AdxHub() => quoteHub.ToAdxHub(14).Results;

    [Benchmark]
    public object AlligatorHub() => quoteHub.ToAlligatorHub().Results;

    [Benchmark]
    public object AlmaHub() => quoteHub.ToAlmaHub(10, 0.85, 6).Results;

    [Benchmark]
    public object AtrHub() => quoteHub.ToAtrHub(14).Results;

    [Benchmark]
    public object AtrStopHub() => quoteHub.ToAtrStopHub().Results;

    [Benchmark]
    public object BollingerBandsHub() => quoteHub.ToBollingerBandsHub(20, 2).Results;

    [Benchmark]
    public object DemaHub() => quoteHub.ToDemaHub(14).Results;

    [Benchmark]
    public object EmaHub() => quoteHub.ToEmaHub(14).Results;

    [Benchmark]
    public object EpmaHub() => quoteHub.ToEpmaHub(14).Results;

    [Benchmark]
    public object HmaHub() => quoteHub.ToHmaHub(14).Results;

    [Benchmark]
    public object KamaHub() => quoteHub.ToKamaHub(10, 2, 30).Results;

    [Benchmark]
    public object MacdHub() => quoteHub.ToMacdHub(12, 26, 9).Results;

    [Benchmark]
    public object MamaHub() => quoteHub.ToMamaHub(0.5, 0.05).Results;

    [Benchmark]
    public object ObvHub() => quoteHub.ToObvHub().Results;

    [Benchmark]
    public object QuoteHub() => quoteHub.ToQuoteHub().Results;

    [Benchmark]
    public object QuotePartHub() => quoteHub.ToQuotePartHub(CandlePart.OHL3).Results;

    [Benchmark]
    public object RenkoHub() => quoteHub.ToRenkoHub(2.5m).Results;

    [Benchmark]
    public object RsiHub() => quoteHub.ToRsiHub(14).Results;

    [Benchmark]
    public object RocHub() => quoteHub.ToRocHub(20).Results;

    [Benchmark]
    public object SmaHub() => quoteHub.ToSma(10).Results;

    [Benchmark]
    public object SmmaHub() => quoteHub.ToSmmaHub(14).Results;

    [Benchmark]
    public object StochHub() => quoteHub.ToStochHub(14, 3, 3).Results;

    [Benchmark]
    public object StochRsiHub() => quoteHub.ToStochRsiHub(14, 14, 3, 1).Results;

    [Benchmark]
    public object T3Hub() => quoteHub.ToT3Hub(5, 0.7).Results;

    [Benchmark]
    public object TemaHub() => quoteHub.ToTemaHub(14).Results;

    [Benchmark]
    public object TrHub() => quoteHub.ToTrHub().Results;

    [Benchmark]
    public object UltimateHub() => quoteHub.ToUltimateHub(7, 14, 28).Results;

    [Benchmark]
    public object VwmaHub() => quoteHub.ToVwmaHub(14).Results;

    [Benchmark]
    public object WmaHub() => quoteHub.ToWmaHub(14).Results;
}
