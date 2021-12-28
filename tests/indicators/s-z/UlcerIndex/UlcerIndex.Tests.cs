using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class UlcerIndex : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<UlcerIndexResult> results = quotes.GetUlcerIndex(14)
            .ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(489, results.Where(x => x.UI != null).Count());

        // sample value
        UlcerIndexResult r = results[501];
        Assert.AreEqual(5.7255, Math.Round((double)r.UI, 4));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<UlcerIndexResult> r = Indicator.GetUlcerIndex(badQuotes, 15);
        Assert.AreEqual(502, r.Count());
    }

    [TestMethod]
    public void Removed()
    {
        List<UlcerIndexResult> results = quotes.GetUlcerIndex(14)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - 13, results.Count);

        UlcerIndexResult last = results.LastOrDefault();
        Assert.AreEqual(5.7255, Math.Round((double)last.UI, 4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetUlcerIndex(quotes, 0));

        // insufficient quotes
        Assert.ThrowsException<BadQuotesException>(() =>
            Indicator.GetUlcerIndex(TestData.GetDefault(29), 30));
    }
}
