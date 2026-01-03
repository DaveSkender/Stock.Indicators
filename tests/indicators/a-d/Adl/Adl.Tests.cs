namespace Tests.Indicators;

[TestClass]
public class AdlTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<AdlResult> results = quotes
            .GetAdl()
            .ToList();

        // proper quantities
        Assert.HasCount(502, results);
        Assert.AreEqual(502, results.Count(static x => x.AdlSma == null));

        // sample values
        AdlResult r1 = results[249];
        Assert.AreEqual(0.7778, r1.MoneyFlowMultiplier.Round(4));
        Assert.AreEqual(36433792.89, r1.MoneyFlowVolume.Round(2));
        Assert.AreEqual(3266400865.74, r1.Adl.Round(2));
        Assert.IsNull(r1.AdlSma);

        AdlResult r2 = results[501];
        Assert.AreEqual(0.8052, r2.MoneyFlowMultiplier.Round(4));
        Assert.AreEqual(118396116.25, r2.MoneyFlowVolume.Round(2));
        Assert.AreEqual(3439986548.42, r2.Adl.Round(2));
        Assert.IsNull(r2.AdlSma);
    }

    [TestMethod]
    public void WithSma()
    {
        List<AdlResult> results = quotes
            .GetAdl(20)
            .ToList();

        // proper quantities
        Assert.HasCount(502, results);
        Assert.AreEqual(483, results.Count(static x => x.AdlSma != null));

        // sample value
        AdlResult r = results[501];
        Assert.AreEqual(0.8052, r.MoneyFlowMultiplier.Round(4));
        Assert.AreEqual(118396116.25, r.MoneyFlowVolume.Round(2));
        Assert.AreEqual(3439986548.42, r.Adl.Round(2));
        Assert.AreEqual(3595352721.16, r.AdlSma.Round(2));
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
        Assert.HasCount(502, results);
        Assert.AreEqual(493, results.Count(static x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        List<AdlResult> r = badQuotes
            .GetAdl()
            .ToList();

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(static x => double.IsNaN(x.Adl)));
    }

    [TestMethod]
    public void BigData()
    {
        List<AdlResult> r = bigQuotes
            .GetAdl()
            .ToList();

        Assert.HasCount(1246, r);
    }

    [TestMethod]
    public void RandomData()
    {
        List<AdlResult> r = randomQuotes
            .GetAdl()
            .ToList();

        Assert.HasCount(1000, r);
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<AdlResult> r0 = noquotes
            .GetAdl()
            .ToList();

        Assert.IsEmpty(r0);

        List<AdlResult> r1 = onequote
            .GetAdl()
            .ToList();

        Assert.HasCount(1, r1);
    }

    // bad SMA period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => quotes.GetAdl(0));
}
