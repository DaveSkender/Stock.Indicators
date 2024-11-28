namespace Test.Application;

using Skender.Stock.Indicators;

public class Program
{
    private static void Main(string[] args)
    {
        if (args.Length != 0)
        {
            Console.WriteLine(args);
        }

        // Load sample data
        List<Quote> quotes = new()
        {
            new Quote(DateTime.Parse("2020-01-01"), 100, 110, 90, 105, 1000),
            new Quote(DateTime.Parse("2020-01-02"), 105, 115, 95, 110, 1200),
            new Quote(DateTime.Parse("2020-01-03"), 110, 120, 100, 115, 1500),
            new Quote(DateTime.Parse("2020-01-04"), 115, 125, 105, 120, 1800),
            new Quote(DateTime.Parse("2020-01-05"), 120, 130, 110, 125, 2000)
        };

        // Exercise the public API
        IEnumerable<AdlResult> adlResults = quotes.GetAdl();
        IEnumerable<ObvResult> obvResults = quotes.GetObv();
        IEnumerable<PrsResult> prsResults = quotes.GetPrs(quotes, quotes, lookbackPeriods: 14, smaPeriods: 3);
        IEnumerable<RocResult> rocResults = quotes.GetRoc(lookbackPeriods: 10);
        IEnumerable<StdDevResult> stdDevResults = quotes.GetStdDev(lookbackPeriods: 10);
        IEnumerable<TrixResult> trixResults = quotes.GetTrix(lookbackPeriods: 15);

        IEnumerable<AdxResult> adxResults = quotes.GetAdx(lookbackPeriods: 14);
        IEnumerable<AlligatorResult> alligatorResults = quotes.GetAlligator(jawPeriods: 13, jawOffset: 8, teethPeriods: 8, teethOffset: 5, lipsPeriods: 5, lipsOffset: 3);
        IEnumerable<AlmaResult> almaResults = quotes.GetAlma(lookbackPeriods: 9, offset: 0.85, sigma: 6);
        IEnumerable<AroonResult> aroonResults = quotes.GetAroon(lookbackPeriods: 25);
        IEnumerable<AtrResult> atrResults = quotes.GetAtr(lookbackPeriods: 14);
        IEnumerable<AtrStopResult> atrStopResults = quotes.GetAtrStop(lookbackPeriods: 21, multiplier: 3, endType: EndType.Close);
        IEnumerable<AwesomeResult> awesomeResults = quotes.GetAwesome(fastPeriods: 5, slowPeriods: 34);
        IEnumerable<BetaResult> betaResults = quotes.GetBeta(sourceEval: quotes, sourceMrkt: quotes, lookbackPeriods: 20, type: BetaType.Standard);
        IEnumerable<BollingerBandsResult> bollingerBandsResults = quotes.GetBollingerBands(lookbackPeriods: 20, standardDeviations: 2);
        IEnumerable<BopResult> bopResults = quotes.GetBop(lookbackPeriods: 14);
        IEnumerable<CciResult> cciResults = quotes.GetCci(lookbackPeriods: 20);
        IEnumerable<ChaikinOscResult> chaikinOscResults = quotes.GetChaikinOsc(fastPeriods: 3, slowPeriods: 10);
        IEnumerable<ChandelierResult> chandelierResults = quotes.GetChandelier(lookbackPeriods: 22, multiplier: 3, type: ChandelierType.Long);
        IEnumerable<ChopResult> chopResults = quotes.GetChop(lookbackPeriods: 14);
        IEnumerable<CmfResult> cmfResults = quotes.GetCmf(lookbackPeriods: 20);
        IEnumerable<CmoResult> cmoResults = quotes.GetCmo(lookbackPeriods: 14);
        IEnumerable<ConnorsRsiResult> connorsRsiResults = quotes.GetConnorsRsi(rsiPeriods: 3, streakPeriods: 2, rankPeriods: 100);
        IEnumerable<CorrResult> corrResults = quotes.GetCorrelation(sourceA: quotes, sourceB: quotes, lookbackPeriods: 20);
        IEnumerable<DemaResult> demaResults = quotes.GetDema(lookbackPeriods: 20);
        IEnumerable<CandleResult> dojiResults = quotes.GetDoji(maxPriceChangePercent: 0.1);
        IEnumerable<DonchianResult> donchianResults = quotes.GetDonchian(lookbackPeriods: 20);
        IEnumerable<DpoResult> dpoResults = quotes.GetDpo(lookbackPeriods: 20);
        IEnumerable<DynamicResult> dynamicResults = quotes.GetDynamic(lookbackPeriods: 20, kFactor: 0.6);
        IEnumerable<ElderRayResult> elderRayResults = quotes.GetElderRay(lookbackPeriods: 13);
        IEnumerable<EmaResult> emaResults = quotes.GetEma(lookbackPeriods: 20);
        IEnumerable<EpmaResult> epmaResults = quotes.GetEpma(lookbackPeriods: 20);
        IEnumerable<FcbResult> fcbResults = quotes.GetFcb(windowSpan: 2);
        IEnumerable<FisherTransformResult> fisherTransformResults = quotes.GetFisherTransform(lookbackPeriods: 10);
        IEnumerable<ForceIndexResult> forceIndexResults = quotes.GetForceIndex(lookbackPeriods: 2);
        IEnumerable<FractalResult> fractalResults = quotes.GetFractal(leftSpan: 2, rightSpan: 2, endType: EndType.HighLow);
        IEnumerable<GatorResult> gatorResults = quotes.GetGator();
        IEnumerable<HeikinAshiResult> heikinAshiResults = quotes.GetHeikinAshi();
        IEnumerable<HmaResult> hmaResults = quotes.GetHma(lookbackPeriods: 20);
        IEnumerable<HtlResult> htTrendlineResults = quotes.GetHtTrendline();
        IEnumerable<HurstResult> hurstResults = quotes.GetHurst(lookbackPeriods: 100);
        IEnumerable<IchimokuResult> ichimokuResults = quotes.GetIchimoku(tenkanPeriods: 9, kijunPeriods: 26, senkouBPeriods: 52);
        IEnumerable<KamaResult> kamaResults = quotes.GetKama(erPeriods: 10, fastPeriods: 2, slowPeriods: 30);
        IEnumerable<KeltnerResult> keltnerResults = quotes.GetKeltner(emaPeriods: 20, multiplier: 2, atrPeriods: 10);
        IEnumerable<KvoResult> kvoResults = quotes.GetKvo(fastPeriods: 34, slowPeriods: 55, signalPeriods: 13);
        IEnumerable<MacdResult> macdResults = quotes.GetMacd(fastPeriods: 12, slowPeriods: 26, signalPeriods: 9);
        IEnumerable<MaEnvelopeResult> maEnvelopesResults = quotes.GetMaEnvelopes(lookbackPeriods: 20, percentOffset: 2.5, movingAverageType: MaType.SMA);
        IEnumerable<MamaResult> mamaResults = quotes.GetMama(lookbackPeriods: 20);
        IEnumerable<CandleResult> marubozuResults = quotes.GetMarubozu(minBodyPercent: 95);
        IEnumerable<MfiResult> mfiResults = quotes.GetMfi(lookbackPeriods: 14);
        IEnumerable<ParabolicSarResult> parabolicSarResults = quotes.GetParabolicSar(accelerationStep: 0.02, maxAccelerationFactor: 0.2);
        IEnumerable<PivotPointsResult> pivotPointsResults = quotes.GetPivotPoints(lookbackPeriods: 20);
        IEnumerable<PivotsResult> pivotsResults = quotes.GetPivots(leftSpan: 2, rightSpan: 2, maxTrendPeriods: 20, endType: EndType.HighLow);
        IEnumerable<PmoResult> pmoResults = quotes.GetPmo(timePeriods: 35, smoothPeriods: 20, signalPeriods: 10);
        IEnumerable<RenkoResult> renkoResults = quotes.GetRenko(brickSize: 2, endType: EndType.Close);
        IEnumerable<RenkoResult> renkoAtrResults = quotes.GetRenkoAtr(atrPeriods: 14, endType: EndType.Close);
        IEnumerable<RocWbResult> rocWbResults = quotes.GetRocWb(lookbackPeriods: 10, emaPeriods: 20, stdDevPeriods: 10);
        IEnumerable<RollingPivotsResult> rollingPivotsResults = quotes.GetRollingPivots(windowPeriods: 20, offsetPeriods: 10, pointType: PivotPointType.Standard);
        IEnumerable<RsiResult> rsiResults = quotes.GetRsi(lookbackPeriods: 14);
        IEnumerable<SlopeResult> slopeResults = quotes.GetSlope(lookbackPeriods: 20);
        IEnumerable<SmaResult> smaResults = quotes.GetSma(lookbackPeriods: 20);
        IEnumerable<SmaAnalysis> smaAnalysisResults = quotes.GetSmaAnalysis(lookbackPeriods: 20);
        IEnumerable<SmiResult> smiResults = quotes.GetSmi(lookbackPeriods: 13, firstSmoothPeriods: 25, secondSmoothPeriods: 2, signalPeriods: 3);
        IEnumerable<SmmaResult> smmaResults = quotes.GetSmma(lookbackPeriods: 20);
        IEnumerable<StarcBandsResult> starcBandsResults = quotes.GetStarcBands(smaPeriods: 20, multiplier: 2, atrPeriods: 10);
        IEnumerable<StcResult> stcResults = quotes.GetStc(cyclePeriods: 10, fastPeriods: 23, slowPeriods: 50);
        IEnumerable<StochResult> stochResults = quotes.GetStoch(lookbackPeriods: 14, signalPeriods: 3, smoothPeriods: 3);
        IEnumerable<StochResult> stochResults2 = quotes.GetStoch(lookbackPeriods: 14, signalPeriods: 3, smoothPeriods: 3, kFactor: 3, dFactor: 3, movingAverageType: MaType.SMA);
        IEnumerable<StochRsiResult> stochRsiResults = quotes.GetStochRsi(rsiPeriods: 14, stochPeriods: 14, signalPeriods: 3, smoothPeriods: 3);
        IEnumerable<SuperTrendResult> superTrendResults = quotes.GetSuperTrend(lookbackPeriods: 10, multiplier: 3);
        IEnumerable<T3Result> t3Results = quotes.GetT3(lookbackPeriods: 5, volumeFactor: 0.7);
        IEnumerable<TemaResult> temaResults = quotes.GetTema(lookbackPeriods: 20);
        IEnumerable<TrResult> trResults = quotes.GetTr();
        IEnumerable<TsiResult> tsiResults = quotes.GetTsi(lookbackPeriods: 25, smoothPeriods: 13, signalPeriods: 7);
        IEnumerable<UlcerIndexResult> ulcerIndexResults = quotes.GetUlcerIndex(lookbackPeriods: 14);
        IEnumerable<UltimateResult> ultimateResults = quotes.GetUltimate(shortPeriods: 7, middlePeriods: 14, longPeriods: 28);
        IEnumerable<VolatilityStopResult> volatilityStopResults = quotes.GetVolatilityStop(lookbackPeriods: 7, multiplier: 3);
        IEnumerable<VortexResult> vortexResults = quotes.GetVortex(lookbackPeriods: 14);
        IEnumerable<VwapResult> vwapResults = quotes.GetVwap();
        IEnumerable<VwmaResult> vwmaResults = quotes.GetVwma(lookbackPeriods: 20);
        IEnumerable<WilliamsResult> williamsRResults = quotes.GetWilliamsR(lookbackPeriods: 14);
        IEnumerable<WmaResult> wmaResults = quotes.GetWma(lookbackPeriods: 20);
        IEnumerable<ZigZagResult> zigZagResults = quotes.GetZigZag(endType: EndType.Close, percentChange: 5);

        // Utility methods
        IEnumerable<(DateTime Timestamp, double Value)> useResults = quotes.Use(CandlePart.Close);
        Collection<Quote> sortedCollection = quotes.ToSortedCollection();
        Collection<(DateTime Timestamp, double Value)> tupleChainable = quotes.ToTupleChainable();
        Quote foundQuote = quotes.Find(DateTime.Parse("2020-01-01"));
        int foundIndex = quotes.FindIndex(DateTime.Parse("2020-01-01"));
    }
}
