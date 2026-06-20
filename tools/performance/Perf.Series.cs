namespace Performance;

// SERIES-STYLE INDICATORS

[ShortRunJob, WarmupCount(5), IterationCount(5)]
public class SeriesIndicators
{
    private static readonly IReadOnlyList<Bar> q = Data.GetDefault();
    private static readonly IReadOnlyList<Bar> o = Data.GetCompare();
    private const int n = 14;

    /* Parameter arguments should match the Catalog default values */
    /* Benchmarks return their result (as object) so BenchmarkDotNet consumes
       it and cannot dead-code-eliminate the call — matching the Buffer/Stream
       classes and keeping the three styles comparable. */

    [Benchmark] public object ToAdlBatch() => q.ToAdl();
    [Benchmark] public object ToAdxBatch() => q.ToAdx(n);
    [Benchmark] public object ToAlligatorBatch() => q.ToAlligator();
    [Benchmark] public object ToAlmaBatch() => q.ToAlma(9, 0.85, 6);
    [Benchmark] public object ToAroonBatch() => q.ToAroon();
    [Benchmark] public object ToAtrBatch() => q.ToAtr(n);
    [Benchmark] public object ToAtrStopBatch() => q.ToAtrStop();
    [Benchmark] public object ToAwesomeBatch() => q.ToAwesome();
    [Benchmark] public object ToBetaBatch() => q.ToBeta(o, 20, BetaType.Standard);
    [Benchmark] public object ToBetaUpBatch() => q.ToBeta(o, 20, BetaType.Up);
    [Benchmark] public object ToBetaDownBatch() => q.ToBeta(o, 20, BetaType.Down);
    [Benchmark] public object ToBetaAllBatch() => q.ToBeta(o, 20, BetaType.All);
    [Benchmark] public object ToBollingerBandsBatch() => q.ToBollingerBands(20, 2);
    [Benchmark] public object ToBopBatch() => q.ToBop(n);
    [Benchmark] public object ToCciBatch() => q.ToCci(n);
    [Benchmark] public object ToChaikinOscBatch() => q.ToChaikinOsc();
    [Benchmark] public object ToChandelierBatch() => q.ToChandelier();
    [Benchmark] public object ToChopBatch() => q.ToChop(n);
    [Benchmark] public object ToCmfBatch() => q.ToCmf(n);
    [Benchmark] public object ToCmoBatch() => q.ToCmo(n);
    [Benchmark] public object ToConnorsRsiBatch() => q.ToConnorsRsi(3, 2, 100);
    [Benchmark] public object ToCorrelationBatch() => q.ToCorrelation(o, 20);
    [Benchmark] public object ToDemaBatch() => q.ToDema(n);
    [Benchmark] public object ToDojiBatch() => q.ToDoji();
    [Benchmark] public object ToDonchianBatch() => q.ToDonchian();
    [Benchmark] public object ToDpoBatch() => q.ToDpo(n);
    [Benchmark] public object ToDynamicBatch() => q.ToDynamic(n);
    [Benchmark] public object ToElderRayBatch() => q.ToElderRay(13);
    [Benchmark] public object ToEmaBatch() => q.ToEma(20);
    [Benchmark] public object ToEpmaBatch() => q.ToEpma(n);
    [Benchmark] public object ToFcbBatch() => q.ToFcb(2);
    [Benchmark] public object ToFisherTransformBatch() => q.ToFisherTransform(10);
    [Benchmark] public object ToForceIndexBatch() => q.ToForceIndex(2);
    [Benchmark] public object ToFractalBatch() => q.ToFractal();
    [Benchmark] public object ToGatorBatch() => q.ToGator();
    [Benchmark] public object ToHeikinAshiBatch() => q.ToHeikinAshi();
    [Benchmark] public object ToHmaBatch() => q.ToHma(n);
    [Benchmark] public object ToHtTrendlineBatch() => q.ToHtTrendline();
    [Benchmark] public object ToHurstBatch() => q.ToHurst(100);
    [Benchmark] public object ToIchimokuBatch() => q.ToIchimoku();
    [Benchmark] public object ToKamaBatch() => q.ToKama(10, 2, 30);
    [Benchmark] public object ToKeltnerBatch() => q.ToKeltner(20, 2, 10);
    [Benchmark] public object ToKvoBatch() => q.ToKvo(34, 55, 13);
    [Benchmark] public object ToMacdBatch() => q.ToMacd(12, 26, 9);
    [Benchmark] public object ToMaEnvelopesBatch() => q.ToMaEnvelopes(20, 2.5, MaType.SMA);
    [Benchmark] public object ToMamaBatch() => q.ToMama(0.5, 0.05);
    [Benchmark] public object ToMarubozuBatch() => q.ToMarubozu(95);
    [Benchmark] public object ToMfiBatch() => q.ToMfi(14);
    [Benchmark] public object ToObvBatch() => q.ToObv();
    [Benchmark] public object ToParabolicSarBatch() => q.ToParabolicSar();
    [Benchmark] public object ToPivotPointsBatch() => q.ToPivotPoints(PeriodSize.Month, PivotPointType.Standard);
    [Benchmark] public object ToPivotsBatch() => q.ToPivots(2, 2, 20);
    [Benchmark] public object ToPmoBatch() => q.ToPmo(35, 20, 10);
    [Benchmark] public object ToPrsBatch() => q.ToPrs(o);
    [Benchmark] public object ToPvoBatch() => q.ToPvo();
    [Benchmark] public object ToQuotePartBatch() => q.ToQuotePart(CandlePart.OHL3);
    [Benchmark] public object ToRenkoBatch() => q.ToRenko(2.5m);
    [Benchmark] public object ToRenkoAtrBatch() => q.ToRenko(n);
    [Benchmark] public object ToRocBatch() => q.ToRoc(20);
    [Benchmark] public object ToRocWbBatch() => q.ToRocWb(20, 5, 5);
    [Benchmark] public object ToRollingPivotsBatch() => q.ToRollingPivots(20, 0, PivotPointType.Standard);
    [Benchmark] public object ToRsiBatch() => q.ToRsi(n);
    [Benchmark] public object ToSlopeBatch() => q.ToSlope(n);
    [Benchmark] public object ToSmaBatch() => q.ToSma(n);
    [Benchmark] public object ToSmaAnalysisBatch() => q.ToSmaAnalysis(n);
    [Benchmark] public object ToSmiBatch() => q.ToSmi(13, 25, 2, 3);
    [Benchmark] public object ToSmmaBatch() => q.ToSmma(n);
    [Benchmark] public object ToStarcBandsBatch() => q.ToStarcBands(5, 2, 10);
    [Benchmark] public object ToStcBatch() => q.ToStc(10, 23, 50);
    [Benchmark] public object ToStdDevBatch() => q.ToStdDev(n);
    [Benchmark] public object ToStdDevChannelsBatch() => q.ToStdDevChannels();
    [Benchmark] public object ToStochBatch() => q.ToStoch(14, 3, 3);
    [Benchmark] public object ToStochSmmaBatch() => q.ToStoch(n, 3, 3);
    [Benchmark] public object ToStochRsiBatch() => q.ToStochRsi(n, n, 3, 1);
    [Benchmark] public object ToSuperTrendBatch() => q.ToSuperTrend(10, 3);
    [Benchmark] public object ToT3Batch() => q.ToT3(5, 0.7);
    [Benchmark] public object ToTemaBatch() => q.ToTema(n);
    [Benchmark] public object ToTrBatch() => q.ToTr();
    [Benchmark] public object ToTrixBatch() => q.ToTrix(n);
    [Benchmark] public object ToTsiBatch() => q.ToTsi(25, 13, 7);
    [Benchmark] public object ToUlcerIndexBatch() => q.ToUlcerIndex(n);
    [Benchmark] public object ToUltimateBatch() => q.ToUltimate(7, 14, 28);
    [Benchmark] public object ToVolatilityStopBatch() => q.ToVolatilityStop(7, 3);
    [Benchmark] public object ToVortexBatch() => q.ToVortex(n);
    [Benchmark] public object ToVwapBatch() => q.ToVwap();
    [Benchmark] public object ToVwmaBatch() => q.ToVwma(n);
    [Benchmark] public object ToWilliamsRBatch() => q.ToWilliamsR();
    [Benchmark] public object ToWmaBatch() => q.ToWma(n);
    [Benchmark] public object ToZigZagBatch() => q.ToZigZag();
}
