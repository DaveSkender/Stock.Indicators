using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using Tests.Common;

namespace Tests.Indicators;

[TestClass]
public class StarcBandsTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        int smaPeriods = 20;
        int multiplier = 2;
        int atrPeriods = 14;

        List<StarcBandsResult> results = quotes
            .GetStarcBands(smaPeriods, multiplier, atrPeriods)
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
        Assert.AreEqual(214.5250, r19.Centerline.Round(4));
        Assert.AreEqual(217.2345, r19.UpperBand.Round(4));
        Assert.AreEqual(211.8155, r19.LowerBand.Round(4));

        StarcBandsResult r249 = results[249];
        Assert.AreEqual(255.5500, r249.Centerline.Round(4));
        Assert.AreEqual(258.2261, r249.UpperBand.Round(4));
        Assert.AreEqual(252.8739, r249.LowerBand.Round(4));

        StarcBandsResult r485 = results[485];
        Assert.AreEqual(265.4855, r485.Centerline.Round(4));
        Assert.AreEqual(275.1161, r485.UpperBand.Round(4));
        Assert.AreEqual(255.8549, r485.LowerBand.Round(4));

        StarcBandsResult r501 = results[501];
        Assert.AreEqual(251.8600, r501.Centerline.Round(4));
        Assert.AreEqual(264.1595, r501.UpperBand.Round(4));
        Assert.AreEqual(239.5605, r501.LowerBand.Round(4));
    }

    [TestMethod]
    public void BadData()
    {
        List<StarcBandsResult> r = badQuotes
            .GetStarcBands(10, 3, 15)
            .ToList();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.UpperBand is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<StarcBandsResult> r0 = noquotes
            .GetStarcBands(10)
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<StarcBandsResult> r1 = onequote
            .GetStarcBands(10)
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Condense()
    {
        int smaPeriods = 20;
        int multiplier = 2;
        int atrPeriods = 14;
        int lookbackPeriods = Math.Max(smaPeriods, atrPeriods);

        List<StarcBandsResult> results = quotes
            .GetStarcBands(smaPeriods, multiplier, atrPeriods)
            .Condense()
            .ToList();

        // assertions
        Assert.AreEqual(502 - lookbackPeriods + 1, results.Count);

        StarcBandsResult last = results.LastOrDefault();
        Assert.AreEqual(251.8600, last.Centerline.Round(4));
        Assert.AreEqual(264.1595, last.UpperBand.Round(4));
        Assert.AreEqual(239.5605, last.LowerBand.Round(4));
    }

    [TestMethod]
    public void Removed()
    {
        int smaPeriods = 20;
        int multiplier = 2;
        int atrPeriods = 14;
        int lookbackPeriods = Math.Max(smaPeriods, atrPeriods);

        List<StarcBandsResult> results = quotes
            .GetStarcBands(smaPeriods, multiplier, atrPeriods)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - (lookbackPeriods + 150), results.Count);

        StarcBandsResult last = results.LastOrDefault();
        Assert.AreEqual(251.8600, last.Centerline.Round(4));
        Assert.AreEqual(264.1595, last.UpperBand.Round(4));
        Assert.AreEqual(239.5605, last.LowerBand.Round(4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad EMA period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetStarcBands(1, 2, 10));

        // bad ATR period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetStarcBands(20, 2, 1));

        // bad multiplier
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetStarcBands(20, 0, 10));
    }
}
