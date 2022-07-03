using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class Hma : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<HmaResult> results = quotes.GetHma(20).ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(480, results.Count(x => x.Hma != null));

        // sample values
        HmaResult r1 = results[149];
        Assert.AreEqual(236.0835, NullMath.Round(r1.Hma, 4));

        HmaResult r2 = results[501];
        Assert.AreEqual(235.6972, NullMath.Round(r2.Hma, 4));
    }

    [TestMethod]
    public void UseTuple()
    {
        IEnumerable<HmaResult> results = quotes
            .Use(CandlePart.Close)
            .GetHma(20);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(480, results.Count(x => x.Hma != null));
    }

    [TestMethod]
    public void TupleNaN()
    {
        IEnumerable<HmaResult> r = tupleNanny.GetHma(6);

        Assert.AreEqual(200, r.Count());
        Assert.AreEqual(0, r.Count(x => x.Hma is double and double.NaN));
    }

    [TestMethod]
    public void Chainee()
    {
        IEnumerable<HmaResult> results = quotes
            .GetSma(2)
            .GetHma(19);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(480, results.Count(x => x.Hma != null));
    }

    [TestMethod]
    public void Chainor()
    {
        IEnumerable<SmaResult> results = quotes
            .GetHma(20)
            .GetSma(10);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(471, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<HmaResult> r = Indicator.GetHma(badQuotes, 15);
        Assert.AreEqual(502, r.Count());
        Assert.AreEqual(0, r.Count(x => x.Hma is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<HmaResult> r0 = noquotes.GetHma(5);
        Assert.AreEqual(0, r0.Count());

        IEnumerable<HmaResult> r1 = onequote.GetHma(5);
        Assert.AreEqual(1, r1.Count());
    }

    [TestMethod]
    public void Removed()
    {
        List<HmaResult> results = quotes.GetHma(20)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(480, results.Count);

        HmaResult last = results.LastOrDefault();
        Assert.AreEqual(235.6972, NullMath.Round(last.Hma, 4));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => quotes.GetHma(1));
}
