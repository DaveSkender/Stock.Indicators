namespace StaticSeries;

[TestClass]
public class Hma : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        IReadOnlyList<HmaResult> results = Quotes
            .ToHma(20);

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(480, results.Count(x => x.Hma != null));

        // sample values
        HmaResult r1 = results[149];
        Assert.AreEqual(236.0835, r1.Hma.Round(4));

        HmaResult r2 = results[501];
        Assert.AreEqual(235.6972, r2.Hma.Round(4));
    }

    [TestMethod]
    public void UseReusable()
    {
        IReadOnlyList<HmaResult> results = Quotes
            .Use(CandlePart.Close)
            .ToHma(20);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(480, results.Count(x => x.Hma != null));
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<HmaResult> results = Quotes
            .ToSma(2)
            .ToHma(19);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(480, results.Count(x => x.Hma != null));
    }

    [TestMethod]
    public void Chainor()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToHma(20)
            .ToSma(10);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(471, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<HmaResult> r = BadQuotes
            .ToHma(15);

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Hma is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<HmaResult> r0 = Noquotes
            .ToHma(5);

        Assert.AreEqual(0, r0.Count);

        IReadOnlyList<HmaResult> r1 = Onequote
            .ToHma(5);

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<HmaResult> results = Quotes
            .ToHma(20)
            .RemoveWarmupPeriods();

        // assertions
        Assert.AreEqual(480, results.Count);

        HmaResult last = results[^1];
        Assert.AreEqual(235.6972, last.Hma.Round(4));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => Quotes.ToHma(1));
}
