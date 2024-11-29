namespace PublicApi.Obsolete.Tests;

[TestClass]
[TestCategory("Integration")]
public class ObsoleteFeatures
{
    private static readonly IReadOnlyList<Quote> quotes = Data.GetDefault();
    private static readonly IReadOnlyList<(DateTime d, double v)> priceTuples = quotes.Select(q => (q.Timestamp, (double)q.Close)).ToList();

    [TestMethod]
    public void DeprecatedMethods()
    {
        // Test deprecated methods in ObsoleteV3.cs
        quotes.GetAlligator();
        quotes.GetAdl(10);
        quotes.GetObv(10);
        quotes.GetPrs(quotes, 10, 5);
        quotes.GetRoc(10, 5);
        quotes.GetStdDev(10, 5);
        quotes.GetTrix(10, 5);
        quotes.Use();
        quotes.ToSortedCollection();
        quotes.ToTupleChainable();
        quotes.Find(DateTime.Now);
        quotes.FindIndex(DateTime.Now);

        // Test deprecated methods in ObsoleteV3.Indicators.cs
        quotes.GetAdl();
        quotes.GetAdx();
        quotes.GetAlligator();
        quotes.GetAlma();
        quotes.GetAroon();
        quotes.GetAtr();
        quotes.GetAtrStop();
        quotes.GetAwesome();
        quotes.GetBeta(quotes, 10);
        quotes.GetBollingerBands();
        quotes.GetBop();
        quotes.GetCci();
        quotes.GetChaikinOsc();
        quotes.GetChandelier();
        quotes.GetChop();
        quotes.GetCmf();
        quotes.GetCmo(10);
        quotes.GetConnorsRsi();
        quotes.GetCorrelation(quotes, 10);
        quotes.GetDema(10);
        quotes.GetDoji();
        quotes.GetDonchian();
        quotes.GetDpo(10);
        quotes.GetDynamic(10);
        quotes.GetElderRay();
        quotes.GetEma(10);
        quotes.GetEpma(10);
        quotes.GetFcb();
        quotes.GetFisherTransform();
        quotes.GetForceIndex();
        quotes.GetFractal();
        quotes.GetGator();
        quotes.GetHeikinAshi();
        quotes.GetHma(10);
        quotes.GetHtTrendline();
        quotes.GetHurst();
        quotes.GetIchimoku();
        quotes.GetKama();
        quotes.GetKeltner();
        quotes.GetKvo();
        quotes.GetMacd();
        quotes.GetMaEnvelopes(10);
        quotes.GetMama(10);
        quotes.GetMarubozu();
        quotes.GetMfi();
        quotes.GetObv();
        quotes.GetParabolicSar();
        quotes.GetPivotPoints(10);
        quotes.GetPivots();
        quotes.GetPmo();
        quotes.GetPrs(quotes);
        quotes.GetPvo();
        quotes.GetRenko(10);
        quotes.GetRenkoAtr(10);
        quotes.GetRoc(10);
        quotes.GetRocWb(10, 10, 10);
        quotes.GetRollingPivots(10, 10);
        quotes.GetRsi(10);
        quotes.GetSlope(10);
        quotes.GetSma(10);
        quotes.GetSmaAnalysis(10);
        quotes.GetSmi();
        quotes.GetSmma(10);
        quotes.GetStarcBands(10);
        quotes.GetStc();
        quotes.GetStdDev(10);
        quotes.GetStdDevChannels();
        quotes.GetStoch();
        quotes.GetStochRsi(10, 10, 10);
        quotes.GetSuperTrend();
        quotes.GetT3();
        quotes.GetTema(10);
        quotes.GetTr();
        quotes.GetTrix(10);
        quotes.GetTsi();
        quotes.GetUlcerIndex();
        quotes.GetUltimate();
        quotes.GetVolatilityStop();
        quotes.GetVortex(10);
        quotes.GetVwap();
        quotes.GetVwma(10);
        quotes.GetWilliamsR(10);
        quotes.GetWma(10);
        quotes.GetZigZag();
    }
}
