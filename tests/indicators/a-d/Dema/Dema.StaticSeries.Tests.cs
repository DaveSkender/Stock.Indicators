namespace StaticSeries;

[TestClass]
public class Dema : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        IReadOnlyList<DemaResult> results = Quotes
            .GetDema(20);

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.Dema != null));

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
            .GetDema(20);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.Dema != null));
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<DemaResult> results = Quotes
            .ToSma(2)
            .GetDema(20);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(482, results.Count(x => x.Dema != null));
    }

    [TestMethod]
    public void Chainor()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .GetDema(20)
            .ToSma(10);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(474, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<DemaResult> r = BadQuotes
            .GetDema(15);

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Dema is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<DemaResult> r0 = Noquotes
            .GetDema(5);

        Assert.AreEqual(0, r0.Count);

        IReadOnlyList<DemaResult> r1 = Onequote
            .GetDema(5);

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<DemaResult> results = Quotes
            .GetDema(20)
            .RemoveWarmupPeriods();

        // assertions
        Assert.AreEqual(502 - (40 + 100), results.Count);

        DemaResult last = results[^1];
        Assert.AreEqual(241.1677, last.Dema.Round(4));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions() =>
        Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => Quotes.GetDema(0));
}
