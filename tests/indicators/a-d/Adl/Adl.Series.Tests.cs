namespace StaticSeries;

[TestClass]
public class AdlTests : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        IReadOnlyList<AdlResult> results = Quotes
            .GetAdl();

        // proper quantities
        Assert.AreEqual(502, results.Count);

        // sample values
        AdlResult r1 = results[249];
        Assert.AreEqual(0.7778, r1.MoneyFlowMultiplier.Round(4));
        Assert.AreEqual(36433792.89, r1.MoneyFlowVolume.Round(2));
        Assert.AreEqual(3266400865.74, r1.Adl.Round(2));

        AdlResult r2 = results[501];
        Assert.AreEqual(0.8052, r2.MoneyFlowMultiplier.Round(4));
        Assert.AreEqual(118396116.25, r2.MoneyFlowVolume.Round(2));
        Assert.AreEqual(3439986548.42, r2.Adl.Round(2));
    }

    [TestMethod]
    public void Chainor()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .GetAdl()
            .GetSma(10);

        // assertions

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(493, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<AdlResult> r = BadQuotes
            .GetAdl();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => double.IsNaN(x.Adl)));
    }

    [TestMethod]
    public void BigData()
    {
        IReadOnlyList<AdlResult> r = BigQuotes
            .GetAdl();

        Assert.AreEqual(1246, r.Count);
    }

    [TestMethod]
    public void RandomData()
    {
        IReadOnlyList<AdlResult> r = RandomQuotes
            .GetAdl();

        Assert.AreEqual(1000, r.Count);
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<AdlResult> r0 = Noquotes
            .GetAdl();

        Assert.AreEqual(0, r0.Count);

        IReadOnlyList<AdlResult> r1 = Onequote
            .GetAdl();

        Assert.AreEqual(1, r1.Count);
    }
}
