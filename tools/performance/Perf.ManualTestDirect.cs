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

    [GlobalSetup]
    public void Setup()
    {
        Console.WriteLine($"Manual Test Configuration:");
        Console.WriteLine($"  Keyword: {Keyword}");
        Console.WriteLine($"  Periods: {Periods:N0}");
        Console.WriteLine($"  Generated {Quotes.Count:N0} quotes");
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
                AdlList list = new();
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "ADX":
            {
                AdxList list = new(14);
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "ALLIGATOR":
            {
                AlligatorList list = new();
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "ALMA":
            {
                AlmaList list = new(9, 0.85, 6);
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "AROON":
            {
                AroonList list = new();
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "ATR":
            {
                AtrList list = new(14);
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "ATR-STOP":
            {
                AtrStopList list = new();
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "AWESOME":
            {
                AwesomeList list = new();
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "BB":
            {
                BollingerBandsList list = new(20, 2);
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "BETA":
            {
                BetaList list = new(CompareQuotes, 20, BetaType.Standard);
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "BOP":
            {
                BopList list = new(14);
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "CCI":
            {
                CciList list = new(14);
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "CHAIKIN-OSC":
            {
                ChaikinOscList list = new();
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "CHANDELIER":
            case "CHEXIT":
            {
                ChandelierList list = new();
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "CHOP":
            {
                ChopList list = new(14);
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "CMF":
            {
                CmfList list = new(14);
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "CMO":
            {
                CmoList list = new(14);
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "CORR":
            {
                CorrelationList list = new(CompareQuotes, 20);
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "CRSI":
            {
                ConnorsRsiList list = new(3, 2, 100);
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "DEMA":
            {
                DemaList list = new(14);
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "DOJI":
            {
                DojiList list = new();
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "DONCHIAN":
            {
                DonchianList list = new();
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "DPO":
            {
                DpoList list = new(14);
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "DYNAMIC":
            {
                DynamicList list = new(14);
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "ELDER-RAY":
            {
                ElderRayList list = new(13);
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "EMA":
            {
                EmaList list = new(20);
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "EPMA":
            {
                EpmaList list = new(14);
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "FCB":
            {
                FcbList list = new(2);
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "FISHER":
            {
                FisherTransformList list = new(10);
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "FORCE":
            {
                ForceIndexList list = new(2);
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "FRACTAL":
            {
                FractalList list = new();
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "GATOR":
            {
                GatorList list = new();
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "HEIKINASHI":
            {
                HeikinAshiList list = new();
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "HMA":
            {
                HmaList list = new(14);
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "HTL":
            {
                HtTrendlineList list = new();
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "HURST":
            {
                HurstList list = new(100);
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "ICHIMOKU":
            {
                IchimokuList list = new();
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "KAMA":
            {
                KamaList list = new(10, 2, 30);
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "KELTNER":
            {
                KeltnerList list = new(20, 2, 10);
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "KVO":
            {
                KvoList list = new(34, 55, 13);
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "MA-ENV":
            {
                MaEnvelopesList list = new(20, 2.5, MaType.SMA);
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "MACD":
            {
                MacdList list = new(12, 26, 9);
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "MAMA":
            {
                MamaList list = new(0.5, 0.05);
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "MARUBOZU":
            {
                MarubozuList list = new(95);
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "MFI":
            {
                MfiList list = new(14);
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "OBV":
            {
                ObvList list = new();
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "PMO":
            {
                PmoList list = new(35, 20, 10);
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "PRS":
            {
                PrsList list = new(CompareQuotes);
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "PSAR":
            {
                ParabolicSarList list = new();
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "PVO":
            {
                PvoList list = new();
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "RENKO":
            {
                RenkoList list = new(2.5m);
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "RENKO-ATR":
            {
                RenkoList list = new(Quotes, 14);
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "ROC":
            {
                RocList list = new(20);
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "ROC-WB":
            {
                RocWbList list = new(20, 5, 5);
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "RSI":
            {
                RsiList list = new(14);
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "SLOPE":
            {
                SlopeList list = new(14);
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "SMA":
            {
                SmaList list = new(14);
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "SMA-ANALYSIS":
            {
                SmaAnalysisList list = new(14);
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "SMI":
            {
                SmiList list = new(13, 25, 2, 3);
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "SMMA":
            {
                SmmaList list = new(14);
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "STARC":
            {
                StarcBandsList list = new(5, 2, 10);
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "STC":
            {
                StcList list = new(10, 23, 50);
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "STDEV":
            {
                StdDevList list = new(14);
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "STDEV-CHANNELS":
            {
                StdDevChannelsList list = new();
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "STOCH":
            {
                StochList list = new(14, 3, 3);
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "STOCH-RSI":
            {
                StochRsiList list = new(14, 14, 3, 1);
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "SUPERTREND":
            {
                SuperTrendList list = new(10, 3);
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "T3":
            {
                T3List list = new(5, 0.7);
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "TEMA":
            {
                TemaList list = new(14);
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "TR":
            {
                TrList list = new();
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "TRIX":
            {
                TrixList list = new(14);
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "TSI":
            {
                TsiList list = new(25, 13, 7);
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "ULCER":
            {
                UlcerIndexList list = new(14);
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "UO":
            {
                UltimateList list = new(7, 14, 28);
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "VOL-STOP":
            {
                VolatilityStopList list = new(7, 3);
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "VORTEX":
            {
                VortexList list = new(14);
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "VWAP":
            {
                VwapList list = new();
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "VWMA":
            {
                VwmaList list = new(14);
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "WILLR":
            {
                WilliamsRList list = new();
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "WMA":
            {
                WmaList list = new(14);
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            case "ZIGZAG":
            {
                ZigZagList list = new();
                foreach (Quote q in Quotes) list.Add(q);
                break;
            }
            default:
                throw new NotSupportedException($"Indicator '{Keyword}' is not supported in ManualTestDirect.");
        }
    }

    /* STREAM BENCHMARKS */

    [Benchmark]
    public void Stream()
    {
        switch (Keyword)
        {
            case "ADL":
            {
                AdlHub hub = new();
                foreach (Quote q in Quotes) hub.Add(q);
                break;
            }
            case "ADX":
            {
                AdxHub hub = new(14);
                foreach (Quote q in Quotes) hub.Add(q);
                break;
            }
            case "ALMA":
            {
                AlmaHub hub = new(9, 0.85, 6);
                foreach (Quote q in Quotes) hub.Add(q);
                break;
            }
            case "AROON":
            {
                AroonHub hub = new();
                foreach (Quote q in Quotes) hub.Add(q);
                break;
            }
            case "ATR":
            {
                AtrHub hub = new(14);
                foreach (Quote q in Quotes) hub.Add(q);
                break;
            }
            case "ATR-STOP":
            {
                AtrStopHub hub = new();
                foreach (Quote q in Quotes) hub.Add(q);
                break;
            }
            case "BB":
            {
                BollingerBandsHub hub = new(20, 2);
                foreach (Quote q in Quotes) hub.Add(q);
                break;
            }
            case "BOP":
            {
                BopHub hub = new(14);
                foreach (Quote q in Quotes) hub.Add(q);
                break;
            }
            case "CCI":
            {
                CciHub hub = new(14);
                foreach (Quote q in Quotes) hub.Add(q);
                break;
            }
            case "CHANDELIER":
            case "CHEXIT":
            {
                ChandelierHub hub = new();
                foreach (Quote q in Quotes) hub.Add(q);
                break;
            }
            case "CMF":
            {
                CmfHub hub = new(14);
                foreach (Quote q in Quotes) hub.Add(q);
                break;
            }
            case "CMO":
            {
                CmoHub hub = new(14);
                foreach (Quote q in Quotes) hub.Add(q);
                break;
            }
            case "DEMA":
            {
                DemaHub hub = new(14);
                foreach (Quote q in Quotes) hub.Add(q);
                break;
            }
            case "DONCHIAN":
            {
                DonchianHub hub = new();
                foreach (Quote q in Quotes) hub.Add(q);
                break;
            }
            case "DPO":
            {
                DpoHub hub = new(14);
                foreach (Quote q in Quotes) hub.Add(q);
                break;
            }
            case "EMA":
            {
                EmaHub hub = new(20);
                foreach (Quote q in Quotes) hub.Add(q);
                break;
            }
            case "EPMA":
            {
                EpmaHub hub = new(14);
                foreach (Quote q in Quotes) hub.Add(q);
                break;
            }
            case "HMA":
            {
                HmaHub hub = new(14);
                foreach (Quote q in Quotes) hub.Add(q);
                break;
            }
            case "KAMA":
            {
                KamaHub hub = new(10, 2, 30);
                foreach (Quote q in Quotes) hub.Add(q);
                break;
            }
            case "KELTNER":
            {
                KeltnerHub hub = new(20, 2, 10);
                foreach (Quote q in Quotes) hub.Add(q);
                break;
            }
            case "MACD":
            {
                MacdHub hub = new(12, 26, 9);
                foreach (Quote q in Quotes) hub.Add(q);
                break;
            }
            case "MFI":
            {
                MfiHub hub = new(14);
                foreach (Quote q in Quotes) hub.Add(q);
                break;
            }
            case "OBV":
            {
                ObvHub hub = new();
                foreach (Quote q in Quotes) hub.Add(q);
                break;
            }
            case "PSAR":
            {
                ParabolicSarHub hub = new();
                foreach (Quote q in Quotes) hub.Add(q);
                break;
            }
            case "ROC":
            {
                RocHub hub = new(20);
                foreach (Quote q in Quotes) hub.Add(q);
                break;
            }
            case "RSI":
            {
                RsiHub hub = new(14);
                foreach (Quote q in Quotes) hub.Add(q);
                break;
            }
            case "SMA":
            {
                SmaHub hub = new(14);
                foreach (Quote q in Quotes) hub.Add(q);
                break;
            }
            case "SMMA":
            {
                SmmaHub hub = new(14);
                foreach (Quote q in Quotes) hub.Add(q);
                break;
            }
            case "STDEV":
            {
                StdDevHub hub = new(14);
                foreach (Quote q in Quotes) hub.Add(q);
                break;
            }
            case "STOCH":
            {
                StochHub hub = new(14, 3, 3);
                foreach (Quote q in Quotes) hub.Add(q);
                break;
            }
            case "STOCH-RSI":
            {
                StochRsiHub hub = new(14, 14, 3, 1);
                foreach (Quote q in Quotes) hub.Add(q);
                break;
            }
            case "SUPERTREND":
            {
                SuperTrendHub hub = new(10, 3);
                foreach (Quote q in Quotes) hub.Add(q);
                break;
            }
            case "T3":
            {
                T3Hub hub = new(5, 0.7);
                foreach (Quote q in Quotes) hub.Add(q);
                break;
            }
            case "TEMA":
            {
                TemaHub hub = new(14);
                foreach (Quote q in Quotes) hub.Add(q);
                break;
            }
            case "TR":
            {
                TrHub hub = new();
                foreach (Quote q in Quotes) hub.Add(q);
                break;
            }
            case "TRIX":
            {
                TrixHub hub = new(14);
                foreach (Quote q in Quotes) hub.Add(q);
                break;
            }
            case "VWAP":
            {
                VwapHub hub = new();
                foreach (Quote q in Quotes) hub.Add(q);
                break;
            }
            case "VWMA":
            {
                VwmaHub hub = new(14);
                foreach (Quote q in Quotes) hub.Add(q);
                break;
            }
            case "WMA":
            {
                WmaHub hub = new(14);
                foreach (Quote q in Quotes) hub.Add(q);
                break;
            }
            default:
                throw new NotSupportedException($"Indicator '{Keyword}' is not supported for Stream style in ManualTestDirect.");
        }
    }
}
