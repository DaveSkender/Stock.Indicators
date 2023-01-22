using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using Tests.Common;

namespace Tests.Indicators;

[TestClass]
public class StdDevChannelsTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        int lookbackPeriods = 20;
        double standardDeviations = 2;

        List<StdDevChannelsResult> results =
            quotes.GetStdDevChannels(lookbackPeriods, standardDeviations)
            .ToList();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(500, results.Count(x => x.Centerline != null));
        Assert.AreEqual(500, results.Count(x => x.UpperChannel != null));
        Assert.AreEqual(500, results.Count(x => x.LowerChannel != null));

        // sample value
        StdDevChannelsResult r1 = results[1];
        Assert.IsNull(r1.Centerline);
        Assert.IsNull(r1.UpperChannel);
        Assert.IsNull(r1.LowerChannel);
        Assert.IsFalse(r1.BreakPoint);

        StdDevChannelsResult r2 = results[2];
        Assert.AreEqual(213.7993, r2.Centerline.Round(4));
        Assert.AreEqual(215.7098, r2.UpperChannel.Round(4));
        Assert.AreEqual(211.8888, r2.LowerChannel.Round(4));
        Assert.IsTrue(r2.BreakPoint);

        StdDevChannelsResult r3 = results[141];
        Assert.AreEqual(236.1744, r3.Centerline.Round(4));
        Assert.AreEqual(240.4784, r3.UpperChannel.Round(4));
        Assert.AreEqual(231.8704, r3.LowerChannel.Round(4));
        Assert.IsFalse(r3.BreakPoint);

        StdDevChannelsResult r4 = results[142];
        Assert.AreEqual(236.3269, r4.Centerline.Round(4));
        Assert.AreEqual(239.5585, r4.UpperChannel.Round(4));
        Assert.AreEqual(233.0953, r4.LowerChannel.Round(4));
        Assert.IsTrue(r4.BreakPoint);

        StdDevChannelsResult r5 = results[249];
        Assert.AreEqual(259.6044, r5.Centerline.Round(4));
        Assert.AreEqual(267.5754, r5.UpperChannel.Round(4));
        Assert.AreEqual(251.6333, r5.LowerChannel.Round(4));
        Assert.IsFalse(r5.BreakPoint);

        StdDevChannelsResult r6 = results[482];
        Assert.AreEqual(267.9069, r6.Centerline.Round(4));
        Assert.AreEqual(289.7473, r6.UpperChannel.Round(4));
        Assert.AreEqual(246.0664, r6.LowerChannel.Round(4));
        Assert.IsTrue(r6.BreakPoint);

        StdDevChannelsResult r7 = results[501];
        Assert.AreEqual(235.8131, r7.Centerline.Round(4));
        Assert.AreEqual(257.6536, r7.UpperChannel.Round(4));
        Assert.AreEqual(213.9727, r7.LowerChannel.Round(4));
        Assert.IsFalse(r7.BreakPoint);
    }

    [TestMethod]
    public void FullHistory()
    {
        // null provided for lookback period

        List<StdDevChannelsResult> results =
            quotes.GetStdDevChannels(null, 2)
            .ToList();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(502, results.Count(x => x.Centerline != null));
        Assert.AreEqual(502, results.Count(x => x.UpperChannel != null));
        Assert.AreEqual(502, results.Count(x => x.LowerChannel != null));
        Assert.AreEqual(501, results.Count(x => x.BreakPoint == false));

        // sample value
        StdDevChannelsResult r1 = results[0];
        Assert.AreEqual(219.2605, r1.Centerline.Round(4));
        Assert.AreEqual(258.7104, r1.UpperChannel.Round(4));
        Assert.AreEqual(179.8105, r1.LowerChannel.Round(4));
        Assert.IsTrue(r1.BreakPoint);

        StdDevChannelsResult r2 = results[249];
        Assert.AreEqual(249.3814, r2.Centerline.Round(4));
        Assert.AreEqual(288.8314, r2.UpperChannel.Round(4));
        Assert.AreEqual(209.9315, r2.LowerChannel.Round(4));

        StdDevChannelsResult r3 = results[501];
        Assert.AreEqual(279.8653, r3.Centerline.Round(4));
        Assert.AreEqual(319.3152, r3.UpperChannel.Round(4));
        Assert.AreEqual(240.4153, r3.LowerChannel.Round(4));
    }

    [TestMethod]
    public void UseTuple()
    {
        List<StdDevChannelsResult> results = quotes
            .Use(CandlePart.Close)
            .GetStdDevChannels(20, 2)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(500, results.Count(x => x.Centerline != null));
    }

    [TestMethod]
    public void TupleNaN()
    {
        List<StdDevChannelsResult> r = tupleNanny
            .GetStdDevChannels(6, 1.1)
            .ToList();

        Assert.AreEqual(200, r.Count);
        Assert.AreEqual(0, r.Count(x => x.UpperChannel is double and double.NaN));
    }

    [TestMethod]
    public void Chainee()
    {
        List<StdDevChannelsResult> results = quotes
            .GetSma(2)
            .GetStdDevChannels(20, 2)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(500, results.Count(x => x.Centerline != null));
    }

    [TestMethod]
    public void BadData()
    {
        List<StdDevChannelsResult> r = badQuotes
            .GetStdDevChannels()
            .ToList();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.UpperChannel is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<StdDevChannelsResult> r0 = noquotes
            .GetStdDevChannels()
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<StdDevChannelsResult> r1 = onequote
            .GetStdDevChannels()
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Condense()
    {
        int lookbackPeriods = 20;
        double standardDeviations = 2;

        List<StdDevChannelsResult> results = quotes
            .GetStdDevChannels(lookbackPeriods, standardDeviations)
            .Condense()
            .ToList();

        // assertions
        Assert.AreEqual(500, results.Count);
        StdDevChannelsResult last = results.LastOrDefault();
        Assert.AreEqual(235.8131, last.Centerline.Round(4));
        Assert.AreEqual(257.6536, last.UpperChannel.Round(4));
        Assert.AreEqual(213.9727, last.LowerChannel.Round(4));
        Assert.IsFalse(last.BreakPoint);
    }

    [TestMethod]
    public void Removed()
    {
        int lookbackPeriods = 20;
        double standardDeviations = 2;

        List<StdDevChannelsResult> results = quotes
            .GetStdDevChannels(lookbackPeriods, standardDeviations)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(500, results.Count);
        StdDevChannelsResult last = results.LastOrDefault();
        Assert.AreEqual(235.8131, last.Centerline.Round(4));
        Assert.AreEqual(257.6536, last.UpperChannel.Round(4));
        Assert.AreEqual(213.9727, last.LowerChannel.Round(4));
        Assert.IsFalse(last.BreakPoint);
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetStdDevChannels(0));

        // bad standard deviations
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetStdDevChannels(20, 0));
    }
}
