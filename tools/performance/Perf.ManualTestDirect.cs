namespace Performance;

// MANUAL PERFORMANCE TESTING
// Supports environment variable-driven testing for specific indicators with custom data sizes
//
// Usage:
//   PERF_TEST_KEYWORD=sma PERF_TEST_PERIODS=100000 dotnet run -c Release --filter "Performance.ManualTestDirect*"
//
// Environment Variables:
//   PERF_TEST_KEYWORD - Indicator keyword (sma, ema, rsi, macd, etc.)
//   PERF_TEST_PERIODS - Number of bar periods to generate (default: 500000)
//   PERF_TEST_CAP     - Optional cache/list cap (MaxListSize / maxCacheSize). When set
//                       below PERF_TEST_PERIODS this exercises the steady-state pruning
//                       path; when unset it defaults to PERF_TEST_PERIODS (no pruning).

[ShortRunJob, WarmupCount(3), IterationCount(5)]
public class ManualTestDirect
{
    private static readonly string Keyword = Environment.GetEnvironmentVariable("PERF_TEST_KEYWORD")?.ToUpperInvariant() ?? "SMA";
    private static readonly int Periods = int.TryParse(Environment.GetEnvironmentVariable("PERF_TEST_PERIODS"), out int p) ? p : 500000;
    private static readonly int Cap = int.TryParse(Environment.GetEnvironmentVariable("PERF_TEST_CAP"), out int c) ? c : Periods;
    private static readonly IReadOnlyList<Bar> Bars = Data.GetRandom(Periods);
    private static readonly IReadOnlyList<Bar> CompareBars = Data.GetRandom(Periods);

    [GlobalSetup]
    public void Setup()
    {
        Console.WriteLine("Manual Test Configuration:");
        Console.WriteLine($"  Keyword: {Keyword}");
        Console.WriteLine($"  Periods: {Periods:N0}");
        Console.WriteLine($"  Cap (MaxListSize/maxCacheSize): {Cap:N0}");
        Console.WriteLine($"  Generated {Bars.Count:N0} bars");
    }

    /* SERIES BENCHMARKS */

