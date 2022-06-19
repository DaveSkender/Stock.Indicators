using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class Chop : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<ChopResult> results = quotes.GetChop(14)
            .ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(488, results.Where(x => x.Chop != null).Count());

        // sample values
        ChopResult r1 = results[13];
        Assert.AreEqual(null, r1.Chop);

        ChopResult r2 = results[14];
        Assert.AreEqual(69.9967, NullMath.Round(r2.Chop, 4));

        ChopResult r3 = results[249];
        Assert.AreEqual(41.8499, NullMath.Round(r3.Chop, 4));

        ChopResult r4 = results[501];
        Assert.AreEqual(38.6526, NullMath.Round(r4.Chop, 4));
    }

    [TestMethod]
    public void Chained()
    {
        IEnumerable<SmaResult> results = quotes
            .GetChop(14)
            .GetSma(10);

        Assert.AreEqual(488, results.Count());
        Assert.AreEqual(479, results.Where(x => x.Sma != null).Count());
    }

    [TestMethod]
    public void SmallLookback()
    {
        int lookbackPeriods = 2;
        List<ChopResult> results = Indicator.GetChop(quotes, lookbackPeriods).ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(500, results.Where(x => x.Chop != null).Count());
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<ChopResult> r = Indicator.GetChop(badQuotes, 20);
        Assert.AreEqual(502, r.Count());
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<ChopResult> r0 = noquotes.GetChop();
        Assert.AreEqual(0, r0.Count());

        IEnumerable<ChopResult> r1 = onequote.GetChop();
        Assert.AreEqual(1, r1.Count());
    }

    [TestMethod]
    public void Removed()
    {
        List<ChopResult> results = quotes.GetChop(14)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - 14, results.Count);

        ChopResult last = results.LastOrDefault();
        Assert.AreEqual(38.6526, NullMath.Round(last.Chop, 4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetChop(quotes, 1));
    }
}
