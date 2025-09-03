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
        Assert.HasCount(502, results);
        Assert.HasCount(483, results.Where(x => x.Cci != null));

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

        Assert.HasCount(502, results);
        Assert.HasCount(474, results.Where(x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<CciResult> r = BadQuotes
            .ToCci(15);

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(x => x.Cci is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<CciResult> r0 = Noquotes
            .ToCci();

        Assert.IsEmpty(r0);

        IReadOnlyList<CciResult> r1 = Onequote
            .ToCci();

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<CciResult> results = Quotes
            .ToCci()
            .RemoveWarmupPeriods();

        // assertions
        Assert.HasCount(502 - 19, results);

        CciResult last = results[^1];
        Assert.AreEqual(-52.9946, last.Cci.Round(4));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => Quotes.ToCci(0));
}
