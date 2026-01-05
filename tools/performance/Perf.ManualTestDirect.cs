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
    public void Series()
        => _ = Keyword switch {
            "ADL" => (object)Quotes.ToAdl(),
            "ADX" => (object)Quotes.ToAdx(14),
            "ALLIGATOR" => (object)Quotes.ToAlligator(),
            "ALMA" => (object)Quotes.ToAlma(9, 0.85, 6),
            "AROON" => (object)Quotes.ToAroon(),
            "ATR" => (object)Quotes.ToAtr(14),
            "ATR-STOP" => (object)Quotes.ToAtrStop(),
            "AWESOME" => (object)Quotes.ToAwesome(),
            "BB" => (object)Quotes.ToBollingerBands(20, 2),
            "BETA" => (object)Quotes.ToBeta(CompareQuotes, 20, BetaType.Standard),
            "BOP" => (object)Quotes.ToBop(14),
            "CCI" => (object)Quotes.ToCci(14),
            "CHAIKIN-OSC" => (object)Quotes.ToChaikinOsc(),
            "CHANDELIER" or "CHEXIT" => (object)Quotes.ToChandelier(),
            "CHOP" => (object)Quotes.ToChop(14),
            "CMF" => (object)Quotes.ToCmf(14),
            "CMO" => (object)Quotes.ToCmo(14),
            "CORR" => (object)Quotes.ToCorrelation(CompareQuotes, 20),
            "CRSI" => (object)Quotes.ToConnorsRsi(3, 2, 100),
            "DEMA" => (object)Quotes.ToDema(14),
            "DOJI" => (object)Quotes.ToDoji(),
            "DONCHIAN" => (object)Quotes.ToDonchian(),
            "DPO" => (object)Quotes.ToDpo(14),
            "DYNAMIC" => (object)Quotes.ToDynamic(14),
            "ELDER-RAY" => (object)Quotes.ToElderRay(13),
            "EMA" => (object)Quotes.ToEma(20),
            "EPMA" => (object)Quotes.ToEpma(14),
            "FCB" => (object)Quotes.ToFcb(2),
            "FISHER" => (object)Quotes.ToFisherTransform(10),
            "FORCE" => (object)Quotes.ToForceIndex(2),
            "FRACTAL" => (object)Quotes.ToFractal(),
            "GATOR" => (object)Quotes.ToGator(),
            "HEIKINASHI" => (object)Quotes.ToHeikinAshi(),
            "HMA" => (object)Quotes.ToHma(14),
            "HTL" => (object)Quotes.ToHtTrendline(),
            "HURST" => (object)Quotes.ToHurst(100),
            "ICHIMOKU" => (object)Quotes.ToIchimoku(),
            "KAMA" => (object)Quotes.ToKama(10, 2, 30),
            "KELTNER" => (object)Quotes.ToKeltner(20, 2, 10),
            "KVO" => (object)Quotes.ToKvo(34, 55, 13),
            "MA-ENV" => (object)Quotes.ToMaEnvelopes(20, 2.5, MaType.SMA),
            "MACD" => (object)Quotes.ToMacd(12, 26, 9),
            "MAMA" => (object)Quotes.ToMama(0.5, 0.05),
            "MARUBOZU" => (object)Quotes.ToMarubozu(95),
            "MFI" => (object)Quotes.ToMfi(14),
            "OBV" => (object)Quotes.ToObv(),
            "PIVOT-POINTS" => (object)Quotes.ToPivotPoints(PeriodSize.Month, PivotPointType.Standard),
            "PIVOTS" => (object)Quotes.ToPivots(2, 2, 20),
            "PMO" => (object)Quotes.ToPmo(35, 20, 10),
            "PRS" => (object)Quotes.ToPrs(CompareQuotes),
            "PSAR" => (object)Quotes.ToParabolicSar(),
            "PVO" => (object)Quotes.ToPvo(),
            "QUOTEPART" => (object)Quotes.ToQuotePart(CandlePart.OHL3),
            "RENKO" => (object)Quotes.ToRenko(2.5m),
            "RENKO-ATR" => (object)Quotes.ToRenko(14),
            "ROC" => (object)Quotes.ToRoc(20),
            "ROC-WB" => (object)Quotes.ToRocWb(20, 5, 5),
            "ROLLING-PIVOTS" => (object)Quotes.ToRollingPivots(20, 0, PivotPointType.Standard),
            "RSI" => (object)Quotes.ToRsi(14),
            "SLOPE" => (object)Quotes.ToSlope(14),
            "SMA" => (object)Quotes.ToSma(14),
            "SMA-ANALYSIS" => (object)Quotes.ToSmaAnalysis(14),
            "SMI" => (object)Quotes.ToSmi(13, 25, 2, 3),
            "SMMA" => (object)Quotes.ToSmma(14),
            "STARC" => (object)Quotes.ToStarcBands(5, 2, 10),
            "STC" => (object)Quotes.ToStc(10, 23, 50),
            "STDEV" => (object)Quotes.ToStdDev(14),
            "STDEV-CHANNELS" => (object)Quotes.ToStdDevChannels(),
            "STOCH" => (object)Quotes.ToStoch(14, 3, 3),
            "STOCH-RSI" => (object)Quotes.ToStochRsi(14, 14, 3, 1),
            "SUPERTREND" => (object)Quotes.ToSuperTrend(10, 3),
            "T3" => (object)Quotes.ToT3(5, 0.7),
            "TEMA" => (object)Quotes.ToTema(14),
            "TR" => (object)Quotes.ToTr(),
            "TRIX" => (object)Quotes.ToTrix(14),
            "TSI" => (object)Quotes.ToTsi(25, 13, 7),
            "ULCER" => (object)Quotes.ToUlcerIndex(14),
            "UO" => (object)Quotes.ToUltimate(7, 14, 28),
            "VOL-STOP" => (object)Quotes.ToVolatilityStop(7, 3),
            "VORTEX" => (object)Quotes.ToVortex(14),
            "VWAP" => (object)Quotes.ToVwap(),
            "VWMA" => (object)Quotes.ToVwma(14),
            "WILLR" => (object)Quotes.ToWilliamsR(),
            "WMA" => (object)Quotes.ToWma(14),
            "ZIGZAG" => (object)Quotes.ToZigZag(),
            _ => throw new NotSupportedException($"Indicator '{Keyword}' is not supported in ManualTestDirect."),
        };

    /* BUFFER BENCHMARKS */

    [Benchmark]
    public void Buffer()
        => _ = Keyword switch {
            "ADL" => (object)Quotes.ToAdlList(),
            "ADX" => (object)Quotes.ToAdxList(14),
            "ALLIGATOR" => (object)Quotes.ToAlligatorList(),
            "ALMA" => (object)Quotes.ToAlmaList(9, 0.85, 6),
            "AROON" => (object)Quotes.ToAroonList(),
            "ATR" => (object)Quotes.ToAtrList(14),
            "ATR-STOP" => (object)Quotes.ToAtrStopList(),
            "AWESOME" => (object)Quotes.ToAwesomeList(),
            "BB" => (object)Quotes.ToBollingerBandsList(20, 2),
            "BOP" => (object)Quotes.ToBopList(14),
            "CCI" => (object)Quotes.ToCciList(14),
            "CHAIKIN-OSC" => (object)Quotes.ToChaikinOscList(),
            "CHANDELIER" or "CHEXIT" => (object)Quotes.ToChandelierList(),
            "CHOP" => (object)Quotes.ToChopList(14),
            "CMF" => (object)Quotes.ToCmfList(14),
            "CMO" => (object)Quotes.ToCmoList(14),
            "CRSI" => (object)Quotes.ToConnorsRsiList(3, 2, 100),
            "DEMA" => (object)Quotes.ToDemaList(14),
            "DOJI" => (object)Quotes.ToDojiList(),
            "DONCHIAN" => (object)Quotes.ToDonchianList(),
            "DPO" => (object)Quotes.ToDpoList(14),
            "DYNAMIC" => (object)Quotes.ToDynamicList(14),
            "ELDER-RAY" => (object)Quotes.ToElderRayList(13),
            "EMA" => (object)Quotes.ToEmaList(20),
            "EPMA" => (object)Quotes.ToEpmaList(14),
            "FCB" => (object)Quotes.ToFcbList(2),
            "FISHER" => (object)Quotes.ToFisherTransformList(10),
            "FORCE" => (object)Quotes.ToForceIndexList(2),
            "FRACTAL" => (object)Quotes.ToFractalList(),
            "GATOR" => (object)Quotes.ToGatorList(),
            "HEIKINASHI" => (object)Quotes.ToHeikinAshiList(),
            "HMA" => (object)Quotes.ToHmaList(14),
            "HTL" => (object)Quotes.ToHtTrendlineList(),
            "HURST" => (object)Quotes.ToHurstList(100),
            "ICHIMOKU" => (object)Quotes.ToIchimokuList(),
            "KAMA" => (object)Quotes.ToKamaList(10, 2, 30),
            "KELTNER" => (object)Quotes.ToKeltnerList(20, 2, 10),
            "KVO" => (object)Quotes.ToKvoList(34, 55, 13),
            "MA-ENV" => (object)Quotes.ToMaEnvelopesList(20, 2.5, MaType.SMA),
            "MACD" => (object)Quotes.ToMacdList(12, 26, 9),
            "MAMA" => (object)Quotes.ToMamaList(0.5, 0.05),
            "MARUBOZU" => (object)Quotes.ToMarubozuList(95),
            "MFI" => (object)Quotes.ToMfiList(14),
            "OBV" => (object)Quotes.ToObvList(),
            "PMO" => (object)Quotes.ToPmoList(35, 20, 10),
            "PSAR" => (object)Quotes.ToParabolicSarList(),
            "PVO" => (object)Quotes.ToPvoList(),
            "RENKO" => (object)Quotes.ToRenkoList(2.5m),
            "RENKO-ATR" => (object)Quotes.ToRenkoList(14m),
            "ROC" => (object)Quotes.ToRocList(20),
            "ROC-WB" => (object)Quotes.ToRocWbList(20, 5, 5),
            "RSI" => (object)Quotes.ToRsiList(14),
            "SLOPE" => (object)Quotes.ToSlopeList(14),
            "SMA" => (object)Quotes.ToSmaList(14),
            "SMA-ANALYSIS" => (object)Quotes.ToSmaAnalysisList(14),
            "SMI" => (object)Quotes.ToSmiList(13, 25, 2, 3),
            "SMMA" => (object)Quotes.ToSmmaList(14),
            "STARC" => (object)Quotes.ToStarcBandsList(5, 2, 10),
            "STC" => (object)Quotes.ToStcList(10, 23, 50),
            "STDEV" => (object)Quotes.ToStdDevList(14),
            "STOCH" => (object)Quotes.ToStochList(14, 3, 3),
            "STOCH-RSI" => (object)Quotes.ToStochRsiList(14, 14, 3, 1),
            "SUPERTREND" => (object)Quotes.ToSuperTrendList(10, 3),
            "T3" => (object)Quotes.ToT3List(5, 0.7),
            "TEMA" => (object)Quotes.ToTemaList(14),
            "TR" => (object)Quotes.ToTrList(),
            "TRIX" => (object)Quotes.ToTrixList(14),
            "TSI" => (object)Quotes.ToTsiList(25, 13, 7),
            "ULCER" => (object)Quotes.ToUlcerIndexList(14),
            "UO" => (object)Quotes.ToUltimateList(7, 14, 28),
            "VOL-STOP" => (object)Quotes.ToVolatilityStopList(7, 3),
            "VORTEX" => (object)Quotes.ToVortexList(14),
            "VWAP" => (object)Quotes.ToVwapList(),
            "VWMA" => (object)Quotes.ToVwmaList(14),
            "WILLR" => (object)Quotes.ToWilliamsRList(),
            "WMA" => (object)Quotes.ToWmaList(14),
            _ => throw new NotSupportedException($"Indicator '{Keyword}' is not supported in ManualTestDirect."),
        };

    /* STREAM HUB BENCHMARKS */

    [Benchmark]
    public void StreamHub()
        => _ = Keyword switch {
            "ADL" => (object)quoteHub.ToAdlHub().Results,
            "ADX" => (object)quoteHub.ToAdxHub(14).Results,
            "ALLIGATOR" => (object)quoteHub.ToAlligatorHub().Results,
            "ALMA" => (object)quoteHub.ToAlmaHub(9, 0.85, 6).Results,
            "AROON" => (object)quoteHub.ToAroonHub().Results,
            "ATR" => (object)quoteHub.ToAtrHub(14).Results,
            "ATR-STOP" => (object)quoteHub.ToAtrStopHub().Results,
            "AWESOME" => (object)quoteHub.ToAwesomeHub().Results,
            "BB" => (object)quoteHub.ToBollingerBandsHub(20, 2).Results,
            "BOP" => (object)quoteHub.ToBopHub(14).Results,
            "CCI" => (object)quoteHub.ToCciHub(14).Results,
            "CHAIKIN-OSC" => (object)quoteHub.ToChaikinOscHub().Results,
            "CHANDELIER" or "CHEXIT" => (object)quoteHub.ToChandelierHub().Results,
            "CHOP" => (object)quoteHub.ToChopHub(14).Results,
            "CMF" => (object)quoteHub.ToCmfHub(14).Results,
            "CMO" => (object)quoteHub.ToCmoHub(14).Results,
            "CRSI" => (object)quoteHub.ToConnorsRsiHub(3, 2, 100).Results,
            "DEMA" => (object)quoteHub.ToDemaHub(14).Results,
            "DOJI" => (object)quoteHub.ToDojiHub().Results,
            "DONCHIAN" => (object)quoteHub.ToDonchianHub(20).Results,
            "DPO" => (object)quoteHub.ToDpoHub(14).Results,
            "DYNAMIC" => (object)quoteHub.ToDynamicHub(14).Results,
            "ELDER-RAY" => (object)quoteHub.ToElderRayHub(13).Results,
            "EMA" => (object)quoteHub.ToEmaHub(20).Results,
            "EPMA" => (object)quoteHub.ToEpmaHub(14).Results,
            "FCB" => (object)quoteHub.ToFcbHub(2).Results,
            "FISHER" => (object)quoteHub.ToFisherTransformHub(10).Results,
            "FORCE" => (object)quoteHub.ToForceIndexHub(2).Results,
            "FRACTAL" => (object)quoteHub.ToFractalHub().Results,
            "GATOR" => (object)quoteHub.ToGatorHub().Results,
            "HEIKINASHI" => (object)quoteHub.ToHeikinAshiHub().Results,
            "HMA" => (object)quoteHub.ToHmaHub(14).Results,
            "HTL" => (object)quoteHub.ToHtTrendlineHub().Results,
            "HURST" => (object)quoteHub.ToHurstHub(100).Results,
            "ICHIMOKU" => (object)quoteHub.ToIchimokuHub().Results,
            "KAMA" => (object)quoteHub.ToKamaHub(10, 2, 30).Results,
            "KELTNER" => (object)quoteHub.ToKeltnerHub(20, 2, 10).Results,
            "KVO" => (object)quoteHub.ToKvoHub(34, 55, 13).Results,
            "MA-ENV" => (object)quoteHub.ToMaEnvelopesHub(20, 2.5, MaType.SMA).Results,
            "MACD" => (object)quoteHub.ToMacdHub(12, 26, 9).Results,
            "MAMA" => (object)quoteHub.ToMamaHub(0.5, 0.05).Results,
            "MARUBOZU" => (object)quoteHub.ToMarubozuHub(95).Results,
            "MFI" => (object)quoteHub.ToMfiHub(14).Results,
            "OBV" => (object)quoteHub.ToObvHub().Results,
            "PIVOT-POINTS" => (object)quoteHub.ToPivotPointsHub(PeriodSize.Month, PivotPointType.Standard).Results,
            "PIVOTS" => (object)quoteHub.ToPivotsHub(2, 2, 20).Results,
            "PMO" => (object)quoteHub.ToPmoHub(35, 20, 10).Results,
            "PSAR" => (object)quoteHub.ToParabolicSarHub().Results,
            "PVO" => (object)quoteHub.ToPvoHub().Results,
            "QUOTEPART" => (object)quoteHub.ToQuotePartHub(CandlePart.OHL3).Results,
            "RENKO" => (object)quoteHub.ToRenkoHub(2.5m).Results,
            "RENKO-ATR" => (object)quoteHub.ToRenkoHub(14).Results,
            "ROC" => (object)quoteHub.ToRocHub(20).Results,
            "ROC-WB" => (object)quoteHub.ToRocWbHub(20, 5, 5).Results,
            "ROLLING-PIVOTS" => (object)quoteHub.ToRollingPivotsHub(20, 0, PivotPointType.Standard).Results,
            "RSI" => (object)quoteHub.ToRsiHub(14).Results,
            "SLOPE" => (object)quoteHub.ToSlopeHub(14).Results,
            "SMA" => (object)quoteHub.ToSmaHub(14).Results,
            "SMA-ANALYSIS" => (object)quoteHub.ToSmaAnalysisHub(14).Results,
            "SMI" => (object)quoteHub.ToSmiHub(13, 25, 2, 3).Results,
            "SMMA" => (object)quoteHub.ToSmmaHub(14).Results,
            "STARC" => (object)quoteHub.ToStarcBandsHub(5, 2, 10).Results,
            "STC" => (object)quoteHub.ToStcHub(10, 23, 50).Results,
            "STDEV" => (object)quoteHub.ToStdDevHub(14).Results,
            "STOCH" => (object)quoteHub.ToStochHub(14, 3, 3).Results,
            "STOCH-RSI" => (object)quoteHub.ToStochRsiHub(14, 14, 3, 1).Results,
            "SUPERTREND" => (object)quoteHub.ToSuperTrendHub(10, 3).Results,
            "T3" => (object)quoteHub.ToT3Hub(5, 0.7).Results,
            "TEMA" => (object)quoteHub.ToTemaHub(14).Results,
            "TR" => (object)quoteHub.ToTrHub().Results,
            "TRIX" => (object)quoteHub.ToTrixHub(14).Results,
            "TSI" => (object)quoteHub.ToTsiHub(25, 13, 7).Results,
            "ULCER" => (object)quoteHub.ToUlcerIndexHub(14).Results,
            "UO" => (object)quoteHub.ToUltimateHub(7, 14, 28).Results,
            "VOL-STOP" => (object)quoteHub.ToVolatilityStopHub(7, 3).Results,
            "VORTEX" => (object)quoteHub.ToVortexHub(14).Results,
            "VWAP" => (object)quoteHub.ToVwapHub().Results,
            "VWMA" => (object)quoteHub.ToVwmaHub(14).Results,
            "WILLR" => (object)quoteHub.ToWilliamsRHub().Results,
            "WMA" => (object)quoteHub.ToWmaHub(14).Results,
            _ => throw new NotSupportedException($"Indicator '{Keyword}' is not supported in ManualTestDirect."),
        };

}
