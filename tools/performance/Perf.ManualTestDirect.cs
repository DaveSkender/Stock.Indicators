namespace Performance;

// MANUAL PERFORMANCE TESTING
// Supports environment variable-driven testing for specific indicators with custom data sizes
//
// Usage:
//   PERF_TEST_KEYWORD=sma PERF_TEST_PERIODS=100000 dotnet run -c Release --filter "Performance.ManualTestDirect*"
//
// Environment Variables:
//   PERF_TEST_KEYWORD - Indicator keyword (sma, ema, rsi, macd, etc.)
//   PERF_TEST_PERIODS - Number of quote periods to generate (default: 500000)

[ShortRunJob, WarmupCount(3), IterationCount(5)]
public class ManualTestDirect
{
    private static readonly string Keyword = Environment.GetEnvironmentVariable("PERF_TEST_KEYWORD")?.ToUpperInvariant() ?? "SMA";
    private static readonly int Periods = int.TryParse(Environment.GetEnvironmentVariable("PERF_TEST_PERIODS"), out int p) ? p : 500000;
    private static readonly IReadOnlyList<Quote> Quotes = Data.GetRandom(Periods);
    private static readonly IReadOnlyList<Quote> CompareQuotes = Data.GetRandom(Periods);

    private readonly QuoteHub quoteHub = new();
    private readonly QuoteHub quoteHubOther = new();

    [GlobalSetup]
    public void Setup()
    {
        Console.WriteLine("Manual Test Configuration:");
        Console.WriteLine($"  Keyword: {Keyword}");
        Console.WriteLine($"  Periods: {Periods:N0}");
        Console.WriteLine($"  Generated {Quotes.Count:N0} quotes");

        quoteHub.Add(Quotes);
        quoteHubOther.Add(CompareQuotes);
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        quoteHub.EndTransmission();
        quoteHub.Cache.Clear();

        quoteHubOther.EndTransmission();
        quoteHubOther.Cache.Clear();
    }

    /* SERIES BENCHMARKS */