    [Benchmark]
    public object Series() => Keyword switch {
        "ADL" => Bars.ToAdl(),
        "ADX" => Bars.ToAdx(14),
        "ALLIGATOR" => Bars.ToAlligator(),
        "ALMA" => Bars.ToAlma(9, 0.85, 6),
        "AROON" => Bars.ToAroon(),
        "ATR" => Bars.ToAtr(14),
        "ATR-STOP" => Bars.ToAtrStop(),
        "AWESOME" => Bars.ToAwesome(),
        "BB" => Bars.ToBollingerBands(20, 2),
        "BETA" => Bars.ToBeta(CompareBars, 20, BetaType.Standard),
        "BOP" => Bars.ToBop(14),
        "CCI" => Bars.ToCci(14),
        "CHAIKIN-OSC" => Bars.ToChaikinOsc(),
        "CHANDELIER" or "CHEXIT" => Bars.ToChandelier(),
        "CHOP" => Bars.ToChop(14),
        "CMF" => Bars.ToCmf(14),
        "CMO" => Bars.ToCmo(14),
        "CORR" => Bars.ToCorrelation(CompareBars, 20),
        "CRSI" => Bars.ToConnorsRsi(3, 2, 100),
        "DEMA" => Bars.ToDema(14),
        "DOJI" => Bars.ToDoji(),
        "DONCHIAN" => Bars.ToDonchian(20),
        "DPO" => Bars.ToDpo(14),
        "DYNAMIC" => Bars.ToDynamic(14),
        "ELDER-RAY" => Bars.ToElderRay(13),
        "EMA" => Bars.ToEma(20),
        "EPMA" => Bars.ToEpma(14),
        "FCB" => Bars.ToFcb(2),
        "FISHER" => Bars.ToFisherTransform(10),
        "FORCE" => Bars.ToForceIndex(2),
        "FRACTAL" => Bars.ToFractal(),
        "GATOR" => Bars.ToGator(),
        "HEIKINASHI" => Bars.ToHeikinAshi(),
        "HMA" => Bars.ToHma(14),
        "HTL" => Bars.ToHtTrendline(),
        "HURST" => Bars.ToHurst(100),
        "ICHIMOKU" => Bars.ToIchimoku(),
        "KAMA" => Bars.ToKama(10, 2, 30),
        "KELTNER" => Bars.ToKeltner(20, 2, 10),
        "KVO" => Bars.ToKvo(34, 55, 13),
        "MA-ENV" => Bars.ToMaEnvelopes(20, 2.5, MaType.SMA),
        "MACD" => Bars.ToMacd(12, 26, 9),
        "MAMA" => Bars.ToMama(0.5, 0.05),
        "MARUBOZU" => Bars.ToMarubozu(95),
        "MFI" => Bars.ToMfi(14),
        "OBV" => Bars.ToObv(),
        "PIVOT-POINTS" => Bars.ToPivotPoints(BarInterval.Month, PivotPointType.Standard),
        "PIVOTS" => Bars.ToPivots(2, 2, 20),
        "PMO" => Bars.ToPmo(35, 20, 10),
        "PRS" => Bars.ToPrs(CompareBars),
        "PSAR" => Bars.ToParabolicSar(),
        "PVO" => Bars.ToPvo(),
        "BARPART" => Bars.ToBarPart(CandlePart.OHL3),
        "RENKO" => Bars.ToRenko(2.5m),
        "RENKO-ATR" => Bars.ToRenkoAtr(14),
        "ROC" => Bars.ToRoc(20),
        "ROC-WB" => Bars.ToRocWb(20, 5, 5),
        "ROLLING-PIVOTS" => Bars.ToRollingPivots(20, 0, PivotPointType.Standard),
        "RSI" => Bars.ToRsi(14),
        "SLOPE" => Bars.ToSlope(14),
        "SMA" => Bars.ToSma(14),
        "SMA-ANALYSIS" => Bars.ToSmaAnalysis(14),
        "SMI" => Bars.ToSmi(13, 25, 2, 3),
        "SMMA" => Bars.ToSmma(14),
        "STARC" => Bars.ToStarcBands(5, 2, 10),
        "STC" => Bars.ToStc(10, 23, 50),
        "STDEV" => Bars.ToStdDev(14),
        "STDEV-CHANNELS" => Bars.ToStdDevChannels(),
        "STOCH" => Bars.ToStoch(14, 3, 3),
        "STOCH-RSI" => Bars.ToStochRsi(14, 14, 3, 1),
        "SUPERTREND" => Bars.ToSuperTrend(10, 3),
        "T3" => Bars.ToT3(5, 0.7),
        "TEMA" => Bars.ToTema(14),
        "TR" => Bars.ToTr(),
        "TRIX" => Bars.ToTrix(14),
        "TSI" => Bars.ToTsi(25, 13, 7),
        "ULCER" => Bars.ToUlcerIndex(14),
        "UO" => Bars.ToUltimate(7, 14, 28),
        "VOL-STOP" => Bars.ToVolatilityStop(7, 3),
        "VORTEX" => Bars.ToVortex(14),
        "VWAP" => Bars.ToVwap(),
        "VWMA" => Bars.ToVwma(14),
        "WILLR" => Bars.ToWilliamsR(),
        "WMA" => Bars.ToWma(14),
        "ZIGZAG" => Bars.ToZigZag(),
        _ => throw new NotSupportedException($"Indicator '{Keyword}' is not supported in ManualTestDirect Series benchmark."),
    };

    /* BUFFER BENCHMARKS */

