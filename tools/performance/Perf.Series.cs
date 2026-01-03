namespace Performance;

// SERIES-STYLE INDICATORS

[ShortRunJob, WarmupCount(5), IterationCount(5)]
public class SeriesIndicators
{
    private static readonly IReadOnlyList<Quote> q = Data.GetDefault();
    private static readonly IReadOnlyList<Quote> o = Data.GetCompare();
    private const int n = 14;

    /* Parameter arguments should match the Catalog default values */

    [Benchmark] public void ToAdlBatch() => q.ToAdl();
    [Benchmark] public void ToAdxBatch() => q.ToAdx();
    [Benchmark] public void ToAlligatorBatch() => q.ToAlligator();
    [Benchmark] public void ToAlmaBatch() => q.ToAlma(9, 0.85, 6);
    [Benchmark] public void ToAroonBatch() => q.ToAroon();
    [Benchmark] public void ToAtrBatch() => q.ToAtr();
    [Benchmark] public void ToAtrStopBatch() => q.ToAtrStop();
    [Benchmark] public void ToAwesomeBatch() => q.ToAwesome();
    [Benchmark] public void ToBetaBatch() => q.ToBeta(o, 20, BetaType.Standard);
    [Benchmark] public void ToBetaUpBatch() => q.ToBeta(o, 20, BetaType.Up);
    [Benchmark] public void ToBetaDownBatch() => q.ToBeta(o, 20, BetaType.Down);
    [Benchmark] public void ToBetaAllBatch() => q.ToBeta(o, 20, BetaType.All);
    [Benchmark] public void ToBollingerBandsBatch() => q.ToBollingerBands();
    [Benchmark] public void ToBopBatch() => q.ToBop();
    [Benchmark] public void ToCciBatch() => q.ToCci();
    [Benchmark] public void ToChaikinOscBatch() => q.ToChaikinOsc();
    [Benchmark] public void ToChandelierBatch() => q.ToChandelier();
    [Benchmark] public void ToChopBatch() => q.ToChop();
    [Benchmark] public void ToCmfBatch() => q.ToCmf();
    [Benchmark] public void ToCmoBatch() => q.ToCmo(n);
    [Benchmark] public void ToConnorsRsiBatch() => q.ToConnorsRsi();
    [Benchmark] public void ToCorrelationBatch() => q.ToCorrelation(o, 20);
    [Benchmark] public void ToDemaBatch() => q.ToDema(n);
    [Benchmark] public void ToDojiBatch() => q.ToDoji();
    [Benchmark] public void ToDonchianBatch() => q.ToDonchian();
    [Benchmark] public void ToDpoBatch() => q.ToDpo(n);
    [Benchmark] public void ToDynamicBatch() => q.ToDynamic(20);
    [Benchmark] public void ToElderRayBatch() => q.ToElderRay();
    [Benchmark] public void ToEmaBatch() => q.ToEma(20);
    [Benchmark] public void ToEpmaBatch() => q.ToEpma(n);
    [Benchmark] public void ToFcbBatch() => q.ToFcb(n);
    [Benchmark] public void ToFisherTransformBatch() => q.ToFisherTransform(10);
    [Benchmark] public void ToForceIndexBatch() => q.ToForceIndex(13);
    [Benchmark] public void ToFractalBatch() => q.ToFractal();
    [Benchmark] public void ToGatorBatch() => q.ToGator();
    [Benchmark] public void ToHeikinAshiBatch() => q.ToHeikinAshi();
    [Benchmark] public void ToHmaBatch() => q.ToHma(n);
    [Benchmark] public void ToHtTrendlineBatch() => q.ToHtTrendline();
    [Benchmark] public void ToHurstBatch() => q.ToHurst();
    [Benchmark] public void ToIchimokuBatch() => q.ToIchimoku();
    [Benchmark] public void ToKamaBatch() => q.ToKama();
    [Benchmark] public void ToKeltnerBatch() => q.ToKeltner();
    [Benchmark] public void ToKvoBatch() => q.ToKvo();
    [Benchmark] public void ToMacdBatch() => q.ToMacd();
    [Benchmark] public void ToMaEnvelopesBatch() => q.ToMaEnvelopes(20, 2.5, MaType.SMA);
    [Benchmark] public void ToMamaBatch() => q.ToMama();
    [Benchmark] public void ToMarubozuBatch() => q.ToMarubozu();
    [Benchmark] public void ToMfiBatch() => q.ToMfi();
    [Benchmark] public void ToObvBatch() => q.ToObv();
    [Benchmark] public void ToParabolicSarBatch() => q.ToParabolicSar();
    [Benchmark] public void ToPivotPointsBatch() => q.ToPivotPoints(PeriodSize.Month, PivotPointType.Standard);
    [Benchmark] public void ToPivotsBatch() => q.ToPivots();
    [Benchmark] public void ToPmoBatch() => q.ToPmo();
    [Benchmark] public void ToPrsBatch() => q.ToPrs(o);
    [Benchmark] public void ToPvoBatch() => q.ToPvo();
    [Benchmark] public void ToQuotePartBatch() => q.ToQuotePart(CandlePart.OHL3);
    [Benchmark] public void ToRenkoBatch() => q.ToRenko(2.5m);
    [Benchmark] public void ToRenkoAtrBatch() => q.ToRenko(n);
    [Benchmark] public void ToRocBatch() => q.ToRoc(20);
    [Benchmark] public void ToRocWbBatch() => q.ToRocWb(12, 3, 12);
    [Benchmark] public void ToRollingPivotsBatch() => q.ToRollingPivots(20, 0, PivotPointType.Standard);
    [Benchmark] public void ToRsiBatch() => q.ToRsi();
    [Benchmark] public void ToSlopeBatch() => q.ToSlope(20);
    [Benchmark] public void ToSmaBatch() => q.ToSma(10);
    [Benchmark] public void ToSmaAnalysisBatch() => q.ToSmaAnalysis(10);
    [Benchmark] public void ToSmiBatch() => q.ToSmi(5, 20, 5, 3);
    [Benchmark] public void ToSmmaBatch() => q.ToSmma(10);
    [Benchmark] public void ToStarcBandsBatch() => q.ToStarcBands(10);
    [Benchmark] public void ToStcBatch() => q.ToStc();
    [Benchmark] public void ToStdDevBatch() => q.ToStdDev(20);
    [Benchmark] public void ToStdDevChannelsBatch() => q.ToStdDevChannels();
    [Benchmark] public void ToStochBatch() => q.ToStoch();
    [Benchmark] public void ToStochSmmaBatch() => q.ToStoch(9, 3, 3, 3, 2, MaType.SMMA);
    [Benchmark] public void ToStochRsiBatch() => q.ToStochRsi(n, n, 3);
    [Benchmark] public void ToSuperTrendBatch() => q.ToSuperTrend();
    [Benchmark] public void ToT3Batch() => q.ToT3();
    [Benchmark] public void ToTemaBatch() => q.ToTema(20);
    [Benchmark] public void ToTrBatch() => q.ToTr();
    [Benchmark] public void ToTrixBatch() => q.ToTrix(n);
    [Benchmark] public void ToTsiBatch() => q.ToTsi();
    [Benchmark] public void ToUlcerIndexBatch() => q.ToUlcerIndex();
    [Benchmark] public void ToUltimateBatch() => q.ToUltimate();
    [Benchmark] public void ToVolatilityStopBatch() => q.ToVolatilityStop();
    [Benchmark] public void ToVortexBatch() => q.ToVortex(n);
    [Benchmark] public void ToVwapBatch() => q.ToVwap();
    [Benchmark] public void ToVwmaBatch() => q.ToVwma(n);
    [Benchmark] public void ToWilliamsRBatch() => q.ToWilliamsR();
    [Benchmark] public void ToWmaBatch() => q.ToWma(n);
    [Benchmark] public void ToZigZagBatch() => q.ToZigZag();
}
