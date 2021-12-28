using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class Prs : TestBase
{
    [TestMethod]
    public void Standard()
    {
        int lookbackPeriods = 30;
        int smaPeriods = 10;

        List<PrsResult> results =
            quotes.GetPrs(otherQuotes, lookbackPeriods, smaPeriods)
            .ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(502, results.Count(x => x.Prs != null));
        Assert.AreEqual(493, results.Where(x => x.PrsSma != null).Count());

        // sample values
        PrsResult r1 = results[8];
        Assert.AreEqual(1.108340, Math.Round((double)r1.Prs, 6));
        Assert.AreEqual(null, r1.PrsSma);
        Assert.AreEqual(null, r1.PrsPercent);

        PrsResult r2 = results[249];
        Assert.AreEqual(1.222373, Math.Round((double)r2.Prs, 6));
        Assert.AreEqual(1.275808, Math.Round((double)r2.PrsSma, 6));
        Assert.AreEqual(-0.023089, Math.Round((double)r2.PrsPercent, 6));

        PrsResult r3 = results[501];
        Assert.AreEqual(1.356817, Math.Round((double)r3.Prs, 6));
        Assert.AreEqual(1.343445, Math.Round((double)r3.PrsSma, 6));
        Assert.AreEqual(0.037082, Math.Round((double)r3.PrsPercent, 6));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<PrsResult> r = Indicator.GetPrs(badQuotes, badQuotes, 15, 4);
        Assert.AreEqual(502, r.Count());
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetPrs(quotes, otherQuotes, 0));

        // bad SMA period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetPrs(quotes, otherQuotes, 14, 0));

        // insufficient quotes
        Assert.ThrowsException<BadQuotesException>(() =>
            Indicator.GetPrs(quotes, TestData.GetCompare(13), 14));

        // insufficient eval quotes
        Assert.ThrowsException<BadQuotesException>(() =>
            Indicator.GetPrs(quotes, TestData.GetCompare(300), 14));

        // mismatch quotes
        Assert.ThrowsException<BadQuotesException>(() =>
            Indicator.GetPrs(mismatchQuotes, otherQuotes, 14));
    }
}
