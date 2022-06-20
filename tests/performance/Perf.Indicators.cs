using BenchmarkDotNet.Attributes;
using Internal.Tests;
using Skender.Stock.Indicators;

namespace Tests.Performance;

[MarkdownExporterAttribute.GitHub]
public class IndicatorPerformance
{
    private static IEnumerable<Quote> h;
    private static IEnumerable<Quote> ho;
    private static List<Quote> hList;

    // SETUP

    [GlobalSetup]
    public void Setup()
    {
        h = TestData.GetDefault();
        hList = h.ToList();
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
    public void SetupCompare()
    {
        h = TestData.GetDefault();
        ho = TestData.GetCompare();
    }

    // BENCHMARKS

    [Benchmark]
    public object GetAdl() => h.GetAdl();

    [Benchmark]
    public object GetAdlWithSma() => h.GetAdl(14);

    [Benchmark]
    public object GetAdx() => h.GetAdx();

    [Benchmark]
    public object GetAlligator() => h.GetAlligator();

    [Benchmark]
    public object GetAlma() => h.GetAlma();

    [Benchmark]
    public object GetAroon() => h.GetAroon();

    [Benchmark]
    public object GetAtr() => h.GetAtr();

    [Benchmark]
    public object GetAwesome() => h.GetAwesome();

    [Benchmark]
    public object GetBeta() => Indicator.GetBeta(h, ho, 20, BetaType.Standard);

    [Benchmark]
    public object GetBetaUp() => Indicator.GetBeta(h, ho, 20, BetaType.Up);

    [Benchmark]
    public object GetBetaDown() => Indicator.GetBeta(h, ho, 20, BetaType.Down);

    [Benchmark]
    public object GetBetaAll() => Indicator.GetBeta(h, ho, 20, BetaType.All);

    [Benchmark]
    public object GetBollingerBands() => h.GetBollingerBands();

    [Benchmark]
    public object GetBop() => h.GetBop();

    [Benchmark]
    public object GetCci() => h.GetCci();

    [Benchmark]
    public object GetChaikinOsc() => h.GetChaikinOsc();

    [Benchmark]
    public object GetChandelier() => h.GetChandelier();

    [Benchmark]
    public object GetChop() => h.GetChop();

    [Benchmark]
    public object GetCmf() => h.GetCmf();

    [Benchmark]
    public object GetConnorsRsi() => h.GetConnorsRsi();

    [Benchmark]
    public object GetCorrelation() => h.GetCorrelation(ho, 20);

    [Benchmark]
    public object GetDema() => h.GetDema(14);

    [Benchmark]
    public object GetDoji() => h.GetDoji();

    [Benchmark]
    public object GetDonchian() => h.GetDonchian();

    [Benchmark]
    public object GetDpo() => h.GetDpo(14);

    [Benchmark]
    public object GetElderRay() => h.GetElderRay();

    [Benchmark]
    public object GetEma() => h.GetEma(14);

    [Benchmark]
    public object GetEmaStream()
    {
        EmaBase emaBase = hList.Take(15).InitEma(14);

        for (int i = 15; i < hList.Count; i++)
        {
            Quote q = hList[i];
            emaBase.Add(q);
        }

        return emaBase.Results;
    }

    [Benchmark]
    public object GetEpma() => h.GetEpma(14);

    [Benchmark]
    public object GetFcb() => h.GetFcb(14);

    [Benchmark]
    public object GetFisherTransform() => h.GetFisherTransform(10);

    [Benchmark]
    public object GetForceIndex() => h.GetForceIndex(13);

    [Benchmark]
    public object GetFractal() => h.GetFractal();

    [Benchmark]
    public object GetGator() => h.GetGator();

    [Benchmark]
    public object GetHeikinAshi() => h.GetHeikinAshi();

    [Benchmark]
    public object GetHma() => h.GetHma(14);

    [Benchmark]
    public object GetHtTrendline() => h.GetHtTrendline();

    [Benchmark]
    public object GetHurst() => h.GetHurst();

    [Benchmark]
    public object GetIchimoku() => h.GetIchimoku();

    [Benchmark]
    public object GetKama() => h.GetKama();

    [Benchmark]
    public object GetKlinger() => h.GetKvo();

    [Benchmark]
    public object GetKeltner() => h.GetKeltner();

    [Benchmark]
    public object GetKvo() => h.GetKvo();

    [Benchmark]
    public object GetMacd() => h.GetMacd();

    [Benchmark]
    public object GetMaEnvelopes() => h.GetMaEnvelopes(20, 2.5, MaType.SMA);

    [Benchmark]
    public object GetMama() => h.GetMama();

    [Benchmark]
    public object GetMarubozu() => h.GetMarubozu();

    [Benchmark]
    public object GetMfi() => h.GetMfi();

    [Benchmark]
    public object GetObv() => h.GetObv();

    [Benchmark]
    public object GetObvWithSma() => h.GetObv(14);

    [Benchmark]
    public object GetParabolicSar() => h.GetParabolicSar();

    [Benchmark]
    public object GetPivotPoints() => h.GetPivotPoints(PeriodSize.Month, PivotPointType.Standard);

    [Benchmark]
    public object GetPivots() => h.GetPivots();

    [Benchmark]
    public object GetPmo() => h.GetPmo();

    [Benchmark]
    public object GetPrs() => h.GetPrs(ho);

    [Benchmark]
    public object GetPrsWithSma() => h.GetPrs(ho, null, 5);

    [Benchmark]
    public object GetPvo() => h.GetPvo();

    [Benchmark]
    public object GetRenko() => h.GetRenko(2.5m);

    [Benchmark]
    public object GetRenkoAtr() => h.GetRenko(14);

    [Benchmark]
    public object GetRoc() => h.GetRoc(20);

    [Benchmark]
    public object GetRocWb() => h.GetRocWb(12, 3, 12);

    [Benchmark]
    public object GetRocWithSma() => h.GetRoc(20, 14);

    [Benchmark]
    public object GetRollingPivots() => h.GetRollingPivots(14, 1);

    [Benchmark]
    public object GetRsi() => h.GetRsi();

    [Benchmark]
    public object GetSlope() => h.GetSlope(20);

    [Benchmark]
    public object GetSma() => h.GetSma(10);

    [Benchmark]
    public object GetSmaAnalysis() => h.GetSmaAnalysis(10);

    [Benchmark]
    public object GetSmi() => h.GetSmi(5, 20, 5, 3);

    [Benchmark]
    public object GetSmma() => h.GetSmma(10);

    [Benchmark]
    public object GetStarcBands() => h.GetStarcBands();

    [Benchmark]
    public object GetStc() => h.GetStc();

    [Benchmark]
    public object GetStdDev() => h.GetStdDev(20);

    [Benchmark]
    public object GetStdDevWithSma() => h.GetStdDev(20, 14);

    [Benchmark]
    public object GetStdDevChannels() => h.GetStdDevChannels();

    [Benchmark]
    public object GetStoch() => h.GetStoch();

    [Benchmark]
    public object GetStochSMMA() => h.GetStoch(9, 3, 3, 3, 2, MaType.SMMA);

    [Benchmark]
    public object GetStochRsi() => h.GetStochRsi(14, 14, 3);

    [Benchmark]
    public object GetSuperTrend() => h.GetSuperTrend();

    [Benchmark]
    public object GetT3() => h.GetT3();

    [Benchmark]
    public object GetTema() => h.GetTema(14);

    [Benchmark]
    public object GetTrix() => h.GetTrix(14);

    [Benchmark]
    public object GetTrixWithSma() => h.GetTrix(14, 5);

    [Benchmark]
    public object GetTsi() => h.GetTsi();

    [Benchmark]
    public object GetUlcerIndex() => h.GetUlcerIndex();

    [Benchmark]
    public object GetUltimate() => h.GetUltimate();

    [Benchmark]
    public object GetVolatilityStop() => h.GetVolatilityStop();

    [Benchmark]
    public object GetVortex() => h.GetVortex(14);

    [Benchmark]
    public object GetVwap() => h.GetVwap();

    [Benchmark]
    public object GetVwma() => h.GetVwma(14);

    [Benchmark]
    public object GetWilliamsR() => h.GetWilliamsR();

    [Benchmark]
    public object GetWma() => h.GetWma(14);

    [Benchmark]
    public object GetZigZag() => h.GetZigZag();
}
