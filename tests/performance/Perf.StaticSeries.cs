namespace Performance;

// TIME-SERIES INDICATORS

[ShortRunJob]
public class SeriesIndicators
{
    private static readonly IReadOnlyList<Quote> q = Data.GetDefault();
    private static readonly IReadOnlyList<Quote> o = Data.GetCompare();

    [Benchmark]
    public object ToAdl() => q.ToAdl();

    [Benchmark]
    public object ToAdx() => q.ToAdx();

    [Benchmark]
    public object ToAlligator() => q.ToAlligator();

    [Benchmark]
    public object ToAlma() => q.ToAlma();

    [Benchmark]
    public object ToAroon() => q.ToAroon();

    [Benchmark]
    public object ToAtr() => q.ToAtr();

    [Benchmark]
    public object ToAtrStop() => q.ToAtrStop();

    [Benchmark]
    public object ToAwesome() => q.ToAwesome();

    [Benchmark]
    public object ToBeta() => Beta.ToBeta(q, o, 20, BetaType.Standard);

    [Benchmark]
    public object ToBetaUp() => Beta.ToBeta(q, o, 20, BetaType.Up);

    [Benchmark]
    public object ToBetaDown() => Beta.ToBeta(q, o, 20, BetaType.Down);

    [Benchmark]
    public object ToBetaAll() => Beta.ToBeta(q, o, 20, BetaType.All);

    [Benchmark]
    public object ToBollingerBands() => q.ToBollingerBands();

    [Benchmark]
    public object ToBop() => q.ToBop();

    [Benchmark]
    public object ToCci() => q.ToCci();

    [Benchmark]
    public object ToChaikinOsc() => q.ToChaikinOsc();

    [Benchmark]
    public object ToChandelier() => q.ToChandelier();

    [Benchmark]
    public object ToChop() => q.ToChop();

    [Benchmark]
    public object ToCmf() => q.ToCmf();

    [Benchmark]
    public object ToCmo() => q.ToCmo(14);

    [Benchmark]
    public object ToConnorsRsi() => q.ToConnorsRsi();

    [Benchmark]
    public object ToCorrelation() => q.ToCorrelation(o, 20);

    [Benchmark]
    public object ToDema() => q.ToDema(14);

    [Benchmark]
    public object ToDoji() => q.ToDoji();

    [Benchmark]
    public object ToDonchian() => q.ToDonchian();

    [Benchmark]
    public object ToDpo() => q.ToDpo(14);

    [Benchmark]
    public object ToDynamic() => q.ToDynamic(20);

    [Benchmark]
    public object ToElderRay() => q.ToElderRay();

    [Benchmark]
    public object ToEma() => q.ToEma(14);

    [Benchmark]
    public object ToEpma() => q.ToEpma(14);

    [Benchmark]
    public object ToFcb() => q.ToFcb(14);

    [Benchmark]
    public object ToFisherTransform() => q.ToFisherTransform(10);

    [Benchmark]
    public object ToForceIndex() => q.ToForceIndex(13);

    [Benchmark]
    public object ToFractal() => q.ToFractal();

    [Benchmark]
    public object ToGator() => q.ToGator();

    [Benchmark]
    public object ToHeikinAshi() => q.ToHeikinAshi();

    [Benchmark]
    public object ToHma() => q.ToHma(14);

    [Benchmark]
    public object ToHtTrendline() => q.ToHtTrendline();

    [Benchmark]
    public object ToHurst() => q.ToHurst();

    [Benchmark]
    public object ToIchimoku() => q.ToIchimoku();

    [Benchmark]
    public object ToKama() => q.ToKama();

    [Benchmark]
    public object ToKlinger() => q.ToKvo();

    [Benchmark]
    public object ToKeltner() => q.ToKeltner();

    [Benchmark]
    public object ToKvo() => q.ToKvo();

    [Benchmark]
    public object ToMacd() => q.ToMacd();

    [Benchmark]
    public object ToMaEnvelopes() => q.ToMaEnvelopes(20, 2.5, MaType.SMA);

    [Benchmark]
    public object ToMama() => q.ToMama();

    [Benchmark]
    public object ToMarubozu() => q.ToMarubozu();

    [Benchmark]
    public object ToMfi() => q.ToMfi();

    [Benchmark]
    public object ToObv() => q.ToObv();

    [Benchmark]
    public object ToParabolicSar() => q.ToParabolicSar();

    [Benchmark]
    public object ToPivotPoints() => q.ToPivotPoints(PeriodSize.Month, PivotPointType.Standard);

    [Benchmark]
    public object ToPivots() => q.ToPivots();

    [Benchmark]
    public object ToPmo() => q.ToPmo();

    [Benchmark]
    public object ToPrs() => q.ToPrs(o);

    [Benchmark]
    public object ToPvo() => q.ToPvo();

    [Benchmark]
    public object ToRenko() => q.ToRenko(2.5m);

    [Benchmark]
    public object ToRenkoAtr() => q.ToRenko(14);

    [Benchmark]
    public object ToRoc() => q.ToRoc(20);

    [Benchmark]
    public object ToRocWb() => q.ToRocWb(12, 3, 12);

    [Benchmark]
    public object ToRollingPivots() => q.ToRollingPivots(14, 1);

    [Benchmark]
    public object ToRsi() => q.ToRsi();

    [Benchmark]
    public object ToSlope() => q.ToSlope(20);

    [Benchmark]
    public object ToSma() => q.ToSma(10);

    [Benchmark]
    public object ToSmaAnalysis() => q.ToSmaAnalysis(10);

    [Benchmark]
    public object ToSmi() => q.ToSmi(5, 20, 5, 3);

    [Benchmark]
    public object ToSmma() => q.ToSmma(10);

    [Benchmark]
    public object ToStarcBands() => q.ToStarcBands(10);

    [Benchmark]
    public object ToStc() => q.ToStc();

    [Benchmark]
    public object ToStdDev() => q.ToStdDev(20);

    [Benchmark]
    public object ToStdDevChannels() => q.ToStdDevChannels();

    [Benchmark]
    public object ToStoch() => q.ToStoch();

    [Benchmark]
    public object ToStochSMMA() => q.ToStoch(9, 3, 3, 3, 2, MaType.SMMA);

    [Benchmark]
    public object ToStochRsi() => q.ToStochRsi(14, 14, 3);

    [Benchmark]
    public object ToSuperTrend() => q.ToSuperTrend();

    [Benchmark]
    public object ToT3() => q.ToT3();

    [Benchmark]
    public object ToTema() => q.ToTema(14);

    [Benchmark]
    public object ToTr() => q.ToTr();

    [Benchmark]
    public object ToTrix() => q.ToTrix(14);

    [Benchmark]
    public object ToTsi() => q.ToTsi();

    [Benchmark]
    public object ToUlcerIndex() => q.ToUlcerIndex();

    [Benchmark]
    public object ToUltimate() => q.ToUltimate();

    [Benchmark]
    public object ToVolatilityStop() => q.ToVolatilityStop();

    [Benchmark]
    public object ToVortex() => q.ToVortex(14);

    [Benchmark]
    public object ToVwap() => q.ToVwap();

    [Benchmark]
    public object ToVwma() => q.ToVwma(14);

    [Benchmark]
    public object ToWilliamsR() => q.ToWilliamsR();

    [Benchmark]
    public object ToWma() => q.ToWma(14);

    [Benchmark]
    public object ToZigZag() => q.ToZigZag();
}
