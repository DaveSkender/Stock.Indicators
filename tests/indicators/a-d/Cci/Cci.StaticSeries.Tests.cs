namespace StaticSeries;

[TestClass]
public class Cci : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        IReadOnlyList<CciResult> results = Quotes
            .ToCci();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.Cci != null));

        // sample value
        CciResult r = results[501];
        Assert.AreEqual(-52.9946, r.Cci.Round(4));
    }

    [TestMethod]
    public void Chainor()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToCci()
            .ToSma(10);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(474, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<CciResult> r = BadQuotes
            .ToCci(15);

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Cci is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<CciResult> r0 = Noquotes
            .ToCci();

        Assert.AreEqual(0, r0.Count);

        IReadOnlyList<CciResult> r1 = Onequote
            .ToCci();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<CciResult> results = Quotes
            .ToCci()
            .RemoveWarmupPeriods();

        // assertions
        Assert.AreEqual(502 - 19, results.Count);

        CciResult last = results[^1];
        Assert.AreEqual(-52.9946, last.Cci.Round(4));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(
            () => Quotes.ToCci(0));
}
