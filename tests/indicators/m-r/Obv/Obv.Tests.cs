namespace Tests.Indicators.Series;

[TestClass]
public class ObvTests : SeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        List<ObvResult> results = Quotes
            .GetObv()
            .ToList();

        // proper quantities
        Assert.AreEqual(502, results.Count);

        // sample values
        ObvResult r1 = results[249];
        Assert.AreEqual(1780918888, r1.Obv);

        ObvResult r2 = results[501];
        Assert.AreEqual(539843504, r2.Obv);
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = Quotes
            .GetObv()
            .GetSma(10)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(493, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        List<ObvResult> r = BadQuotes
            .GetObv()
            .ToList();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => double.IsNaN(x.Obv)));
    }

    [TestMethod]
    public void BigData()
    {
        List<ObvResult> r = BigQuotes
            .GetObv()
            .ToList();

        Assert.AreEqual(1246, r.Count);
    }

    [TestMethod]
    public override void NoQuotes()
    {
        List<ObvResult> r0 = Noquotes
            .GetObv()
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<ObvResult> r1 = Onequote
            .GetObv()
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }
}
