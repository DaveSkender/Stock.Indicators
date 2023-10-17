using BenchmarkDotNet.Attributes;
using Skender.Stock.Indicators;
using Tests.Common;

namespace Tests.Performance;

public class IndicatorsStatic
{
    private static IEnumerable<Quote> q;
    private static IEnumerable<Quote> o;
    private static List<Quote> ql;
    private static List<Quote> ll;

    // SETUP

    [GlobalSetup]
    public static void Setup()
    {
        q = TestData.GetDefault();
        ql = q.ToList();
        ll = TestData.GetLongest().ToList();
    }

    [GlobalSetup(Targets = new[]
    {
        nameof(GetBeta),
        nameof(GetBetaUp),
        nameof(GetBetaDown),
        nameof(GetBetaAll),
        nameof(GetCorrelation),
        nameof(GetPrs),
        nameof(GetPrsWithSma)
    })]
    public static void SetupCompare()
    {
        q = TestData.GetDefault();
        o = TestData.GetCompare();
    }

    // BENCHMARKS

    [Benchmark]
    public object GetAdl() => q.GetAdl();

    [Benchmark]
    [Obsolete("Deprecated in v3.0.0", false)]
    public object GetAdlWithSma() => q.GetAdl(14);

    [Benchmark]
    public object GetAdx() => q.GetAdx();

    [Benchmark]
    public object GetAlligator() => q.GetAlligator();

    [Benchmark]
    public object GetAlma() => q.GetAlma();

    [Benchmark]
    public object GetAroon() => q.GetAroon();

    [Benchmark]
    public object GetAtr() => q.GetAtr();

    [Benchmark]
    public object GetAtrStop() => q.GetAtrStop();

    [Benchmark]
    public object GetAwesome() => q.GetAwesome();

    [Benchmark]
    public object GetBeta() => Indicator.GetBeta(q, o, 20, BetaType.Standard);

    [Benchmark]
    public object GetBetaUp() => Indicator.GetBeta(q, o, 20, BetaType.Up);

    [Benchmark]
    public object GetBetaDown() => Indicator.GetBeta(q, o, 20, BetaType.Down);

    [Benchmark]
    public object GetBetaAll() => Indicator.GetBeta(q, o, 20, BetaType.All);

    [Benchmark]
    public object GetBollingerBands() => q.GetBollingerBands();

    [Benchmark]
    public object GetBop() => q.GetBop();

    [Benchmark]
    public object GetCci() => q.GetCci();

    [Benchmark]
    public object GetChaikinOsc() => q.GetChaikinOsc();

    [Benchmark]
    public object GetChandelier() => q.GetChandelier();

    [Benchmark]
    public object GetChop() => q.GetChop();

    [Benchmark]
    public object GetCmf() => q.GetCmf();

    [Benchmark]
    public object GetCmo() => q.GetCmo(14);

    [Benchmark]
    public object GetConnorsRsi() => q.GetConnorsRsi();

    [Benchmark]
    public object GetCorrelation() => q.GetCorrelation(o, 20);

    [Benchmark]
    public object GetDema() => q.GetDema(14);

    [Benchmark]
    public object GetDoji() => q.GetDoji();

    [Benchmark]
    public object GetDonchian() => q.GetDonchian();

    [Benchmark]
    public object GetDpo() => q.GetDpo(14);

    [Benchmark]
    public object GetElderRay() => q.GetElderRay();

    [Benchmark]
    public object GetEma() => q.GetEma(14);

    [Benchmark]
    public object GetEpma() => q.GetEpma(14);

    [Benchmark]
    public object GetFcb() => q.GetFcb(14);

    [Benchmark]
    public object GetFisherTransform() => q.GetFisherTransform(10);

    [Benchmark]
    public object GetForceIndex() => q.GetForceIndex(13);

    [Benchmark]
    public object GetFractal() => q.GetFractal();

    [Benchmark]
    public object GetGator() => q.GetGator();

    [Benchmark]
    public object GetHeikinAshi() => q.GetHeikinAshi();

    [Benchmark]
    public object GetHma() => q.GetHma(14);

    [Benchmark]
    public object GetHtTrendline() => q.GetHtTrendline();

    [Benchmark]
    public object GetHurst() => q.GetHurst();

    [Benchmark]
    public object GetIchimoku() => q.GetIchimoku();

    [Benchmark]
    public object GetKama() => q.GetKama();

