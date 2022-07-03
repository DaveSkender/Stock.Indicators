using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class StdDevChannels : TestBase
{
    [TestMethod]
    public void Standard()
    {
        int lookbackPeriods = 20;
        double standardDeviations = 2;

        List<StdDevChannelsResult> results =
            quotes.GetStdDevChannels(lookbackPeriods, standardDeviations)
            .ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
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
        Assert.AreEqual(213.7993, NullMath.Round(r2.Centerline, 4));
        Assert.AreEqual(215.7098, NullMath.Round(r2.UpperChannel, 4));
        Assert.AreEqual(211.8888, NullMath.Round(r2.LowerChannel, 4));
        Assert.IsTrue(r2.BreakPoint);

        StdDevChannelsResult r3 = results[141];
        Assert.AreEqual(236.1744, NullMath.Round(r3.Centerline, 4));
        Assert.AreEqual(240.4784, NullMath.Round(r3.UpperChannel, 4));
        Assert.AreEqual(231.8704, NullMath.Round(r3.LowerChannel, 4));
        Assert.IsFalse(r3.BreakPoint);

        StdDevChannelsResult r4 = results[142];
        Assert.AreEqual(236.3269, NullMath.Round(r4.Centerline, 4));
        Assert.AreEqual(239.5585, NullMath.Round(r4.UpperChannel, 4));
        Assert.AreEqual(233.0953, NullMath.Round(r4.LowerChannel, 4));
        Assert.IsTrue(r4.BreakPoint);

        StdDevChannelsResult r5 = results[249];
        Assert.AreEqual(259.6044, NullMath.Round(r5.Centerline, 4));
        Assert.AreEqual(267.5754, NullMath.Round(r5.UpperChannel, 4));
        Assert.AreEqual(251.6333, NullMath.Round(r5.LowerChannel, 4));
        Assert.IsFalse(r5.BreakPoint);

        StdDevChannelsResult r6 = results[482];
        Assert.AreEqual(267.9069, NullMath.Round(r6.Centerline, 4));
        Assert.AreEqual(289.7473, NullMath.Round(r6.UpperChannel, 4));
        Assert.AreEqual(246.0664, NullMath.Round(r6.LowerChannel, 4));
        Assert.IsTrue(r6.BreakPoint);

        StdDevChannelsResult r7 = results[501];
        Assert.AreEqual(235.8131, NullMath.Round(r7.Centerline, 4));
        Assert.AreEqual(257.6536, NullMath.Round(r7.UpperChannel, 4));
        Assert.AreEqual(213.9727, NullMath.Round(r7.LowerChannel, 4));
        Assert.IsFalse(r7.BreakPoint);
    }

    [TestMethod]
    public void FullHistory()
    {
        // null provided for lookback period

        List<StdDevChannelsResult> results =
            quotes.GetStdDevChannels(null, 2)
            .ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(502, results.Count(x => x.Centerline != null));
        Assert.AreEqual(502, results.Count(x => x.UpperChannel != null));
        Assert.AreEqual(502, results.Count(x => x.LowerChannel != null));
        Assert.AreEqual(501, results.Count(x => x.BreakPoint == false));

        // sample value
        StdDevChannelsResult r1 = results[0];
        Assert.AreEqual(219.2605, NullMath.Round(r1.Centerline, 4));
        Assert.AreEqual(258.7104, NullMath.Round(r1.UpperChannel, 4));
        Assert.AreEqual(179.8105, NullMath.Round(r1.LowerChannel, 4));
        Assert.IsTrue(r1.BreakPoint);

        StdDevChannelsResult r2 = results[249];
        Assert.AreEqual(249.3814, NullMath.Round(r2.Centerline, 4));
        Assert.AreEqual(288.8314, NullMath.Round(r2.UpperChannel, 4));
        Assert.AreEqual(209.9315, NullMath.Round(r2.LowerChannel, 4));

        StdDevChannelsResult r3 = results[501];
        Assert.AreEqual(279.8653, NullMath.Round(r3.Centerline, 4));
        Assert.AreEqual(319.3152, NullMath.Round(r3.UpperChannel, 4));
        Assert.AreEqual(240.4153, NullMath.Round(r3.LowerChannel, 4));
    }

    [TestMethod]
    public void UseTuple()
    {
        IEnumerable<StdDevChannelsResult> results = quotes
            .Use(CandlePart.Close)
            .GetStdDevChannels(20, 2);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(500, results.Count(x => x.Centerline != null));
    }

    [TestMethod]
    public void TupleNaN()
    {
        IEnumerable<StdDevChannelsResult> r
            = tupleNanny.GetStdDevChannels(6, 1.1);

        Assert.AreEqual(200, r.Count());
        Assert.AreEqual(0, r.Count(x => x.UpperChannel is double and double.NaN));
    }

    [TestMethod]
    public void Chainee()
    {
        IEnumerable<StdDevChannelsResult> results = quotes
            .GetSma(2)
            .GetStdDevChannels(20, 2);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(500, results.Count(x => x.Centerline != null));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<StdDevChannelsResult> r = Indicator.GetStdDevChannels(badQuotes);
        Assert.AreEqual(502, r.Count());
        Assert.AreEqual(0, r.Count(x => x.UpperChannel is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<StdDevChannelsResult> r0 = noquotes.GetStdDevChannels();
        Assert.AreEqual(0, r0.Count());

        IEnumerable<StdDevChannelsResult> r1 = onequote.GetStdDevChannels();
        Assert.AreEqual(1, r1.Count());
    }

    [TestMethod]
    public void Condense()
    {
        int lookbackPeriods = 20;
        double standardDeviations = 2;

        List<StdDevChannelsResult> results =
            quotes.GetStdDevChannels(lookbackPeriods, standardDeviations)
                .Condense()
                .ToList();

        // assertions
        Assert.AreEqual(500, results.Count);
        StdDevChannelsResult last = results.LastOrDefault();
        Assert.AreEqual(235.8131, NullMath.Round(last.Centerline, 4));
        Assert.AreEqual(257.6536, NullMath.Round(last.UpperChannel, 4));
        Assert.AreEqual(213.9727, NullMath.Round(last.LowerChannel, 4));
        Assert.IsFalse(last.BreakPoint);
    }

    [TestMethod]
    public void Removed()
    {
        int lookbackPeriods = 20;
        double standardDeviations = 2;

        List<StdDevChannelsResult> results =
            quotes.GetStdDevChannels(lookbackPeriods, standardDeviations)
                .RemoveWarmupPeriods()
                .ToList();

        // assertions
        Assert.AreEqual(500, results.Count);
        StdDevChannelsResult last = results.LastOrDefault();
        Assert.AreEqual(235.8131, NullMath.Round(last.Centerline, 4));
        Assert.AreEqual(257.6536, NullMath.Round(last.UpperChannel, 4));
        Assert.AreEqual(213.9727, NullMath.Round(last.LowerChannel, 4));
        Assert.IsFalse(last.BreakPoint);
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetStdDevChannels(quotes, 0));

        // bad standard deviations
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetStdDevChannels(quotes, 20, 0));
    }
}
