namespace Performance;

// SERIES-STYLE INDICATORS

[ShortRunJob]
public class SeriesIndicators
{
    private static readonly IReadOnlyList<Quote> q = Data.GetRandom(500000);
    private static readonly IReadOnlyList<Quote> o = Data.GetCompare();
    private static readonly double[] v = q.ToValueArray();
    private const int n = 14;

    /* Parameter arguments should match the Catalog default values */

    [Benchmark] public void ToAdl() => q.ToAdl();
    [Benchmark] public void ToAdx() => q.ToAdx();
    [Benchmark] public void ToAlligator() => q.ToAlligator();
    [Benchmark] public void ToAlma() => q.ToAlma(9, 0.85, 6);
    [Benchmark] public void ToAroon() => q.ToAroon();
    [Benchmark] public void ToAtr() => q.ToAtr();
    [Benchmark] public void ToAtrStop() => q.ToAtrStop();
    [Benchmark] public void ToAwesome() => q.ToAwesome();
    [Benchmark] public void ToBeta() => q.ToBeta(o, 20, BetaType.Standard);
    [Benchmark] public void ToBetaUp() => q.ToBeta(o, 20, BetaType.Up);
    [Benchmark] public void ToBetaDown() => q.ToBeta(o, 20, BetaType.Down);
    [Benchmark] public void ToBetaAll() => q.ToBeta(o, 20, BetaType.All);
    [Benchmark] public void ToBollingerBands() => q.ToBollingerBands();
    [Benchmark] public void ToBop() => q.ToBop();
    [Benchmark] public void ToCci() => q.ToCci();
    [Benchmark] public void ToChaikinOsc() => q.ToChaikinOsc();
    [Benchmark] public void ToChandelier() => q.ToChandelier();
    [Benchmark] public void ToChop() => q.ToChop();
    [Benchmark] public void ToCmf() => q.ToCmf();
    [Benchmark] public void ToCmo() => q.ToCmo(n);
    [Benchmark] public void ToConnorsRsi() => q.ToConnorsRsi();
    [Benchmark] public void ToCorrelation() => q.ToCorrelation(o, 20);
    [Benchmark] public void ToDema() => q.ToDema(n);
    [Benchmark] public void ToDoji() => q.ToDoji();
    [Benchmark] public void ToDonchian() => q.ToDonchian();
    [Benchmark] public void ToDpo() => q.ToDpo(n);
    [Benchmark] public void ToDynamic() => q.ToDynamic(20);
    [Benchmark] public void ToElderRay() => q.ToElderRay();
    [Benchmark] public void ToEma() => q.ToEma(20);
    [Benchmark] public void ToEpma() => q.ToEpma(n);
    [Benchmark] public void ToFcb() => q.ToFcb(n);
    [Benchmark] public void ToFisherTransform() => q.ToFisherTransform(10);
    [Benchmark] public void ToForceIndex() => q.ToForceIndex(13);
    [Benchmark] public void ToFractal() => q.ToFractal();
    [Benchmark] public void ToGator() => q.ToGator();
    [Benchmark] public void ToHeikinAshi() => q.ToHeikinAshi();
    [Benchmark] public void ToHma() => q.ToHma(n);
    [Benchmark] public void ToHtTrendline() => q.ToHtTrendline();
    [Benchmark] public void ToHurst() => q.ToHurst();
    [Benchmark] public void ToIchimoku() => q.ToIchimoku();
    [Benchmark] public void ToKama() => q.ToKama();
    [Benchmark] public void ToKeltner() => q.ToKeltner();
    [Benchmark] public void ToKvo() => q.ToKvo();
    [Benchmark] public void ToMacd() => q.ToMacd();
    [Benchmark] public void ToMaEnvelopes() => q.ToMaEnvelopes(20, 2.5, MaType.SMA);
    [Benchmark] public void ToMama() => q.ToMama();
    [Benchmark] public void ToMarubozu() => q.ToMarubozu();
    [Benchmark] public void ToMfi() => q.ToMfi();
    [Benchmark] public void ToObv() => q.ToObv();
    [Benchmark] public void ToParabolicSar() => q.ToParabolicSar();
    [Benchmark] public void ToPivotPoints() => q.ToPivotPoints(PeriodSize.Month, PivotPointType.Standard);
    [Benchmark] public void ToPivots() => q.ToPivots();
    [Benchmark] public void ToPmo() => q.ToPmo();
    [Benchmark] public void ToPrs() => q.ToPrs(o);
    [Benchmark] public void ToPvo() => q.ToPvo();
    [Benchmark] public void ToRenko() => q.ToRenko(2.5m);
    [Benchmark] public void ToRenkoAtr() => q.ToRenko(n);
    [Benchmark] public void ToRoc() => q.ToRoc(20);
    [Benchmark] public void ToRocWb() => q.ToRocWb(12, 3, 12);
    [Benchmark] public void ToRollingPivots() => q.ToRollingPivots(20, 0, PivotPointType.Standard);
    [Benchmark] public void ToRsi() => q.ToRsi();
    [Benchmark] public void ToSlope() => q.ToSlope(20);

    // TODO: this is for experimental comparison only - remove later
    [Benchmark] public void ToSmaOrig() => q.ToSma(10);
    [Benchmark] public void ToSmaArray() => q.ToSmaArray(10);
    [Benchmark] public void ToSmaArrayLoop() => v.ToSmaArrayLoop(10);
    [Benchmark] public void ToSmaArrayRoll() => v.ToSmaArrayRoll(10);
    //[Benchmark] public void ToSmaAnalysis() => q.ToSmaAnalysis(10);

    [Benchmark] public void ToSmi() => q.ToSmi(5, 20, 5, 3);
    [Benchmark] public void ToSmma() => q.ToSmma(10);
    [Benchmark] public void ToStarcBands() => q.ToStarcBands(10);
    [Benchmark] public void ToStc() => q.ToStc();
    [Benchmark] public void ToStdDev() => q.ToStdDev(20);
    [Benchmark] public void ToStdDevChannels() => q.ToStdDevChannels();
    [Benchmark] public void ToStoch() => q.ToStoch();
    [Benchmark] public void ToStochSmma() => q.ToStoch(9, 3, 3, 3, 2, MaType.SMMA);
    [Benchmark] public void ToStochRsi() => q.ToStochRsi(n, n, 3);
    [Benchmark] public void ToSuperTrend() => q.ToSuperTrend();
    [Benchmark] public void ToT3() => q.ToT3();
    [Benchmark] public void ToTema() => q.ToTema(20);
    [Benchmark] public void ToTr() => q.ToTr();
    [Benchmark] public void ToTrix() => q.ToTrix(n);
    [Benchmark] public void ToTsi() => q.ToTsi();
    [Benchmark] public void ToUlcerIndex() => q.ToUlcerIndex();
    [Benchmark] public void ToUltimate() => q.ToUltimate();
    [Benchmark] public void ToVolatilityStop() => q.ToVolatilityStop();
    [Benchmark] public void ToVortex() => q.ToVortex(n);
    [Benchmark] public void ToVwap() => q.ToVwap();
    [Benchmark] public void ToVwma() => q.ToVwma(n);
    [Benchmark] public void ToWilliamsR() => q.ToWilliamsR();
    [Benchmark] public void ToWma() => q.ToWma(n);
    [Benchmark] public void ToZigZag() => q.ToZigZag();
}
