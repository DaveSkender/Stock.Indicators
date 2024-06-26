namespace Tests.Indicators;

[TestClass]
public class AdlTests : SeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        List<AdlResult> results = quotes
            .GetAdl()
            .ToList();

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
        List<SmaResult> results = quotes
            .GetAdl()
            .GetSma(10)
            .ToList();

        // assertions

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(493, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        List<AdlResult> r = badQuotes
            .GetAdl()
            .ToList();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => double.IsNaN(x.Adl)));
    }

    [TestMethod]
    public void BigData()
    {
        List<AdlResult> r = bigQuotes
            .GetAdl()
            .ToList();

        Assert.AreEqual(1246, r.Count);
    }

    [TestMethod]
    public void RandomData()
    {
        List<AdlResult> r = randomQuotes
            .GetAdl()
            .ToList();

        Assert.AreEqual(1000, r.Count);
    }

    [TestMethod]
    public override void NoQuotes()
    {
        List<AdlResult> r0 = noquotes
            .GetAdl()
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<AdlResult> r1 = onequote
            .GetAdl()
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }
}
