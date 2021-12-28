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
        Assert.AreEqual(448, results.Where(x => x.Pmo != null).Count());
        Assert.AreEqual(439, results.Where(x => x.Signal != null).Count());

        // sample values
        PmoResult r1 = results[92];
        Assert.AreEqual(0.6159, Math.Round((double)r1.Pmo, 4));
        Assert.AreEqual(0.5582, Math.Round((double)r1.Signal, 4));

        PmoResult r2 = results[501];
        Assert.AreEqual(-2.7016, Math.Round((double)r2.Pmo, 4));
        Assert.AreEqual(-2.3117, Math.Round((double)r2.Signal, 4));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<PmoResult> r = Indicator.GetPmo(badQuotes, 25, 15, 5);
        Assert.AreEqual(502, r.Count());
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
        Assert.AreEqual(-2.7016, Math.Round((double)last.Pmo, 4));
        Assert.AreEqual(-2.3117, Math.Round((double)last.Signal, 4));
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

        // insufficient quotes
        Assert.ThrowsException<BadQuotesException>(() =>
            Indicator.GetPmo(TestData.GetDefault(54), 35, 20, 10));
    }
}
