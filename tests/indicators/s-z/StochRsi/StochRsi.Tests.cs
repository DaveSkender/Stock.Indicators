using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class StochRsi : TestBase
{
    [TestMethod]
    public void FastRsi()
    {
        int rsiPeriods = 14;
        int stochPeriods = 14;
        int signalPeriods = 3;
        int smoothPeriods = 1;

        List<StochRsiResult> results =
            quotes.GetStochRsi(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods)
            .ToList();

        // assertions

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(475, results.Count(x => x.StochRsi != null));
        Assert.AreEqual(473, results.Count(x => x.Signal != null));

        // sample values
        StochRsiResult r1 = results[31];
        Assert.AreEqual(93.3333, NullMath.Round(r1.StochRsi, 4));
        Assert.AreEqual(97.7778, NullMath.Round(r1.Signal, 4));

        StochRsiResult r2 = results[152];
        Assert.AreEqual(0, r2.StochRsi);
        Assert.AreEqual(0, r2.Signal);

        StochRsiResult r3 = results[249];
        Assert.AreEqual(36.5517, NullMath.Round(r3.StochRsi, 4));
        Assert.AreEqual(27.3094, NullMath.Round(r3.Signal, 4));

        StochRsiResult r4 = results[501];
        Assert.AreEqual(97.5244, NullMath.Round(r4.StochRsi, 4));
        Assert.AreEqual(89.8385, NullMath.Round(r4.Signal, 4));
    }

    [TestMethod]
    public void SlowRsi()
    {
        int rsiPeriods = 14;
        int stochPeriods = 14;
        int signalPeriods = 3;
        int smoothPeriods = 3;

        List<StochRsiResult> results =
            Indicator.GetStochRsi(quotes, rsiPeriods, stochPeriods, signalPeriods, smoothPeriods)
            .ToList();

        // assertions

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(473, results.Count(x => x.StochRsi != null));
        Assert.AreEqual(471, results.Count(x => x.Signal != null));

        // sample values
        StochRsiResult r1 = results[31];
        Assert.AreEqual(97.7778, NullMath.Round(r1.StochRsi, 4));
        Assert.AreEqual(99.2593, NullMath.Round(r1.Signal, 4));

        StochRsiResult r2 = results[152];
        Assert.AreEqual(0, r2.StochRsi);
        Assert.AreEqual(20.0263, NullMath.Round(r2.Signal, 4));

        StochRsiResult r3 = results[249];
        Assert.AreEqual(27.3094, NullMath.Round(r3.StochRsi, 4));
        Assert.AreEqual(33.2716, NullMath.Round(r3.Signal, 4));

        StochRsiResult r4 = results[501];
        Assert.AreEqual(89.8385, NullMath.Round(r4.StochRsi, 4));
        Assert.AreEqual(73.4176, NullMath.Round(r4.Signal, 4));
    }

    [TestMethod]
    public void Chainor()
    {
        IEnumerable<SmaResult> results = quotes
            .GetStochRsi(14, 14, 3, 3)
            .GetSma(10);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(464, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<StochRsiResult> r = Indicator.GetStochRsi(badQuotes, 15, 20, 3, 2);
        Assert.AreEqual(502, r.Count());
        Assert.AreEqual(0, r.Count(x => x.StochRsi is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<StochRsiResult> r0 = noquotes.GetStochRsi(10, 20, 3);
        Assert.AreEqual(0, r0.Count());

        IEnumerable<StochRsiResult> r1 = onequote.GetStochRsi(8, 13, 2);
        Assert.AreEqual(1, r1.Count());
    }

    [TestMethod]
    public void Removed()
    {
        int rsiPeriods = 14;
        int stochPeriods = 14;
        int signalPeriods = 3;
        int smoothPeriods = 3;

        List<StochRsiResult> results =
            Indicator.GetStochRsi(quotes, rsiPeriods, stochPeriods, signalPeriods, smoothPeriods)
                .RemoveWarmupPeriods()
                .ToList();

        // assertions
        int removeQty = rsiPeriods + stochPeriods + smoothPeriods + 100;
        Assert.AreEqual(502 - removeQty, results.Count);

        StochRsiResult last = results.LastOrDefault();
        Assert.AreEqual(89.8385, NullMath.Round(last.StochRsi, 4));
        Assert.AreEqual(73.4176, NullMath.Round(last.Signal, 4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad RSI period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetStochRsi(quotes, 0, 14, 3, 1));

        // bad STO period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetStochRsi(quotes, 14, 0, 3, 3));

        // bad STO signal period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetStochRsi(quotes, 14, 14, 0));

        // bad STO smoothing period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetStochRsi(quotes, 14, 14, 3, 0));
    }
}
