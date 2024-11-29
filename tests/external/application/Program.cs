using System.Collections.ObjectModel;
using System.Globalization;

namespace Test.Application;

public class Program
{
    private static void Main(string[] args)
    {
        if (args.Length != 0)
        {
            Console.WriteLine(args);
        }

        // QUOTES for testing
        CultureInfo culture = CultureInfo.InvariantCulture;
        IEnumerable<Quote> quotes =
        [
            new Quote { Date = DateTime.Parse("2023-01-02", culture), Open = 100, High = 110, Low = 90,  Close = 105, Volume = 1000 }, // Monday
            new Quote { Date = DateTime.Parse("2023-01-03", culture), Open = 106, High = 112, Low = 104, Close = 108, Volume = 1500 }, // Tuesday
            new Quote { Date = DateTime.Parse("2023-01-04", culture), Open = 109, High = 115, Low = 107, Close = 112, Volume = 1200 }, // Wednesday
            new Quote { Date = DateTime.Parse("2023-01-05", culture), Open = 113, High = 118, Low = 110, Close = 116, Volume = 1300 }, // Thursday
            new Quote { Date = DateTime.Parse("2023-01-06", culture), Open = 117, High = 120, Low = 114, Close = 119, Volume = 1400 }, // Friday
            new Quote { Date = DateTime.Parse("2023-01-09", culture), Open = 120, High = 125, Low = 118, Close = 123, Volume = 1500 }, // Monday
            new Quote { Date = DateTime.Parse("2023-01-10", culture), Open = 124, High = 128, Low = 122, Close = 127, Volume = 1600 }, // Tuesday
            new Quote { Date = DateTime.Parse("2023-01-11", culture), Open = 128, High = 132, Low = 126, Close = 130, Volume = 1700 }, // Wednesday
            new Quote { Date = DateTime.Parse("2023-01-12", culture), Open = 131, High = 135, Low = 129, Close = 133, Volume = 1800 }, // Thursday
            new Quote { Date = DateTime.Parse("2023-01-13", culture), Open = 134, High = 138, Low = 132, Close = 136, Volume = 1900 }, // Friday
            new Quote { Date = DateTime.Parse("2023-01-16", culture), Open = 137, High = 141, Low = 135, Close = 139, Volume = 2000 }, // Monday
            new Quote { Date = DateTime.Parse("2023-01-17", culture), Open = 140, High = 144, Low = 138, Close = 142, Volume = 2100 }, // Tuesday
            new Quote { Date = DateTime.Parse("2023-01-18", culture), Open = 143, High = 147, Low = 141, Close = 145, Volume = 2200 }, // Wednesday
            new Quote { Date = DateTime.Parse("2023-01-19", culture), Open = 146, High = 150, Low = 144, Close = 148, Volume = 2300 }, // Thursday
            new Quote { Date = DateTime.Parse("2023-01-20", culture), Open = 149, High = 153, Low = 147, Close = 151, Volume = 2400 }  // Friday
        ];

        // TUPLES for testings
        IEnumerable<(DateTime d, double v)> tuples = quotes.Select(x => (x.Date, (double)x.Close));

        // SERIES INDICATORS
        IEnumerable<AdlResult> adl1 = quotes.GetAdl();
        IEnumerable<AdxResult> adx1 = quotes.GetAdx(lookbackPeriods: 14);
        IEnumerable<AlligatorResult> alligator1 = quotes.GetAlligator();
        IEnumerable<AlligatorResult> alligator2 = quotes.GetAlligator(jawPeriods: 13, jawOffset: 8, teethPeriods: 8, teethOffset: 5, lipsPeriods: 5, lipsOffset: 3);
        IEnumerable<AlligatorResult> alligatorT = tuples.GetAlligator(jawPeriods: 13, jawOffset: 8, teethPeriods: 8, teethOffset: 5, lipsPeriods: 5, lipsOffset: 3);
        IEnumerable<AlmaResult> alma1 = quotes.GetAlma(lookbackPeriods: 9, offset: 0.85, sigma: 6);
        IEnumerable<AlmaResult> almaT = tuples.GetAlma(lookbackPeriods: 9, offset: 0.85, sigma: 6);
        IEnumerable<AroonResult> aroon = quotes.GetAroon(lookbackPeriods: 25);
        IEnumerable<AtrResult> atr = quotes.GetAtr(lookbackPeriods: 14);
        IEnumerable<AtrStopResult> atrStop1 = quotes.GetAtrStop(lookbackPeriods: 14);
        IEnumerable<AtrStopResult> atrStop2 = quotes.GetAtrStop(lookbackPeriods: 21, multiplier: 3, endType: EndType.Close);
        IEnumerable<AwesomeResult> awesome1 = quotes.GetAwesome();
        IEnumerable<AwesomeResult> awesome2 = quotes.GetAwesome(fastPeriods: 5, slowPeriods: 34);
        IEnumerable<BetaResult> beta = quotes.GetBeta(quotesMarket: quotes, lookbackPeriods: 20);
        IEnumerable<BetaResult> betaAll = quotes.GetBeta(quotesMarket: quotes, lookbackPeriods: 20, type: BetaType.All);
        IEnumerable<BetaResult> betaStd = quotes.GetBeta(quotesMarket: quotes, lookbackPeriods: 20, type: BetaType.Standard);
        IEnumerable<BetaResult> betaDwn = quotes.GetBeta(quotesMarket: quotes, lookbackPeriods: 20, type: BetaType.Down);
        IEnumerable<BetaResult> betaUps = quotes.GetBeta(quotesMarket: quotes, lookbackPeriods: 20, type: BetaType.Up);
        IEnumerable<BollingerBandsResult> bb = quotes.GetBollingerBands(lookbackPeriods: 20, standardDeviations: 2);
        IEnumerable<BollingerBandsResult> bollingerBands = quotes.GetBollingerBands(lookbackPeriods: 20, standardDeviations: 2);
        IEnumerable<BopResult> bop1 = quotes.GetBop();
        IEnumerable<BopResult> bop2 = quotes.GetBop(smoothPeriods: 14);
        IEnumerable<CandleResult> doji = quotes.GetDoji(maxPriceChangePercent: 0.1);
        IEnumerable<CandleResult> marubozu = quotes.GetMarubozu(minBodyPercent: 95);
        IEnumerable<CciResult> cci = quotes.GetCci(lookbackPeriods: 20);
        IEnumerable<ChaikinOscResult> chaikinOsc1 = quotes.GetChaikinOsc();
        IEnumerable<ChaikinOscResult> chaikinOsc2 = quotes.GetChaikinOsc(fastPeriods: 3, slowPeriods: 10);
        IEnumerable<ChandelierResult> chandelier = quotes.GetChandelier(lookbackPeriods: 22, multiplier: 3, type: ChandelierType.Long);
        IEnumerable<ChopResult> chop = quotes.GetChop(lookbackPeriods: 14);
        IEnumerable<CmfResult> cmf = quotes.GetCmf(lookbackPeriods: 20);
        IEnumerable<CmoResult> cmo = quotes.GetCmo(lookbackPeriods: 14);
        IEnumerable<ConnorsRsiResult> connorsRsi = quotes.GetConnorsRsi(rsiPeriods: 3, streakPeriods: 2, rankPeriods: 100);
        IEnumerable<CorrResult> corr = quotes.GetCorrelation(quotesB: quotes, lookbackPeriods: 20);
        IEnumerable<DemaResult> dema = quotes.GetDema(lookbackPeriods: 20);
        IEnumerable<DonchianResult> donchian = quotes.GetDonchian(lookbackPeriods: 20);
        IEnumerable<DpoResult> dpo = quotes.GetDpo(lookbackPeriods: 20);
        IEnumerable<DynamicResult> dynamic1 = quotes.GetDynamic(lookbackPeriods: 20);
        IEnumerable<DynamicResult> dynamic2 = quotes.GetDynamic(lookbackPeriods: 20, kFactor: 0.6);
        IEnumerable<ElderRayResult> elderRay = quotes.GetElderRay(lookbackPeriods: 13);
        IEnumerable<EmaResult> ema = quotes.GetEma(lookbackPeriods: 20);
        IEnumerable<EpmaResult> epma = quotes.GetEpma(lookbackPeriods: 20);
        IEnumerable<FcbResult> fcb = quotes.GetFcb(windowSpan: 2);
        IEnumerable<FisherTransformResult> fisherTransform = quotes.GetFisherTransform(lookbackPeriods: 10);
        IEnumerable<ForceIndexResult> forceIndex = quotes.GetForceIndex(lookbackPeriods: 2);
        IEnumerable<FractalResult> fractal1 = quotes.GetFractal();
        IEnumerable<FractalResult> fractal2 = quotes.GetFractal(leftSpan: 2, rightSpan: 2, endType: EndType.HighLow);
        IEnumerable<GatorResult> gator = quotes.GetGator();
        IEnumerable<HeikinAshiResult> heikinAshi = quotes.GetHeikinAshi();
        IEnumerable<HmaResult> hma = quotes.GetHma(lookbackPeriods: 20);
        IEnumerable<HtlResult> htTrendline = quotes.GetHtTrendline();
        IEnumerable<HurstResult> hurst = quotes.GetHurst(lookbackPeriods: 100);
        IEnumerable<IchimokuResult> ichimoku1 = quotes.GetIchimoku();
        IEnumerable<IchimokuResult> ichimoku2 = quotes.GetIchimoku(tenkanPeriods: 9, kijunPeriods: 26, senkouBPeriods: 52);
        IEnumerable<KamaResult> kama = quotes.GetKama(erPeriods: 10, fastPeriods: 2, slowPeriods: 30);
        IEnumerable<KeltnerResult> keltner = quotes.GetKeltner(emaPeriods: 20, multiplier: 2, atrPeriods: 10);
        IEnumerable<KvoResult> kvo1 = quotes.GetKvo();
        IEnumerable<KvoResult> kvo2 = quotes.GetKvo(fastPeriods: 34, slowPeriods: 55, signalPeriods: 13);
        IEnumerable<MacdResult> macd = quotes.GetMacd(fastPeriods: 12, slowPeriods: 26, signalPeriods: 9);
        IEnumerable<MaEnvelopeResult> maEnvelopes = quotes.GetMaEnvelopes(lookbackPeriods: 20, percentOffset: 3.5, movingAverageType: MaType.SMA);
        IEnumerable<MamaResult> mama1 = quotes.GetMama();
        IEnumerable<MamaResult> mama2 = quotes.GetMama(fastLimit: 0.5, slowLimit: 0.05);
        IEnumerable<MfiResult> mfi = quotes.GetMfi(lookbackPeriods: 14);
        IEnumerable<ObvResult> obv1 = quotes.GetObv();
        IEnumerable<ObvResult> obv2 = quotes.GetObv(smaPeriods: 3);
        IEnumerable<ParabolicSarResult> pSar1 = quotes.GetParabolicSar(accelerationStep: 0.02, maxAccelerationFactor: 0.2);
        IEnumerable<ParabolicSarResult> pSar2 = quotes.GetParabolicSar(accelerationStep: 0.02, maxAccelerationFactor: 0.2, initialFactor: 0.02);
        IEnumerable<PivotPointsResult> pivotPoints1 = quotes.GetPivotPoints(windowSize: PeriodSize.Week);
        IEnumerable<PivotPointsResult> pivotPoints2 = quotes.GetPivotPoints(windowSize: PeriodSize.Week, pointType: PivotPointType.Standard);
        IEnumerable<PivotsResult> pivots1 = quotes.GetPivots();
        IEnumerable<PivotsResult> pivots2 = quotes.GetPivots(leftSpan: 2, rightSpan: 2, maxTrendPeriods: 20, endType: EndType.HighLow);
        IEnumerable<PmoResult> pmo = quotes.GetPmo(timePeriods: 35, smoothPeriods: 20, signalPeriods: 10);
        IEnumerable<PrsResult> prs1 = quotes.GetPrs(quotesBase: quotes);
        IEnumerable<PrsResult> prs2 = quotes.GetPrs(quotesBase: quotes, lookbackPeriods: 5, smaPeriods: 3);
        IEnumerable<PvoResult> pvo = quotes.GetPvo();
        IEnumerable<RenkoResult> renko1 = quotes.GetRenko(brickSize: 2.0m, endType: EndType.Close);
        IEnumerable<RenkoResult> renko2 = quotes.GetRenko(brickSize: 2, endType: EndType.Close);
        IEnumerable<RenkoResult> renkoAtr = quotes.GetRenkoAtr(atrPeriods: 14, endType: EndType.Close);
        IEnumerable<RocResult> roc1 = quotes.GetRoc(lookbackPeriods: 20);
        IEnumerable<RocResult> roc2 = quotes.GetRoc(lookbackPeriods: 20, smaPeriods: 3);
        IEnumerable<RocWbResult> rocWb = quotes.GetRocWb(lookbackPeriods: 20, emaPeriods: 9, stdDevPeriods: 2);
        IEnumerable<RollingPivotsResult> rollingPivots = quotes.GetRollingPivots(windowPeriods: 14, offsetPeriods: 7, pointType: PivotPointType.Standard);
        IEnumerable<RsiResult> rsi = quotes.GetRsi(lookbackPeriods: 14);
        IEnumerable<SlopeResult> slope = quotes.GetSlope(lookbackPeriods: 20);
        IEnumerable<SmaResult> sma = quotes.GetSma(lookbackPeriods: 20);
        IEnumerable<SmaAnalysis> smaAnalysis = quotes.GetSmaAnalysis(lookbackPeriods: 20);
        IEnumerable<SmiResult> smi1 = quotes.GetSmi(lookbackPeriods: 20);
        IEnumerable<SmiResult> smi2 = quotes.GetSmi(lookbackPeriods: 13, firstSmoothPeriods: 25, secondSmoothPeriods: 2, signalPeriods: 3);
        IEnumerable<SmmaResult> smma = quotes.GetSmma(lookbackPeriods: 20);
        IEnumerable<StarcBandsResult> starcBands = quotes.GetStarcBands(smaPeriods: 20, multiplier: 2, atrPeriods: 10);
        IEnumerable<StcResult> stc = quotes.GetStc(cyclePeriods: 10, fastPeriods: 23, slowPeriods: 50);
        IEnumerable<StdDevChannelsResult> stdDevChannels = quotes.GetStdDevChannels(lookbackPeriods: 20, stdDeviations: 2);
        IEnumerable<StdDevResult> stdDev1 = quotes.GetStdDev(lookbackPeriods: 20);
        IEnumerable<StdDevResult> stdDev2 = quotes.GetStdDev(lookbackPeriods: 20, smaPeriods: 20);
        IEnumerable<StochResult> stoch1 = quotes.GetStoch(lookbackPeriods: 14, signalPeriods: 3, smoothPeriods: 3);
        IEnumerable<StochResult> stoch2 = quotes.GetStoch(lookbackPeriods: 14, signalPeriods: 3, smoothPeriods: 3, kFactor: 3, dFactor: 2, movingAverageType: MaType.SMA);
        IEnumerable<StochRsiResult> stochRsi = quotes.GetStochRsi(rsiPeriods: 14, stochPeriods: 14, signalPeriods: 3, smoothPeriods: 3);
        IEnumerable<SuperTrendResult> superTrend = quotes.GetSuperTrend(lookbackPeriods: 10, multiplier: 3);
        IEnumerable<T3Result> t3a = quotes.GetT3();
        IEnumerable<T3Result> t3b = quotes.GetT3(lookbackPeriods: 5, volumeFactor: 0.7);
        IEnumerable<TemaResult> tema = quotes.GetTema(lookbackPeriods: 20);
        IEnumerable<TrixResult> trix1 = quotes.GetTrix(lookbackPeriods: 15);
        IEnumerable<TrixResult> trix2 = quotes.GetTrix(lookbackPeriods: 20, signalPeriods: 3);
        IEnumerable<TrResult> tr = quotes.GetTr();
        IEnumerable<TsiResult> tsi1 = quotes.GetTsi();
        IEnumerable<TsiResult> tsi2 = quotes.GetTsi(lookbackPeriods: 20, smoothPeriods: 13, signalPeriods: 7);
        IEnumerable<UlcerIndexResult> ui1 = quotes.GetUlcerIndex();
        IEnumerable<UlcerIndexResult> ui2 = quotes.GetUlcerIndex(lookbackPeriods: 14);
        IEnumerable<UltimateResult> ultimate1 = quotes.GetUltimate();
        IEnumerable<UltimateResult> ultimate2 = quotes.GetUltimate(shortPeriods: 7, middlePeriods: 14, longPeriods: 28);
        IEnumerable<VolatilityStopResult> volStop1 = quotes.GetVolatilityStop();
        IEnumerable<VolatilityStopResult> volStop2 = quotes.GetVolatilityStop(lookbackPeriods: 7, multiplier: 3);
        IEnumerable<VortexResult> vortex = quotes.GetVortex(lookbackPeriods: 14);
        IEnumerable<VwapResult> vwap = quotes.GetVwap();
        IEnumerable<VwmaResult> vwma = quotes.GetVwma(lookbackPeriods: 20);
        IEnumerable<WilliamsResult> williamsR = quotes.GetWilliamsR(lookbackPeriods: 14);
        IEnumerable<WmaResult> wma = quotes.GetWma(lookbackPeriods: 20);
        IEnumerable<ZigZagResult> zigZag1 = quotes.GetZigZag(endType: EndType.Close, percentChange: 5m);
        IEnumerable<ZigZagResult> zigZag2 = quotes.GetZigZag(endType: EndType.Close, percentChange: 5);

        // Utilities
        IEnumerable<(DateTime Date, double Value)> useResults = quotes.Use(candlePart: CandlePart.Close);
        Collection<Quote> sortedCollection = quotes.ToSortedCollection();
        Collection<(DateTime, double)> tupleCollection = quotes.ToTupleCollection(candlePart: CandlePart.Close);

        Quote? foundQuote = quotes.Find(DateTime.Parse("2023-01-05", culture));
        EmaResult? foundResult = ema.Find(DateTime.Parse("2023-01-03", culture));
    }
}
