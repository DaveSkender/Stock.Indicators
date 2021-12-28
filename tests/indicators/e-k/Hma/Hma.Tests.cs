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
        Assert.AreEqual(480, results.Where(x => x.Hma != null).Count());

        // sample values
        HmaResult r1 = results[149];
        Assert.AreEqual(236.0835m, Math.Round((decimal)r1.Hma, 4));

        HmaResult r2 = results[501];
        Assert.AreEqual(235.6972m, Math.Round((decimal)r2.Hma, 4));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<HmaResult> r = Indicator.GetHma(badQuotes, 15);
        Assert.AreEqual(502, r.Count());
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
        Assert.AreEqual(235.6972m, Math.Round((decimal)last.Hma, 4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetHma(1));

        // insufficient quotes
        Assert.ThrowsException<BadQuotesException>(() =>
            Indicator.GetHma(TestData.GetDefault(10), 9));
    }
}
