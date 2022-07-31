using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class Pmo : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<PmoResult> results = quotes.GetPmo(35, 20, 10).ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(448, results.Count(x => x.Pmo != null));
        Assert.AreEqual(439, results.Count(x => x.Signal != null));

        // sample values
        PmoResult r1 = results[92];
        Assert.AreEqual(0.6159, NullMath.Round(r1.Pmo, 4));
        Assert.AreEqual(0.5582, NullMath.Round(r1.Signal, 4));

        PmoResult r2 = results[501];
        Assert.AreEqual(-2.7016, NullMath.Round(r2.Pmo, 4));
        Assert.AreEqual(-2.3117, NullMath.Round(r2.Signal, 4));
    }

    [TestMethod]
    public void UseTuple()
    {
        IEnumerable<PmoResult> results = quotes
            .Use(CandlePart.Close)
            .GetPmo();

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(448, results.Count(x => x.Pmo != null));
    }

    [TestMethod]
    public void TupleNaN()
    {
        IEnumerable<PmoResult> r = tupleNanny.GetPmo();

        Assert.AreEqual(200, r.Count());
        Assert.AreEqual(0, r.Count(x => x.Pmo is double and double.NaN));
    }

    [TestMethod]
    public void Chainee()
    {
        IEnumerable<PmoResult> results = quotes
            .GetSma(2)
            .GetPmo();

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(447, results.Count(x => x.Pmo != null));
    }

    [TestMethod]
    public void Chainor()
    {
        IEnumerable<SmaResult> results = quotes
            .GetPmo()
            .GetSma(10);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(439, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<PmoResult> r = Indicator.GetPmo(badQuotes, 25, 15, 5);
        Assert.AreEqual(502, r.Count());
        Assert.AreEqual(0, r.Count(x => x.Pmo is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<PmoResult> r0 = noquotes.GetPmo();
        Assert.AreEqual(0, r0.Count());

        IEnumerable<PmoResult> r1 = onequote.GetPmo();
        Assert.AreEqual(1, r1.Count());
    }

    [TestMethod]
    public void Removed()
    {
        List<PmoResult> results = quotes.GetPmo(35, 20, 10)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - (35 + 20 + 250), results.Count);

        PmoResult last = results.LastOrDefault();
        Assert.AreEqual(-2.7016, NullMath.Round(last.Pmo, 4));
        Assert.AreEqual(-2.3117, NullMath.Round(last.Signal, 4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad time period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetPmo(quotes, 1));

        // bad smoothing period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetPmo(quotes, 5, 0));

        // bad signal period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetPmo(quotes, 5, 5, 0));
    }
}
