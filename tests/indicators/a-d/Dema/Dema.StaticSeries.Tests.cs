namespace StaticSeries;

[TestClass]
public class Dema : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        IReadOnlyList<DemaResult> results = Quotes
            .ToDema(20);

        // proper quantities
        Assert.HasCount(502, results);
        Assert.HasCount(483, results.Where(x => x.Dema != null));

        // sample values
        DemaResult r25 = results[25];
        Assert.AreEqual(215.7605, r25.Dema.Round(4));

        DemaResult r51 = results[51];
        Assert.AreEqual(225.8259, r51.Dema.Round(4));

        DemaResult r249 = results[249];
        Assert.AreEqual(258.4452, r249.Dema.Round(4));

        DemaResult r251 = results[501];
        Assert.AreEqual(241.1677, r251.Dema.Round(4));
    }

    [TestMethod]
    public void UseReusable()
    {
        IReadOnlyList<DemaResult> results = Quotes
            .Use(CandlePart.Close)
            .ToDema(20);

        Assert.HasCount(502, results);
        Assert.HasCount(483, results.Where(x => x.Dema != null));
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<DemaResult> results = Quotes
            .ToSma(2)
            .ToDema(20);

        Assert.HasCount(502, results);
        Assert.HasCount(482, results.Where(x => x.Dema != null));
    }

    [TestMethod]
    public void Chainor()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToDema(20)
            .ToSma(10);

        Assert.HasCount(502, results);
        Assert.HasCount(474, results.Where(x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<DemaResult> r = BadQuotes
            .ToDema(15);

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(x => x.Dema is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<DemaResult> r0 = Noquotes
            .ToDema(5);

        Assert.IsEmpty(r0);

        IReadOnlyList<DemaResult> r1 = Onequote
            .ToDema(5);

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<DemaResult> results = Quotes
            .ToDema(20)
            .RemoveWarmupPeriods();

        // assertions
        Assert.HasCount(502 - (40 + 100), results);

        DemaResult last = results[^1];
        Assert.AreEqual(241.1677, last.Dema.Round(4));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions() =>
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => Quotes.ToDema(0));
}
