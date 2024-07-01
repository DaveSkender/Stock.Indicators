namespace Tests.Indicators.Series;

[TestClass]
public class CmfTests : SeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        List<CmfResult> results = Quotes
            .GetCmf()
            .ToList();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.Cmf != null));

        // sample values
        CmfResult r1 = results[49];
        Assert.AreEqual(0.5468, r1.MoneyFlowMultiplier.Round(4));
        Assert.AreEqual(55609259, r1.MoneyFlowVolume.Round(2));
        Assert.AreEqual(0.350596, r1.Cmf.Round(6));

        CmfResult r2 = results[249];
        Assert.AreEqual(0.7778, r2.MoneyFlowMultiplier.Round(4));
        Assert.AreEqual(36433792.89, r2.MoneyFlowVolume.Round(2));
        Assert.AreEqual(-0.040226, r2.Cmf.Round(6));

        CmfResult r3 = results[501];
        Assert.AreEqual(0.8052, r3.MoneyFlowMultiplier.Round(4));
        Assert.AreEqual(118396116.25, r3.MoneyFlowVolume.Round(2));
        Assert.AreEqual(-0.123754, r3.Cmf.Round(6));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = Quotes
            .GetCmf()
            .GetSma(10)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(474, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        List<CmfResult> r = BadQuotes
            .GetCmf(15)
            .ToList();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Cmf is double.NaN));
    }

    [TestMethod]
    public void BigData()
    {
        List<CmfResult> r = BigQuotes
            .GetCmf(150)
            .ToList();

        Assert.AreEqual(1246, r.Count);
    }

    [TestMethod]
    public override void NoQuotes()
    {
        List<CmfResult> r0 = Noquotes
            .GetCmf()
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<CmfResult> r1 = Onequote
            .GetCmf()
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        List<CmfResult> results = Quotes
            .GetCmf()
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - 19, results.Count);

        CmfResult last = results.LastOrDefault();
        Assert.AreEqual(0.8052, last.MoneyFlowMultiplier.Round(4));
        Assert.AreEqual(118396116.25, last.MoneyFlowVolume.Round(2));
        Assert.AreEqual(-0.123754, last.Cmf.Round(6));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => Quotes.GetCmf(0));
}