    [Benchmark]
    public object Buffer()
    {
        // Create BufferList and set MaxListSize to match test data size
        // This prevents O(n²) pruning behavior when Periods > default MaxListSize (100,000)
        dynamic list = Keyword switch {
            "ADL" => new AdlList() { MaxListSize = Cap },
            "ADX" => new AdxList(14) { MaxListSize = Cap },
            "ALLIGATOR" => new AlligatorList() { MaxListSize = Cap },
            "ALMA" => new AlmaList(9, 0.85, 6) { MaxListSize = Cap },
            "AROON" => new AroonList() { MaxListSize = Cap },
            "ATR" => new AtrList(14) { MaxListSize = Cap },
            "ATR-STOP" => new AtrStopList() { MaxListSize = Cap },
            "AWESOME" => new AwesomeList() { MaxListSize = Cap },
            "BB" => new BollingerBandsList(20, 2) { MaxListSize = Cap },
            "BOP" => new BopList(14) { MaxListSize = Cap },
            "CCI" => new CciList(14) { MaxListSize = Cap },
            "CHAIKIN-OSC" => new ChaikinOscList() { MaxListSize = Cap },
            "CHANDELIER" or "CHEXIT" => new ChandelierList() { MaxListSize = Cap },
            "CHOP" => new ChopList(14) { MaxListSize = Cap },
            "CMF" => new CmfList(14) { MaxListSize = Cap },
            "CMO" => new CmoList(14) { MaxListSize = Cap },
            "CRSI" => new ConnorsRsiList(3, 2, 100) { MaxListSize = Cap },
            "DEMA" => new DemaList(14) { MaxListSize = Cap },
            "DOJI" => new DojiList() { MaxListSize = Cap },
            "DONCHIAN" => new DonchianList(20) { MaxListSize = Cap },
            "DPO" => new DpoList(14) { MaxListSize = Cap },
            "DYNAMIC" => new DynamicList(14) { MaxListSize = Cap },
            "ELDER-RAY" => new ElderRayList(13) { MaxListSize = Cap },
            "EMA" => new EmaList(20) { MaxListSize = Cap },
            "EPMA" => new EpmaList(14) { MaxListSize = Cap },
            "FCB" => new FcbList(2) { MaxListSize = Cap },
            "FISHER" => new FisherTransformList(10) { MaxListSize = Cap },
            "FORCE" => new ForceIndexList(2) { MaxListSize = Cap },
            "FRACTAL" => new FractalList() { MaxListSize = Cap },
            "GATOR" => new GatorList() { MaxListSize = Cap },
            "HEIKINASHI" => new HeikinAshiList() { MaxListSize = Cap },
            "HMA" => new HmaList(14) { MaxListSize = Cap },
            "HTL" => new HtTrendlineList() { MaxListSize = Cap },
            "HURST" => new HurstList(100) { MaxListSize = Cap },
            "ICHIMOKU" => new IchimokuList() { MaxListSize = Cap },
            "KAMA" => new KamaList(10, 2, 30) { MaxListSize = Cap },
            "KELTNER" => new KeltnerList(20, 2, 10) { MaxListSize = Cap },
            "KVO" => new KvoList(34, 55, 13) { MaxListSize = Cap },
            "MA-ENV" => new MaEnvelopesList(20, 2.5, MaType.SMA) { MaxListSize = Cap },
            "MACD" => new MacdList(12, 26, 9) { MaxListSize = Cap },
            "MAMA" => new MamaList(0.5, 0.05) { MaxListSize = Cap },
            "MARUBOZU" => new MarubozuList(95) { MaxListSize = Cap },
            "MFI" => new MfiList(14) { MaxListSize = Cap },
            "OBV" => new ObvList() { MaxListSize = Cap },
            "PMO" => new PmoList(35, 20, 10) { MaxListSize = Cap },
            "PSAR" => new ParabolicSarList() { MaxListSize = Cap },
            "PVO" => new PvoList() { MaxListSize = Cap },
            "RENKO" => new RenkoList(2.5m) { MaxListSize = Cap },
            "ROC" => new RocList(20) { MaxListSize = Cap },
            "ROC-WB" => new RocWbList(20, 5, 5) { MaxListSize = Cap },
            "RSI" => new RsiList(14) { MaxListSize = Cap },
            "SLOPE" => new SlopeList(14) { MaxListSize = Cap },
            "SMA" => new SmaList(14) { MaxListSize = Cap },
            "SMA-ANALYSIS" => new SmaAnalysisList(14) { MaxListSize = Cap },
            "SMI" => new SmiList(13, 25, 2, 3) { MaxListSize = Cap },
            "SMMA" => new SmmaList(14) { MaxListSize = Cap },
            "STARC" => new StarcBandsList(5, 2, 10) { MaxListSize = Cap },
            "STC" => new StcList(10, 23, 50) { MaxListSize = Cap },
            "STDEV" => new StdDevList(14) { MaxListSize = Cap },
            "STOCH" => new StochList(14, 3, 3) { MaxListSize = Cap },
            "STOCH-RSI" => new StochRsiList(14, 14, 3, 1) { MaxListSize = Cap },
            "SUPERTREND" => new SuperTrendList(10, 3) { MaxListSize = Cap },
            "T3" => new T3List(5, 0.7) { MaxListSize = Cap },
            "TEMA" => new TemaList(14) { MaxListSize = Cap },
            "TR" => new TrList() { MaxListSize = Cap },
            "TRIX" => new TrixList(14) { MaxListSize = Cap },
            "TSI" => new TsiList(25, 13, 7) { MaxListSize = Cap },
            "ULCER" => new UlcerIndexList(14) { MaxListSize = Cap },
            "UO" => new UltimateList(7, 14, 28) { MaxListSize = Cap },
            "VOL-STOP" => new VolatilityStopList(7, 3) { MaxListSize = Cap },
            "VORTEX" => new VortexList(14) { MaxListSize = Cap },
            "VWAP" => new VwapList() { MaxListSize = Cap },
            "VWMA" => new VwmaList(14) { MaxListSize = Cap },
            "WILLR" => new WilliamsRList() { MaxListSize = Cap },
            "WMA" => new WmaList(14) { MaxListSize = Cap },
            _ => throw new NotSupportedException($"Indicator '{Keyword}' is not supported in ManualTestDirect Buffer benchmark. Some indicators only have Series implementations."),
        };