    [Benchmark]
    public object Series() => Keyword switch {
        "ADL" => Quotes.ToAdl(),
        "ADX" => Quotes.ToAdx(14),
        "ALLIGATOR" => Quotes.ToAlligator(),
        "ALMA" => Quotes.ToAlma(9, 0.85, 6),
        "AROON" => Quotes.ToAroon(),
        "ATR" => Quotes.ToAtr(14),
        "ATR-STOP" => Quotes.ToAtrStop(),
        "AWESOME" => Quotes.ToAwesome(),
        "BB" => Quotes.ToBollingerBands(20, 2),
        "BETA" => Quotes.ToBeta(CompareQuotes, 20, BetaType.Standard),
        "BOP" => Quotes.ToBop(14),
        "CCI" => Quotes.ToCci(14),
        "CHAIKIN-OSC" => Quotes.ToChaikinOsc(),
        "CHANDELIER" or "CHEXIT" => Quotes.ToChandelier(),
        "CHOP" => Quotes.ToChop(14),
        "CMF" => Quotes.ToCmf(14),
        "CMO" => Quotes.ToCmo(14),
        "CORR" => Quotes.ToCorrelation(CompareQuotes, 20),
        "CRSI" => Quotes.ToConnorsRsi(3, 2, 100),
        "DEMA" => Quotes.ToDema(14),
        "DOJI" => Quotes.ToDoji(),
        "DONCHIAN" => Quotes.ToDonchian(20),
        "DPO" => Quotes.ToDpo(14),
        "DYNAMIC" => Quotes.ToDynamic(14),
        "ELDER-RAY" => Quotes.ToElderRay(13),
        "EMA" => Quotes.ToEma(20),
        "EPMA" => Quotes.ToEpma(14),
        "FCB" => Quotes.ToFcb(2),
        "FISHER" => Quotes.ToFisherTransform(10),
        "FORCE" => Quotes.ToForceIndex(2),
        "FRACTAL" => Quotes.ToFractal(),
        "GATOR" => Quotes.ToGator(),
        "HEIKINASHI" => Quotes.ToHeikinAshi(),
        "HMA" => Quotes.ToHma(14),
        "HTL" => Quotes.ToHtTrendline(),
        "HURST" => Quotes.ToHurst(100),
        "ICHIMOKU" => Quotes.ToIchimoku(),
        "KAMA" => Quotes.ToKama(10, 2, 30),
        "KELTNER" => Quotes.ToKeltner(20, 2, 10),
        "KVO" => Quotes.ToKvo(34, 55, 13),
        "MA-ENV" => Quotes.ToMaEnvelopes(20, 2.5, MaType.SMA),
        "MACD" => Quotes.ToMacd(12, 26, 9),
        "MAMA" => Quotes.ToMama(0.5, 0.05),
        "MARUBOZU" => Quotes.ToMarubozu(95),
        "MFI" => Quotes.ToMfi(14),
        "OBV" => Quotes.ToObv(),
        "PIVOT-POINTS" => Quotes.ToPivotPoints(PeriodSize.Month, PivotPointType.Standard),
        "PIVOTS" => Quotes.ToPivots(2, 2, 20),
        "PMO" => Quotes.ToPmo(35, 20, 10),
        "PRS" => Quotes.ToPrs(CompareQuotes),
        "PSAR" => Quotes.ToParabolicSar(),
        "PVO" => Quotes.ToPvo(),
        "QUOTEPART" => Quotes.ToQuotePart(CandlePart.OHL3),
        "RENKO" => Quotes.ToRenko(2.5m),
        "RENKO-ATR" => Quotes.ToRenkoAtr(14),
        "ROC" => Quotes.ToRoc(20),
        "ROC-WB" => Quotes.ToRocWb(20, 5, 5),
        "ROLLING-PIVOTS" => Quotes.ToRollingPivots(20, 0, PivotPointType.Standard),
        "RSI" => Quotes.ToRsi(14),
        "SLOPE" => Quotes.ToSlope(14),
        "SMA" => Quotes.ToSma(14),
        "SMA-ANALYSIS" => Quotes.ToSmaAnalysis(14),
        "SMI" => Quotes.ToSmi(13, 25, 2, 3),
        "SMMA" => Quotes.ToSmma(14),
        "STARC" => Quotes.ToStarcBands(5, 2, 10),
        "STC" => Quotes.ToStc(10, 23, 50),
        "STDEV" => Quotes.ToStdDev(14),
        "STDEV-CHANNELS" => Quotes.ToStdDevChannels(),
        "STOCH" => Quotes.ToStoch(14, 3, 3),
        "STOCH-RSI" => Quotes.ToStochRsi(14, 14, 3, 1),
        "SUPERTREND" => Quotes.ToSuperTrend(10, 3),
        "T3" => Quotes.ToT3(5, 0.7),
        "TEMA" => Quotes.ToTema(14),
        "TR" => Quotes.ToTr(),
        "TRIX" => Quotes.ToTrix(14),
        "TSI" => Quotes.ToTsi(25, 13, 7),
        "ULCER" => Quotes.ToUlcerIndex(14),
        "UO" => Quotes.ToUltimate(7, 14, 28),
        "VOL-STOP" => Quotes.ToVolatilityStop(7, 3),
        "VORTEX" => Quotes.ToVortex(14),
        "VWAP" => Quotes.ToVwap(),
        "VWMA" => Quotes.ToVwma(14),
        "WILLR" => Quotes.ToWilliamsR(),
        "WMA" => Quotes.ToWma(14),
        "ZIGZAG" => Quotes.ToZigZag(),
        _ => throw new NotSupportedException($"Indicator '{Keyword}' is not supported in ManualTestDirect."),
    };

    /* BUFFER BENCHMARKS */

