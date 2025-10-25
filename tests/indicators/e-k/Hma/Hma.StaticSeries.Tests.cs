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
        Assert.HasCount(502, results);
        Assert.HasCount(480, results.Where(static x => x.Hma != null));

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

        Assert.HasCount(502, results);
        Assert.HasCount(480, results.Where(static x => x.Hma != null));
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<HmaResult> results = Quotes
            .ToSma(2)
            .ToHma(19);

        Assert.HasCount(502, results);
        Assert.HasCount(480, results.Where(static x => x.Hma != null));
    }

    [TestMethod]
    public void Chainor()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToHma(20)
            .ToSma(10);

        Assert.HasCount(502, results);
        Assert.HasCount(471, results.Where(static x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<HmaResult> r = BadQuotes
            .ToHma(15);

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(static x => x.Hma is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<HmaResult> r0 = Noquotes
            .ToHma(5);

        Assert.IsEmpty(r0);

        IReadOnlyList<HmaResult> r1 = Onequote
            .ToHma(5);

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<HmaResult> results = Quotes
            .ToHma(20)
            .RemoveWarmupPeriods();

        // assertions
        Assert.HasCount(480, results);

        HmaResult last = results[^1];
        Assert.AreEqual(235.6972, last.Hma.Round(4));
    }

    /// <summary>
    /// bad lookback period
    /// </summary>
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToHma(1));
}
