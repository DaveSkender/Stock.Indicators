using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class AtrStop : TestBase
{
    [TestMethod]
    public void Standard()
    {
        int lookbackPeriods = 21;
        double multiplier = 3;

        List<AtrStopResult> results = quotes
            .GetAtrStop(lookbackPeriods, multiplier, EndType.Close)
            .ToList();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(481, results.Count(x => x.AtrStop != null));

        // sample values
        AtrStopResult r20 = results[20];
        Assert.AreEqual(null, r20.AtrStop);
        Assert.AreEqual(null, r20.BuyStop);
        Assert.AreEqual(null, r20.SellStop);

        AtrStopResult r21 = results[21];
        Assert.AreEqual(211.13m, NullMath.Round(r21.AtrStop, 4));
        Assert.AreEqual(null, r21.BuyStop);
        Assert.AreEqual(r21.AtrStop, r21.SellStop);

        AtrStopResult r151 = results[151];
        Assert.AreEqual(232.7861m, NullMath.Round(r151.AtrStop, 4));
        Assert.AreEqual(null, r151.BuyStop);
        Assert.AreEqual(r151.AtrStop, r151.SellStop);

        AtrStopResult r152 = results[152];
        Assert.AreEqual(236.3913m, NullMath.Round(r152.AtrStop, 4));
        Assert.AreEqual(r152.AtrStop, r152.BuyStop);
        Assert.AreEqual(null, r152.SellStop);

        AtrStopResult r249 = results[249];
        Assert.AreEqual(253.8863m, NullMath.Round(r249.AtrStop, 4));
        Assert.AreEqual(null, r249.BuyStop);
        Assert.AreEqual(r249.AtrStop, r249.SellStop);

        AtrStopResult r501 = results[501];
        Assert.AreEqual(246.3232m, NullMath.Round(r501.AtrStop, 4));
        Assert.AreEqual(r501.AtrStop, r501.BuyStop);
        Assert.AreEqual(null, r501.SellStop);
    }

    [TestMethod]
    public void HighLow()
    {
        int lookbackPeriods = 21;
        double multiplier = 3;

        List<AtrStopResult> results = quotes
            .GetAtrStop(lookbackPeriods, multiplier, EndType.HighLow)
            .ToList();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(481, results.Count(x => x.AtrStop != null));

        // sample values
        AtrStopResult r20 = results[20];
        Assert.AreEqual(null, r20.AtrStop);
        Assert.AreEqual(null, r20.BuyStop);
        Assert.AreEqual(null, r20.SellStop);

        AtrStopResult r21 = results[21];
        Assert.AreEqual(210.23m, NullMath.Round(r21.AtrStop, 4));
        Assert.AreEqual(null, r21.BuyStop);
        Assert.AreEqual(r21.AtrStop, r21.SellStop);

        AtrStopResult r69 = results[69];
        Assert.AreEqual(221.0594m, NullMath.Round(r69.AtrStop, 4));
        Assert.AreEqual(null, r69.BuyStop);
        Assert.AreEqual(r69.AtrStop, r69.SellStop);

        AtrStopResult r70 = results[70];
        Assert.AreEqual(226.4624m, NullMath.Round(r70.AtrStop, 4));
        Assert.AreEqual(r70.AtrStop, r70.BuyStop);
        Assert.AreEqual(null, r70.SellStop);

        AtrStopResult r249 = results[249];
        Assert.AreEqual(253.4863m, NullMath.Round(r249.AtrStop, 4));
        Assert.AreEqual(null, r249.BuyStop);
        Assert.AreEqual(r249.AtrStop, r249.SellStop);

        AtrStopResult r501 = results[501];
        Assert.AreEqual(252.6932m, NullMath.Round(r501.AtrStop, 4));
        Assert.AreEqual(r501.AtrStop, r501.BuyStop);
        Assert.AreEqual(null, r501.SellStop);
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<AtrStopResult> r = Indicator.GetAtrStop(badQuotes, 7);
        Assert.AreEqual(502, r.Count());
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<AtrStopResult> r0 = noquotes.GetAtrStop();
        Assert.AreEqual(0, r0.Count());

        IEnumerable<AtrStopResult> r1 = onequote.GetAtrStop();
        Assert.AreEqual(1, r1.Count());
    }

    [TestMethod]
    public void Condense()
    {
        int lookbackPeriods = 21;
        double multiplier = 3;

        List<AtrStopResult> results =
            quotes.GetAtrStop(lookbackPeriods, multiplier)
             .Condense()
             .ToList();

        // assertions
        Assert.AreEqual(481, results.Count);

        AtrStopResult last = results.LastOrDefault();
        Assert.AreEqual(246.3232m, NullMath.Round(last.AtrStop, 4));
        Assert.AreEqual(last.AtrStop, last.BuyStop);
        Assert.AreEqual(null, last.SellStop);
    }

    [TestMethod]
    public void Removed()
    {
        int lookbackPeriods = 21;
        double multiplier = 3;

        List<AtrStopResult> results =
            quotes.GetAtrStop(lookbackPeriods, multiplier)
             .RemoveWarmupPeriods()
             .ToList();

        // assertions
        Assert.AreEqual(481, results.Count);

        AtrStopResult last = results.LastOrDefault();
        Assert.AreEqual(246.3232m, NullMath.Round(last.AtrStop, 4));
        Assert.AreEqual(last.AtrStop, last.BuyStop);
        Assert.AreEqual(null, last.SellStop);
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => quotes.GetAtrStop(1));

        // bad multiplier
        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => quotes.GetAtrStop(7, 0));
    }
}