    [Benchmark]
    public object Buffer() => Keyword switch {
        "ADL" => Quotes.ToAdlList(),
        "ADX" => Quotes.ToAdxList(14),
        "ALLIGATOR" => Quotes.ToAlligatorList(),
        "ALMA" => Quotes.ToAlmaList(9, 0.85, 6),
        "AROON" => Quotes.ToAroonList(),
        "ATR" => Quotes.ToAtrList(14),
        "ATR-STOP" => Quotes.ToAtrStopList(),
        "AWESOME" => Quotes.ToAwesomeList(),
        "BB" => Quotes.ToBollingerBandsList(20, 2),
        "BOP" => Quotes.ToBopList(14),
        "CCI" => Quotes.ToCciList(14),
        "CHAIKIN-OSC" => Quotes.ToChaikinOscList(),
        "CHANDELIER" or "CHEXIT" => Quotes.ToChandelierList(),
        "CHOP" => Quotes.ToChopList(14),
        "CMF" => Quotes.ToCmfList(14),
        "CMO" => Quotes.ToCmoList(14),
        "CRSI" => Quotes.ToConnorsRsiList(3, 2, 100),
        "DEMA" => Quotes.ToDemaList(14),
        "DOJI" => Quotes.ToDojiList(),
        "DONCHIAN" => Quotes.ToDonchianList(20),
        "DPO" => Quotes.ToDpoList(14),
        "DYNAMIC" => Quotes.ToDynamicList(14),
        "ELDER-RAY" => Quotes.ToElderRayList(13),
        "EMA" => Quotes.ToEmaList(20),
        "EPMA" => Quotes.ToEpmaList(14),
        "FCB" => Quotes.ToFcbList(2),
        "FISHER" => Quotes.ToFisherTransformList(10),
        "FORCE" => Quotes.ToForceIndexList(2),
        "FRACTAL" => Quotes.ToFractalList(),
        "GATOR" => Quotes.ToGatorList(),
        "HEIKINASHI" => Quotes.ToHeikinAshiList(),
        "HMA" => Quotes.ToHmaList(14),
        "HTL" => Quotes.ToHtTrendlineList(),
        "HURST" => Quotes.ToHurstList(100),
        "ICHIMOKU" => Quotes.ToIchimokuList(),
        "KAMA" => Quotes.ToKamaList(10, 2, 30),
        "KELTNER" => Quotes.ToKeltnerList(20, 2, 10),
        "KVO" => Quotes.ToKvoList(34, 55, 13),
        "MA-ENV" => Quotes.ToMaEnvelopesList(20, 2.5, MaType.SMA),
        "MACD" => Quotes.ToMacdList(12, 26, 9),
        "MAMA" => Quotes.ToMamaList(0.5, 0.05),
        "MARUBOZU" => Quotes.ToMarubozuList(95),
        "MFI" => Quotes.ToMfiList(14),
        "OBV" => Quotes.ToObvList(),
        "PMO" => Quotes.ToPmoList(35, 20, 10),
        "PSAR" => Quotes.ToParabolicSarList(),
        "PVO" => Quotes.ToPvoList(),
        "RENKO" => Quotes.ToRenkoList(2.5m),
        "ROC" => Quotes.ToRocList(20),
        "ROC-WB" => Quotes.ToRocWbList(20, 5, 5),
        "RSI" => Quotes.ToRsiList(14),
        "SLOPE" => Quotes.ToSlopeList(14),
        "SMA" => Quotes.ToSmaList(14),
        "SMA-ANALYSIS" => Quotes.ToSmaAnalysisList(14),
        "SMI" => Quotes.ToSmiList(13, 25, 2, 3),
        "SMMA" => Quotes.ToSmmaList(14),
        "STARC" => Quotes.ToStarcBandsList(5, 2, 10),
        "STC" => Quotes.ToStcList(10, 23, 50),
        "STDEV" => Quotes.ToStdDevList(14),
        "STOCH" => Quotes.ToStochList(14, 3, 3),
        "STOCH-RSI" => Quotes.ToStochRsiList(14, 14, 3, 1),
        "SUPERTREND" => Quotes.ToSuperTrendList(10, 3),
        "T3" => Quotes.ToT3List(5, 0.7),
        "TEMA" => Quotes.ToTemaList(14),
        "TR" => Quotes.ToTrList(),
        "TRIX" => Quotes.ToTrixList(14),
        "TSI" => Quotes.ToTsiList(25, 13, 7),
        "ULCER" => Quotes.ToUlcerIndexList(14),
        "UO" => Quotes.ToUltimateList(7, 14, 28),
        "VOL-STOP" => Quotes.ToVolatilityStopList(7, 3),
        "VORTEX" => Quotes.ToVortexList(14),
        "VWAP" => Quotes.ToVwapList(),
        "VWMA" => Quotes.ToVwmaList(14),
        "WILLR" => Quotes.ToWilliamsRList(),
        "WMA" => Quotes.ToWmaList(14),
        _ => throw new NotSupportedException($"Indicator '{Keyword}' is not supported in ManualTestDirect."),
    };

