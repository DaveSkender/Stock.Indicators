namespace Tests.Indicators;

[TestClass]
public class SlopeTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<SlopeResult> results = quotes
            .GetSlope(20)
            .ToList();

        // proper quantities
        Assert.HasCount(502, results);
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
    public void UseTuple()
    {
        List<SlopeResult> results = quotes
            .Use(CandlePart.Close)
            .GetSlope(20)
            .ToList();

        Assert.HasCount(502, results);
        Assert.AreEqual(483, results.Count(x => x.Slope != null));
    }

    [TestMethod]
    public void TupleNaN()
    {
        List<SlopeResult> r = tupleNanny
            .GetSlope(6)
            .ToList();

        Assert.HasCount(200, r);
        Assert.AreEqual(0, r.Count(x => x.Slope is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public void Chainee()
    {
        List<SlopeResult> results = quotes
            .GetSma(2)
            .GetSlope(20)
            .ToList();

        Assert.HasCount(502, results);
        Assert.AreEqual(482, results.Count(x => x.Slope != null));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = quotes
            .GetSlope(20)
            .GetSma(10)
            .ToList();

        Assert.HasCount(502, results);
        Assert.AreEqual(474, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        List<SlopeResult> r = badQuotes
            .GetSlope(15)
            .ToList();

        Assert.HasCount(502, r);
        Assert.AreEqual(0, r.Count(x => x.Slope is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public void BigData()
    {
        List<SlopeResult> r = bigQuotes
            .GetSlope(250)
            .ToList();

        Assert.HasCount(1246, r);
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<SlopeResult> r0 = noquotes
            .GetSlope(5)
            .ToList();

        Assert.IsEmpty(r0);

        List<SlopeResult> r1 = onequote
            .GetSlope(5)
            .ToList();

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Removed()
    {
        List<SlopeResult> results = quotes
            .GetSlope(20)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.HasCount(502 - 19, results);

        SlopeResult last = results.LastOrDefault();
        Assert.AreEqual(-1.689143, last.Slope.Round(6));
        Assert.AreEqual(1083.7629, last.Intercept.Round(4));
        Assert.AreEqual(0.7955, last.RSquared.Round(4));
        Assert.AreEqual(10.9202, last.StdDev.Round(4));
        Assert.AreEqual(235.8131m, last.Line.Round(4));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => quotes.GetSlope(1));
}
