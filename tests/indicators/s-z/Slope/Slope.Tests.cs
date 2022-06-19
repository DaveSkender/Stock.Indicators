using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class Slope : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<SlopeResult> results = quotes.GetSlope(20).ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Where(x => x.Slope != null).Count());
        Assert.AreEqual(483, results.Where(x => x.StdDev != null).Count());
        Assert.AreEqual(20, results.Where(x => x.Line != null).Count());

        // sample values
        SlopeResult r1 = results[249];
        Assert.AreEqual(0.312406, NullMath.Round(r1.Slope, 6));
        Assert.AreEqual(180.4164, NullMath.Round(r1.Intercept, 4));
        Assert.AreEqual(0.8056, NullMath.Round(r1.RSquared, 4));
        Assert.AreEqual(2.0071, NullMath.Round(r1.StdDev, 4));
        Assert.AreEqual(null, r1.Line);

        SlopeResult r2 = results[482];
        Assert.AreEqual(-0.337015, NullMath.Round(r2.Slope, 6));
        Assert.AreEqual(425.1111, NullMath.Round(r2.Intercept, 4));
        Assert.AreEqual(0.1730, NullMath.Round(r2.RSquared, 4));
        Assert.AreEqual(4.6719, NullMath.Round(r2.StdDev, 4));
        Assert.AreEqual(267.9069m, NullMath.Round(r2.Line, 4));

        SlopeResult r3 = results[501];
        Assert.AreEqual(-1.689143, NullMath.Round(r3.Slope, 6));
        Assert.AreEqual(1083.7629, NullMath.Round(r3.Intercept, 4));
        Assert.AreEqual(0.7955, NullMath.Round(r3.RSquared, 4));
        Assert.AreEqual(10.9202, NullMath.Round(r3.StdDev, 4));
        Assert.AreEqual(235.8131m, NullMath.Round(r3.Line, 4));
    }

    [TestMethod]
    public void Use()
    {
        IEnumerable<SlopeResult> results = quotes
            .Use(CandlePart.Close)
            .GetSlope(20);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(483, results.Where(x => x.Slope != null).Count());
    }

    [TestMethod]
    public void Chained()
    {
        IEnumerable<SmaResult> results = quotes
            .GetSlope(20)
            .GetSma(10);

        Assert.AreEqual(483, results.Count());
        Assert.AreEqual(474, results.Where(x => x.Sma != null).Count());
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<SlopeResult> r = badQuotes.GetSlope(15);
        Assert.AreEqual(502, r.Count());
    }

    [TestMethod]
    public void BigData()
    {
        IEnumerable<SlopeResult> r = bigQuotes.GetSlope(250);
        Assert.AreEqual(1246, r.Count());
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<SlopeResult> r0 = noquotes.GetSlope(5);
        Assert.AreEqual(0, r0.Count());

        IEnumerable<SlopeResult> r1 = onequote.GetSlope(5);
        Assert.AreEqual(1, r1.Count());
    }

    [TestMethod]
    public void Removed()
    {
        List<SlopeResult> results = quotes.GetSlope(20)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - 19, results.Count);

        SlopeResult last = results.LastOrDefault();
        Assert.AreEqual(-1.689143, NullMath.Round(last.Slope, 6));
        Assert.AreEqual(1083.7629, NullMath.Round(last.Intercept, 4));
        Assert.AreEqual(0.7955, NullMath.Round(last.RSquared, 4));
        Assert.AreEqual(10.9202, NullMath.Round(last.StdDev, 4));
        Assert.AreEqual(235.8131m, NullMath.Round(last.Line, 4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetSlope(quotes, 0));
    }
}
