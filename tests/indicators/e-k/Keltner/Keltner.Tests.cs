using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using Tests.Common;

namespace Tests.Indicators;

[TestClass]
public class KeltnerTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        int emaPeriods = 20;
        int multiplier = 2;
        int atrPeriods = 10;

        List<KeltnerResult> results = quotes
            .GetKeltner(emaPeriods, multiplier, atrPeriods)
            .ToList();

        // proper quantities
        Assert.AreEqual(502, results.Count);

        int warmupPeriod = 502 - Math.Max(emaPeriods, atrPeriods) + 1;
        Assert.AreEqual(warmupPeriod, results.Count(x => x.Centerline != null));
        Assert.AreEqual(warmupPeriod, results.Count(x => x.UpperBand != null));
        Assert.AreEqual(warmupPeriod, results.Count(x => x.LowerBand != null));
        Assert.AreEqual(warmupPeriod, results.Count(x => x.Width != null));

        // sample value
        KeltnerResult r1 = results[485];
        Assert.AreEqual(275.4260, r1.UpperBand.Round(4));
        Assert.AreEqual(265.4599, r1.Centerline.Round(4));
        Assert.AreEqual(255.4938, r1.LowerBand.Round(4));
        Assert.AreEqual(0.075085, r1.Width.Round(6));

        KeltnerResult r2 = results[501];
        Assert.AreEqual(262.1873, r2.UpperBand.Round(4));
        Assert.AreEqual(249.3519, r2.Centerline.Round(4));
        Assert.AreEqual(236.5165, r2.LowerBand.Round(4));
        Assert.AreEqual(0.102950, r2.Width.Round(6));
    }

    [TestMethod]
    public void BadData()
    {
        List<KeltnerResult> r = badQuotes
            .GetKeltner(10, 3, 15)
            .ToList();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.UpperBand is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<KeltnerResult> r0 = noquotes
            .GetKeltner()
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<KeltnerResult> r1 = onequote
            .GetKeltner()
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Condense()
    {
        int emaPeriods = 20;
        int multiplier = 2;
        int atrPeriods = 10;

        List<KeltnerResult> results = quotes
            .GetKeltner(emaPeriods, multiplier, atrPeriods)
            .Condense()
            .ToList();

        // assertions
        Assert.AreEqual(483, results.Count);

        KeltnerResult last = results.LastOrDefault();
        Assert.AreEqual(262.1873, last.UpperBand.Round(4));
        Assert.AreEqual(249.3519, last.Centerline.Round(4));
        Assert.AreEqual(236.5165, last.LowerBand.Round(4));
        Assert.AreEqual(0.102950, last.Width.Round(6));
    }

    [TestMethod]
    public void Removed()
    {
        int emaPeriods = 20;
        int multiplier = 2;
        int atrPeriods = 10;
        int n = Math.Max(emaPeriods, atrPeriods);

        List<KeltnerResult> results = quotes
            .GetKeltner(emaPeriods, multiplier, atrPeriods)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - Math.Max(2 * n, n + 100), results.Count);

        KeltnerResult last = results.LastOrDefault();
        Assert.AreEqual(262.1873, last.UpperBand.Round(4));
        Assert.AreEqual(249.3519, last.Centerline.Round(4));
        Assert.AreEqual(236.5165, last.LowerBand.Round(4));
        Assert.AreEqual(0.102950, last.Width.Round(6));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad EMA period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetKeltner(1, 2, 10));

        // bad ATR period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetKeltner(20, 2, 1));

        // bad multiplier
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetKeltner(20, 0, 10));
    }
}
