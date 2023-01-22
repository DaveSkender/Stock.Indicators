using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using Tests.Common;

namespace Tests.Indicators;

[TestClass]
public class SuperTrendTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        int lookbackPeriods = 14;
        double multiplier = 3;

        List<SuperTrendResult> results = quotes
            .GetSuperTrend(lookbackPeriods, multiplier)
            .ToList();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(488, results.Count(x => x.SuperTrend != null));

        // sample values
        SuperTrendResult r13 = results[13];
        Assert.AreEqual(null, r13.SuperTrend);
        Assert.AreEqual(null, r13.UpperBand);
        Assert.AreEqual(null, r13.LowerBand);

        SuperTrendResult r14 = results[14];
        Assert.AreEqual(210.6157m, r14.SuperTrend.Round(4));
        Assert.AreEqual(null, r14.UpperBand);
        Assert.AreEqual(r14.SuperTrend, r14.LowerBand);

        SuperTrendResult r151 = results[151];
        Assert.AreEqual(232.8520m, r151.SuperTrend.Round(4));
        Assert.AreEqual(null, r151.UpperBand);
        Assert.AreEqual(r151.SuperTrend, r151.LowerBand);

        SuperTrendResult r152 = results[152];
        Assert.AreEqual(237.6436m, r152.SuperTrend.Round(4));
        Assert.AreEqual(r152.SuperTrend, r152.UpperBand);
        Assert.AreEqual(null, r152.LowerBand);

        SuperTrendResult r249 = results[249];
        Assert.AreEqual(253.8008m, r249.SuperTrend.Round(4));
        Assert.AreEqual(null, r249.UpperBand);
        Assert.AreEqual(r249.SuperTrend, r249.LowerBand);

        SuperTrendResult r501 = results[501];
        Assert.AreEqual(250.7954m, r501.SuperTrend.Round(4));
        Assert.AreEqual(r501.SuperTrend, r501.UpperBand);
        Assert.AreEqual(null, r501.LowerBand);
    }

    [TestMethod]
    public void Bitcoin()
    {
        IEnumerable<Quote> h = TestData.GetBitcoin();

        List<SuperTrendResult> results = h
            .GetSuperTrend(10, 3)
            .ToList();

        Assert.AreEqual(1246, results.Count);

        SuperTrendResult r = results[1208];
        Assert.AreEqual(16242.2704m, r.LowerBand.Round(4));
    }

    [TestMethod]
    public void BadData()
    {
        List<SuperTrendResult> r = badQuotes
            .GetSuperTrend(7)
            .ToList();

        Assert.AreEqual(502, r.Count);
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<SuperTrendResult> r0 = noquotes
            .GetSuperTrend()
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<SuperTrendResult> r1 = onequote
            .GetSuperTrend()
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Condense()
    {
        int lookbackPeriods = 14;
        double multiplier = 3;

        List<SuperTrendResult> results = quotes
            .GetSuperTrend(lookbackPeriods, multiplier)
            .Condense()
            .ToList();

        // assertions
        Assert.AreEqual(488, results.Count);

        SuperTrendResult last = results.LastOrDefault();
        Assert.AreEqual(250.7954m, last.SuperTrend.Round(4));
        Assert.AreEqual(last.SuperTrend, last.UpperBand);
        Assert.AreEqual(null, last.LowerBand);
    }

    [TestMethod]
    public void Removed()
    {
        int lookbackPeriods = 14;
        double multiplier = 3;

        List<SuperTrendResult> results = quotes
            .GetSuperTrend(lookbackPeriods, multiplier)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(488, results.Count);

        SuperTrendResult last = results.LastOrDefault();
        Assert.AreEqual(250.7954m, last.SuperTrend.Round(4));
        Assert.AreEqual(last.SuperTrend, last.UpperBand);
        Assert.AreEqual(null, last.LowerBand);
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetSuperTrend(1));

        // bad multiplier
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetSuperTrend(7, 0));
    }
}