    [Benchmark]
    public object GetKlinger() => q.GetKvo();

    [Benchmark]
    public object GetKeltner() => q.GetKeltner();

    [Benchmark]
    public object GetKvo() => q.GetKvo();

    [Benchmark]
    public object GetMacd() => q.GetMacd();

    [Benchmark]
    public object GetMaEnvelopes() => q.GetMaEnvelopes(20, 2.5, MaType.SMA);

    [Benchmark]
    public object GetMama() => q.GetMama();

    [Benchmark]
    public object GetMarubozu() => q.GetMarubozu();

    [Benchmark]
    public object GetMfi() => q.GetMfi();

    [Benchmark]
    public object GetObv() => q.GetObv();

    [Benchmark]
    public object GetObvWithSma() => q.GetObv(14);

    [Benchmark]
    public object GetParabolicSar() => q.GetParabolicSar();

    [Benchmark]
    public object GetPivotPoints() => q.GetPivotPoints(PeriodSize.Month, PivotPointType.Standard);

    [Benchmark]
    public object GetPivots() => q.GetPivots();

    [Benchmark]
    public object GetPmo() => q.GetPmo();

    [Benchmark]
    public object GetPrs() => q.GetPrs(o);

    [Benchmark]
    public object GetPrsWithSma() => q.GetPrs(o, null, 5);

    [Benchmark]
    public object GetPvo() => q.GetPvo();

    [Benchmark]
    public object GetRenko() => q.GetRenko(2.5m);

    [Benchmark]
    public object GetRenkoAtr() => q.GetRenko(14);

    [Benchmark]
    public object GetRoc() => q.GetRoc(20);

    [Benchmark]
    public object GetRocWb() => q.GetRocWb(12, 3, 12);

    [Benchmark]
    public object GetRocWithSma() => q.GetRoc(20, 14);

    [Benchmark]
    public object GetRollingPivots() => q.GetRollingPivots(14, 1);

    [Benchmark]
    public object GetRsi() => q.GetRsi();

    [Benchmark]
    public object GetSlope() => q.GetSlope(20);

    [Benchmark]
    public object GetSma() => q.GetSma(10);

    [Benchmark]
    public object GetSmaAnalysis() => q.GetSmaAnalysis(10);

    [Benchmark]
    public object GetSmi() => q.GetSmi(5, 20, 5, 3);

    [Benchmark]
    public object GetSmma() => q.GetSmma(10);

    [Benchmark]
    public object GetStarcBands() => q.GetStarcBands(10);

    [Benchmark]
    public object GetStc() => q.GetStc();

    [Benchmark]
    public object GetStdDev() => q.GetStdDev(20);

    [Benchmark]
    public object GetStdDevWithSma() => q.GetStdDev(20, 14);

    [Benchmark]
    public object GetStdDevChannels() => q.GetStdDevChannels();

    [Benchmark]
    public object GetStoch() => q.GetStoch();

    [Benchmark]
    public object GetStochSMMA() => q.GetStoch(9, 3, 3, 3, 2, MaType.SMMA);

    [Benchmark]
    public object GetStochRsi() => q.GetStochRsi(14, 14, 3);

    [Benchmark]
    public object GetSuperTrend() => q.GetSuperTrend();

    [Benchmark]
    public object GetT3() => q.GetT3();

    [Benchmark]
    public object GetTema() => q.GetTema(14);

    [Benchmark]
    public object GetTr() => q.GetTr();

    [Benchmark]
    public object GetTrix() => q.GetTrix(14);

    [Benchmark]
    public object GetTrixWithSma() => q.GetTrix(14, 5);

    [Benchmark]
    public object GetTsi() => q.GetTsi();

    [Benchmark]
    public object GetUlcerIndex() => q.GetUlcerIndex();

    [Benchmark]
    public object GetUltimate() => q.GetUltimate();

    [Benchmark]
    public object GetVolatilityStop() => q.GetVolatilityStop();

    [Benchmark]
    public object GetVortex() => q.GetVortex(14);

    [Benchmark]
    public object GetVwap() => q.GetVwap();

    [Benchmark]
    public object GetVwma() => q.GetVwma(14);

    [Benchmark]
    public object GetWilliamsR() => q.GetWilliamsR();

    [Benchmark]
    public object GetWma() => q.GetWma(14);

    [Benchmark]
    public object GetZigZag() => q.GetZigZag();
}
