using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using Tests.Common;

namespace Tests.Indicators;

[TestClass]
public class PvoTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        int fastPeriods = 12;
        int slowPeriods = 26;
        int signalPeriods = 9;

        List<PvoResult> results =
            quotes.GetPvo(fastPeriods, slowPeriods, signalPeriods)
            .ToList();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(477, results.Count(x => x.Pvo != null));
        Assert.AreEqual(469, results.Count(x => x.Signal != null));
        Assert.AreEqual(469, results.Count(x => x.Histogram != null));

        // sample values
        PvoResult r1 = results[24];
        Assert.AreEqual(null, r1.Pvo);
        Assert.AreEqual(null, r1.Signal);
        Assert.AreEqual(null, r1.Histogram);

        PvoResult r2 = results[33];
        Assert.AreEqual(1.5795, r2.Pvo.Round(4));
        Assert.AreEqual(-3.5530, r2.Signal.Round(4));
        Assert.AreEqual(5.1325, r2.Histogram.Round(4));

        PvoResult r3 = results[149];
        Assert.AreEqual(-7.1910, r3.Pvo.Round(4));
        Assert.AreEqual(-5.1159, r3.Signal.Round(4));
        Assert.AreEqual(-2.0751, r3.Histogram.Round(4));

        PvoResult r4 = results[249];
        Assert.AreEqual(-6.3667, r4.Pvo.Round(4));
        Assert.AreEqual(1.7333, r4.Signal.Round(4));
        Assert.AreEqual(-8.1000, r4.Histogram.Round(4));

        PvoResult r5 = results[501];
        Assert.AreEqual(10.4395, r5.Pvo.Round(4));
        Assert.AreEqual(12.2681, r5.Signal.Round(4));
        Assert.AreEqual(-1.8286, r5.Histogram.Round(4));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = quotes
            .GetPvo()
            .GetSma(10)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(468, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        List<PvoResult> r = badQuotes
            .GetPvo(10, 20, 5)
            .ToList();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Pvo is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<PvoResult> r0 = noquotes
            .GetPvo()
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<PvoResult> r1 = onequote
            .GetPvo()
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        int fastPeriods = 12;
        int slowPeriods = 26;
        int signalPeriods = 9;

        List<PvoResult> results = quotes
            .GetPvo(fastPeriods, slowPeriods, signalPeriods)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - (slowPeriods + signalPeriods + 250), results.Count);

        PvoResult last = results.LastOrDefault();
        Assert.AreEqual(10.4395, last.Pvo.Round(4));
        Assert.AreEqual(12.2681, last.Signal.Round(4));
        Assert.AreEqual(-1.8286, last.Histogram.Round(4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad fast period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetPvo(0, 26, 9));

        // bad slow periods must be larger than faster period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetPvo(12, 12, 9));

        // bad signal period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetPvo(12, 26, -1));
    }
}
