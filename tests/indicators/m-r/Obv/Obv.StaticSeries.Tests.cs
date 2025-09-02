namespace StaticSeries;

[TestClass]
public class Obv : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        IReadOnlyList<ObvResult> results = Quotes
            .ToObv();

        // proper quantities
        Assert.HasCount(502, results);

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
            .ToObv()
            .ToSma(10);

        Assert.HasCount(502, results);
        Assert.HasCount(493, results.Where(x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<ObvResult> r = BadQuotes
            .ToObv();

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(x => double.IsNaN(x.Obv)));
    }

    [TestMethod]
    public void BigData()
    {
        IReadOnlyList<ObvResult> r = BigQuotes
            .ToObv();

        Assert.HasCount(1246, r);
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<ObvResult> r0 = Noquotes
            .ToObv();

        Assert.IsEmpty(r0);

        IReadOnlyList<ObvResult> r1 = Onequote
            .ToObv();

        Assert.HasCount(1, r1);
    }
}
