namespace Series;

[TestClass]
public class ObvTests : SeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        IReadOnlyList<ObvResult> results = Quotes
            .GetObv();

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
        IReadOnlyList<SmaResult> results = Quotes
            .GetObv()
            .GetSma(10);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(493, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<ObvResult> r = BadQuotes
            .GetObv();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => double.IsNaN(x.Obv)));
    }

    [TestMethod]
    public void BigData()
    {
        IReadOnlyList<ObvResult> r = BigQuotes
            .GetObv();

        Assert.AreEqual(1246, r.Count);
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<ObvResult> r0 = Noquotes
            .GetObv();

        Assert.AreEqual(0, r0.Count);

        IReadOnlyList<ObvResult> r1 = Onequote
            .GetObv();

        Assert.AreEqual(1, r1.Count);
    }
}