        // Add bars to the BufferList
        list.Add(Bars);

        return list;
    }

    /* STREAM HUB BENCHMARKS */

    [Benchmark]
    public object StreamHub()
    {
        // Create fresh hub and set maxCacheSize to match test data size
        // This prevents O(n²) pruning behavior when Periods > default maxCacheSize (100,000)
        BarHub barHub = new(maxCacheSize: Cap);

        // Subscribe FIRST — observer wired up before bars arrive
        object hub = Keyword switch {
            "ADL" => barHub.ToAdlHub(),
            "ADX" => barHub.ToAdxHub(14),
            "ALLIGATOR" => barHub.ToAlligatorHub(),
            "ALMA" => barHub.ToAlmaHub(9, 0.85, 6),
            "AROON" => barHub.ToAroonHub(),
            "ATR" => barHub.ToAtrHub(14),
            "ATR-STOP" => barHub.ToAtrStopHub(),
            "AWESOME" => barHub.ToAwesomeHub(),
            "BB" => barHub.ToBollingerBandsHub(20, 2),
            "BOP" => barHub.ToBopHub(14),
            "CCI" => barHub.ToCciHub(14),
            "CHAIKIN-OSC" => barHub.ToChaikinOscHub(),
            "CHANDELIER" or "CHEXIT" => barHub.ToChandelierHub(),
            "CHOP" => barHub.ToChopHub(14),
            "CMF" => barHub.ToCmfHub(14),
            "CMO" => barHub.ToCmoHub(14),
            "CRSI" => barHub.ToConnorsRsiHub(3, 2, 100),
            "DEMA" => barHub.ToDemaHub(14),
            "DOJI" => barHub.ToDojiHub(),
            "DONCHIAN" => barHub.ToDonchianHub(20),
            "DPO" => barHub.ToDpoHub(14),
            "DYNAMIC" => barHub.ToDynamicHub(14),
            "ELDER-RAY" => barHub.ToElderRayHub(13),
            "EMA" => barHub.ToEmaHub(20),
            "EPMA" => barHub.ToEpmaHub(14),
            "FCB" => barHub.ToFcbHub(2),
            "FISHER" => barHub.ToFisherTransformHub(10),
            "FORCE" => barHub.ToForceIndexHub(2),
            "FRACTAL" => barHub.ToFractalHub(),
            "GATOR" => barHub.ToGatorHub(),
            "HEIKINASHI" => barHub.ToHeikinAshiHub(),
            "HMA" => barHub.ToHmaHub(14),
            "HTL" => barHub.ToHtTrendlineHub(),
            "HURST" => barHub.ToHurstHub(100),
            "ICHIMOKU" => barHub.ToIchimokuHub(),
            "KAMA" => barHub.ToKamaHub(10, 2, 30),
            "KELTNER" => barHub.ToKeltnerHub(20, 2, 10),
            "KVO" => barHub.ToKvoHub(34, 55, 13),
            "MA-ENV" => barHub.ToMaEnvelopesHub(20, 2.5, MaType.SMA),
            "MACD" => barHub.ToMacdHub(12, 26, 9),
            "MAMA" => barHub.ToMamaHub(0.5, 0.05),
            "MARUBOZU" => barHub.ToMarubozuHub(95),
            "MFI" => barHub.ToMfiHub(14),
            "OBV" => barHub.ToObvHub(),
            "PIVOT-POINTS" => barHub.ToPivotPointsHub(BarInterval.Month, PivotPointType.Standard),
            "PIVOTS" => barHub.ToPivotsHub(2, 2, 20),
            "PMO" => barHub.ToPmoHub(35, 20, 10),
            "PSAR" => barHub.ToParabolicSarHub(),
            "PVO" => barHub.ToPvoHub(),
            "BARPART" => barHub.ToBarPartHub(CandlePart.OHL3),
            "RENKO" => barHub.ToRenkoHub(2.5m),
            "ROC" => barHub.ToRocHub(20),
            "ROC-WB" => barHub.ToRocWbHub(20, 5, 5),
            "ROLLING-PIVOTS" => barHub.ToRollingPivotsHub(20, 0, PivotPointType.Standard),
            "RSI" => barHub.ToRsiHub(14),
            "SLOPE" => barHub.ToSlopeHub(14),
            "SMA" => barHub.ToSmaHub(14),
            "SMA-ANALYSIS" => barHub.ToSmaAnalysisHub(14),
            "SMI" => barHub.ToSmiHub(13, 25, 2, 3),
            "SMMA" => barHub.ToSmmaHub(14),
            "STARC" => barHub.ToStarcBandsHub(5, 2, 10),
            "STC" => barHub.ToStcHub(10, 23, 50),
            "STDEV" => barHub.ToStdDevHub(14),
            "STOCH" => barHub.ToStochHub(14, 3, 3),
            "STOCH-RSI" => barHub.ToStochRsiHub(14, 14, 3, 1),
            "SUPERTREND" => barHub.ToSuperTrendHub(10, 3),
            "T3" => barHub.ToT3Hub(5, 0.7),
            "TEMA" => barHub.ToTemaHub(14),
            "TR" => barHub.ToTrHub(),
            "TRIX" => barHub.ToTrixHub(14),
            "TSI" => barHub.ToTsiHub(25, 13, 7),
            "ULCER" => barHub.ToUlcerIndexHub(14),
            "UO" => barHub.ToUltimateHub(7, 14, 28),
            "VOL-STOP" => barHub.ToVolatilityStopHub(7, 3),
            "VORTEX" => barHub.ToVortexHub(14),
            "VWAP" => barHub.ToVwapHub(),
            "VWMA" => barHub.ToVwmaHub(14),
            "WILLR" => barHub.ToWilliamsRHub(),
            "WMA" => barHub.ToWmaHub(14),
            _ => throw new NotSupportedException($"Indicator '{Keyword}' is not supported in ManualTestDirect StreamHub benchmark. Some indicators only have Series implementations."),
        };

        // Feed bars AFTER observer is subscribed — real-time processing
        barHub.Add(Bars);
        barHub.EndTransmission();

        return ((dynamic)hub).Results;
    }

}

