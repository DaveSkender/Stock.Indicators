namespace Performance;

// STYLE COMPARISON BENCHMARKS
// Compares performance between different indicator styles

[ShortRunJob]
[GroupBenchmarksBy(BenchmarkDotNet.Configs.BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public class StyleComparison
{
    private static readonly IReadOnlyList<Bar> bars = Data.GetDefault();
    private static readonly IReadOnlyList<Bar> o = Data.GetCompare();
    private const int n = 14;

    private readonly BarHub barHub = new();
    private readonly BarHub barHubOther = new();


    [GlobalSetup]
    public void Setup()
    {
        barHub.Add(bars);
        barHubOther.Add(o);
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        barHub.EndTransmission();
        barHub.Cache.Clear();

        barHubOther.EndTransmission();
        barHubOther.Cache.Clear();
    }


    [BenchmarkCategory("Adl")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<AdlResult> AdlSeries() => bars.ToAdl();

    [BenchmarkCategory("Adl")]
    [Benchmark]
    public IReadOnlyList<AdlResult> AdlBuffer() => bars.ToAdlList();

    [BenchmarkCategory("Adl")]
    [Benchmark]
    public IReadOnlyList<AdlResult> AdlStream() => barHub.ToAdlHub().Results;

    [BenchmarkCategory("Adx")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<AdxResult> AdxSeries() => bars.ToAdx(n);

    [BenchmarkCategory("Adx")]
    [Benchmark]
    public IReadOnlyList<AdxResult> AdxBuffer() => bars.ToAdxList(n);

    [BenchmarkCategory("Adx")]
    [Benchmark]
    public IReadOnlyList<AdxResult> AdxStream() => barHub.ToAdxHub(n).Results;

    [BenchmarkCategory("Alligator")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<AlligatorResult> AlligatorSeries() => bars.ToAlligator();

    [BenchmarkCategory("Alligator")]
    [Benchmark]
    public IReadOnlyList<AlligatorResult> AlligatorBuffer() => bars.ToAlligatorList();

    [BenchmarkCategory("Alligator")]
    [Benchmark]
    public IReadOnlyList<AlligatorResult> AlligatorStream() => barHub.ToAlligatorHub().Results;

    [BenchmarkCategory("Alma")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<AlmaResult> AlmaSeries() => bars.ToAlma(9, 0.85, 6);

    [BenchmarkCategory("Alma")]
    [Benchmark]
    public IReadOnlyList<AlmaResult> AlmaBuffer() => bars.ToAlmaList(9, 0.85, 6);

    [BenchmarkCategory("Alma")]
    [Benchmark]
    public IReadOnlyList<AlmaResult> AlmaStream() => barHub.ToAlmaHub(9, 0.85, 6).Results;

    [BenchmarkCategory("Aroon")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<AroonResult> AroonSeries() => bars.ToAroon();

    [BenchmarkCategory("Aroon")]
    [Benchmark]
    public IReadOnlyList<AroonResult> AroonBuffer() => bars.ToAroonList();

    [BenchmarkCategory("Aroon")]
    [Benchmark]
    public IReadOnlyList<AroonResult> AroonStream() => barHub.ToAroonHub().Results;

    [BenchmarkCategory("Atr")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<AtrResult> AtrSeries() => bars.ToAtr(n);

    [BenchmarkCategory("Atr")]
    [Benchmark]
    public IReadOnlyList<AtrResult> AtrBuffer() => bars.ToAtrList(n);

    [BenchmarkCategory("Atr")]
    [Benchmark]
    public IReadOnlyList<AtrResult> AtrStream() => barHub.ToAtrHub(n).Results;

    [BenchmarkCategory("AtrStop")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<AtrStopResult> AtrStopSeries() => bars.ToAtrStop();

    [BenchmarkCategory("AtrStop")]
    [Benchmark]
    public IReadOnlyList<AtrStopResult> AtrStopBuffer() => bars.ToAtrStopList();

    [BenchmarkCategory("AtrStop")]
    [Benchmark]
    public IReadOnlyList<AtrStopResult> AtrStopStream() => barHub.ToAtrStopHub().Results;

    [BenchmarkCategory("Awesome")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<AwesomeResult> AwesomeSeries() => bars.ToAwesome();

    [BenchmarkCategory("Awesome")]
    [Benchmark]
    public IReadOnlyList<AwesomeResult> AwesomeBuffer() => bars.ToAwesomeList();

    [BenchmarkCategory("Awesome")]
    [Benchmark]
    public IReadOnlyList<AwesomeResult> AwesomeStream() => barHub.ToAwesomeHub().Results;

    [BenchmarkCategory("Beta")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<BetaResult> BetaSeries() => bars.ToBeta(o, 20, BetaType.Standard);

    [BenchmarkCategory("BollingerBands")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<BollingerBandsResult> BollingerBandsSeries() => bars.ToBollingerBands(20, 2);

    [BenchmarkCategory("BollingerBands")]
    [Benchmark]
    public IReadOnlyList<BollingerBandsResult> BollingerBandsBuffer() => bars.ToBollingerBandsList(20, 2);

    [BenchmarkCategory("BollingerBands")]
    [Benchmark]
    public IReadOnlyList<BollingerBandsResult> BollingerBandsStream() => barHub.ToBollingerBandsHub(20, 2).Results;

    [BenchmarkCategory("Bop")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<BopResult> BopSeries() => bars.ToBop(n);

    [BenchmarkCategory("Bop")]
    [Benchmark]
    public IReadOnlyList<BopResult> BopBuffer() => bars.ToBopList(n);

    [BenchmarkCategory("Bop")]
    [Benchmark]
    public IReadOnlyList<BopResult> BopStream() => barHub.ToBopHub(n).Results;

    [BenchmarkCategory("Cci")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<CciResult> CciSeries() => bars.ToCci(n);

    [BenchmarkCategory("Cci")]
    [Benchmark]
    public IReadOnlyList<CciResult> CciBuffer() => bars.ToCciList(n);

    [BenchmarkCategory("Cci")]
    [Benchmark]
    public IReadOnlyList<CciResult> CciStream() => barHub.ToCciHub(n).Results;

    [BenchmarkCategory("ChaikinOsc")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<ChaikinOscResult> ChaikinOscSeries() => bars.ToChaikinOsc();

    [BenchmarkCategory("ChaikinOsc")]
    [Benchmark]
    public IReadOnlyList<ChaikinOscResult> ChaikinOscBuffer() => bars.ToChaikinOscList();

    [BenchmarkCategory("ChaikinOsc")]
    [Benchmark]
    public IReadOnlyList<ChaikinOscResult> ChaikinOscStream() => barHub.ToChaikinOscHub().Results;

    [BenchmarkCategory("Chandelier")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<ChandelierResult> ChandelierSeries() => bars.ToChandelier();

    [BenchmarkCategory("Chandelier")]
    [Benchmark]
    public IReadOnlyList<ChandelierResult> ChandelierBuffer() => bars.ToChandelierList();

    [BenchmarkCategory("Chandelier")]
    [Benchmark]
    public IReadOnlyList<ChandelierResult> ChandelierStream() => barHub.ToChandelierHub().Results;

    [BenchmarkCategory("Chop")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<ChopResult> ChopSeries() => bars.ToChop(n);

    [BenchmarkCategory("Chop")]
    [Benchmark]
    public IReadOnlyList<ChopResult> ChopBuffer() => bars.ToChopList(n);

    [BenchmarkCategory("Chop")]
    [Benchmark]
    public IReadOnlyList<ChopResult> ChopStream() => barHub.ToChopHub(n).Results;

    [BenchmarkCategory("Cmf")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<CmfResult> CmfSeries() => bars.ToCmf(n);

    [BenchmarkCategory("Cmf")]
    [Benchmark]
    public IReadOnlyList<CmfResult> CmfBuffer() => bars.ToCmfList(n);

    [BenchmarkCategory("Cmf")]
    [Benchmark]
    public IReadOnlyList<CmfResult> CmfStream() => barHub.ToCmfHub(n).Results;

    [BenchmarkCategory("Cmo")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<CmoResult> CmoSeries() => bars.ToCmo(n);

    [BenchmarkCategory("Cmo")]
    [Benchmark]
    public IReadOnlyList<CmoResult> CmoBuffer() => bars.ToCmoList(n);

    [BenchmarkCategory("Cmo")]
    [Benchmark]
    public IReadOnlyList<CmoResult> CmoStream() => barHub.ToCmoHub(n).Results;

    [BenchmarkCategory("ConnorsRsi")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<ConnorsRsiResult> ConnorsRsiSeries() => bars.ToConnorsRsi(3, 2, 100);

    [BenchmarkCategory("ConnorsRsi")]
    [Benchmark]
    public IReadOnlyList<ConnorsRsiResult> ConnorsRsiBuffer() => bars.ToConnorsRsiList(3, 2, 100);

    [BenchmarkCategory("ConnorsRsi")]
    [Benchmark]
    public IReadOnlyList<ConnorsRsiResult> ConnorsRsiStream() => barHub.ToConnorsRsiHub(3, 2, 100).Results;

    [BenchmarkCategory("Correlation")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<CorrResult> CorrelationSeries() => bars.ToCorrelation(o, 20);

    [BenchmarkCategory("Dema")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<DemaResult> DemaSeries() => bars.ToDema(n);

    [BenchmarkCategory("Dema")]
    [Benchmark]
    public IReadOnlyList<DemaResult> DemaBuffer() => bars.ToDemaList(n);

    [BenchmarkCategory("Dema")]
    [Benchmark]
    public IReadOnlyList<DemaResult> DemaStream() => barHub.ToDemaHub(n).Results;

    [BenchmarkCategory("Doji")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<CandleResult> DojiSeries() => bars.ToDoji();

    [BenchmarkCategory("Doji")]
    [Benchmark]
    public IReadOnlyList<CandleResult> DojiBuffer() => bars.ToDojiList();

    [BenchmarkCategory("Doji")]
    [Benchmark]
    public IReadOnlyList<CandleResult> DojiStream() => barHub.ToDojiHub().Results;

    [BenchmarkCategory("Donchian")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<DonchianResult> DonchianSeries() => bars.ToDonchian(20);

    [BenchmarkCategory("Donchian")]
    [Benchmark]
    public IReadOnlyList<DonchianResult> DonchianBuffer() => bars.ToDonchianList(20);

    [BenchmarkCategory("Donchian")]
    [Benchmark]
    public IReadOnlyList<DonchianResult> DonchianStream() => barHub.ToDonchianHub(20).Results;

    [BenchmarkCategory("Dpo")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<DpoResult> DpoSeries() => bars.ToDpo(n);

    [BenchmarkCategory("Dpo")]
    [Benchmark]
    public IReadOnlyList<DpoResult> DpoBuffer() => bars.ToDpoList(n);

    [BenchmarkCategory("Dpo")]
    [Benchmark]
    public IReadOnlyList<DpoResult> DpoStream() => barHub.ToDpoHub(n).Results;

    [BenchmarkCategory("Dynamic")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<DynamicResult> DynamicSeries() => bars.ToDynamic(n);

    [BenchmarkCategory("Dynamic")]
    [Benchmark]
    public IReadOnlyList<DynamicResult> DynamicBuffer() => bars.ToDynamicList(n);

    [BenchmarkCategory("Dynamic")]
    [Benchmark]
    public IReadOnlyList<DynamicResult> DynamicStream() => barHub.ToDynamicHub(n).Results;

    [BenchmarkCategory("ElderRay")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<ElderRayResult> ElderRaySeries() => bars.ToElderRay(13);

    [BenchmarkCategory("ElderRay")]
    [Benchmark]
    public IReadOnlyList<ElderRayResult> ElderRayBuffer() => bars.ToElderRayList(13);

    [BenchmarkCategory("ElderRay")]
    [Benchmark]
    public IReadOnlyList<ElderRayResult> ElderRayStream() => barHub.ToElderRayHub(13).Results;

    [BenchmarkCategory("Ema")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<EmaResult> EmaSeries() => bars.ToEma(20);

    [BenchmarkCategory("Ema")]
    [Benchmark]
    public IReadOnlyList<EmaResult> EmaBuffer() => bars.ToEmaList(20);

    [BenchmarkCategory("Ema")]
    [Benchmark]
    public IReadOnlyList<EmaResult> EmaStream() => barHub.ToEmaHub(20).Results;

    [BenchmarkCategory("Epma")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<EpmaResult> EpmaSeries() => bars.ToEpma(n);

    [BenchmarkCategory("Epma")]
    [Benchmark]
    public IReadOnlyList<EpmaResult> EpmaBuffer() => bars.ToEpmaList(n);

    [BenchmarkCategory("Epma")]
    [Benchmark]
    public IReadOnlyList<EpmaResult> EpmaStream() => barHub.ToEpmaHub(n).Results;

    [BenchmarkCategory("Fcb")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<FcbResult> FcbSeries() => bars.ToFcb(2);

    [BenchmarkCategory("Fcb")]
    [Benchmark]
    public IReadOnlyList<FcbResult> FcbBuffer() => bars.ToFcbList(2);

    [BenchmarkCategory("Fcb")]
    [Benchmark]
    public IReadOnlyList<FcbResult> FcbStream() => barHub.ToFcbHub(2).Results;

    [BenchmarkCategory("FisherTransform")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<FisherTransformResult> FisherTransformSeries() => bars.ToFisherTransform(10);

    [BenchmarkCategory("FisherTransform")]
    [Benchmark]
    public IReadOnlyList<FisherTransformResult> FisherTransformBuffer() => bars.ToFisherTransformList(10);

    [BenchmarkCategory("FisherTransform")]
    [Benchmark]
    public IReadOnlyList<FisherTransformResult> FisherTransformStream() => barHub.ToFisherTransformHub(10).Results;

    [BenchmarkCategory("ForceIndex")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<ForceIndexResult> ForceIndexSeries() => bars.ToForceIndex(2);

    [BenchmarkCategory("ForceIndex")]
    [Benchmark]
    public IReadOnlyList<ForceIndexResult> ForceIndexBuffer() => bars.ToForceIndexList(2);

    [BenchmarkCategory("ForceIndex")]
    [Benchmark]
    public IReadOnlyList<ForceIndexResult> ForceIndexStream() => barHub.ToForceIndexHub(2).Results;

    [BenchmarkCategory("Fractal")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<FractalResult> FractalSeries() => bars.ToFractal();

    [BenchmarkCategory("Fractal")]
    [Benchmark]
    public IReadOnlyList<FractalResult> FractalBuffer() => bars.ToFractalList();

    [BenchmarkCategory("Fractal")]
    [Benchmark]
    public IReadOnlyList<FractalResult> FractalStream() => barHub.ToFractalHub().Results;

    [BenchmarkCategory("Gator")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<GatorResult> GatorSeries() => bars.ToGator();

    [BenchmarkCategory("Gator")]
    [Benchmark]
    public IReadOnlyList<GatorResult> GatorBuffer() => bars.ToGatorList();

    [BenchmarkCategory("Gator")]
    [Benchmark]
    public IReadOnlyList<GatorResult> GatorStream() => barHub.ToGatorHub().Results;

    [BenchmarkCategory("HeikinAshi")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<HeikinAshiResult> HeikinAshiSeries() => bars.ToHeikinAshi();

    [BenchmarkCategory("HeikinAshi")]
    [Benchmark]
    public IReadOnlyList<HeikinAshiResult> HeikinAshiBuffer() => bars.ToHeikinAshiList();

    [BenchmarkCategory("HeikinAshi")]
    [Benchmark]
    public IReadOnlyList<HeikinAshiResult> HeikinAshiStream() => barHub.ToHeikinAshiHub().Results;

    [BenchmarkCategory("Hma")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<HmaResult> HmaSeries() => bars.ToHma(n);

    [BenchmarkCategory("Hma")]
    [Benchmark]
    public IReadOnlyList<HmaResult> HmaBuffer() => bars.ToHmaList(n);

    [BenchmarkCategory("Hma")]
    [Benchmark]
    public IReadOnlyList<HmaResult> HmaStream() => barHub.ToHmaHub(n).Results;

    [BenchmarkCategory("HtTrendline")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<HtlResult> HtTrendlineSeries() => bars.ToHtTrendline();

    [BenchmarkCategory("HtTrendline")]
    [Benchmark]
    public IReadOnlyList<HtlResult> HtTrendlineBuffer() => bars.ToHtTrendlineList();

    [BenchmarkCategory("HtTrendline")]
    [Benchmark]
    public IReadOnlyList<HtlResult> HtTrendlineStream() => barHub.ToHtTrendlineHub().Results;

    [BenchmarkCategory("Hurst")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<HurstResult> HurstSeries() => bars.ToHurst(100);

    [BenchmarkCategory("Hurst")]
    [Benchmark]
    public IReadOnlyList<HurstResult> HurstBuffer() => bars.ToHurstList(100);

    [BenchmarkCategory("Hurst")]
    [Benchmark]
    public IReadOnlyList<HurstResult> HurstStream() => barHub.ToHurstHub(100).Results;

    [BenchmarkCategory("Ichimoku")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<IchimokuResult> IchimokuSeries() => bars.ToIchimoku();

    [BenchmarkCategory("Ichimoku")]
    [Benchmark]
    public IReadOnlyList<IchimokuResult> IchimokuBuffer() => bars.ToIchimokuList();

    [BenchmarkCategory("Ichimoku")]
    [Benchmark]
    public IReadOnlyList<IchimokuResult> IchimokuStream() => barHub.ToIchimokuHub().Results;

    [BenchmarkCategory("Kama")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<KamaResult> KamaSeries() => bars.ToKama(10, 2, 30);

    [BenchmarkCategory("Kama")]
    [Benchmark]
    public IReadOnlyList<KamaResult> KamaBuffer() => bars.ToKamaList(10, 2, 30);

    [BenchmarkCategory("Kama")]
    [Benchmark]
    public IReadOnlyList<KamaResult> KamaStream() => barHub.ToKamaHub(10, 2, 30).Results;

    [BenchmarkCategory("Keltner")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<KeltnerResult> KeltnerSeries() => bars.ToKeltner(20, 2, 10);

    [BenchmarkCategory("Keltner")]
    [Benchmark]
    public IReadOnlyList<KeltnerResult> KeltnerBuffer() => bars.ToKeltnerList(20, 2, 10);

    [BenchmarkCategory("Keltner")]
    [Benchmark]
    public IReadOnlyList<KeltnerResult> KeltnerStream() => barHub.ToKeltnerHub(20, 2, 10).Results;

    [BenchmarkCategory("Kvo")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<KvoResult> KvoSeries() => bars.ToKvo(34, 55, 13);

    [BenchmarkCategory("Kvo")]
    [Benchmark]
    public IReadOnlyList<KvoResult> KvoBuffer() => bars.ToKvoList(34, 55, 13);

    [BenchmarkCategory("Kvo")]
    [Benchmark]
    public IReadOnlyList<KvoResult> KvoStream() => barHub.ToKvoHub(34, 55, 13).Results;

    [BenchmarkCategory("MaEnvelopes")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<MaEnvelopeResult> MaEnvelopesSeries() => bars.ToMaEnvelopes(20, 2.5, MaType.SMA);

    [BenchmarkCategory("MaEnvelopes")]
    [Benchmark]
    public IReadOnlyList<MaEnvelopeResult> MaEnvelopesBuffer() => bars.ToMaEnvelopesList(20, 2.5, MaType.SMA);

    [BenchmarkCategory("MaEnvelopes")]
    [Benchmark]
    public IReadOnlyList<MaEnvelopeResult> MaEnvelopesStream() => barHub.ToMaEnvelopesHub(20, 2.5, MaType.SMA).Results;

    [BenchmarkCategory("Macd")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<MacdResult> MacdSeries() => bars.ToMacd(12, 26, 9);

    [BenchmarkCategory("Macd")]
    [Benchmark]
    public IReadOnlyList<MacdResult> MacdBuffer() => bars.ToMacdList(12, 26, 9);

    [BenchmarkCategory("Macd")]
    [Benchmark]
    public IReadOnlyList<MacdResult> MacdStream() => barHub.ToMacdHub(12, 26, 9).Results;

    [BenchmarkCategory("Mama")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<MamaResult> MamaSeries() => bars.ToMama(0.5, 0.05);

    [BenchmarkCategory("Mama")]
    [Benchmark]
    public IReadOnlyList<MamaResult> MamaBuffer() => bars.ToMamaList(0.5, 0.05);

    [BenchmarkCategory("Mama")]
    [Benchmark]
    public IReadOnlyList<MamaResult> MamaStream() => barHub.ToMamaHub(0.5, 0.05).Results;

    [BenchmarkCategory("Marubozu")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<CandleResult> MarubozuSeries() => bars.ToMarubozu(95);

    [BenchmarkCategory("Marubozu")]
    [Benchmark]
    public IReadOnlyList<CandleResult> MarubozuBuffer() => bars.ToMarubozuList(95);

    [BenchmarkCategory("Marubozu")]
    [Benchmark]
    public IReadOnlyList<CandleResult> MarubozuStream() => barHub.ToMarubozuHub(95).Results;

    [BenchmarkCategory("Mfi")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<MfiResult> MfiSeries() => bars.ToMfi(n);

    [BenchmarkCategory("Mfi")]
    [Benchmark]
    public IReadOnlyList<MfiResult> MfiBuffer() => bars.ToMfiList(n);

    [BenchmarkCategory("Mfi")]
    [Benchmark]
    public IReadOnlyList<MfiResult> MfiStream() => barHub.ToMfiHub(n).Results;

    [BenchmarkCategory("Obv")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<ObvResult> ObvSeries() => bars.ToObv();

    [BenchmarkCategory("Obv")]
    [Benchmark]
    public IReadOnlyList<ObvResult> ObvBuffer() => bars.ToObvList();

    [BenchmarkCategory("Obv")]
    [Benchmark]
    public IReadOnlyList<ObvResult> ObvStream() => barHub.ToObvHub().Results;

    [BenchmarkCategory("ParabolicSar")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<ParabolicSarResult> ParabolicSarSeries() => bars.ToParabolicSar();

    [BenchmarkCategory("ParabolicSar")]
    [Benchmark]
    public IReadOnlyList<ParabolicSarResult> ParabolicSarBuffer() => bars.ToParabolicSarList();

    [BenchmarkCategory("ParabolicSar")]
    [Benchmark]
    public IReadOnlyList<ParabolicSarResult> ParabolicSarStream() => barHub.ToParabolicSarHub().Results;

    [BenchmarkCategory("PivotPoints")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<PivotPointsResult> PivotPointsSeries() => bars.ToPivotPoints(BarInterval.Month, PivotPointType.Standard);

    [BenchmarkCategory("PivotPoints")]
    [Benchmark]
    public IReadOnlyList<PivotPointsResult> PivotPointsBuffer() => bars.ToPivotPointsList(BarInterval.Month, PivotPointType.Standard);

    [BenchmarkCategory("PivotPoints")]
    [Benchmark]
    public IReadOnlyList<PivotPointsResult> PivotPointsStream() => barHub.ToPivotPointsHub(BarInterval.Month, PivotPointType.Standard).Results;

    [BenchmarkCategory("Pivots")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<PivotsResult> PivotsSeries() => bars.ToPivots(2, 2, 20);

    [BenchmarkCategory("Pivots")]
    [Benchmark]
    public IReadOnlyList<PivotsResult> PivotsBuffer() => bars.ToPivotsList(2, 2, 20);

    [BenchmarkCategory("Pivots")]
    [Benchmark]
    public IReadOnlyList<PivotsResult> PivotsStream() => barHub.ToPivotsHub(2, 2, 20).Results;

    [BenchmarkCategory("Pmo")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<PmoResult> PmoSeries() => bars.ToPmo(35, 20, 10);

    [BenchmarkCategory("Pmo")]
    [Benchmark]
    public IReadOnlyList<PmoResult> PmoBuffer() => bars.ToPmoList(35, 20, 10);

    [BenchmarkCategory("Pmo")]
    [Benchmark]
    public IReadOnlyList<PmoResult> PmoStream() => barHub.ToPmoHub(35, 20, 10).Results;

    [BenchmarkCategory("Prs")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<PrsResult> PrsSeries() => bars.ToPrs(o);

    [BenchmarkCategory("Pvo")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<PvoResult> PvoSeries() => bars.ToPvo();

    [BenchmarkCategory("Pvo")]
    [Benchmark]
    public IReadOnlyList<PvoResult> PvoBuffer() => bars.ToPvoList();

    [BenchmarkCategory("Pvo")]
    [Benchmark]
    public IReadOnlyList<PvoResult> PvoStream() => barHub.ToPvoHub().Results;

    [BenchmarkCategory("BarPart")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<TimeValue> BarPartSeries() => bars.ToBarPart(CandlePart.OHL3);

    [BenchmarkCategory("BarPart")]
    [Benchmark]
    public IReadOnlyList<TimeValue> BarPartBuffer() => bars.ToBarPartList(CandlePart.OHL3);

    [BenchmarkCategory("BarPart")]
    [Benchmark]
    public IReadOnlyList<TimeValue> BarPartStream() => barHub.ToBarPartHub(CandlePart.OHL3).Results;

    [BenchmarkCategory("Renko")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<RenkoResult> RenkoSeries() => bars.ToRenko(2.5m);

    [BenchmarkCategory("Renko")]
    [Benchmark]
    public IReadOnlyList<RenkoResult> RenkoBuffer() => bars.ToRenkoList(2.5m);

    [BenchmarkCategory("Renko")]
    [Benchmark]
    public IReadOnlyList<RenkoResult> RenkoStream() => barHub.ToRenkoHub(2.5m).Results;

    [BenchmarkCategory("RenkoAtr")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<RenkoResult> RenkoAtrSeries() => bars.ToRenkoAtr(n);

    [BenchmarkCategory("Roc")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<RocResult> RocSeries() => bars.ToRoc(20);

    [BenchmarkCategory("Roc")]
    [Benchmark]
    public IReadOnlyList<RocResult> RocBuffer() => bars.ToRocList(20);

    [BenchmarkCategory("Roc")]
    [Benchmark]
    public IReadOnlyList<RocResult> RocStream() => barHub.ToRocHub(20).Results;

    [BenchmarkCategory("RocWb")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<RocWbResult> RocWbSeries() => bars.ToRocWb(20, 5, 5);

    [BenchmarkCategory("RocWb")]
    [Benchmark]
    public IReadOnlyList<RocWbResult> RocWbBuffer() => bars.ToRocWbList(20, 5, 5);

    [BenchmarkCategory("RocWb")]
    [Benchmark]
    public IReadOnlyList<RocWbResult> RocWbStream() => barHub.ToRocWbHub(20, 5, 5).Results;

    [BenchmarkCategory("RollingPivots")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<RollingPivotsResult> RollingPivotsSeries() => bars.ToRollingPivots(20, 0, PivotPointType.Standard);

    [BenchmarkCategory("RollingPivots")]
    [Benchmark]
    public IReadOnlyList<RollingPivotsResult> RollingPivotsBuffer() => bars.ToRollingPivotsList(20, 0, PivotPointType.Standard);

    [BenchmarkCategory("RollingPivots")]
    [Benchmark]
    public IReadOnlyList<RollingPivotsResult> RollingPivotsStream() => barHub.ToRollingPivotsHub(20, 0, PivotPointType.Standard).Results;

    [BenchmarkCategory("Rsi")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<RsiResult> RsiSeries() => bars.ToRsi(n);

    [BenchmarkCategory("Rsi")]
    [Benchmark]
    public IReadOnlyList<RsiResult> RsiBuffer() => bars.ToRsiList(n);

    [BenchmarkCategory("Rsi")]
    [Benchmark]
    public IReadOnlyList<RsiResult> RsiStream() => barHub.ToRsiHub(n).Results;

    [BenchmarkCategory("Slope")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<SlopeResult> SlopeSeries() => bars.ToSlope(n);

    [BenchmarkCategory("Slope")]
    [Benchmark]
    public IReadOnlyList<SlopeResult> SlopeBuffer() => bars.ToSlopeList(n);

    [BenchmarkCategory("Slope")]
    [Benchmark]
    public IReadOnlyList<SlopeResult> SlopeStream() => barHub.ToSlopeHub(n).Results;

    [BenchmarkCategory("Sma")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<SmaResult> SmaSeries() => bars.ToSma(n);

    [BenchmarkCategory("Sma")]
    [Benchmark]
    public IReadOnlyList<SmaResult> SmaBuffer() => bars.ToSmaList(n);

    [BenchmarkCategory("Sma")]
    [Benchmark]
    public IReadOnlyList<SmaResult> SmaStream() => barHub.ToSmaHub(n).Results;

    [BenchmarkCategory("SmaAnalysis")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<SmaAnalysisResult> SmaAnalysisSeries() => bars.ToSmaAnalysis(n);

    [BenchmarkCategory("SmaAnalysis")]
    [Benchmark]
    public IReadOnlyList<SmaAnalysisResult> SmaAnalysisBuffer() => bars.ToSmaAnalysisList(n);

    [BenchmarkCategory("SmaAnalysis")]
    [Benchmark]
    public IReadOnlyList<SmaAnalysisResult> SmaAnalysisStream() => barHub.ToSmaAnalysisHub(n).Results;

    [BenchmarkCategory("Smi")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<SmiResult> SmiSeries() => bars.ToSmi(13, 25, 2, 3);

    [BenchmarkCategory("Smi")]
    [Benchmark]
    public IReadOnlyList<SmiResult> SmiBuffer() => bars.ToSmiList(13, 25, 2, 3);

    [BenchmarkCategory("Smi")]
    [Benchmark]
    public IReadOnlyList<SmiResult> SmiStream() => barHub.ToSmiHub(13, 25, 2, 3).Results;

    [BenchmarkCategory("Smma")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<SmmaResult> SmmaSeries() => bars.ToSmma(n);

    [BenchmarkCategory("Smma")]
    [Benchmark]
    public IReadOnlyList<SmmaResult> SmmaBuffer() => bars.ToSmmaList(n);

    [BenchmarkCategory("Smma")]
    [Benchmark]
    public IReadOnlyList<SmmaResult> SmmaStream() => barHub.ToSmmaHub(n).Results;

    [BenchmarkCategory("StarcBands")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<StarcBandsResult> StarcBandsSeries() => bars.ToStarcBands(5, 2, 10);

    [BenchmarkCategory("StarcBands")]
    [Benchmark]
    public IReadOnlyList<StarcBandsResult> StarcBandsBuffer() => bars.ToStarcBandsList(5, 2, 10);

    [BenchmarkCategory("StarcBands")]
    [Benchmark]
    public IReadOnlyList<StarcBandsResult> StarcBandsStream() => barHub.ToStarcBandsHub(5, 2, 10).Results;

    [BenchmarkCategory("Stc")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<StcResult> StcSeries() => bars.ToStc(10, 23, 50);

    [BenchmarkCategory("Stc")]
    [Benchmark]
    public IReadOnlyList<StcResult> StcBuffer() => bars.ToStcList(10, 23, 50);

    [BenchmarkCategory("Stc")]
    [Benchmark]
    public IReadOnlyList<StcResult> StcStream() => barHub.ToStcHub(10, 23, 50).Results;

    [BenchmarkCategory("StdDev")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<StdDevResult> StdDevSeries() => bars.ToStdDev(n);

    [BenchmarkCategory("StdDev")]
    [Benchmark]
    public IReadOnlyList<StdDevResult> StdDevBuffer() => bars.ToStdDevList(n);

    [BenchmarkCategory("StdDev")]
    [Benchmark]
    public IReadOnlyList<StdDevResult> StdDevStream() => barHub.ToStdDevHub(n).Results;

    [BenchmarkCategory("StdDevChannels")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<StdDevChannelsResult> StdDevChannelsSeries() => bars.ToStdDevChannels();

    [BenchmarkCategory("Stoch")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<StochResult> StochSeries() => bars.ToStoch(14, 3, 3);

    [BenchmarkCategory("Stoch")]
    [Benchmark]
    public IReadOnlyList<StochResult> StochBuffer() => bars.ToStochList(14, 3, 3);

    [BenchmarkCategory("Stoch")]
    [Benchmark]
    public IReadOnlyList<StochResult> StochStream() => barHub.ToStochHub(14, 3, 3).Results;

    [BenchmarkCategory("StochRsi")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<StochRsiResult> StochRsiSeries() => bars.ToStochRsi(14, 14, 3, 1);

    [BenchmarkCategory("StochRsi")]
    [Benchmark]
    public IReadOnlyList<StochRsiResult> StochRsiBuffer() => bars.ToStochRsiList(14, 14, 3, 1);

    [BenchmarkCategory("StochRsi")]
    [Benchmark]
    public IReadOnlyList<StochRsiResult> StochRsiStream() => barHub.ToStochRsiHub(14, 14, 3, 1).Results;

    [BenchmarkCategory("StochSmma")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<StochResult> StochSmmaSeries() => bars.ToStoch(14, 3, 3);

    [BenchmarkCategory("StochSmma")]
    [Benchmark]
    public IReadOnlyList<StochResult> StochSmmaBuffer() => bars.ToStochList(14, 3, 3);

    [BenchmarkCategory("StochSmma")]
    [Benchmark]
    public IReadOnlyList<StochResult> StochSmmaStream() => barHub.ToStochHub(14, 3, 3).Results;

    [BenchmarkCategory("SuperTrend")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<SuperTrendResult> SuperTrendSeries() => bars.ToSuperTrend(10, 3);

    [BenchmarkCategory("SuperTrend")]
    [Benchmark]
    public IReadOnlyList<SuperTrendResult> SuperTrendBuffer() => bars.ToSuperTrendList(10, 3);

    [BenchmarkCategory("SuperTrend")]
    [Benchmark]
    public IReadOnlyList<SuperTrendResult> SuperTrendStream() => barHub.ToSuperTrendHub(10, 3).Results;

    [BenchmarkCategory("T3")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<T3Result> T3Series() => bars.ToT3(5, 0.7);

    [BenchmarkCategory("T3")]
    [Benchmark]
    public IReadOnlyList<T3Result> T3Buffer() => bars.ToT3List(5, 0.7);

    [BenchmarkCategory("T3")]
    [Benchmark]
    public IReadOnlyList<T3Result> T3Stream() => barHub.ToT3Hub(5, 0.7).Results;

    [BenchmarkCategory("Tema")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<TemaResult> TemaSeries() => bars.ToTema(n);

    [BenchmarkCategory("Tema")]
    [Benchmark]
    public IReadOnlyList<TemaResult> TemaBuffer() => bars.ToTemaList(n);

    [BenchmarkCategory("Tema")]
    [Benchmark]
    public IReadOnlyList<TemaResult> TemaStream() => barHub.ToTemaHub(n).Results;

    [BenchmarkCategory("Tr")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<TrResult> TrSeries() => bars.ToTr();

    [BenchmarkCategory("Tr")]
    [Benchmark]
    public IReadOnlyList<TrResult> TrBuffer() => bars.ToTrList();

    [BenchmarkCategory("Tr")]
    [Benchmark]
    public IReadOnlyList<TrResult> TrStream() => barHub.ToTrHub().Results;

    [BenchmarkCategory("Trix")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<TrixResult> TrixSeries() => bars.ToTrix(n);

    [BenchmarkCategory("Trix")]
    [Benchmark]
    public IReadOnlyList<TrixResult> TrixBuffer() => bars.ToTrixList(n);

    [BenchmarkCategory("Trix")]
    [Benchmark]
    public IReadOnlyList<TrixResult> TrixStream() => barHub.ToTrixHub(n).Results;

    [BenchmarkCategory("Tsi")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<TsiResult> TsiSeries() => bars.ToTsi(25, 13, 7);

    [BenchmarkCategory("Tsi")]
    [Benchmark]
    public IReadOnlyList<TsiResult> TsiBuffer() => bars.ToTsiList(25, 13, 7);

    [BenchmarkCategory("Tsi")]
    [Benchmark]
    public IReadOnlyList<TsiResult> TsiStream() => barHub.ToTsiHub(25, 13, 7).Results;

    [BenchmarkCategory("UlcerIndex")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<UlcerIndexResult> UlcerIndexSeries() => bars.ToUlcerIndex(n);

    [BenchmarkCategory("UlcerIndex")]
    [Benchmark]
    public IReadOnlyList<UlcerIndexResult> UlcerIndexBuffer() => bars.ToUlcerIndexList(n);

    [BenchmarkCategory("UlcerIndex")]
    [Benchmark]
    public IReadOnlyList<UlcerIndexResult> UlcerIndexStream() => barHub.ToUlcerIndexHub(n).Results;

    [BenchmarkCategory("Ultimate")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<UltimateResult> UltimateSeries() => bars.ToUltimate(7, 14, 28);

    [BenchmarkCategory("Ultimate")]
    [Benchmark]
    public IReadOnlyList<UltimateResult> UltimateBuffer() => bars.ToUltimateList(7, 14, 28);

    [BenchmarkCategory("Ultimate")]
    [Benchmark]
    public IReadOnlyList<UltimateResult> UltimateStream() => barHub.ToUltimateHub(7, 14, 28).Results;

    [BenchmarkCategory("VolatilityStop")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<VolatilityStopResult> VolatilityStopSeries() => bars.ToVolatilityStop(7, 3);

    [BenchmarkCategory("VolatilityStop")]
    [Benchmark]
    public IReadOnlyList<VolatilityStopResult> VolatilityStopBuffer() => bars.ToVolatilityStopList(7, 3);

    [BenchmarkCategory("VolatilityStop")]
    [Benchmark]
    public IReadOnlyList<VolatilityStopResult> VolatilityStopStream() => barHub.ToVolatilityStopHub(7, 3).Results;

    [BenchmarkCategory("Vortex")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<VortexResult> VortexSeries() => bars.ToVortex(n);

    [BenchmarkCategory("Vortex")]
    [Benchmark]
    public IReadOnlyList<VortexResult> VortexBuffer() => bars.ToVortexList(n);

    [BenchmarkCategory("Vortex")]
    [Benchmark]
    public IReadOnlyList<VortexResult> VortexStream() => barHub.ToVortexHub(n).Results;

    [BenchmarkCategory("Vwap")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<VwapResult> VwapSeries() => bars.ToVwap();

    [BenchmarkCategory("Vwap")]
    [Benchmark]
    public IReadOnlyList<VwapResult> VwapBuffer() => bars.ToVwapList();

    [BenchmarkCategory("Vwap")]
    [Benchmark]
    public IReadOnlyList<VwapResult> VwapStream() => barHub.ToVwapHub().Results;

    [BenchmarkCategory("Vwma")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<VwmaResult> VwmaSeries() => bars.ToVwma(n);

    [BenchmarkCategory("Vwma")]
    [Benchmark]
    public IReadOnlyList<VwmaResult> VwmaBuffer() => bars.ToVwmaList(n);

    [BenchmarkCategory("Vwma")]
    [Benchmark]
    public IReadOnlyList<VwmaResult> VwmaStream() => barHub.ToVwmaHub(n).Results;

    [BenchmarkCategory("WilliamsR")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<WilliamsResult> WilliamsRSeries() => bars.ToWilliamsR();

    [BenchmarkCategory("WilliamsR")]
    [Benchmark]
    public IReadOnlyList<WilliamsResult> WilliamsRBuffer() => bars.ToWilliamsRList();

    [BenchmarkCategory("WilliamsR")]
    [Benchmark]
    public IReadOnlyList<WilliamsResult> WilliamsRStream() => barHub.ToWilliamsRHub().Results;

    [BenchmarkCategory("Wma")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<WmaResult> WmaSeries() => bars.ToWma(n);

    [BenchmarkCategory("Wma")]
    [Benchmark]
    public IReadOnlyList<WmaResult> WmaBuffer() => bars.ToWmaList(n);

    [BenchmarkCategory("Wma")]
    [Benchmark]
    public IReadOnlyList<WmaResult> WmaStream() => barHub.ToWmaHub(n).Results;

    [BenchmarkCategory("ZigZag")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<ZigZagResult> ZigZagSeries() => bars.ToZigZag();
}
