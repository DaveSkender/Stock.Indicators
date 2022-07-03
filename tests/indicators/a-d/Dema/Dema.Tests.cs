using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class Dema : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<DemaResult> results = quotes.GetDema(20)
            .ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.Dema != null));

        // sample values
        DemaResult r25 = results[25];
        Assert.AreEqual(215.7605, NullMath.Round(r25.Dema, 4));

        DemaResult r51 = results[51];
        Assert.AreEqual(225.8259, NullMath.Round(r51.Dema, 4));

        DemaResult r249 = results[249];
        Assert.AreEqual(258.4452, NullMath.Round(r249.Dema, 4));

        DemaResult r251 = results[501];
        Assert.AreEqual(241.1677, NullMath.Round(r251.Dema, 4));
    }

    [TestMethod]
    public void UseTuple()
    {
        IEnumerable<DemaResult> results = quotes
            .Use(CandlePart.Close)
            .GetDema(20);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(483, results.Count(x => x.Dema != null));
    }

    [TestMethod]
    public void TupleNaN()
    {
        IEnumerable<DemaResult> r = tupleNanny.GetDema(6);

        Assert.AreEqual(200, r.Count());
        Assert.AreEqual(0, r.Count(x => x.Dema is double and double.NaN));
    }

    [TestMethod]
    public void Chainee()
    {
        IEnumerable<DemaResult> results = quotes
            .GetSma(2)
            .GetDema(20);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(482, results.Count(x => x.Dema != null));
    }

    [TestMethod]
    public void Chainor()
    {
        IEnumerable<SmaResult> results = quotes
            .GetDema(20)
            .GetSma(10);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(474, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<DemaResult> r = Indicator.GetDema(badQuotes, 15);
        Assert.AreEqual(502, r.Count());
        Assert.AreEqual(0, r.Count(x => x.Dema is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<DemaResult> r0 = noquotes.GetDema(5);
        Assert.AreEqual(0, r0.Count());

        IEnumerable<DemaResult> r1 = onequote.GetDema(5);
        Assert.AreEqual(1, r1.Count());
    }

    [TestMethod]
    public void Removed()
    {
        List<DemaResult> results = quotes.GetDema(20)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - (40 + 100), results.Count);

        DemaResult last = results.LastOrDefault();
        Assert.AreEqual(241.1677, NullMath.Round(last.Dema, 4));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions() =>
        Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => quotes.GetDema(0));
}
