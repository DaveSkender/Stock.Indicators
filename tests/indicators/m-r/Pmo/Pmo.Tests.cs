using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using Tests.Common;

namespace Tests.Indicators;

[TestClass]
public class PmoTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<PmoResult> results = quotes
            .GetPmo(35, 20, 10)
            .ToList();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(448, results.Count(x => x.Pmo != null));
        Assert.AreEqual(439, results.Count(x => x.Signal != null));

        // sample values
        PmoResult r1 = results[92];
        Assert.AreEqual(0.6159, r1.Pmo.Round(4));
        Assert.AreEqual(0.5582, r1.Signal.Round(4));

        PmoResult r2 = results[501];
        Assert.AreEqual(-2.7016, r2.Pmo.Round(4));
        Assert.AreEqual(-2.3117, r2.Signal.Round(4));
    }

    [TestMethod]
    public void UseTuple()
    {
        List<PmoResult> results = quotes
            .Use(CandlePart.Close)
            .GetPmo()
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(448, results.Count(x => x.Pmo != null));
    }

    [TestMethod]
    public void TupleNaN()
    {
        List<PmoResult> r = tupleNanny
            .GetPmo()
            .ToList();

        Assert.AreEqual(200, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Pmo is double and double.NaN));
    }

    [TestMethod]
    public void Chainee()
    {
        List<PmoResult> results = quotes
            .GetSma(2)
            .GetPmo()
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(447, results.Count(x => x.Pmo != null));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = quotes
            .GetPmo()
            .GetSma(10)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(439, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        List<PmoResult> r = badQuotes
            .GetPmo(25, 15, 5)
            .ToList();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Pmo is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<PmoResult> r0 = noquotes
            .GetPmo()
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<PmoResult> r1 = onequote
            .GetPmo()
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        List<PmoResult> results = quotes
            .GetPmo(35, 20, 10)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - (35 + 20 + 250), results.Count);

        PmoResult last = results.LastOrDefault();
        Assert.AreEqual(-2.7016, last.Pmo.Round(4));
        Assert.AreEqual(-2.3117, last.Signal.Round(4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad time period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetPmo(1));

        // bad smoothing period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetPmo(5, 0));

        // bad signal period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetPmo(5, 5, 0));
    }
}