    /* STREAM HUB BENCHMARKS */

    [Benchmark]
    public object StreamHub() => Keyword switch {
        "ADL" => quoteHub.ToAdlHub().Results,
        "ADX" => quoteHub.ToAdxHub(14).Results,
        "ALLIGATOR" => quoteHub.ToAlligatorHub().Results,
        "ALMA" => quoteHub.ToAlmaHub(9, 0.85, 6).Results,
        "AROON" => quoteHub.ToAroonHub().Results,
        "ATR" => quoteHub.ToAtrHub(14).Results,
        "ATR-STOP" => quoteHub.ToAtrStopHub().Results,
        "AWESOME" => quoteHub.ToAwesomeHub().Results,
        "BB" => quoteHub.ToBollingerBandsHub(20, 2).Results,
        "BOP" => quoteHub.ToBopHub(14).Results,
        "CCI" => quoteHub.ToCciHub(14).Results,
        "CHAIKIN-OSC" => quoteHub.ToChaikinOscHub().Results,
        "CHANDELIER" or "CHEXIT" => quoteHub.ToChandelierHub().Results,
        "CHOP" => quoteHub.ToChopHub(14).Results,
        "CMF" => quoteHub.ToCmfHub(14).Results,
        "CMO" => quoteHub.ToCmoHub(14).Results,
        "CRSI" => quoteHub.ToConnorsRsiHub(3, 2, 100).Results,
        "DEMA" => quoteHub.ToDemaHub(14).Results,
        "DOJI" => quoteHub.ToDojiHub().Results,
        "DONCHIAN" => quoteHub.ToDonchianHub(20).Results,
        "DPO" => quoteHub.ToDpoHub(14).Results,
        "DYNAMIC" => quoteHub.ToDynamicHub(14).Results,
        "ELDER-RAY" => quoteHub.ToElderRayHub(13).Results,
        "EMA" => quoteHub.ToEmaHub(20).Results,
        "EPMA" => quoteHub.ToEpmaHub(14).Results,
        "FCB" => quoteHub.ToFcbHub(2).Results,
        "FISHER" => quoteHub.ToFisherTransformHub(10).Results,
        "FORCE" => quoteHub.ToForceIndexHub(2).Results,
        "FRACTAL" => quoteHub.ToFractalHub().Results,
        "GATOR" => quoteHub.ToGatorHub().Results,
        "HEIKINASHI" => quoteHub.ToHeikinAshiHub().Results,
        "HMA" => quoteHub.ToHmaHub(14).Results,
        "HTL" => quoteHub.ToHtTrendlineHub().Results,
        "HURST" => quoteHub.ToHurstHub(100).Results,
        "ICHIMOKU" => quoteHub.ToIchimokuHub().Results,
        "KAMA" => quoteHub.ToKamaHub(10, 2, 30).Results,
        "KELTNER" => quoteHub.ToKeltnerHub(20, 2, 10).Results,
        "KVO" => quoteHub.ToKvoHub(34, 55, 13).Results,
        "MA-ENV" => quoteHub.ToMaEnvelopesHub(20, 2.5, MaType.SMA).Results,
        "MACD" => quoteHub.ToMacdHub(12, 26, 9).Results,
        "MAMA" => quoteHub.ToMamaHub(0.5, 0.05).Results,
        "MARUBOZU" => quoteHub.ToMarubozuHub(95).Results,
        "MFI" => quoteHub.ToMfiHub(14).Results,
        "OBV" => quoteHub.ToObvHub().Results,
        "PIVOT-POINTS" => quoteHub.ToPivotPointsHub(PeriodSize.Month, PivotPointType.Standard).Results,
        "PIVOTS" => quoteHub.ToPivotsHub(2, 2, 20).Results,
        "PMO" => quoteHub.ToPmoHub(35, 20, 10).Results,
        "PSAR" => quoteHub.ToParabolicSarHub().Results,
        "PVO" => quoteHub.ToPvoHub().Results,
        "QUOTEPART" => quoteHub.ToQuotePartHub(CandlePart.OHL3).Results,
        "RENKO" => quoteHub.ToRenkoHub(2.5m).Results,
        "ROC" => quoteHub.ToRocHub(20).Results,
        "ROC-WB" => quoteHub.ToRocWbHub(20, 5, 5).Results,
        "ROLLING-PIVOTS" => quoteHub.ToRollingPivotsHub(20, 0, PivotPointType.Standard).Results,
        "RSI" => quoteHub.ToRsiHub(14).Results,
        "SLOPE" => quoteHub.ToSlopeHub(14).Results,
        "SMA" => quoteHub.ToSmaHub(14).Results,
        "SMA-ANALYSIS" => quoteHub.ToSmaAnalysisHub(14).Results,
        "SMI" => quoteHub.ToSmiHub(13, 25, 2, 3).Results,
        "SMMA" => quoteHub.ToSmmaHub(14).Results,
        "STARC" => quoteHub.ToStarcBandsHub(5, 2, 10).Results,
        "STC" => quoteHub.ToStcHub(10, 23, 50).Results,
        "STDEV" => quoteHub.ToStdDevHub(14).Results,
        "STOCH" => quoteHub.ToStochHub(14, 3, 3).Results,
        "STOCH-RSI" => quoteHub.ToStochRsiHub(14, 14, 3, 1).Results,
        "SUPERTREND" => quoteHub.ToSuperTrendHub(10, 3).Results,
        "T3" => quoteHub.ToT3Hub(5, 0.7).Results,
        "TEMA" => quoteHub.ToTemaHub(14).Results,
        "TR" => quoteHub.ToTrHub().Results,
        "TRIX" => quoteHub.ToTrixHub(14).Results,
        "TSI" => quoteHub.ToTsiHub(25, 13, 7).Results,
        "ULCER" => quoteHub.ToUlcerIndexHub(14).Results,
        "UO" => quoteHub.ToUltimateHub(7, 14, 28).Results,
        "VOL-STOP" => quoteHub.ToVolatilityStopHub(7, 3).Results,
        "VORTEX" => quoteHub.ToVortexHub(14).Results,
        "VWAP" => quoteHub.ToVwapHub().Results,
        "VWMA" => quoteHub.ToVwmaHub(14).Results,
        "WILLR" => quoteHub.ToWilliamsRHub().Results,
        "WMA" => quoteHub.ToWmaHub(14).Results,
        _ => throw new NotSupportedException($"Indicator '{Keyword}' is not supported in ManualTestDirect."),
    };

}
