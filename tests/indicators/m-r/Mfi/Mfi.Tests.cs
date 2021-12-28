using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class Mfi : TestBase
{

    [TestMethod]
    public void Standard()
    {
        int lookbackPeriods = 14;
        List<MfiResult> results = quotes.GetMfi(lookbackPeriods).ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(488, results.Where(x => x.Mfi != null).Count());

        // sample values
        MfiResult r1 = results[439];
        Assert.AreEqual(69.0622m, Math.Round((decimal)r1.Mfi, 4));

        MfiResult r2 = results[501];
        Assert.AreEqual(39.9494m, Math.Round((decimal)r2.Mfi, 4));
    }

    [TestMethod]
    public void SmallLookback()
    {
        int lookbackPeriods = 4;

        List<MfiResult> results = Indicator.GetMfi(quotes, lookbackPeriods)
            .ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(498, results.Where(x => x.Mfi != null).Count());

        // sample values
        MfiResult r1 = results[31];
        Assert.AreEqual(100m, Math.Round((decimal)r1.Mfi, 4));

        MfiResult r2 = results[43];
        Assert.AreEqual(0m, Math.Round((decimal)r2.Mfi, 4));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<MfiResult> r = Indicator.GetMfi(badQuotes, 15);
        Assert.AreEqual(502, r.Count());
    }

    [TestMethod]
    public void Removed()
    {
        int lookbackPeriods = 14;
        List<MfiResult> results = quotes.GetMfi(lookbackPeriods)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - 14, results.Count);

        MfiResult last = results.LastOrDefault();
        Assert.AreEqual(39.9494m, Math.Round((decimal)last.Mfi, 4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetMfi(quotes, 1));

        // insufficient quotes
        Assert.ThrowsException<BadQuotesException>(() =>
            Indicator.GetMfi(TestData.GetDefault(14), 14));
    }
}
