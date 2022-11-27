using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class StarcBands : TestBase
{
    [TestMethod]
    public void Standard()
    {
        int smaPeriods = 20;
        int multiplier = 2;
        int atrPeriods = 14;
        int lookbackPeriods = Math.Max(smaPeriods, atrPeriods);

        List<StarcBandsResult> results =
            quotes.GetStarcBands(smaPeriods, multiplier, atrPeriods)
            .ToList();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.Centerline != null));
        Assert.AreEqual(483, results.Count(x => x.UpperBand != null));
        Assert.AreEqual(483, results.Count(x => x.LowerBand != null));

        // sample value
        StarcBandsResult r1 = results[18];
        Assert.AreEqual(null, r1.Centerline);
        Assert.AreEqual(null, r1.UpperBand);
        Assert.AreEqual(null, r1.LowerBand);

        StarcBandsResult r19 = results[19];
        Assert.AreEqual(214.5250, NullMath.Round(r19.Centerline, 4));
        Assert.AreEqual(217.2345, NullMath.Round(r19.UpperBand, 4));
        Assert.AreEqual(211.8155, NullMath.Round(r19.LowerBand, 4));

        StarcBandsResult r249 = results[249];
        Assert.AreEqual(255.5500, NullMath.Round(r249.Centerline, 4));
        Assert.AreEqual(258.2261, NullMath.Round(r249.UpperBand, 4));
        Assert.AreEqual(252.8739, NullMath.Round(r249.LowerBand, 4));

        StarcBandsResult r485 = results[485];
        Assert.AreEqual(265.4855, NullMath.Round(r485.Centerline, 4));
        Assert.AreEqual(275.1161, NullMath.Round(r485.UpperBand, 4));
        Assert.AreEqual(255.8549, NullMath.Round(r485.LowerBand, 4));

        StarcBandsResult r501 = results[501];
        Assert.AreEqual(251.8600, NullMath.Round(r501.Centerline, 4));
        Assert.AreEqual(264.1595, NullMath.Round(r501.UpperBand, 4));
        Assert.AreEqual(239.5605, NullMath.Round(r501.LowerBand, 4));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<StarcBandsResult> r = Indicator.GetStarcBands(badQuotes, 10, 3, 15);
        Assert.AreEqual(502, r.Count());
        Assert.AreEqual(0, r.Count(x => x.UpperBand is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<StarcBandsResult> r0 = noquotes.GetStarcBands();
        Assert.AreEqual(0, r0.Count());

        IEnumerable<StarcBandsResult> r1 = onequote.GetStarcBands();
        Assert.AreEqual(1, r1.Count());
    }

    [TestMethod]
    public void Condense()
    {
        int smaPeriods = 20;
        int multiplier = 2;
        int atrPeriods = 14;
        int lookbackPeriods = Math.Max(smaPeriods, atrPeriods);

        List<StarcBandsResult> results =
            quotes.GetStarcBands(smaPeriods, multiplier, atrPeriods)
                .Condense()
                .ToList();

        // assertions
        Assert.AreEqual(502 - lookbackPeriods + 1, results.Count);

        StarcBandsResult last = results.LastOrDefault();
        Assert.AreEqual(251.8600, NullMath.Round(last.Centerline, 4));
        Assert.AreEqual(264.1595, NullMath.Round(last.UpperBand, 4));
        Assert.AreEqual(239.5605, NullMath.Round(last.LowerBand, 4));
    }

    [TestMethod]
    public void Removed()
    {
        int smaPeriods = 20;
        int multiplier = 2;
        int atrPeriods = 14;
        int lookbackPeriods = Math.Max(smaPeriods, atrPeriods);

        List<StarcBandsResult> results =
            quotes.GetStarcBands(smaPeriods, multiplier, atrPeriods)
                .RemoveWarmupPeriods()
                .ToList();

        // assertions
        Assert.AreEqual(502 - (lookbackPeriods + 150), results.Count);

        StarcBandsResult last = results.LastOrDefault();
        Assert.AreEqual(251.8600, NullMath.Round(last.Centerline, 4));
        Assert.AreEqual(264.1595, NullMath.Round(last.UpperBand, 4));
        Assert.AreEqual(239.5605, NullMath.Round(last.LowerBand, 4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad EMA period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetStarcBands(quotes, 1, 2, 10));

        // bad ATR period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetStarcBands(quotes, 20, 2, 1));

        // bad multiplier
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetStarcBands(quotes, 20, 0, 10));
    }
}
