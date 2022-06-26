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
        Assert.AreEqual(483, results.Where(x => x.Smma != null).Count());

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
        Assert.AreEqual(483, results.Where(x => x.Smma != null).Count());
    }

    [TestMethod]
    public void Chained()
    {
        IEnumerable<SmaResult> results = quotes
            .GetSmma(20)
            .GetSma(10);

        Assert.AreEqual(483, results.Count());
        Assert.AreEqual(474, results.Where(x => x.Sma != null).Count());
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<SmmaResult> r = Indicator.GetSmma(badQuotes, 15);
        Assert.AreEqual(502, r.Count());
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

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetSmma(quotes, 0));
    }
}
