namespace Performance;

/// <summary>
/// Manual performance test using direct method calls (no catalog/reflection overhead).
/// Uses environment variables PERF_TEST_KEYWORD and PERF_TEST_PERIODS.
/// Supports common indicators with precompiled delegates for zero-overhead benchmarking.
/// </summary>
[Config(typeof(ManualTestDirectConfig))]
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
    private string seriesMethodName = string.Empty;
    private string streamMethodName = string.Empty;
    private string bufferMethodName = string.Empty;

    [GlobalSetup]
    public void Setup()
    {
        quotes = Data.GetRandom(Periods);
        quoteHub = new QuoteHub();
        quoteHub.Add(quotes);

        // Map keyword to direct method calls (alphabetically sorted)
        switch (Keyword.ToUpperInvariant())
        {
            case "ADX":
                seriesAction = q => q.ToAdx(14);
                streamAction = h => h.ToAdxHub(14).Results;
                bufferAction = q => new AdxList(14) { q };
                seriesMethodName = "ToAdx";
                streamMethodName = "ToAdxHub";
                bufferMethodName = "AdxList";
                break;

            case "AROON":
                seriesAction = q => q.ToAroon(25);
                streamAction = h => h.ToAroonHub(25).Results;
                bufferAction = q => new AroonList(25) { q };
                seriesMethodName = "ToAroon";
                streamMethodName = "ToAroonHub";
                bufferMethodName = "AroonList";
                break;

            case "ATR":
                seriesAction = q => q.ToAtr(14);
                streamAction = h => h.ToAtrHub(14).Results;
                bufferAction = q => new AtrList(14) { q };
                seriesMethodName = "ToAtr";
                streamMethodName = "ToAtrHub";
                bufferMethodName = "AtrList";
                break;

            case "BOLLINGER":
            case "BOLLINGERBANDS":
                seriesAction = q => q.ToBollingerBands(20, 2);
                streamAction = h => h.ToBollingerBandsHub(20, 2).Results;
                bufferAction = q => new BollingerBandsList(20, 2) { q };
                seriesMethodName = "ToBollingerBands";
                streamMethodName = "ToBollingerBandsHub";
                bufferMethodName = "BollingerBandsList";
                break;

            case "CCI":
                seriesAction = q => q.ToCci(20);
                streamAction = h => h.ToCciHub(20).Results;
                bufferAction = q => new CciList(20) { q };
                seriesMethodName = "ToCci";
                streamMethodName = "ToCciHub";
                bufferMethodName = "CciList";
                break;

            case "DEMA":
                seriesAction = q => q.ToDema(20);
                streamAction = h => h.ToDemaHub(20).Results;
                bufferAction = q => new DemaList(20) { q };
                seriesMethodName = "ToDema";
                streamMethodName = "ToDemaHub";
                bufferMethodName = "DemaList";
                break;

            case "DONCHIAN":
                seriesAction = q => q.ToDonchian(20);
                streamAction = h => h.ToDonchianHub(20).Results;
                bufferAction = q => new DonchianList(20) { q };
                seriesMethodName = "ToDonchian";
                streamMethodName = "ToDonchianHub";
                bufferMethodName = "DonchianList";
                break;

            case "EMA":
                seriesAction = q => q.ToEma(20);
                streamAction = h => h.ToEmaHub(20).Results;
                bufferAction = q => new EmaList(20) { q };
                seriesMethodName = "ToEma";
                streamMethodName = "ToEmaHub";
                bufferMethodName = "EmaList";
                break;

            case "HMA":
                seriesAction = q => q.ToHma(20);
                streamAction = h => h.ToHmaHub(20).Results;
                bufferAction = q => new HmaList(20) { q };
                seriesMethodName = "ToHma";
                streamMethodName = "ToHmaHub";
                bufferMethodName = "HmaList";
                break;

            case "KELTNER":
                seriesAction = q => q.ToKeltner(20, 2, 10);
                streamAction = h => h.ToKeltnerHub(20, 2, 10).Results;
                bufferAction = q => new KeltnerList(20, 2, 10) { q };
                seriesMethodName = "ToKeltner";
                streamMethodName = "ToKeltnerHub";
                bufferMethodName = "KeltnerList";
                break;

            case "MACD":
                seriesAction = q => q.ToMacd(12, 26, 9);
                streamAction = h => h.ToMacdHub(12, 26, 9).Results;
                bufferAction = q => new MacdList(12, 26, 9) { q };
                seriesMethodName = "ToMacd";
                streamMethodName = "ToMacdHub";
                bufferMethodName = "MacdList";
                break;

            case "OBV":
                seriesAction = q => q.ToObv();
                streamAction = h => h.ToObvHub().Results;
                bufferAction = q => new ObvList() { q };
                seriesMethodName = "ToObv";
                streamMethodName = "ToObvHub";
                bufferMethodName = "ObvList";
                break;

            case "ROC":
                seriesAction = q => q.ToRoc(20);
                streamAction = h => h.ToRocHub(20).Results;
                bufferAction = q => new RocList(20) { q };
                seriesMethodName = "ToRoc";
                streamMethodName = "ToRocHub";
                bufferMethodName = "RocList";
                break;

            case "RSI":
                seriesAction = q => q.ToRsi(14);
                streamAction = h => h.ToRsiHub(14).Results;
                bufferAction = q => new RsiList(14) { q };
                seriesMethodName = "ToRsi";
                streamMethodName = "ToRsiHub";
                bufferMethodName = "RsiList";
                break;

            case "SMA":
                seriesAction = q => q.ToSma(20);
                streamAction = h => h.ToSmaHub(20).Results;
                bufferAction = q => new SmaList(20) { q };
                seriesMethodName = "ToSma";
                streamMethodName = "ToSmaHub";
                bufferMethodName = "SmaList";
                break;

            case "STOCH":
            case "STOCHASTIC":
                seriesAction = q => q.ToStoch(14, 3, 3);
                streamAction = h => h.ToStochHub(14, 3, 3).Results;
                bufferAction = q => new StochList(14, 3, 3) { q };
                seriesMethodName = "ToStoch";
                streamMethodName = "ToStochHub";
                bufferMethodName = "StochList";
                break;

            case "T3":
                seriesAction = q => q.ToT3(20);
                streamAction = h => h.ToT3Hub(20).Results;
                bufferAction = q => new T3List(20) { q };
                seriesMethodName = "ToT3";
                streamMethodName = "ToT3Hub";
                bufferMethodName = "T3List";
                break;

            case "TEMA":
                seriesAction = q => q.ToTema(20);
                streamAction = h => h.ToTemaHub(20).Results;
                bufferAction = q => new TemaList(20) { q };
                seriesMethodName = "ToTema";
                streamMethodName = "ToTemaHub";
                bufferMethodName = "TemaList";
                break;

            case "VWAP":
                seriesAction = q => q.ToVwap();
                streamAction = h => h.ToVwapHub().Results;
                bufferAction = q => new VwapList() { q };
                seriesMethodName = "ToVwap";
                streamMethodName = "ToVwapHub";
                bufferMethodName = "VwapList";
                break;

            case "VWMA":
                seriesAction = q => q.ToVwma(20);
                streamAction = h => h.ToVwmaHub(20).Results;
                bufferAction = q => new VwmaList(20) { q };
                seriesMethodName = "ToVwma";
                streamMethodName = "ToVwmaHub";
                bufferMethodName = "VwmaList";
                break;

            case "WMA":
                seriesAction = q => q.ToWma(20);
                streamAction = h => h.ToWmaHub(20).Results;
                bufferAction = q => new WmaList(20) { q };
                seriesMethodName = "ToWma";
                streamMethodName = "ToWmaHub";
                bufferMethodName = "WmaList";
                break;

            default:
                string hint = Keyword.StartsWith("To", StringComparison.OrdinalIgnoreCase)
                    ? $" Did you mean '{Keyword[2..]}'?"
                    : string.Empty;
                throw new NotSupportedException(
                    $"Keyword '{Keyword}' is not supported.{hint} " +
                    "Add a direct method call mapping in ManualTestDirect.cs or use ManualTest (catalog-based) instead.");
        }

        Console.WriteLine("Manual Direct Performance Test Configuration:");
        Console.WriteLine($"  Keyword: {Keyword}");
        Console.WriteLine($"  Periods: {Periods:N0}");
        Console.WriteLine($"  Series Method: {seriesMethodName}");
        Console.WriteLine($"  Stream Method: {streamMethodName}");
        Console.WriteLine($"  Buffer Method: {bufferMethodName}");
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

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1024:Use properties where appropriate", Justification = "Method for informational display")]
    public string GetBenchmarkDisplayInfo() =>
        $"{Keyword}: Series={seriesMethodName}, Stream={streamMethodName}, Buffer={bufferMethodName}";
}

/// <summary>
/// Configuration for ManualTestDirect that passes environment variables to child processes.
/// </summary>
internal sealed class ManualTestDirectConfig : PerformanceConfig
{
    public ManualTestDirectConfig()
    {
        // Read environment variables from parent process
        string keyword = Environment.GetEnvironmentVariable("PERF_TEST_KEYWORD") ?? "sma";
        string periods = Environment.GetEnvironmentVariable("PERF_TEST_PERIODS") ?? "10000";

        // Create jobs with environment variables
        AddJob(BenchmarkDotNet.Jobs.Job.ShortRun
            .WithEnvironmentVariable("PERF_TEST_KEYWORD", keyword)
            .WithEnvironmentVariable("PERF_TEST_PERIODS", periods));
    }
}
