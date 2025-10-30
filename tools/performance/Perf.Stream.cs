namespace Performance;

// STREAM-STYLE INDICATORS

[ShortRunJob]
public class StreamIndicators
{
    private static readonly IReadOnlyList<Quote> q = Data.GetDefault();
    private static readonly IReadOnlyList<Quote> o = Data.GetCompare();
    private const int n = 14;

    private readonly QuoteHub quoteHub = new();      // prepopulated
    private readonly QuoteHub quoteHubOther = new(); // for correlation, beta, etc.

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
    public void Setup()
    {
        quoteHub.Add(q);
        quoteHubOther.Add(o);
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        quoteHub.EndTransmission();
        quoteHub.Cache.Clear();

        quoteHubOther.EndTransmission();
        quoteHubOther.Cache.Clear();
    }

    // BENCHMARKS

    /* Parameter arguments should match the Perf.Series.cs equivalents */

    [Benchmark] public object AdlHub() => quoteHub.ToAdlHub().Results;
    [Benchmark] public object AdxHub() => quoteHub.ToAdxHub(n).Results;
    [Benchmark] public object AlligatorHub() => quoteHub.ToAlligatorHub().Results;
    [Benchmark] public object AlmaHub() => quoteHub.ToAlmaHub(9, 0.85, 6).Results;
    [Benchmark] public object AroonHub() => quoteHub.ToAroonHub().Results;
    [Benchmark] public object AtrHub() => quoteHub.ToAtrHub(n).Results;
    [Benchmark] public object AtrStopHub() => quoteHub.ToAtrStopHub().Results;
    [Benchmark] public object AwesomeHub() => quoteHub.ToAwesomeHub().Results;
    [Benchmark] public object BetaHub() => quoteHub.ToBetaHub(quoteHubOther, 20);
    [Benchmark] public object BollingerBandsHub() => quoteHub.ToBollingerBandsHub(20, 2).Results;
    [Benchmark] public object BopHub() => quoteHub.ToBopHub(n).Results;
    [Benchmark] public object CciHub() => quoteHub.ToCciHub(20).Results;
    [Benchmark] public object ChaikinOscHub() => quoteHub.ToChaikinOscHub().Results;
    [Benchmark] public object ChandelierHub() => quoteHub.ToChandelierHub().Results;
    [Benchmark] public object ChopHub() => quoteHub.ToChopHub(n).Results;
    [Benchmark] public object CmfHub() => quoteHub.ToCmfHub(20).Results;
    [Benchmark] public object CmoHub() => quoteHub.ToCmoHub(n).Results;
    [Benchmark] public object CorrelationHub() => quoteHub.ToCorrelationHub(quoteHubOther, 20);
    [Benchmark] public object DemaHub() => quoteHub.ToDemaHub(n).Results;
    [Benchmark] public object DojiHub() => quoteHub.ToDojiHub().Results;
    [Benchmark] public object DonchianHub() => quoteHub.ToDonchianHub(20).Results;
    [Benchmark] public object DynamicHub() => quoteHub.ToDynamicHub(n, 0.6).Results;
    [Benchmark] public object ElderRayHub() => quoteHub.ToElderRayHub(13).Results;
    [Benchmark] public object EmaHub() => quoteHub.ToEmaHub(20).Results;
    [Benchmark] public object EpmaHub() => quoteHub.ToEpmaHub(n).Results;
    [Benchmark] public object FisherTransformHub() => quoteHub.ToFisherTransformHub(10).Results;
    [Benchmark] public object FcbHub() => quoteHub.ToFcbHub(2).Results;
    [Benchmark] public object FractalHub() => quoteHub.ToFractalHub().Results;
    [Benchmark] public object ForceIndexHub() => quoteHub.ToForceIndexHub(2).Results;
    [Benchmark] public object GatorHub() => quoteHub.ToGatorHub().Results;
    [Benchmark] public object HmaHub() => quoteHub.ToHmaHub(n).Results;
    [Benchmark] public object HeikinAshiHub() => quoteHub.ToHeikinAshiHub().Results;
    [Benchmark] public object KamaHub() => quoteHub.ToKamaHub(10, 2, 30).Results;
    [Benchmark] public object KvoHub() => quoteHub.ToKvoHub(34, 55, 13).Results;
    [Benchmark] public object KeltnerHub() => quoteHub.ToKeltnerHub(20, 2, 10).Results;
    [Benchmark] public object MacdHub() => quoteHub.ToMacdHub(12, 26, 9).Results;
    [Benchmark] public object MaEnvelopesHub() => quoteHub.ToMaEnvelopesHub(20, 2.5, MaType.SMA).Results;
    [Benchmark] public object MamaHub() => quoteHub.ToMamaHub(0.5, 0.05).Results;
    [Benchmark] public object MfiHub() => quoteHub.ToMfiHub(14).Results;
    [Benchmark] public object MarubozuHub() => quoteHub.ToMarubozuHub(95).Results;
    [Benchmark] public object ObvHub() => quoteHub.ToObvHub().Results;
    [Benchmark] public object ParabolicSarHub() => quoteHub.ToParabolicSarHub().Results;
    [Benchmark] public object PivotsHub() => quoteHub.ToPivotsHub().Results;
    [Benchmark] public object PmoHub() => quoteHub.ToPmoHub(35, 20, 10).Results;
    [Benchmark] public object PrsHub() => quoteHub.ToPrsHub(quoteHubOther, 20);
    [Benchmark] public object QuoteHub() => quoteHub.ToQuoteHub().Results;
    [Benchmark] public object QuotePartHub() => quoteHub.ToQuotePartHub(CandlePart.OHL3).Results;
    [Benchmark] public object RenkoHub() => quoteHub.ToRenkoHub(2.5m).Results;
    [Benchmark] public object RsiHub() => quoteHub.ToRsiHub(n).Results;
    [Benchmark] public object RocHub() => quoteHub.ToRocHub(20).Results;
    [Benchmark] public object SmaHub() => quoteHub.ToSmaHub(10).Results;
    [Benchmark] public object SmmaHub() => quoteHub.ToSmmaHub(n).Results;
    [Benchmark] public object StochHub() => quoteHub.ToStochHub(n, 3, 3).Results;
    [Benchmark] public object StochRsiHub() => quoteHub.ToStochRsiHub(n, n, 3, 1).Results;
    [Benchmark] public object T3Hub() => quoteHub.ToT3Hub(5, 0.7).Results;
    [Benchmark] public object TemaHub() => quoteHub.ToTemaHub(20).Results;
    [Benchmark] public object TrHub() => quoteHub.ToTrHub().Results;
    [Benchmark] public object TrixHub() => quoteHub.ToTrixHub(n).Results;
    [Benchmark] public object UlcerIndexHub() => quoteHub.ToUlcerIndexHub(n).Results;
    [Benchmark] public object UltimateHub() => quoteHub.ToUltimateHub(7, n, 28).Results;
    [Benchmark] public object VwmaHub() => quoteHub.ToVwmaHub(n).Results;
    [Benchmark] public object WilliamsRHub() => quoteHub.ToWilliamsRHub().Results;
    [Benchmark] public object WmaHub() => quoteHub.ToWmaHub(n).Results;
}
