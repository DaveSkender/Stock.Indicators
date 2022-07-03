using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class Pvo : TestBase
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

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
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
        Assert.AreEqual(1.5795, NullMath.Round(r2.Pvo, 4));
        Assert.AreEqual(-3.5530, NullMath.Round(r2.Signal, 4));
        Assert.AreEqual(5.1325, NullMath.Round(r2.Histogram, 4));

        PvoResult r3 = results[149];
        Assert.AreEqual(-7.1910, NullMath.Round(r3.Pvo, 4));
        Assert.AreEqual(-5.1159, NullMath.Round(r3.Signal, 4));
        Assert.AreEqual(-2.0751, NullMath.Round(r3.Histogram, 4));

        PvoResult r4 = results[249];
        Assert.AreEqual(-6.3667, NullMath.Round(r4.Pvo, 4));
        Assert.AreEqual(1.7333, NullMath.Round(r4.Signal, 4));
        Assert.AreEqual(-8.1000, NullMath.Round(r4.Histogram, 4));

        PvoResult r5 = results[501];
        Assert.AreEqual(10.4395, NullMath.Round(r5.Pvo, 4));
        Assert.AreEqual(12.2681, NullMath.Round(r5.Signal, 4));
        Assert.AreEqual(-1.8286, NullMath.Round(r5.Histogram, 4));
    }

    [TestMethod]
    public void Chainor()
    {
        IEnumerable<SmaResult> results = quotes
            .GetPvo()
            .GetSma(10);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(468, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<PvoResult> r = Indicator.GetPvo(badQuotes, 10, 20, 5);
        Assert.AreEqual(502, r.Count());
        Assert.AreEqual(0, r.Count(x => x.Pvo is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<PvoResult> r0 = noquotes.GetPvo();
        Assert.AreEqual(0, r0.Count());

        IEnumerable<PvoResult> r1 = onequote.GetPvo();
        Assert.AreEqual(1, r1.Count());
    }

    [TestMethod]
    public void Removed()
    {
        int fastPeriods = 12;
        int slowPeriods = 26;
        int signalPeriods = 9;

        List<PvoResult> results =
            quotes.GetPvo(fastPeriods, slowPeriods, signalPeriods)
                .RemoveWarmupPeriods()
                .ToList();

        // assertions
        Assert.AreEqual(502 - (slowPeriods + signalPeriods + 250), results.Count);

        PvoResult last = results.LastOrDefault();
        Assert.AreEqual(10.4395, NullMath.Round(last.Pvo, 4));
        Assert.AreEqual(12.2681, NullMath.Round(last.Signal, 4));
        Assert.AreEqual(-1.8286, NullMath.Round(last.Histogram, 4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad fast period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetPvo(quotes, 0, 26, 9));

        // bad slow periods must be larger than faster period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetPvo(quotes, 12, 12, 9));

        // bad signal period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetPvo(quotes, 12, 26, -1));
    }
}
