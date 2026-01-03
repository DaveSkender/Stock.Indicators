namespace Performance;

// STYLE COMPARISON BENCHMARKS
// Compares performance between different indicator styles

[ShortRunJob]
[GroupBenchmarksBy(BenchmarkDotNet.Configs.BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public class StyleComparison
{
    private static readonly IReadOnlyList<Quote> quotes = Data.GetDefault();
    private static readonly IReadOnlyList<Quote> o = Data.GetCompare();
    private const int n = 14;

    private readonly QuoteHub quoteHub = new();
    private readonly QuoteHub quoteHubOther = new();


    [GlobalSetup]
    public void Setup()
    {
        quoteHub.Add(quotes);
        quoteHubOther.Add(o);
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        quoteHub.EndTransmission();
        quoteHub.Cache.Clear();

        quoteHubOther.EndTransmission();
        quoteHubOther.Cache.Clear();
    }


    [BenchmarkCategory("Adl")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<AdlResult> AdlSeries() => quotes.ToAdl();

    [BenchmarkCategory("Adl")]
    [Benchmark]
    public IReadOnlyList<AdlResult> AdlBuffer() => quotes.ToAdlList();

    [BenchmarkCategory("Adl")]
    [Benchmark]
    public IReadOnlyList<AdlResult> AdlStream() => quoteHub.ToAdlHub().Results;

    [BenchmarkCategory("Adx")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<AdxResult> AdxSeries() => quotes.ToAdx();

    [BenchmarkCategory("Adx")]
    [Benchmark]
    public IReadOnlyList<AdxResult> AdxBuffer() => quotes.ToAdxList(n);

    [BenchmarkCategory("Adx")]
    [Benchmark]
    public IReadOnlyList<AdxResult> AdxStream() => quoteHub.ToAdxHub(n).Results;

    [BenchmarkCategory("Alligator")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<AlligatorResult> AlligatorSeries() => quotes.ToAlligator();

    [BenchmarkCategory("Alligator")]
    [Benchmark]
    public IReadOnlyList<AlligatorResult> AlligatorBuffer() => quotes.ToAlligatorList();

    [BenchmarkCategory("Alligator")]
    [Benchmark]
    public IReadOnlyList<AlligatorResult> AlligatorStream() => quoteHub.ToAlligatorHub().Results;

    [BenchmarkCategory("Alma")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<AlmaResult> AlmaSeries() => quotes.ToAlma(9, 0.85, 6);

    [BenchmarkCategory("Alma")]
    [Benchmark]
    public IReadOnlyList<AlmaResult> AlmaBuffer() => quotes.ToAlmaList(9, 0.85, 6);

    [BenchmarkCategory("Alma")]
    [Benchmark]
    public IReadOnlyList<AlmaResult> AlmaStream() => quoteHub.ToAlmaHub(9, 0.85, 6).Results;

    [BenchmarkCategory("Aroon")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<AroonResult> AroonSeries() => quotes.ToAroon();

    [BenchmarkCategory("Aroon")]
    [Benchmark]
    public IReadOnlyList<AroonResult> AroonBuffer() => quotes.ToAroonList();

    [BenchmarkCategory("Aroon")]
    [Benchmark]
    public IReadOnlyList<AroonResult> AroonStream() => quoteHub.ToAroonHub().Results;

    [BenchmarkCategory("Atr")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<AtrResult> AtrSeries() => quotes.ToAtr();

    [BenchmarkCategory("Atr")]
    [Benchmark]
    public IReadOnlyList<AtrResult> AtrBuffer() => quotes.ToAtrList(n);

    [BenchmarkCategory("Atr")]
    [Benchmark]
    public IReadOnlyList<AtrResult> AtrStream() => quoteHub.ToAtrHub(n).Results;

    [BenchmarkCategory("AtrStop")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<AtrStopResult> AtrStopSeries() => quotes.ToAtrStop();

    [BenchmarkCategory("AtrStop")]
    [Benchmark]
    public IReadOnlyList<AtrStopResult> AtrStopBuffer() => quotes.ToAtrStopList();

    [BenchmarkCategory("AtrStop")]
    [Benchmark]
    public IReadOnlyList<AtrStopResult> AtrStopStream() => quoteHub.ToAtrStopHub().Results;

    [BenchmarkCategory("Awesome")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<AwesomeResult> AwesomeSeries() => quotes.ToAwesome();

    [BenchmarkCategory("Awesome")]
    [Benchmark]
    public IReadOnlyList<AwesomeResult> AwesomeBuffer() => quotes.ToAwesomeList();

    [BenchmarkCategory("Awesome")]
    [Benchmark]
    public IReadOnlyList<AwesomeResult> AwesomeStream() => quoteHub.ToAwesomeHub().Results;

    [BenchmarkCategory("Beta")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<BetaResult> BetaSeries() => quotes.ToBeta(o, 20, BetaType.Standard);

    [BenchmarkCategory("BetaAll")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<BetaResult> BetaAllSeries() => quotes.ToBeta(o, 20, BetaType.All);

    [BenchmarkCategory("BetaDown")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<BetaResult> BetaDownSeries() => quotes.ToBeta(o, 20, BetaType.Down);

    [BenchmarkCategory("BetaUp")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<BetaResult> BetaUpSeries() => quotes.ToBeta(o, 20, BetaType.Up);

    [BenchmarkCategory("BollingerBands")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<BollingerBandsResult> BollingerBandsSeries() => quotes.ToBollingerBands();

    [BenchmarkCategory("BollingerBands")]
    [Benchmark]
    public IReadOnlyList<BollingerBandsResult> BollingerBandsBuffer() => quotes.ToBollingerBandsList(20, 2);

    [BenchmarkCategory("BollingerBands")]
    [Benchmark]
    public IReadOnlyList<BollingerBandsResult> BollingerBandsStream() => quoteHub.ToBollingerBandsHub(20, 2).Results;

    [BenchmarkCategory("Bop")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<BopResult> BopSeries() => quotes.ToBop();

    [BenchmarkCategory("Bop")]
    [Benchmark]
    public IReadOnlyList<BopResult> BopBuffer() => quotes.ToBopList(n);

    [BenchmarkCategory("Bop")]
    [Benchmark]
    public IReadOnlyList<BopResult> BopStream() => quoteHub.ToBopHub(n).Results;

    [BenchmarkCategory("Cci")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<CciResult> CciSeries() => quotes.ToCci();

    [BenchmarkCategory("Cci")]
    [Benchmark]
    public IReadOnlyList<CciResult> CciStream() => quoteHub.ToCciHub(20).Results;

    [BenchmarkCategory("ChaikinOsc")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<ChaikinOscResult> ChaikinOscSeries() => quotes.ToChaikinOsc();

    [BenchmarkCategory("ChaikinOsc")]
    [Benchmark]
    public IReadOnlyList<ChaikinOscResult> ChaikinOscBuffer() => quotes.ToChaikinOscList();

    [BenchmarkCategory("ChaikinOsc")]
    [Benchmark]
    public IReadOnlyList<ChaikinOscResult> ChaikinOscStream() => quoteHub.ToChaikinOscHub().Results;

    [BenchmarkCategory("Chandelier")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<ChandelierResult> ChandelierSeries() => quotes.ToChandelier();

    [BenchmarkCategory("Chandelier")]
    [Benchmark]
    public IReadOnlyList<ChandelierResult> ChandelierBuffer() => quotes.ToChandelierList();

    [BenchmarkCategory("Chandelier")]
    [Benchmark]
    public IReadOnlyList<ChandelierResult> ChandelierStream() => quoteHub.ToChandelierHub().Results;

    [BenchmarkCategory("Chop")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<ChopResult> ChopSeries() => quotes.ToChop();

    [BenchmarkCategory("Chop")]
    [Benchmark]
    public IReadOnlyList<ChopResult> ChopBuffer() => quotes.ToChopList(n);

    [BenchmarkCategory("Chop")]
    [Benchmark]
    public IReadOnlyList<ChopResult> ChopStream() => quoteHub.ToChopHub(n).Results;

    [BenchmarkCategory("Cmf")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<CmfResult> CmfSeries() => quotes.ToCmf();

    [BenchmarkCategory("Cmf")]
    [Benchmark]
    public IReadOnlyList<CmfResult> CmfBuffer() => quotes.ToCmfList();

    [BenchmarkCategory("Cmf")]
    [Benchmark]
    public IReadOnlyList<CmfResult> CmfStream() => quoteHub.ToCmfHub(20).Results;

    [BenchmarkCategory("Cmo")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<CmoResult> CmoSeries() => quotes.ToCmo(n);

    [BenchmarkCategory("Cmo")]
    [Benchmark]
    public IReadOnlyList<CmoResult> CmoBuffer() => quotes.ToCmoList(n);

    [BenchmarkCategory("Cmo")]
    [Benchmark]
    public IReadOnlyList<CmoResult> CmoStream() => quoteHub.ToCmoHub(n).Results;

    [BenchmarkCategory("ConnorsRsi")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<ConnorsRsiResult> ConnorsRsiSeries() => quotes.ToConnorsRsi();

    [BenchmarkCategory("ConnorsRsi")]
    [Benchmark]
    public IReadOnlyList<ConnorsRsiResult> ConnorsRsiBuffer() => quotes.ToConnorsRsiList();

    [BenchmarkCategory("ConnorsRsi")]
    [Benchmark]
    public IReadOnlyList<ConnorsRsiResult> ConnorsRsiStream() => quoteHub.ToConnorsRsiHub(3, 2, 100).Results;

    [BenchmarkCategory("Correlation")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<CorrResult> CorrelationSeries() => quotes.ToCorrelation(o, 20);

    [BenchmarkCategory("Dema")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<DemaResult> DemaSeries() => quotes.ToDema(n);

    [BenchmarkCategory("Dema")]
    [Benchmark]
    public IReadOnlyList<DemaResult> DemaBuffer() => quotes.ToDemaList(n);

    [BenchmarkCategory("Dema")]
    [Benchmark]
    public IReadOnlyList<DemaResult> DemaStream() => quoteHub.ToDemaHub(n).Results;

    [BenchmarkCategory("Doji")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<CandleResult> DojiSeries() => quotes.ToDoji();

    [BenchmarkCategory("Doji")]
    [Benchmark]
    public IReadOnlyList<CandleResult> DojiBuffer() => quotes.ToDojiList();

    [BenchmarkCategory("Doji")]
    [Benchmark]
    public IReadOnlyList<CandleResult> DojiStream() => quoteHub.ToDojiHub().Results;

    [BenchmarkCategory("Donchian")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<DonchianResult> DonchianSeries() => quotes.ToDonchian();

    [BenchmarkCategory("Donchian")]
    [Benchmark]
    public IReadOnlyList<DonchianResult> DonchianBuffer() => quotes.ToDonchianList(20);

    [BenchmarkCategory("Donchian")]
    [Benchmark]
    public IReadOnlyList<DonchianResult> DonchianStream() => quoteHub.ToDonchianHub(20).Results;

    [BenchmarkCategory("Dpo")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<DpoResult> DpoSeries() => quotes.ToDpo(n);

    [BenchmarkCategory("Dpo")]
    [Benchmark]
    public IReadOnlyList<DpoResult> DpoBuffer() => quotes.ToDpoList(n);

    [BenchmarkCategory("Dpo")]
    [Benchmark]
    public IReadOnlyList<DpoResult> DpoStream() => quoteHub.ToDpoHub(n).Results;

    [BenchmarkCategory("Dynamic")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<DynamicResult> DynamicSeries() => quotes.ToDynamic(20);

    [BenchmarkCategory("Dynamic")]
    [Benchmark]
    public IReadOnlyList<DynamicResult> DynamicBuffer() => quotes.ToDynamicList(20);

    [BenchmarkCategory("Dynamic")]
    [Benchmark]
    public IReadOnlyList<DynamicResult> DynamicStream() => quoteHub.ToDynamicHub(n, 0.6).Results;

    [BenchmarkCategory("ElderRay")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<ElderRayResult> ElderRaySeries() => quotes.ToElderRay();

    [BenchmarkCategory("ElderRay")]
    [Benchmark]
    public IReadOnlyList<ElderRayResult> ElderRayBuffer() => quotes.ToElderRayList();

    [BenchmarkCategory("ElderRay")]
    [Benchmark]
    public IReadOnlyList<ElderRayResult> ElderRayStream() => quoteHub.ToElderRayHub(13).Results;

    [BenchmarkCategory("Ema")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<EmaResult> EmaSeries() => quotes.ToEma(20);

    [BenchmarkCategory("Ema")]
    [Benchmark]
    public IReadOnlyList<EmaResult> EmaBuffer() => quotes.ToEmaList(20);

    [BenchmarkCategory("Ema")]
    [Benchmark]
    public IReadOnlyList<EmaResult> EmaStream() => quoteHub.ToEmaHub(20).Results;

    [BenchmarkCategory("Epma")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<EpmaResult> EpmaSeries() => quotes.ToEpma(n);

    [BenchmarkCategory("Epma")]
    [Benchmark]
    public IReadOnlyList<EpmaResult> EpmaBuffer() => quotes.ToEpmaList(n);

    [BenchmarkCategory("Epma")]
    [Benchmark]
    public IReadOnlyList<EpmaResult> EpmaStream() => quoteHub.ToEpmaHub(n).Results;

    [BenchmarkCategory("Fcb")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<FcbResult> FcbSeries() => quotes.ToFcb(n);

    [BenchmarkCategory("Fcb")]
    [Benchmark]
    public IReadOnlyList<FcbResult> FcbBuffer() => quotes.ToFcbList(n);

    [BenchmarkCategory("Fcb")]
    [Benchmark]
    public IReadOnlyList<FcbResult> FcbStream() => quoteHub.ToFcbHub(2).Results;

    [BenchmarkCategory("FisherTransform")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<FisherTransformResult> FisherTransformSeries() => quotes.ToFisherTransform(10);

    [BenchmarkCategory("FisherTransform")]
    [Benchmark]
    public IReadOnlyList<FisherTransformResult> FisherTransformBuffer() => quotes.ToFisherTransformList(10);

    [BenchmarkCategory("FisherTransform")]
    [Benchmark]
    public IReadOnlyList<FisherTransformResult> FisherTransformStream() => quoteHub.ToFisherTransformHub(10).Results;

    [BenchmarkCategory("ForceIndex")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<ForceIndexResult> ForceIndexSeries() => quotes.ToForceIndex(13);

    [BenchmarkCategory("ForceIndex")]
    [Benchmark]
    public IReadOnlyList<ForceIndexResult> ForceIndexBuffer() => quotes.ToForceIndexList(13);

    [BenchmarkCategory("ForceIndex")]
    [Benchmark]
    public IReadOnlyList<ForceIndexResult> ForceIndexStream() => quoteHub.ToForceIndexHub(2).Results;

    [BenchmarkCategory("Fractal")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<FractalResult> FractalSeries() => quotes.ToFractal();

    [BenchmarkCategory("Fractal")]
    [Benchmark]
    public IReadOnlyList<FractalResult> FractalBuffer() => quotes.ToFractalList();

    [BenchmarkCategory("Fractal")]
    [Benchmark]
    public IReadOnlyList<FractalResult> FractalStream() => quoteHub.ToFractalHub().Results;

    [BenchmarkCategory("Gator")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<GatorResult> GatorSeries() => quotes.ToGator();

    [BenchmarkCategory("Gator")]
    [Benchmark]
    public IReadOnlyList<GatorResult> GatorBuffer() => quotes.ToGatorList();

    [BenchmarkCategory("Gator")]
    [Benchmark]
    public IReadOnlyList<GatorResult> GatorStream() => quoteHub.ToGatorHub().Results;

    [BenchmarkCategory("HeikinAshi")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<HeikinAshiResult> HeikinAshiSeries() => quotes.ToHeikinAshi();

    [BenchmarkCategory("HeikinAshi")]
    [Benchmark]
    public IReadOnlyList<HeikinAshiResult> HeikinAshiBuffer() => quotes.ToHeikinAshiList();

    [BenchmarkCategory("HeikinAshi")]
    [Benchmark]
    public IReadOnlyList<HeikinAshiResult> HeikinAshiStream() => quoteHub.ToHeikinAshiHub().Results;

    [BenchmarkCategory("Hma")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<HmaResult> HmaSeries() => quotes.ToHma(n);

    [BenchmarkCategory("Hma")]
    [Benchmark]
    public IReadOnlyList<HmaResult> HmaBuffer() => quotes.ToHmaList(n);

    [BenchmarkCategory("Hma")]
    [Benchmark]
    public IReadOnlyList<HmaResult> HmaStream() => quoteHub.ToHmaHub(n).Results;

    [BenchmarkCategory("HtTrendline")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<HtlResult> HtTrendlineSeries() => quotes.ToHtTrendline();

    [BenchmarkCategory("HtTrendline")]
    [Benchmark]
    public IReadOnlyList<HtlResult> HtTrendlineBuffer() => quotes.ToHtTrendlineList();

    [BenchmarkCategory("HtTrendline")]
    [Benchmark]
    public IReadOnlyList<HtlResult> HtTrendlineStream() => quoteHub.ToHtTrendlineHub().Results;

    [BenchmarkCategory("Hurst")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<HurstResult> HurstSeries() => quotes.ToHurst();

    [BenchmarkCategory("Hurst")]
    [Benchmark]
    public IReadOnlyList<HurstResult> HurstBuffer() => quotes.ToHurstList(100);

    [BenchmarkCategory("Hurst")]
    [Benchmark]
    public IReadOnlyList<HurstResult> HurstStream() => quoteHub.ToHurstHub(100).Results;

    [BenchmarkCategory("Ichimoku")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<IchimokuResult> IchimokuSeries() => quotes.ToIchimoku();

    [BenchmarkCategory("Ichimoku")]
    [Benchmark]
    public IReadOnlyList<IchimokuResult> IchimokuBuffer() => quotes.ToIchimokuList();

    [BenchmarkCategory("Ichimoku")]
    [Benchmark]
    public IReadOnlyList<IchimokuResult> IchimokuStream() => quoteHub.ToIchimokuHub().Results;

    [BenchmarkCategory("Kama")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<KamaResult> KamaSeries() => quotes.ToKama();

    [BenchmarkCategory("Kama")]
    [Benchmark]
    public IReadOnlyList<KamaResult> KamaBuffer() => quotes.ToKamaList(10, 2, 30);

    [BenchmarkCategory("Kama")]
    [Benchmark]
    public IReadOnlyList<KamaResult> KamaStream() => quoteHub.ToKamaHub(10, 2, 30).Results;

    [BenchmarkCategory("Keltner")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<KeltnerResult> KeltnerSeries() => quotes.ToKeltner();

    [BenchmarkCategory("Keltner")]
    [Benchmark]
    public IReadOnlyList<KeltnerResult> KeltnerBuffer() => quotes.ToKeltnerList();

    [BenchmarkCategory("Keltner")]
    [Benchmark]
    public IReadOnlyList<KeltnerResult> KeltnerStream() => quoteHub.ToKeltnerHub(20, 2, 10).Results;

    [BenchmarkCategory("Kvo")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<KvoResult> KvoSeries() => quotes.ToKvo();

    [BenchmarkCategory("Kvo")]
    [Benchmark]
    public IReadOnlyList<KvoResult> KvoBuffer() => quotes.ToKvoList();

    [BenchmarkCategory("Kvo")]
    [Benchmark]
    public IReadOnlyList<KvoResult> KvoStream() => quoteHub.ToKvoHub(34, 55, 13).Results;

    [BenchmarkCategory("MaEnvelopes")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<MaEnvelopeResult> MaEnvelopesSeries() => quotes.ToMaEnvelopes(20, 2.5, MaType.SMA);

    [BenchmarkCategory("MaEnvelopes")]
    [Benchmark]
    public IReadOnlyList<MaEnvelopeResult> MaEnvelopesBuffer() => quotes.ToMaEnvelopesList(20, 2.5, MaType.SMA);

    [BenchmarkCategory("MaEnvelopes")]
    [Benchmark]
    public IReadOnlyList<MaEnvelopeResult> MaEnvelopesStream() => quoteHub.ToMaEnvelopesHub(20, 2.5, MaType.SMA).Results;

    [BenchmarkCategory("Macd")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<MacdResult> MacdSeries() => quotes.ToMacd();

    [BenchmarkCategory("Macd")]
    [Benchmark]
    public IReadOnlyList<MacdResult> MacdBuffer() => quotes.ToMacdList(12, 26, 9);

    [BenchmarkCategory("Macd")]
    [Benchmark]
    public IReadOnlyList<MacdResult> MacdStream() => quoteHub.ToMacdHub(12, 26, 9).Results;

    [BenchmarkCategory("Mama")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<MamaResult> MamaSeries() => quotes.ToMama();

    [BenchmarkCategory("Mama")]
    [Benchmark]
    public IReadOnlyList<MamaResult> MamaBuffer() => quotes.ToMamaList(0.5, 0.05);

    [BenchmarkCategory("Mama")]
    [Benchmark]
    public IReadOnlyList<MamaResult> MamaStream() => quoteHub.ToMamaHub(0.5, 0.05).Results;

    [BenchmarkCategory("Marubozu")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<CandleResult> MarubozuSeries() => quotes.ToMarubozu();

    [BenchmarkCategory("Marubozu")]
    [Benchmark]
    public IReadOnlyList<CandleResult> MarubozuBuffer() => quotes.ToMarubozuList();

    [BenchmarkCategory("Marubozu")]
    [Benchmark]
    public IReadOnlyList<CandleResult> MarubozuStream() => quoteHub.ToMarubozuHub(95).Results;

    [BenchmarkCategory("Mfi")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<MfiResult> MfiSeries() => quotes.ToMfi();

    [BenchmarkCategory("Mfi")]
    [Benchmark]
    public IReadOnlyList<MfiResult> MfiBuffer() => quotes.ToMfiList();

    [BenchmarkCategory("Mfi")]
    [Benchmark]
    public IReadOnlyList<MfiResult> MfiStream() => quoteHub.ToMfiHub(14).Results;

    [BenchmarkCategory("Obv")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<ObvResult> ObvSeries() => quotes.ToObv();

    [BenchmarkCategory("Obv")]
    [Benchmark]
    public IReadOnlyList<ObvResult> ObvBuffer() => quotes.ToObvList();

    [BenchmarkCategory("Obv")]
    [Benchmark]
    public IReadOnlyList<ObvResult> ObvStream() => quoteHub.ToObvHub().Results;

    [BenchmarkCategory("ParabolicSar")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<ParabolicSarResult> ParabolicSarSeries() => quotes.ToParabolicSar();

    [BenchmarkCategory("ParabolicSar")]
    [Benchmark]
    public IReadOnlyList<ParabolicSarResult> ParabolicSarBuffer() => quotes.ToParabolicSarList();

    [BenchmarkCategory("ParabolicSar")]
    [Benchmark]
    public IReadOnlyList<ParabolicSarResult> ParabolicSarStream() => quoteHub.ToParabolicSarHub().Results;

    [BenchmarkCategory("PivotPoints")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<PivotPointsResult> PivotPointsSeries() => quotes.ToPivotPoints(PeriodSize.Month, PivotPointType.Standard);

    [BenchmarkCategory("PivotPoints")]
    [Benchmark]
    public IReadOnlyList<PivotPointsResult> PivotPointsBuffer() => quotes.ToPivotPointsList(PeriodSize.Month, PivotPointType.Standard);

    [BenchmarkCategory("PivotPoints")]
    [Benchmark]
    public IReadOnlyList<PivotPointsResult> PivotPointsStream() => quoteHub.ToPivotPointsHub().Results;

    [BenchmarkCategory("Pivots")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<PivotsResult> PivotsSeries() => quotes.ToPivots();

    [BenchmarkCategory("Pivots")]
    [Benchmark]
    public IReadOnlyList<PivotsResult> PivotsBuffer() => quotes.ToPivotsList(2, 2, 20);

    [BenchmarkCategory("Pivots")]
    [Benchmark]
    public IReadOnlyList<PivotsResult> PivotsStream() => quoteHub.ToPivotsHub().Results;

    [BenchmarkCategory("Pmo")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<PmoResult> PmoSeries() => quotes.ToPmo();

    [BenchmarkCategory("Pmo")]
    [Benchmark]
    public IReadOnlyList<PmoResult> PmoBuffer() => quotes.ToPmoList();

    [BenchmarkCategory("Pmo")]
    [Benchmark]
    public IReadOnlyList<PmoResult> PmoStream() => quoteHub.ToPmoHub(35, 20, 10).Results;

    [BenchmarkCategory("Prs")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<PrsResult> PrsSeries() => quotes.ToPrs(o);

    [BenchmarkCategory("Pvo")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<PvoResult> PvoSeries() => quotes.ToPvo();

    [BenchmarkCategory("Pvo")]
    [Benchmark]
    public IReadOnlyList<PvoResult> PvoBuffer() => quotes.ToPvoList();

    [BenchmarkCategory("Pvo")]
    [Benchmark]
    public IReadOnlyList<PvoResult> PvoStream() => quoteHub.ToPvoHub().Results;

    [BenchmarkCategory("QuotePart")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<QuotePart> QuotePartSeries() => quotes.ToQuotePart(CandlePart.OHL3);

    [BenchmarkCategory("QuotePart")]
    [Benchmark]
    public IReadOnlyList<QuotePart> QuotePartBuffer() => quotes.ToQuotePartList(CandlePart.OHL3);

    [BenchmarkCategory("QuotePart")]
    [Benchmark]
    public IReadOnlyList<QuotePart> QuotePartStream() => quoteHub.ToQuotePartHub(CandlePart.OHL3).Results;

    [BenchmarkCategory("Renko")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<RenkoResult> RenkoSeries() => quotes.ToRenko(2.5m);

    [BenchmarkCategory("Renko")]
    [Benchmark]
    public IReadOnlyList<RenkoResult> RenkoBuffer() => quotes.ToRenkoList(2.5m);

    [BenchmarkCategory("Renko")]
    [Benchmark]
    public IReadOnlyList<RenkoResult> RenkoStream() => quoteHub.ToRenkoHub(2.5m).Results;

    [BenchmarkCategory("RenkoAtr")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<RenkoResult> RenkoAtrSeries() => quotes.ToRenko(n);

    [BenchmarkCategory("RenkoAtr")]
    [Benchmark]
    public IReadOnlyList<RenkoResult> RenkoAtrBuffer() => quotes.ToRenkoList(2.5m);

    [BenchmarkCategory("RenkoAtr")]
    [Benchmark]
    public IReadOnlyList<RenkoResult> RenkoAtrStream() => quoteHub.ToRenkoHub(2.5m).Results;

    [BenchmarkCategory("Roc")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<RocResult> RocSeries() => quotes.ToRoc(20);

    [BenchmarkCategory("Roc")]
    [Benchmark]
    public IReadOnlyList<RocResult> RocBuffer() => quotes.ToRocList(20);

    [BenchmarkCategory("Roc")]
    [Benchmark]
    public IReadOnlyList<RocResult> RocStream() => quoteHub.ToRocHub(20).Results;

    [BenchmarkCategory("RocWb")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<RocWbResult> RocWbSeries() => quotes.ToRocWb(12, 3, 12);

    [BenchmarkCategory("RocWb")]
    [Benchmark]
    public IReadOnlyList<RocWbResult> RocWbBuffer() => quotes.ToRocWbList(20, 5, 5);

    [BenchmarkCategory("RocWb")]
    [Benchmark]
    public IReadOnlyList<RocWbResult> RocWbStream() => quoteHub.ToRocWbHub(20, 5, 5).Results;

    [BenchmarkCategory("RollingPivots")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<RollingPivotsResult> RollingPivotsSeries() => quotes.ToRollingPivots(20, 0, PivotPointType.Standard);

    [BenchmarkCategory("RollingPivots")]
    [Benchmark]
    public IReadOnlyList<RollingPivotsResult> RollingPivotsBuffer() => quotes.ToRollingPivotsList(20, 0, PivotPointType.Standard);

    [BenchmarkCategory("RollingPivots")]
    [Benchmark]
    public IReadOnlyList<RollingPivotsResult> RollingPivotsStream() => quoteHub.ToRollingPivotsHub(20, 0, PivotPointType.Standard).Results;

    [BenchmarkCategory("Rsi")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<RsiResult> RsiSeries() => quotes.ToRsi();

    [BenchmarkCategory("Rsi")]
    [Benchmark]
    public IReadOnlyList<RsiResult> RsiBuffer() => quotes.ToRsiList(n);

    [BenchmarkCategory("Rsi")]
    [Benchmark]
    public IReadOnlyList<RsiResult> RsiStream() => quoteHub.ToRsiHub(n).Results;

    [BenchmarkCategory("Slope")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<SlopeResult> SlopeSeries() => quotes.ToSlope(20);

    [BenchmarkCategory("Slope")]
    [Benchmark]
    public IReadOnlyList<SlopeResult> SlopeBuffer() => quotes.ToSlopeList(n);

    [BenchmarkCategory("Slope")]
    [Benchmark]
    public IReadOnlyList<SlopeResult> SlopeStream() => quoteHub.ToSlopeHub(20).Results;

    [BenchmarkCategory("Sma")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<SmaResult> SmaSeries() => quotes.ToSma(10);

    [BenchmarkCategory("Sma")]
    [Benchmark]
    public IReadOnlyList<SmaResult> SmaBuffer() => quotes.ToSmaList(n);

    [BenchmarkCategory("Sma")]
    [Benchmark]
    public IReadOnlyList<SmaResult> SmaStream() => quoteHub.ToSmaHub(10).Results;

    [BenchmarkCategory("SmaAnalysis")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<SmaAnalysisResult> SmaAnalysisSeries() => quotes.ToSmaAnalysis(10);

    [BenchmarkCategory("SmaAnalysis")]
    [Benchmark]
    public IReadOnlyList<SmaAnalysisResult> SmaAnalysisBuffer() => quotes.ToSmaAnalysisList(10);

    [BenchmarkCategory("SmaAnalysis")]
    [Benchmark]
    public IReadOnlyList<SmaAnalysisResult> SmaAnalysisStream() => quoteHub.ToSmaAnalysisHub(20).Results;

    [BenchmarkCategory("Smi")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<SmiResult> SmiSeries() => quotes.ToSmi(5, 20, 5, 3);

    [BenchmarkCategory("Smi")]
    [Benchmark]
    public IReadOnlyList<SmiResult> SmiBuffer() => quotes.ToSmiList(13, 25, 2, 3);

    [BenchmarkCategory("Smi")]
    [Benchmark]
    public IReadOnlyList<SmiResult> SmiStream() => quoteHub.ToSmiHub(13, 25, 2, 3).Results;

    [BenchmarkCategory("Smma")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<SmmaResult> SmmaSeries() => quotes.ToSmma(10);

    [BenchmarkCategory("Smma")]
    [Benchmark]
    public IReadOnlyList<SmmaResult> SmmaBuffer() => quotes.ToSmmaList(n);

    [BenchmarkCategory("Smma")]
    [Benchmark]
    public IReadOnlyList<SmmaResult> SmmaStream() => quoteHub.ToSmmaHub(n).Results;

    [BenchmarkCategory("StarcBands")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<StarcBandsResult> StarcBandsSeries() => quotes.ToStarcBands(10);

    [BenchmarkCategory("StarcBands")]
    [Benchmark]
    public IReadOnlyList<StarcBandsResult> StarcBandsBuffer() => quotes.ToStarcBandsList(5, 2, 10);

    [BenchmarkCategory("StarcBands")]
    [Benchmark]
    public IReadOnlyList<StarcBandsResult> StarcBandsStream() => quoteHub.ToStarcBandsHub(5, 2, 10).Results;

    [BenchmarkCategory("Stc")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<StcResult> StcSeries() => quotes.ToStc();

    [BenchmarkCategory("Stc")]
    [Benchmark]
    public IReadOnlyList<StcResult> StcBuffer() => quotes.ToStcList(10, 23, 50);

    [BenchmarkCategory("Stc")]
    [Benchmark]
    public IReadOnlyList<StcResult> StcStream() => quoteHub.ToStcHub(10, 23, 50).Results;

    [BenchmarkCategory("StdDev")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<StdDevResult> StdDevSeries() => quotes.ToStdDev(20);

    [BenchmarkCategory("StdDev")]
    [Benchmark]
    public IReadOnlyList<StdDevResult> StdDevBuffer() => quotes.ToStdDevList(20);

    [BenchmarkCategory("StdDev")]
    [Benchmark]
    public IReadOnlyList<StdDevResult> StdDevStream() => quoteHub.ToStdDevHub(n).Results;

    [BenchmarkCategory("StdDevChannels")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<StdDevChannelsResult> StdDevChannelsSeries() => quotes.ToStdDevChannels();

    [BenchmarkCategory("Stoch")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<StochResult> StochSeries() => quotes.ToStoch();

    [BenchmarkCategory("Stoch")]
    [Benchmark]
    public IReadOnlyList<StochResult> StochBuffer() => quotes.ToStochList(14, 3, 3);

    [BenchmarkCategory("Stoch")]
    [Benchmark]
    public IReadOnlyList<StochResult> StochStream() => quoteHub.ToStochHub(n, 3, 3).Results;

    [BenchmarkCategory("StochRsi")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<StochRsiResult> StochRsiSeries() => quotes.ToStochRsi(n, n, 3);

    [BenchmarkCategory("StochRsi")]
    [Benchmark]
    public IReadOnlyList<StochRsiResult> StochRsiBuffer() => quotes.ToStochRsiList(14, 14, 3, 1);

    [BenchmarkCategory("StochRsi")]
    [Benchmark]
    public IReadOnlyList<StochRsiResult> StochRsiStream() => quoteHub.ToStochRsiHub(n, n, 3, 1).Results;

    [BenchmarkCategory("StochSmma")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<StochResult> StochSmmaSeries() => quotes.ToStoch(9, 3, 3, 3, 2, MaType.SMMA);

    [BenchmarkCategory("StochSmma")]
    [Benchmark]
    public IReadOnlyList<StochResult> StochSmmaBuffer() => quotes.ToStochList(14, 3, 3);

    [BenchmarkCategory("StochSmma")]
    [Benchmark]
    public IReadOnlyList<StochResult> StochSmmaStream() => quoteHub.ToStochHub(n, 3, 3).Results;

    [BenchmarkCategory("SuperTrend")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<SuperTrendResult> SuperTrendSeries() => quotes.ToSuperTrend();

    [BenchmarkCategory("SuperTrend")]
    [Benchmark]
    public IReadOnlyList<SuperTrendResult> SuperTrendBuffer() => quotes.ToSuperTrendList();

    [BenchmarkCategory("SuperTrend")]
    [Benchmark]
    public IReadOnlyList<SuperTrendResult> SuperTrendStream() => quoteHub.ToSuperTrendHub(10, 3).Results;

    [BenchmarkCategory("T3")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<T3Result> T3Series() => quotes.ToT3();

    [BenchmarkCategory("T3")]
    [Benchmark]
    public IReadOnlyList<T3Result> T3Buffer() => quotes.ToT3List(5, 0.7);

    [BenchmarkCategory("T3")]
    [Benchmark]
    public IReadOnlyList<T3Result> T3Stream() => quoteHub.ToT3Hub(5, 0.7).Results;

    [BenchmarkCategory("Tema")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<TemaResult> TemaSeries() => quotes.ToTema(20);

    [BenchmarkCategory("Tema")]
    [Benchmark]
    public IReadOnlyList<TemaResult> TemaBuffer() => quotes.ToTemaList(20);

    [BenchmarkCategory("Tema")]
    [Benchmark]
    public IReadOnlyList<TemaResult> TemaStream() => quoteHub.ToTemaHub(20).Results;

    [BenchmarkCategory("Tr")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<TrResult> TrSeries() => quotes.ToTr();

    [BenchmarkCategory("Tr")]
    [Benchmark]
    public IReadOnlyList<TrResult> TrBuffer() => quotes.ToTrList();

    [BenchmarkCategory("Tr")]
    [Benchmark]
    public IReadOnlyList<TrResult> TrStream() => quoteHub.ToTrHub().Results;

    [BenchmarkCategory("Trix")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<TrixResult> TrixSeries() => quotes.ToTrix(n);

    [BenchmarkCategory("Trix")]
    [Benchmark]
    public IReadOnlyList<TrixResult> TrixBuffer() => quotes.ToTrixList(n);

    [BenchmarkCategory("Trix")]
    [Benchmark]
    public IReadOnlyList<TrixResult> TrixStream() => quoteHub.ToTrixHub(n).Results;

    [BenchmarkCategory("Tsi")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<TsiResult> TsiSeries() => quotes.ToTsi();

    [BenchmarkCategory("Tsi")]
    [Benchmark]
    public IReadOnlyList<TsiResult> TsiBuffer() => quotes.ToTsiList(25, 13, 7);

    [BenchmarkCategory("Tsi")]
    [Benchmark]
    public IReadOnlyList<TsiResult> TsiStream() => quoteHub.ToTsiHub(25, 13, 7).Results;

    [BenchmarkCategory("UlcerIndex")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<UlcerIndexResult> UlcerIndexSeries() => quotes.ToUlcerIndex();

    [BenchmarkCategory("UlcerIndex")]
    [Benchmark]
    public IReadOnlyList<UlcerIndexResult> UlcerIndexBuffer() => quotes.ToUlcerIndexList(n);

    [BenchmarkCategory("UlcerIndex")]
    [Benchmark]
    public IReadOnlyList<UlcerIndexResult> UlcerIndexStream() => quoteHub.ToUlcerIndexHub(n).Results;

    [BenchmarkCategory("Ultimate")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<UltimateResult> UltimateSeries() => quotes.ToUltimate();

    [BenchmarkCategory("Ultimate")]
    [Benchmark]
    public IReadOnlyList<UltimateResult> UltimateBuffer() => quotes.ToUltimateList(7, 14, 28);

    [BenchmarkCategory("Ultimate")]
    [Benchmark]
    public IReadOnlyList<UltimateResult> UltimateStream() => quoteHub.ToUltimateHub(7, n, 28).Results;

    [BenchmarkCategory("VolatilityStop")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<VolatilityStopResult> VolatilityStopSeries() => quotes.ToVolatilityStop();

    [BenchmarkCategory("VolatilityStop")]
    [Benchmark]
    public IReadOnlyList<VolatilityStopResult> VolatilityStopBuffer() => quotes.ToVolatilityStopList(7, 3);

    [BenchmarkCategory("VolatilityStop")]
    [Benchmark]
    public IReadOnlyList<VolatilityStopResult> VolatilityStopStream() => quoteHub.ToVolatilityStopHub().Results;

    [BenchmarkCategory("Vortex")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<VortexResult> VortexSeries() => quotes.ToVortex(n);

    [BenchmarkCategory("Vortex")]
    [Benchmark]
    public IReadOnlyList<VortexResult> VortexBuffer() => quotes.ToVortexList(n);

    [BenchmarkCategory("Vortex")]
    [Benchmark]
    public IReadOnlyList<VortexResult> VortexStream() => quoteHub.ToVortexHub(n).Results;

    [BenchmarkCategory("Vwap")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<VwapResult> VwapSeries() => quotes.ToVwap();

    [BenchmarkCategory("Vwap")]
    [Benchmark]
    public IReadOnlyList<VwapResult> VwapBuffer() => quotes.ToVwapList(quotes[0].Timestamp);

    [BenchmarkCategory("Vwap")]
    [Benchmark]
    public IReadOnlyList<VwapResult> VwapStream() => quoteHub.ToVwapHub().Results;

    [BenchmarkCategory("Vwma")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<VwmaResult> VwmaSeries() => quotes.ToVwma(n);

    [BenchmarkCategory("Vwma")]
    [Benchmark]
    public IReadOnlyList<VwmaResult> VwmaBuffer() => quotes.ToVwmaList(n);

    [BenchmarkCategory("Vwma")]
    [Benchmark]
    public IReadOnlyList<VwmaResult> VwmaStream() => quoteHub.ToVwmaHub(n).Results;

    [BenchmarkCategory("WilliamsR")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<WilliamsResult> WilliamsRSeries() => quotes.ToWilliamsR();

    [BenchmarkCategory("WilliamsR")]
    [Benchmark]
    public IReadOnlyList<WilliamsResult> WilliamsRBuffer() => quotes.ToWilliamsRList();

    [BenchmarkCategory("WilliamsR")]
    [Benchmark]
    public IReadOnlyList<WilliamsResult> WilliamsRStream() => quoteHub.ToWilliamsRHub().Results;

    [BenchmarkCategory("Wma")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<WmaResult> WmaSeries() => quotes.ToWma(n);

    [BenchmarkCategory("Wma")]
    [Benchmark]
    public IReadOnlyList<WmaResult> WmaBuffer() => quotes.ToWmaList(n);

    [BenchmarkCategory("Wma")]
    [Benchmark]
    public IReadOnlyList<WmaResult> WmaStream() => quoteHub.ToWmaHub(n).Results;

    [BenchmarkCategory("ZigZag")]
    [Benchmark(Baseline = true)]
    public IReadOnlyList<ZigZagResult> ZigZagSeries() => quotes.ToZigZag();
}
