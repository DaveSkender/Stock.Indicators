namespace StaticSeries;

[TestClass]
public class CmoTests : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        IReadOnlyList<CmoResult> results = Quotes
            .GetCmo(14);

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(488, results.Count(x => x.Cmo != null));

        // sample values
        CmoResult r13 = results[13];
        Assert.IsNull(r13.Cmo);

        CmoResult r14 = results[14];
        Assert.AreEqual(24.1081, r14.Cmo.Round(4));

        CmoResult r249 = results[249];
        Assert.AreEqual(48.9614, r249.Cmo.Round(4));

        CmoResult r501 = results[501];
        Assert.AreEqual(-26.7502, r501.Cmo.Round(4));
    }

    [TestMethod]
    public void UseReusable()
    {
        IReadOnlyList<CmoResult> results = Quotes
            .Use(CandlePart.Close)
            .GetCmo(14);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(488, results.Count(x => x.Cmo != null));
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<CmoResult> results = Quotes
            .GetSma(2)
            .GetCmo(20);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(481, results.Count(x => x.Cmo != null));
    }

    [TestMethod]
    public void Chainor()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .GetCmo(20)
            .GetSma(10);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(473, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<CmoResult> r = BadQuotes
            .GetCmo(35);

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Cmo is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<CmoResult> r0 = Noquotes
            .GetCmo(5);

        Assert.AreEqual(0, r0.Count);

        IReadOnlyList<CmoResult> r1 = Onequote
            .GetCmo(5);

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<CmoResult> results = Quotes
            .GetCmo(14)
            .RemoveWarmupPeriods();

        // assertions
        Assert.AreEqual(488, results.Count);

        CmoResult last = results[^1];
        Assert.AreEqual(-26.7502, last.Cmo.Round(4));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => Quotes.GetCmo(0));
}
