using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using Tests.Common;

namespace Tests.Indicators;

[TestClass]
public class DemaTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<DemaResult> results = quotes
            .GetDema(20)
            .ToList();

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
    public void UseTuple()
    {
        List<DemaResult> results = quotes
            .Use(CandlePart.Close)
            .GetDema(20)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.Dema != null));
    }

    [TestMethod]
    public void TupleNaN()
    {
        List<DemaResult> r = tupleNanny
            .GetDema(6)
            .ToList();

        Assert.AreEqual(200, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Dema is double and double.NaN));
    }

    [TestMethod]
    public void Chainee()
    {
        List<DemaResult> results = quotes
            .GetSma(2)
            .GetDema(20)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(482, results.Count(x => x.Dema != null));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = quotes
            .GetDema(20)
            .GetSma(10)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(474, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        List<DemaResult> r = badQuotes
            .GetDema(15)
            .ToList();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Dema is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<DemaResult> r0 = noquotes
            .GetDema(5)
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<DemaResult> r1 = onequote
            .GetDema(5)
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        List<DemaResult> results = quotes
            .GetDema(20)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - (40 + 100), results.Count);

        DemaResult last = results.LastOrDefault();
        Assert.AreEqual(241.1677, last.Dema.Round(4));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions() =>
        Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => quotes.GetDema(0));
}
