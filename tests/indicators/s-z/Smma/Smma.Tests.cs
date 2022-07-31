using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class Smma : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<SmmaResult> results = quotes.GetSmma(20).ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
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
        IEnumerable<SmmaResult> results = quotes
            .Use(CandlePart.Close)
            .GetSmma(20);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(483, results.Count(x => x.Smma != null));
    }

    [TestMethod]
    public void TupleNaN()
    {
        IEnumerable<SmmaResult> r = tupleNanny.GetSmma(6);

        Assert.AreEqual(200, r.Count());
        Assert.AreEqual(0, r.Count(x => x.Smma is double and double.NaN));
    }

    [TestMethod]
    public void Chainee()
    {
        IEnumerable<SmmaResult> results = quotes
            .GetSma(2)
            .GetSmma(20);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(482, results.Count(x => x.Smma != null));
    }

    [TestMethod]
    public void Chainor()
    {
        IEnumerable<SmaResult> results = quotes
            .GetSmma(20)
            .GetSma(10);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(474, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<SmmaResult> r = Indicator.GetSmma(badQuotes, 15);
        Assert.AreEqual(502, r.Count());
        Assert.AreEqual(0, r.Count(x => x.Smma is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<SmmaResult> r0 = noquotes.GetSmma(5);
        Assert.AreEqual(0, r0.Count());

        IEnumerable<SmmaResult> r1 = onequote.GetSmma(5);
        Assert.AreEqual(1, r1.Count());
    }

    [TestMethod]
    public void Removed()
    {
        List<SmmaResult> results = quotes.GetSmma(20)
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
            => Indicator.GetSmma(quotes, 0));
}
