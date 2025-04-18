namespace StaticSeries;

[TestClass]
public class Slope : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        IReadOnlyList<SlopeResult> results = Quotes
            .ToSlope(20);

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.Slope != null));
        Assert.AreEqual(483, results.Count(x => x.StdDev != null));
        Assert.AreEqual(20, results.Count(x => x.Line != null));

        // sample values
        SlopeResult r1 = results[249];
        Assert.AreEqual(0.312406, r1.Slope.Round(6));
        Assert.AreEqual(180.4164, r1.Intercept.Round(4));
        Assert.AreEqual(0.8056, r1.RSquared.Round(4));
        Assert.AreEqual(2.0071, r1.StdDev.Round(4));
        Assert.IsNull(r1.Line);

        SlopeResult r2 = results[482];
        Assert.AreEqual(-0.337015, r2.Slope.Round(6));
        Assert.AreEqual(425.1111, r2.Intercept.Round(4));
        Assert.AreEqual(0.1730, r2.RSquared.Round(4));
        Assert.AreEqual(4.6719, r2.StdDev.Round(4));
        Assert.AreEqual(267.9069m, r2.Line.Round(4));

        SlopeResult r3 = results[501];
        Assert.AreEqual(-1.689143, r3.Slope.Round(6));
        Assert.AreEqual(1083.7629, r3.Intercept.Round(4));
        Assert.AreEqual(0.7955, r3.RSquared.Round(4));
        Assert.AreEqual(10.9202, r3.StdDev.Round(4));
        Assert.AreEqual(235.8131m, r3.Line.Round(4));
    }

    [TestMethod]
    public void UseReusable()
    {
        IReadOnlyList<SlopeResult> results = Quotes
            .Use(CandlePart.Close)
            .ToSlope(20);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.Slope != null));
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<SlopeResult> results = Quotes
            .ToSma(2)
            .ToSlope(20);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(482, results.Count(x => x.Slope != null));
    }

    [TestMethod]
    public void Chainor()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToSlope(20)
            .ToSma(10);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(474, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<SlopeResult> r = BadQuotes
            .ToSlope(15);

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Slope is double.NaN));
    }

    [TestMethod]
    public void BigData()
    {
        IReadOnlyList<SlopeResult> r = BigQuotes
            .ToSlope(250);

        Assert.AreEqual(1246, r.Count);
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<SlopeResult> r0 = Noquotes
            .ToSlope(5);

        Assert.AreEqual(0, r0.Count);

        IReadOnlyList<SlopeResult> r1 = Onequote
            .ToSlope(5);

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<SlopeResult> results = Quotes
            .ToSlope(20)
            .RemoveWarmupPeriods();

        // assertions
        Assert.AreEqual(502 - 19, results.Count);

        SlopeResult last = results[^1];
        Assert.AreEqual(-1.689143, last.Slope.Round(6));
        Assert.AreEqual(1083.7629, last.Intercept.Round(4));
        Assert.AreEqual(0.7955, last.RSquared.Round(4));
        Assert.AreEqual(10.9202, last.StdDev.Round(4));
        Assert.AreEqual(235.8131m, last.Line.Round(4));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsExactly<ArgumentOutOfRangeException>(()
            => Quotes.ToSlope(1));
}
