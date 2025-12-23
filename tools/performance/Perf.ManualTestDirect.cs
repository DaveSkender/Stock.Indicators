namespace Performance;

/// <summary>
/// Manual performance test using direct method calls (no catalog/reflection overhead).
/// Uses environment variables PERF_TEST_KEYWORD and PERF_TEST_PERIODS.
/// Supports common indicators with precompiled delegates for zero-overhead benchmarking.
/// </summary>
[ShortRunJob]
public class ManualTestDirect
{
    private static readonly string Keyword = Environment.GetEnvironmentVariable("PERF_TEST_KEYWORD") ?? "sma";
    private static readonly int Periods = int.TryParse(
        Environment.GetEnvironmentVariable("PERF_TEST_PERIODS"),
        out int p) ? p : 500000;

    private IReadOnlyList<Quote> quotes = Array.Empty<Quote>();
    private QuoteHub quoteHub = null!;
    private Func<IReadOnlyList<Quote>, object> seriesAction = null!;
    private Func<QuoteHub, object> streamAction = null!;
    private Func<IReadOnlyList<Quote>, object> bufferAction = null!;

    [GlobalSetup]
    public void Setup()
    {
        quotes = Data.GetRandom(Periods);
        quoteHub = new QuoteHub();
        quoteHub.Add(quotes);

        // Map keyword to direct method calls
        switch (Keyword.ToUpperInvariant())
        {
            case "SMA":
                seriesAction = q => q.ToSma(20);
                streamAction = h => h.ToSmaHub(20).Results;
                bufferAction = q => new SmaList(20) { q };
                break;

            case "EMA":
                seriesAction = q => q.ToEma(20);
                streamAction = h => h.ToEmaHub(20).Results;
                bufferAction = q => new EmaList(20) { q };
                break;

            case "RSI":
                seriesAction = q => q.ToRsi(14);
                streamAction = h => h.ToRsiHub(14).Results;
                bufferAction = q => new RsiList(14) { q };
                break;

            case "MACD":
                seriesAction = q => q.ToMacd(12, 26, 9);
                streamAction = h => h.ToMacdHub(12, 26, 9).Results;
                bufferAction = q => new MacdList(12, 26, 9) { q };
                break;

            case "ADX":
                seriesAction = q => q.ToAdx(14);
                streamAction = h => h.ToAdxHub(14).Results;
                bufferAction = q => new AdxList(14) { q };
                break;

            case "ATR":
                seriesAction = q => q.ToAtr(14);
                streamAction = h => h.ToAtrHub(14).Results;
                bufferAction = q => new AtrList(14) { q };
                break;

            case "BOLLINGER":
            case "BOLLINGERBANDS":
                seriesAction = q => q.ToBollingerBands(20, 2);
                streamAction = h => h.ToBollingerBandsHub(20, 2).Results;
                bufferAction = q => new BollingerBandsList(20, 2) { q };
                break;

            case "STOCH":
            case "STOCHASTIC":
                seriesAction = q => q.ToStoch(14, 3, 3);
                streamAction = h => h.ToStochHub(14, 3, 3).Results;
                bufferAction = q => new StochList(14, 3, 3) { q };
                break;

            case "CCI":
                seriesAction = q => q.ToCci(20);
                streamAction = h => h.ToCciHub(20).Results;
                bufferAction = q => new CciList(20) { q };
                break;

            case "OBV":
                seriesAction = q => q.ToObv();
                streamAction = h => h.ToObvHub().Results;
                bufferAction = q => new ObvList() { q };
                break;

            case "ROC":
                seriesAction = q => q.ToRoc(20);
                streamAction = h => h.ToRocHub(20).Results;
                bufferAction = q => new RocList(20) { q };
                break;

            case "WMA":
                seriesAction = q => q.ToWma(20);
                streamAction = h => h.ToWmaHub(20).Results;
                bufferAction = q => new WmaList(20) { q };
                break;

            case "HMA":
                seriesAction = q => q.ToHma(20);
                streamAction = h => h.ToHmaHub(20).Results;
                bufferAction = q => new HmaList(20) { q };
                break;

            case "KELTNER":
                seriesAction = q => q.ToKeltner(20, 2, 10);
                streamAction = h => h.ToKeltnerHub(20, 2, 10).Results;
                bufferAction = q => new KeltnerList(20, 2, 10) { q };
                break;

            case "DONCHIAN":
                seriesAction = q => q.ToDonchian(20);
                streamAction = h => h.ToDonchianHub(20).Results;
                bufferAction = q => new DonchianList(20) { q };
                break;

            case "AROON":
                seriesAction = q => q.ToAroon(25);
                streamAction = h => h.ToAroonHub(25).Results;
                bufferAction = q => new AroonList(25) { q };
                break;

            case "VWMA":
                seriesAction = q => q.ToVwma(20);
                streamAction = h => h.ToVwmaHub(20).Results;
                bufferAction = q => new VwmaList(20) { q };
                break;

            case "VWAP":
                seriesAction = q => q.ToVwap();
                streamAction = h => h.ToVwapHub().Results;
                bufferAction = q => new VwapList() { q };
                break;

            default:
                throw new NotSupportedException(
                    $"Keyword '{Keyword}' is not supported. " +
                    "Add a direct method call mapping in ManualTestDirect.cs or use ManualTest (catalog-based) instead.");
        }

        Console.WriteLine("Manual Direct Performance Test Configuration:");
        Console.WriteLine($"  Keyword: {Keyword}");
        Console.WriteLine($"  Periods: {Periods:N0}");
        Console.WriteLine("  Method: Direct (no catalog/reflection overhead)");
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        if (quoteHub is not null)
        {
            quoteHub.EndTransmission();
            quoteHub.Cache.Clear();
        }
    }

    [Benchmark]
    public object TestSeriesDirect()
    {
        return seriesAction(quotes);
    }

    [Benchmark]
    public object TestStreamDirect()
    {
        return streamAction(quoteHub);
    }

    [Benchmark]
    public object TestBufferDirect()
    {
        return bufferAction(quotes);
    }
}
