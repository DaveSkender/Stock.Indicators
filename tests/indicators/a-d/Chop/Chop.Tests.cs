using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using Tests.Common;

namespace Tests.Indicators;

[TestClass]
public class ChopTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<ChopResult> results = quotes
            .GetChop(14)
            .ToList();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(488, results.Count(x => x.Chop != null));

        // sample values
        ChopResult r1 = results[13];
        Assert.AreEqual(null, r1.Chop);

        ChopResult r2 = results[14];
        Assert.AreEqual(69.9967, r2.Chop.Round(4));

        ChopResult r3 = results[249];
        Assert.AreEqual(41.8499, r3.Chop.Round(4));

        ChopResult r4 = results[501];
        Assert.AreEqual(38.6526, r4.Chop.Round(4));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = quotes
            .GetChop(14)
            .GetSma(10)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(479, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void SmallLookback()
    {
        int lookbackPeriods = 2;
        List<ChopResult> results = quotes
            .GetChop(lookbackPeriods)
            .ToList();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(500, results.Count(x => x.Chop != null));
    }

    [TestMethod]
    public void BadData()
    {
        List<ChopResult> r = badQuotes
            .GetChop(20)
            .ToList();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Chop is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<ChopResult> r0 = noquotes
            .GetChop()
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<ChopResult> r1 = onequote
            .GetChop()
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        List<ChopResult> results = quotes
            .GetChop(14)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - 14, results.Count);

        ChopResult last = results.LastOrDefault();
        Assert.AreEqual(38.6526, last.Chop.Round(4));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => quotes.GetChop(1));
}
