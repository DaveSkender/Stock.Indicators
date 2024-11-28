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

        // Sample quotes
        CultureInfo invariantCulture = CultureInfo.InvariantCulture;
        List<Quote> quotes =
        [
            new Quote { Date = DateTime.Parse("2023-01-01",invariantCulture), Open = 100, High = 110, Low = 90, Close = 105, Volume = 1000 },
            new Quote { Date = DateTime.Parse("2023-01-02",invariantCulture), Open = 106, High = 112, Low = 104, Close = 108, Volume = 1500 }
        ];

        // Moving Averages
        IEnumerable<AlmaResult> alma = quotes.GetAlma(lookbackPeriods: 20, offset: 0.85, sigma: 6);
        IEnumerable<DemaResult> dema = quotes.GetDema(lookbackPeriods: 20);
        IEnumerable<EmaResult> ema = quotes.GetEma(lookbackPeriods: 20);
        IEnumerable<EpmaResult> epma = quotes.GetEpma(lookbackPeriods: 20);
        IEnumerable<HmaResult> hma = quotes.GetHma(lookbackPeriods: 20);
        IEnumerable<HtlResult> htTrendline = quotes.GetHtTrendline();
        IEnumerable<KamaResult> kama = quotes.GetKama(erPeriods: 20, fastPeriods: 2, slowPeriods: 30);
        IEnumerable<MamaResult> mama = quotes.GetMama(fastLimit: 0.5, slowLimit: 0.05);
        IEnumerable<SmaResult> sma = quotes.GetSma(lookbackPeriods: 20);
        IEnumerable<SmmaResult> smma = quotes.GetSmma(lookbackPeriods: 20);
        IEnumerable<T3Result> t3 = quotes.GetT3(lookbackPeriods: 20);
        IEnumerable<TemaResult> tema = quotes.GetTema(lookbackPeriods: 20);
        IEnumerable<VwmaResult> vwma = quotes.GetVwma(lookbackPeriods: 20);
        IEnumerable<WmaResult> wma = quotes.GetWma(lookbackPeriods: 20);

        // Bands and Channels
        IEnumerable<BollingerBandsResult> bb = quotes.GetBollingerBands(lookbackPeriods: 20, standardDeviations: 2);
        IEnumerable<ChandelierResult> chandelier = quotes.GetChandelier(lookbackPeriods: 22, multiplier: 3, type: ChandelierType.Long);
        IEnumerable<DonchianResult> dc = quotes.GetDonchian(lookbackPeriods: 20);
        IEnumerable<FcbResult> fcb = quotes.GetFcb(windowSpan: 2);
        IEnumerable<KeltnerResult> kc = quotes.GetKeltner(emaPeriods: 20, multiplier: 2, atrPeriods: 10);
        IEnumerable<MaEnvelopeResult> maEnvelopes = quotes.GetMaEnvelopes(lookbackPeriods: 20, percentOffset: 3.5, movingAverageType: MaType.SMA);
        IEnumerable<StarcBandsResult> starcBands = quotes.GetStarcBands(smaPeriods: 20, multiplier: 2, atrPeriods: 10);
        IEnumerable<StdDevChannelsResult> stdDevChannels = quotes.GetStdDevChannels(lookbackPeriods: 20, stdDeviations: 2);

        // Momentum and Oscillators
        IEnumerable<AdxResult> adx = quotes.GetAdx(lookbackPeriods: 14);
        IEnumerable<AroonResult> aroon = quotes.GetAroon(lookbackPeriods: 25);
        IEnumerable<AwesomeResult> awesome = quotes.GetAwesome();
        IEnumerable<CciResult> cci = quotes.GetCci(lookbackPeriods: 20);
        IEnumerable<CmoResult> cmo = quotes.GetCmo(lookbackPeriods: 20);
        IEnumerable<ConnorsRsiResult> connorsRsi = quotes.GetConnorsRsi(rsiPeriods: 3, streakPeriods: 2, rankPeriods: 100);
        IEnumerable<DpoResult> dpo = quotes.GetDpo(lookbackPeriods: 20);
        IEnumerable<DynamicResult> dynamic = quotes.GetDynamic(lookbackPeriods: 20);
        IEnumerable<ElderRayResult> elderRay = quotes.GetElderRay(lookbackPeriods: 13);
        IEnumerable<FisherTransformResult> fisherTransform = quotes.GetFisherTransform(lookbackPeriods: 10);
        IEnumerable<MacdResult> macd = quotes.GetMacd(fastPeriods: 12, slowPeriods: 26, signalPeriods: 9);
        IEnumerable<PmoResult> pmo = quotes.GetPmo(timePeriods: 35, smoothPeriods: 20, signalPeriods: 10);
        IEnumerable<RocResult> roc1 = quotes.GetRoc(lookbackPeriods: 20);
        IEnumerable<RocResult> roc2 = quotes.GetRoc(lookbackPeriods: 20, smaPeriods: 3);
        IEnumerable<RocWbResult> rocWb = quotes.GetRocWb(lookbackPeriods: 20, emaPeriods: 9, stdDevPeriods: 2);
        IEnumerable<RsiResult> rsi = quotes.GetRsi(lookbackPeriods: 14);
        IEnumerable<SmiResult> smi = quotes.GetSmi(lookbackPeriods: 20);
        IEnumerable<StcResult> stc = quotes.GetStc(cyclePeriods: 10, fastPeriods: 23, slowPeriods: 50);
        IEnumerable<StochResult> stoch1 = quotes.GetStoch(lookbackPeriods: 14, signalPeriods: 3, smoothPeriods: 3);
        IEnumerable<StochResult> stoch2 = quotes.GetStoch(lookbackPeriods: 14, signalPeriods: 3, smoothPeriods: 3, kFactor: 3, dFactor: 2, movingAverageType: MaType.SMA);
        IEnumerable<StochRsiResult> stochRsi = quotes.GetStochRsi(rsiPeriods: 14, stochPeriods: 13, signalPeriods: 9, smoothPeriods: 3);
        IEnumerable<TrixResult> trix = quotes.GetTrix(lookbackPeriods: 20, signalPeriods: 3);
        IEnumerable<TsiResult> tsi = quotes.GetTsi(lookbackPeriods: 20, smoothPeriods: 13, signalPeriods: 7);
        IEnumerable<UltimateResult> ultimate = quotes.GetUltimate(shortPeriods: 7, middlePeriods: 14, longPeriods: 28);
        IEnumerable<WilliamsResult> williamsR = quotes.GetWilliamsR(lookbackPeriods: 14);

        // Volume
        IEnumerable<AdlResult> adl = quotes.GetAdl();
        IEnumerable<ChaikinOscResult> chaikinOsc = quotes.GetChaikinOsc();
        IEnumerable<CmfResult> cmf = quotes.GetCmf(lookbackPeriods: 20);
        IEnumerable<ForceIndexResult> forceIndex = quotes.GetForceIndex(lookbackPeriods: 13);
        IEnumerable<KvoResult> kvo = quotes.GetKvo();
        IEnumerable<MfiResult> mfi = quotes.GetMfi(lookbackPeriods: 14);
        IEnumerable<ObvResult> obv1 = quotes.GetObv();
        IEnumerable<ObvResult> obv2 = quotes.GetObv(smaPeriods: 3);
        IEnumerable<PvoResult> pvo = quotes.GetPvo();
        IEnumerable<VwapResult> vwap = quotes.GetVwap();

        // Price Transforms and Studies
        IEnumerable<AtrResult> atr = quotes.GetAtr(lookbackPeriods: 14);
        IEnumerable<AtrStopResult> atrStop = quotes.GetAtrStop(lookbackPeriods: 14);
        IEnumerable<BopResult> bop = quotes.GetBop();
        IEnumerable<ChopResult> chop = quotes.GetChop(lookbackPeriods: 14);
        IEnumerable<CorrResult> correlation = quotes.GetCorrelation(quotesB: quotes, lookbackPeriods: 20);
        IEnumerable<CandleResult> doji = quotes.GetDoji(maxPriceChangePercent: 0.2);
        IEnumerable<FractalResult> fractal = quotes.GetFractal();
        IEnumerable<HeikinAshiResult> heikinAshi = quotes.GetHeikinAshi();
        IEnumerable<HurstResult> hurst = quotes.GetHurst(lookbackPeriods: 20);
        IEnumerable<IchimokuResult> ichimoku = quotes.GetIchimoku();
        IEnumerable<CandleResult> marubozu = quotes.GetMarubozu(minBodyPercent: 95);
        IEnumerable<ParabolicSarResult> pSar1 = quotes.GetParabolicSar(accelerationStep: 0.02, maxAccelerationFactor: 0.2);
        IEnumerable<ParabolicSarResult> pSar2 = quotes.GetParabolicSar(accelerationStep: 0.02, maxAccelerationFactor: 0.2, initialFactor: 0);
        IEnumerable<PivotsResult> pivots = quotes.GetPivots();
        IEnumerable<PivotPointsResult> pivotPoints = quotes.GetPivotPoints(windowSize: PeriodSize.Week, pointType: PivotPointType.Standard);
        IEnumerable<PrsResult> prs = quotes.GetPrs(quotesBase: quotes);
        IEnumerable<PrsResult> prsWithSma = quotes.GetPrs(quotesBase: quotes, lookbackPeriods: 20, smaPeriods: 3);
        IEnumerable<RenkoResult> renko = quotes.GetRenko(brickSize: 2.0m, endType: EndType.Close);
        IEnumerable<RenkoResult> renkoAtr = quotes.GetRenkoAtr(atrPeriods: 14, endType: EndType.Close);
        IEnumerable<RollingPivotsResult> rollingPivots = quotes.GetRollingPivots(windowPeriods: 14, offsetPeriods: 7, pointType: PivotPointType.Standard);
        IEnumerable<SlopeResult> slope = quotes.GetSlope(lookbackPeriods: 20);
        IEnumerable<StdDevResult> stdDev = quotes.GetStdDev(lookbackPeriods: 20);
        IEnumerable<StdDevResult> stdDevWithSma = quotes.GetStdDev(lookbackPeriods: 20, smaPeriods: 20);
        IEnumerable<SuperTrendResult> superTrend = quotes.GetSuperTrend(lookbackPeriods: 10, multiplier: 3);
        IEnumerable<TrResult> tr = quotes.GetTr();
        IEnumerable<VolatilityStopResult> volatilityStop = quotes.GetVolatilityStop(lookbackPeriods: 20);
        IEnumerable<VortexResult> vortex = quotes.GetVortex(lookbackPeriods: 14);
        IEnumerable<ZigZagResult> zigZag = quotes.GetZigZag(endType: EndType.Close, percentChange: 5m);

        // Beta Variants
        IEnumerable<BetaResult> beta = quotes.GetBeta(quotesMarket: quotes, lookbackPeriods: 20);
        IEnumerable<BetaResult> betaUp = quotes.GetBeta(quotesMarket: quotes, lookbackPeriods: 20, type: BetaType.Up);
        IEnumerable<BetaResult> betaDown = quotes.GetBeta(quotesMarket: quotes, lookbackPeriods: 20, type: BetaType.Down);
        IEnumerable<BetaResult> betaAll = quotes.GetBeta(quotesMarket: quotes, lookbackPeriods: 20, type: BetaType.All);

        // Williams Suite
        IEnumerable<AlligatorResult> alligator = quotes.GetAlligator();
        IEnumerable<GatorResult> gator = quotes.GetGator();
    }
}
