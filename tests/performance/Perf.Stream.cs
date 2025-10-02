namespace Performance;

// STREAM-STYLE INDICATORS

[ShortRunJob]
public class StreamIndicators
{
    private static readonly IReadOnlyList<Quote> quotes
        = Data.GetDefault();

    private readonly QuoteHub<Quote> provider = new();  // prepopulated

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
    public void Setup() => provider.Add(quotes);

    [GlobalCleanup]
    public void Cleanup()
    {
        provider.EndTransmission();
        provider.Cache.Clear();
    }

    // BENCHMARKS

    [Benchmark]
    public object AdlHub() => provider.ToAdl().Results;

    [Benchmark]
    public object AdxHub() => provider.ToAdx(14).Results;

    [Benchmark]
    public object AlligatorHub() => provider.ToAlligator().Results;

    [Benchmark]
    public object AlmaHub() => provider.ToAlma(10, 0.85, 6).Results;

    [Benchmark]
    public object AtrHub() => provider.ToAtr(14).Results;

    [Benchmark]
    public object AtrStopHub() => provider.ToAtrStop().Results;

    [Benchmark]
    public object BollingerBandsHub() => provider.ToBollingerBands(20, 2).Results;

    [Benchmark]
    public object DemaHub() => provider.ToDema(14).Results;

    [Benchmark]
    public object EmaHub() => provider.ToEma(14).Results;

    [Benchmark]
    public object EpmaHub() => provider.ToEpma(14).Results;

    [Benchmark]
    public object HmaHub() => provider.ToHma(14).Results;

    [Benchmark]
    public object KamaHub() => provider.ToKama(10, 2, 30).Results;

    [Benchmark]
    public object MacdHub() => provider.ToMacd(12, 26, 9).Results;

    [Benchmark]
    public object MamaHub() => provider.ToMama(0.5, 0.05).Results;

    [Benchmark]
    public object ObvHub() => provider.ToObv().Results;

    [Benchmark]
    public object QuoteHub() => provider.ToQuote().Results;

    [Benchmark]
    public object QuotePartHub() => provider.ToQuotePart(CandlePart.OHL3).Results;

    [Benchmark]
    public object RenkoHub() => provider.ToRenko(2.5m).Results;

    [Benchmark]
    public object RsiHub() => provider.ToRsi(14).Results;

    [Benchmark]
    public object SmaHub() => provider.ToSma(10).Results;

    [Benchmark]
    public object SmmaHub() => provider.ToSmma(14).Results;

    [Benchmark]
    public object StochHub() => provider.ToStoch(14, 3, 3).Results;

    [Benchmark]
    public object T3Hub() => provider.ToT3(5, 0.7).Results;

    [Benchmark]
    public object TemaHub() => provider.ToTema(14).Results;

    [Benchmark]
    public object TrHub() => provider.ToTr().Results;

    [Benchmark]
    public object UltimateHub() => provider.ToUltimate(7, 14, 28).Results;

    [Benchmark]
    public object VwmaHub() => provider.ToVwma(14).Results;

    [Benchmark]
    public object WmaHub() => provider.ToWma(14).Results;
}
