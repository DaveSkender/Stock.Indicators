using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class Keltner : TestBase
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

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);

        int warmupPeriod = 502 - Math.Max(emaPeriods, atrPeriods) + 1;
        Assert.AreEqual(warmupPeriod, results.Count(x => x.Centerline != null));
        Assert.AreEqual(warmupPeriod, results.Count(x => x.UpperBand != null));
        Assert.AreEqual(warmupPeriod, results.Count(x => x.LowerBand != null));
        Assert.AreEqual(warmupPeriod, results.Count(x => x.Width != null));

        // sample value
        KeltnerResult r1 = results[485];
        Assert.AreEqual(275.4260, NullMath.Round(r1.UpperBand, 4));
        Assert.AreEqual(265.4599, NullMath.Round(r1.Centerline, 4));
        Assert.AreEqual(255.4938, NullMath.Round(r1.LowerBand, 4));
        Assert.AreEqual(0.075085, NullMath.Round(r1.Width, 6));

        KeltnerResult r2 = results[501];
        Assert.AreEqual(262.1873, NullMath.Round(r2.UpperBand, 4));
        Assert.AreEqual(249.3519, NullMath.Round(r2.Centerline, 4));
        Assert.AreEqual(236.5165, NullMath.Round(r2.LowerBand, 4));
        Assert.AreEqual(0.102950, NullMath.Round(r2.Width, 6));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<KeltnerResult> r = Indicator.GetKeltner(badQuotes, 10, 3, 15);
        Assert.AreEqual(502, r.Count());
        Assert.AreEqual(0, r.Count(x => x.UpperBand is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<KeltnerResult> r0 = noquotes.GetKeltner();
        Assert.AreEqual(0, r0.Count());

        IEnumerable<KeltnerResult> r1 = onequote.GetKeltner();
        Assert.AreEqual(1, r1.Count());
    }

    [TestMethod]
    public void Condense()
    {
        int emaPeriods = 20;
        int multiplier = 2;
        int atrPeriods = 10;
        int n = Math.Max(emaPeriods, atrPeriods);

        List<KeltnerResult> results =
            quotes.GetKeltner(emaPeriods, multiplier, atrPeriods)
                .Condense()
                .ToList();

        // assertions
        Assert.AreEqual(483, results.Count);

        KeltnerResult last = results.LastOrDefault();
        Assert.AreEqual(262.1873, NullMath.Round(last.UpperBand, 4));
        Assert.AreEqual(249.3519, NullMath.Round(last.Centerline, 4));
        Assert.AreEqual(236.5165, NullMath.Round(last.LowerBand, 4));
        Assert.AreEqual(0.102950, NullMath.Round(last.Width, 6));
    }

    [TestMethod]
    public void Removed()
    {
        int emaPeriods = 20;
        int multiplier = 2;
        int atrPeriods = 10;
        int n = Math.Max(emaPeriods, atrPeriods);

        List<KeltnerResult> results =
            quotes.GetKeltner(emaPeriods, multiplier, atrPeriods)
                .RemoveWarmupPeriods()
                .ToList();

        // assertions
        Assert.AreEqual(502 - Math.Max(2 * n, n + 100), results.Count);

        KeltnerResult last = results.LastOrDefault();
        Assert.AreEqual(262.1873, NullMath.Round(last.UpperBand, 4));
        Assert.AreEqual(249.3519, NullMath.Round(last.Centerline, 4));
        Assert.AreEqual(236.5165, NullMath.Round(last.LowerBand, 4));
        Assert.AreEqual(0.102950, NullMath.Round(last.Width, 6));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad EMA period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetKeltner(quotes, 1, 2, 10));

        // bad ATR period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetKeltner(quotes, 20, 2, 1));

        // bad multiplier
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetKeltner(quotes, 20, 0, 10));
    }
}
