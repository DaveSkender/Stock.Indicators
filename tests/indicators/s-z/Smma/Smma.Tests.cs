using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using Tests.Common;

namespace Tests.Indicators;

[TestClass]
public class SmmaTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<SmmaResult> results = quotes
            .GetSmma(20)
            .ToList();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.Smma != null));

        // starting calculations at proper index
        Assert.IsNull(results[18].Smma);
        Assert.IsNotNull(results[19].Smma);

        // sample values
        Assert.AreEqual(214.52500, Math.Round(results[19].Smma.Value, 5));
        Assert.AreEqual(214.55125, Math.Round(results[20].Smma.Value, 5));
        Assert.AreEqual(214.58319, Math.Round(results[21].Smma.Value, 5));
        Assert.AreEqual(225.78071, Math.Round(results[100].Smma.Value, 5));
        Assert.AreEqual(255.67462, Math.Round(results[501].Smma.Value, 5));
    }

    [TestMethod]
    public void UseTuple()
    {
        List<SmmaResult> results = quotes
            .Use(CandlePart.Close)
            .GetSmma(20)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.Smma != null));
    }

    [TestMethod]
    public void TupleNaN()
    {
        List<SmmaResult> r = tupleNanny
            .GetSmma(6)
            .ToList();

        Assert.AreEqual(200, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Smma is double and double.NaN));
    }

    [TestMethod]
    public void Chainee()
    {
        List<SmmaResult> results = quotes
            .GetSma(2)
            .GetSmma(20)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(482, results.Count(x => x.Smma != null));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = quotes
            .GetSmma(20)
            .GetSma(10)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(474, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        List<SmmaResult> r = badQuotes
            .GetSmma(15)
            .ToList();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Smma is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<SmmaResult> r0 = noquotes
            .GetSmma(5)
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<SmmaResult> r1 = onequote
            .GetSmma(5)
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        List<SmmaResult> results = quotes
            .GetSmma(20)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - (20 + 100), results.Count);
        Assert.AreEqual(255.67462, Math.Round(results.LastOrDefault().Smma.Value, 5));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => quotes.GetSmma(0));
}
