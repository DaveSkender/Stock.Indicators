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
    {
        switch (Keyword)
        {
            case "ADL":
                Quotes.ToAdl();
                break;
            case "ADX":
                Quotes.ToAdx(14);
                break;
            case "ALLIGATOR":
                Quotes.ToAlligator();
                break;
            case "ALMA":
                Quotes.ToAlma(9, 0.85, 6);
                break;
            case "AROON":
                Quotes.ToAroon();
                break;
            case "ATR":
                Quotes.ToAtr(14);
                break;
            case "ATR-STOP":
                Quotes.ToAtrStop();
                break;
            case "AWESOME":
                Quotes.ToAwesome();
                break;
            case "BB":
                Quotes.ToBollingerBands(20, 2);
                break;
            case "BETA":
                Quotes.ToBeta(CompareQuotes, 20, BetaType.Standard);
                break;
            case "BOP":
                Quotes.ToBop(14);
                break;
            case "CCI":
                Quotes.ToCci(14);
                break;
            case "CHAIKIN-OSC":
                Quotes.ToChaikinOsc();
                break;
            case "CHANDELIER":
            case "CHEXIT":
                Quotes.ToChandelier();
                break;
            case "CHOP":
                Quotes.ToChop(14);
                break;
            case "CMF":
                Quotes.ToCmf(14);
                break;
            case "CMO":
                Quotes.ToCmo(14);
                break;
            case "CORR":
                Quotes.ToCorrelation(CompareQuotes, 20);
                break;
            case "CRSI":
                Quotes.ToConnorsRsi(3, 2, 100);
                break;
            case "DEMA":
                Quotes.ToDema(14);
                break;
            case "DOJI":
                Quotes.ToDoji();
                break;
            case "DONCHIAN":
                Quotes.ToDonchian();
                break;
            case "DPO":
                Quotes.ToDpo(14);
                break;
            case "DYNAMIC":
                Quotes.ToDynamic(14);
                break;
            case "ELDER-RAY":
                Quotes.ToElderRay(13);
                break;
            case "EMA":
                Quotes.ToEma(20);
                break;
            case "EPMA":
                Quotes.ToEpma(14);
                break;
            case "FCB":
                Quotes.ToFcb(2);
                break;
            case "FISHER":
                Quotes.ToFisherTransform(10);
                break;
            case "FORCE":
                Quotes.ToForceIndex(2);
                break;
            case "FRACTAL":
                Quotes.ToFractal();
                break;
            case "GATOR":
                Quotes.ToGator();
                break;
            case "HEIKINASHI":
                Quotes.ToHeikinAshi();
                break;
            case "HMA":
                Quotes.ToHma(14);
                break;
            case "HTL":
                Quotes.ToHtTrendline();
                break;
            case "HURST":
                Quotes.ToHurst(100);
                break;
            case "ICHIMOKU":
                Quotes.ToIchimoku();
                break;
            case "KAMA":
                Quotes.ToKama(10, 2, 30);
                break;
            case "KELTNER":
                Quotes.ToKeltner(20, 2, 10);
                break;
            case "KVO":
                Quotes.ToKvo(34, 55, 13);
                break;
            case "MA-ENV":
                Quotes.ToMaEnvelopes(20, 2.5, MaType.SMA);
                break;
            case "MACD":
                Quotes.ToMacd(12, 26, 9);
                break;
            case "MAMA":
                Quotes.ToMama(0.5, 0.05);
                break;
            case "MARUBOZU":
                Quotes.ToMarubozu(95);
                break;
            case "MFI":
                Quotes.ToMfi(14);
                break;
            case "OBV":
                Quotes.ToObv();
                break;
            case "PIVOT-POINTS":
                Quotes.ToPivotPoints(PeriodSize.Month, PivotPointType.Standard);
                break;
            case "PIVOTS":
                Quotes.ToPivots(2, 2, 20);
                break;
            case "PMO":
                Quotes.ToPmo(35, 20, 10);
                break;
            case "PRS":
                Quotes.ToPrs(CompareQuotes);
                break;
            case "PSAR":
                Quotes.ToParabolicSar();
                break;
            case "PVO":
                Quotes.ToPvo();
                break;
            case "QUOTEPART":
                Quotes.ToQuotePart(CandlePart.OHL3);
                break;
            case "RENKO":
                Quotes.ToRenko(2.5m);
                break;
            case "RENKO-ATR":
                Quotes.ToRenko(14);
                break;
            case "ROC":
                Quotes.ToRoc(20);
                break;
            case "ROC-WB":
                Quotes.ToRocWb(20, 5, 5);
                break;
            case "ROLLING-PIVOTS":
                Quotes.ToRollingPivots(20, 0, PivotPointType.Standard);
                break;
            case "RSI":
                Quotes.ToRsi(14);
                break;
            case "SLOPE":
                Quotes.ToSlope(14);
                break;
            case "SMA":
                Quotes.ToSma(14);
                break;
            case "SMA-ANALYSIS":
                Quotes.ToSmaAnalysis(14);
                break;
            case "SMI":
                Quotes.ToSmi(13, 25, 2, 3);
                break;
            case "SMMA":
                Quotes.ToSmma(14);
                break;
            case "STARC":
                Quotes.ToStarcBands(5, 2, 10);
                break;
            case "STC":
                Quotes.ToStc(10, 23, 50);
                break;
            case "STDEV":
                Quotes.ToStdDev(14);
                break;
            case "STDEV-CHANNELS":
                Quotes.ToStdDevChannels();
                break;
            case "STOCH":
                Quotes.ToStoch(14, 3, 3);
                break;
            case "STOCH-RSI":
                Quotes.ToStochRsi(14, 14, 3, 1);
                break;
            case "SUPERTREND":
                Quotes.ToSuperTrend(10, 3);
                break;
            case "T3":
                Quotes.ToT3(5, 0.7);
                break;
            case "TEMA":
                Quotes.ToTema(14);
                break;
            case "TR":
                Quotes.ToTr();
                break;
            case "TRIX":
                Quotes.ToTrix(14);
                break;
            case "TSI":
                Quotes.ToTsi(25, 13, 7);
                break;
            case "ULCER":
                Quotes.ToUlcerIndex(14);
                break;
            case "UO":
                Quotes.ToUltimate(7, 14, 28);
                break;
            case "VOL-STOP":
                Quotes.ToVolatilityStop(7, 3);
                break;
            case "VORTEX":
                Quotes.ToVortex(14);
                break;
            case "VWAP":
                Quotes.ToVwap();
                break;
            case "VWMA":
                Quotes.ToVwma(14);
                break;
            case "WILLR":
                Quotes.ToWilliamsR();
                break;
            case "WMA":
                Quotes.ToWma(14);
                break;
            case "ZIGZAG":
                Quotes.ToZigZag();
                break;
            default:
                throw new NotSupportedException($"Indicator '{Keyword}' is not supported in ManualTestDirect.");
        }
    }

    /* BUFFER BENCHMARKS */

    [Benchmark]
    public void Buffer()
    {
        switch (Keyword)
        {
            case "ADL":
                {
                    Quotes.ToAdlList();
                    break;
                }
            case "ADX":
                {
                    Quotes.ToAdxList(14);
                    break;
                }
            case "ALLIGATOR":
                {
                    Quotes.ToAlligatorList();
                    break;
                }
            case "ALMA":
                {
                    Quotes.ToAlmaList(9, 0.85, 6);
                    break;
                }
            case "AROON":
                {
                    Quotes.ToAroonList();
                    break;
                }
            case "ATR":
                {
                    Quotes.ToAtrList(14);
                    break;
                }
            case "ATR-STOP":
                {
                    Quotes.ToAtrStopList();
                    break;
                }
            case "AWESOME":
                {
                    Quotes.ToAwesomeList();
                    break;
                }
            case "BB":
                {
                    Quotes.ToBollingerBandsList(20, 2);
                    break;
                }
            case "BOP":
                {
                    Quotes.ToBopList(14);
                    break;
                }
            case "CCI":
                {
                    Quotes.ToCciList(14);
                    break;
                }
            case "CHAIKIN-OSC":
                {
                    Quotes.ToChaikinOscList();
                    break;
                }
            case "CHANDELIER":
            case "CHEXIT":
                {
                    Quotes.ToChandelierList();
                    break;
                }
            case "CHOP":
                {
                    Quotes.ToChopList(14);
                    break;
                }
            case "CMF":
                {
                    Quotes.ToCmfList(14);
                    break;
                }
            case "CMO":
                {
                    Quotes.ToCmoList(14);
                    break;
                }
            case "CRSI":
                {
                    Quotes.ToConnorsRsiList(3, 2, 100);
                    break;
                }
            case "DEMA":
                {
                    Quotes.ToDemaList(14);
                    break;
                }
            case "DOJI":
                {
                    Quotes.ToDojiList();
                    break;
                }
            case "DONCHIAN":
                {
                    Quotes.ToDonchianList();
                    break;
                }
            case "DPO":
                {
                    Quotes.ToDpoList(14);
                    break;
                }
            case "DYNAMIC":
                {
                    Quotes.ToDynamicList(14);
                    break;
                }
            case "ELDER-RAY":
                {
                    Quotes.ToElderRayList(13);
                    break;
                }
            case "EMA":
                {
                    Quotes.ToEmaList(20);
                    break;
                }
            case "EPMA":
                {
                    Quotes.ToEpmaList(14);
                    break;
                }
            case "FCB":
                {
                    Quotes.ToFcbList(2);
                    break;
                }
            case "FISHER":
                {
                    Quotes.ToFisherTransformList(10);
                    break;
                }
            case "FORCE":
                {
                    Quotes.ToForceIndexList(2);
                    break;
                }
            case "FRACTAL":
                {
                    Quotes.ToFractalList();
                    break;
                }
            case "GATOR":
                {
                    Quotes.ToGatorList();
                    break;
                }
            case "HEIKINASHI":
                {
                    Quotes.ToHeikinAshiList();
                    break;
                }
            case "HMA":
                {
                    Quotes.ToHmaList(14);
                    break;
                }
            case "HTL":
                {
                    Quotes.ToHtTrendlineList();
                    break;
                }
            case "HURST":
                {
                    Quotes.ToHurstList(100);
                    break;
                }
            case "ICHIMOKU":
                {
                    Quotes.ToIchimokuList();
                    break;
                }
            case "KAMA":
                {
                    Quotes.ToKamaList(10, 2, 30);
                    break;
                }
            case "KELTNER":
                {
                    Quotes.ToKeltnerList(20, 2, 10);
                    break;
                }
            case "KVO":
                {
                    Quotes.ToKvoList(34, 55, 13);
                    break;
                }
            case "MA-ENV":
                {
                    Quotes.ToMaEnvelopesList(20, 2.5, MaType.SMA);
                    break;
                }
            case "MACD":
                {
                    Quotes.ToMacdList(12, 26, 9);
                    break;
                }
            case "MAMA":
                {
                    Quotes.ToMamaList(0.5, 0.05);
                    break;
                }
            case "MARUBOZU":
                {
                    Quotes.ToMarubozuList(95);
                    break;
                }
            case "MFI":
                {
                    Quotes.ToMfiList(14);
                    break;
                }
            case "OBV":
                {
                    Quotes.ToObvList();
                    break;
                }
            case "PMO":
                {
                    Quotes.ToPmoList(35, 20, 10);
                    break;
                }
            case "PSAR":
                {
                    Quotes.ToParabolicSarList();
                    break;
                }
            case "PVO":
                {
                    Quotes.ToPvoList();
                    break;
                }
            case "RENKO":
                {
                    Quotes.ToRenkoList(2.5m);
                    break;
                }
            case "RENKO-ATR":
                {
                    Quotes.ToRenkoList(14m);
                    break;
                }
            case "ROC":
                {
                    Quotes.ToRocList(20);
                    break;
                }
            case "ROC-WB":
                {
                    Quotes.ToRocWbList(20, 5, 5);
                    break;
                }
            case "RSI":
                {
                    Quotes.ToRsiList(14);
                    break;
                }
            case "SLOPE":
                {
                    Quotes.ToSlopeList(14);
                    break;
                }
            case "SMA":
                {
                    Quotes.ToSmaList(14);
                    break;
                }
            case "SMA-ANALYSIS":
                {
                    Quotes.ToSmaAnalysisList(14);
                    break;
                }
            case "SMI":
                {
                    Quotes.ToSmiList(13, 25, 2, 3);
                    break;
                }
            case "SMMA":
                {
                    Quotes.ToSmmaList(14);
                    break;
                }
            case "STARC":
                {
                    Quotes.ToStarcBandsList(5, 2, 10);
                    break;
                }
            case "STC":
                {
                    Quotes.ToStcList(10, 23, 50);
                    break;
                }
            case "STDEV":
                {
                    Quotes.ToStdDevList(14);
                    break;
                }
            case "STOCH":
                {
                    Quotes.ToStochList(14, 3, 3);
                    break;
                }
            case "STOCH-RSI":
                {
                    Quotes.ToStochRsiList(14, 14, 3, 1);
                    break;
                }
            case "SUPERTREND":
                {
                    Quotes.ToSuperTrendList(10, 3);
                    break;
                }
            case "T3":
                {
                    Quotes.ToT3List(5, 0.7);
                    break;
                }
            case "TEMA":
                {
                    Quotes.ToTemaList(14);
                    break;
                }
            case "TR":
                {
                    Quotes.ToTrList();
                    break;
                }
            case "TRIX":
                {
                    Quotes.ToTrixList(14);
                    break;
                }
            case "TSI":
                {
                    Quotes.ToTsiList(25, 13, 7);
                    break;
                }
            case "ULCER":
                {
                    Quotes.ToUlcerIndexList(14);
                    break;
                }
            case "UO":
                {
                    Quotes.ToUltimateList(7, 14, 28);
                    break;
                }
            case "VOL-STOP":
                {
                    Quotes.ToVolatilityStopList(7, 3);
                    break;
                }
            case "VORTEX":
                {
                    Quotes.ToVortexList(14);
                    break;
                }
            case "VWAP":
                {
                    Quotes.ToVwapList();
                    break;
                }
            case "VWMA":
                {
                    Quotes.ToVwmaList(14);
                    break;
                }
            case "WILLR":
                {
                    Quotes.ToWilliamsRList();
                    break;
                }
            case "WMA":
                {
                    Quotes.ToWmaList(14);
                    break;
                }
            default:
                throw new NotSupportedException($"Indicator '{Keyword}' is not supported in ManualTestDirect.");
        }
    }

    /* STREAM HUB BENCHMARKS */

    [Benchmark]
    public void StreamHub()
    {
        switch (Keyword)
        {
            case "ADL":
                _ = quoteHub.ToAdlHub().Results;
                break;
            case "ADX":
                _ = quoteHub.ToAdxHub(14).Results;
                break;
            case "ALLIGATOR":
                _ = quoteHub.ToAlligatorHub().Results;
                break;
            case "ALMA":
                _ = quoteHub.ToAlmaHub(9, 0.85, 6).Results;
                break;
            case "AROON":
                _ = quoteHub.ToAroonHub().Results;
                break;
            case "ATR":
                _ = quoteHub.ToAtrHub(14).Results;
                break;
            case "ATR-STOP":
                _ = quoteHub.ToAtrStopHub().Results;
                break;
            case "AWESOME":
                _ = quoteHub.ToAwesomeHub().Results;
                break;
            case "BB":
                _ = quoteHub.ToBollingerBandsHub(20, 2).Results;
                break;
            case "BOP":
                _ = quoteHub.ToBopHub(14).Results;
                break;
            case "CCI":
                _ = quoteHub.ToCciHub(14).Results;
                break;
            case "CHAIKIN-OSC":
                _ = quoteHub.ToChaikinOscHub().Results;
                break;
            case "CHANDELIER":
            case "CHEXIT":
                _ = quoteHub.ToChandelierHub().Results;
                break;
            case "CHOP":
                _ = quoteHub.ToChopHub(14).Results;
                break;
            case "CMF":
                _ = quoteHub.ToCmfHub(14).Results;
                break;
            case "CMO":
                _ = quoteHub.ToCmoHub(14).Results;
                break;
            case "CRSI":
                _ = quoteHub.ToConnorsRsiHub(3, 2, 100).Results;
                break;
            case "DEMA":
                _ = quoteHub.ToDemaHub(14).Results;
                break;
            case "DOJI":
                _ = quoteHub.ToDojiHub().Results;
                break;
            case "DONCHIAN":
                _ = quoteHub.ToDonchianHub(20).Results;
                break;
            case "DPO":
                _ = quoteHub.ToDpoHub(14).Results;
                break;
            case "DYNAMIC":
                _ = quoteHub.ToDynamicHub(14).Results;
                break;
            case "ELDER-RAY":
                _ = quoteHub.ToElderRayHub(13).Results;
                break;
            case "EMA":
                _ = quoteHub.ToEmaHub(20).Results;
                break;
            case "EPMA":
                _ = quoteHub.ToEpmaHub(14).Results;
                break;
            case "FCB":
                _ = quoteHub.ToFcbHub(2).Results;
                break;
            case "FISHER":
                _ = quoteHub.ToFisherTransformHub(10).Results;
                break;
            case "FORCE":
                _ = quoteHub.ToForceIndexHub(2).Results;
                break;
            case "FRACTAL":
                _ = quoteHub.ToFractalHub().Results;
                break;
            case "GATOR":
                _ = quoteHub.ToGatorHub().Results;
                break;
            case "HEIKINASHI":
                _ = quoteHub.ToHeikinAshiHub().Results;
                break;
            case "HMA":
                _ = quoteHub.ToHmaHub(14).Results;
                break;
            case "HTL":
                _ = quoteHub.ToHtTrendlineHub().Results;
                break;
            case "HURST":
                _ = quoteHub.ToHurstHub(100).Results;
                break;
            case "ICHIMOKU":
                _ = quoteHub.ToIchimokuHub().Results;
                break;
            case "KAMA":
                _ = quoteHub.ToKamaHub(10, 2, 30).Results;
                break;
            case "KELTNER":
                _ = quoteHub.ToKeltnerHub(20, 2, 10).Results;
                break;
            case "KVO":
                _ = quoteHub.ToKvoHub(34, 55, 13).Results;
                break;
            case "MA-ENV":
                _ = quoteHub.ToMaEnvelopesHub(20, 2.5, MaType.SMA).Results;
                break;
            case "MACD":
                _ = quoteHub.ToMacdHub(12, 26, 9).Results;
                break;
            case "MAMA":
                _ = quoteHub.ToMamaHub(0.5, 0.05).Results;
                break;
            case "MARUBOZU":
                _ = quoteHub.ToMarubozuHub(95).Results;
                break;
            case "MFI":
                _ = quoteHub.ToMfiHub(14).Results;
                break;
            case "OBV":
                _ = quoteHub.ToObvHub().Results;
                break;
            case "PIVOT-POINTS":
                _ = quoteHub.ToPivotPointsHub(PeriodSize.Month, PivotPointType.Standard).Results;
                break;
            case "PIVOTS":
                _ = quoteHub.ToPivotsHub(2, 2, 20).Results;
                break;
            case "PMO":
                _ = quoteHub.ToPmoHub(35, 20, 10).Results;
                break;
            case "PSAR":
                _ = quoteHub.ToParabolicSarHub().Results;
                break;
            case "PVO":
                _ = quoteHub.ToPvoHub().Results;
                break;
            case "QUOTEPART":
                _ = quoteHub.ToQuotePartHub(CandlePart.OHL3).Results;
                break;
            case "RENKO":
                _ = quoteHub.ToRenkoHub(2.5m).Results;
                break;
            case "RENKO-ATR":
                _ = quoteHub.ToRenkoHub(14).Results;
                break;
            case "ROC":
                _ = quoteHub.ToRocHub(20).Results;
                break;
            case "ROC-WB":
                _ = quoteHub.ToRocWbHub(20, 5, 5).Results;
                break;
            case "ROLLING-PIVOTS":
                _ = quoteHub.ToRollingPivotsHub(20, 0, PivotPointType.Standard).Results;
                break;
            case "RSI":
                _ = quoteHub.ToRsiHub(14).Results;
                break;
            case "SLOPE":
                _ = quoteHub.ToSlopeHub(14).Results;
                break;
            case "SMA":
                _ = quoteHub.ToSmaHub(14).Results;
                break;
            case "SMA-ANALYSIS":
                _ = quoteHub.ToSmaAnalysisHub(14).Results;
                break;
            case "SMI":
                _ = quoteHub.ToSmiHub(13, 25, 2, 3).Results;
                break;
            case "SMMA":
                _ = quoteHub.ToSmmaHub(14).Results;
                break;
            case "STARC":
                _ = quoteHub.ToStarcBandsHub(5, 2, 10).Results;
                break;
            case "STC":
                _ = quoteHub.ToStcHub(10, 23, 50).Results;
                break;
            case "STDEV":
                _ = quoteHub.ToStdDevHub(14).Results;
                break;
            case "STOCH":
                _ = quoteHub.ToStochHub(14, 3, 3).Results;
                break;
            case "STOCH-RSI":
                _ = quoteHub.ToStochRsiHub(14, 14, 3, 1).Results;
                break;
            case "SUPERTREND":
                _ = quoteHub.ToSuperTrendHub(10, 3).Results;
                break;
            case "T3":
                _ = quoteHub.ToT3Hub(5, 0.7).Results;
                break;
            case "TEMA":
                _ = quoteHub.ToTemaHub(14).Results;
                break;
            case "TR":
                _ = quoteHub.ToTrHub().Results;
                break;
            case "TRIX":
                _ = quoteHub.ToTrixHub(14).Results;
                break;
            case "TSI":
                _ = quoteHub.ToTsiHub(25, 13, 7).Results;
                break;
            case "ULCER":
                _ = quoteHub.ToUlcerIndexHub(14).Results;
                break;
            case "UO":
                _ = quoteHub.ToUltimateHub(7, 14, 28).Results;
                break;
            case "VOL-STOP":
                _ = quoteHub.ToVolatilityStopHub(7, 3).Results;
                break;
            case "VORTEX":
                _ = quoteHub.ToVortexHub(14).Results;
                break;
            case "VWAP":
                _ = quoteHub.ToVwapHub().Results;
                break;
            case "VWMA":
                _ = quoteHub.ToVwmaHub(14).Results;
                break;
            case "WILLR":
                _ = quoteHub.ToWilliamsRHub().Results;
                break;
            case "WMA":
                _ = quoteHub.ToWmaHub(14).Results;
                break;
            default:
                throw new NotSupportedException($"Indicator '{Keyword}' is not supported in ManualTestDirect.");
        }
    }

}
