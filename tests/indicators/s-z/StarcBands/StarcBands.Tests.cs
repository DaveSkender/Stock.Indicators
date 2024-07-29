namespace StaticSeries;

[TestClass]
public class StarcBandsTests : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        int smaPeriods = 20;
        int multiplier = 2;
        int atrPeriods = 14;

        IReadOnlyList<StarcBandsResult> results = Quotes
            .GetStarcBands(smaPeriods, multiplier, atrPeriods);

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
    public override void BadData()
    {
        IReadOnlyList<StarcBandsResult> r = BadQuotes
            .GetStarcBands(10, 3, 15);

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.UpperBand is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<StarcBandsResult> r0 = Noquotes
            .GetStarcBands(10);

        Assert.AreEqual(0, r0.Count);

        IReadOnlyList<StarcBandsResult> r1 = Onequote
            .GetStarcBands(10);

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Condense()
    {
        int smaPeriods = 20;
        int multiplier = 2;
        int atrPeriods = 14;
        int lookbackPeriods = Math.Max(smaPeriods, atrPeriods);

        IReadOnlyList<StarcBandsResult> results = Quotes
            .GetStarcBands(smaPeriods, multiplier, atrPeriods)
            .Condense();

        // assertions
        Assert.AreEqual(502 - lookbackPeriods + 1, results.Count);

        StarcBandsResult last = results[^1];
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

        IReadOnlyList<StarcBandsResult> results = Quotes
            .GetStarcBands(smaPeriods, multiplier, atrPeriods)
            .RemoveWarmupPeriods();

        // assertions
        Assert.AreEqual(502 - (lookbackPeriods + 150), results.Count);

        StarcBandsResult last = results[^1];
        Assert.AreEqual(251.8600, last.Centerline.Round(4));
        Assert.AreEqual(264.1595, last.UpperBand.Round(4));
        Assert.AreEqual(239.5605, last.LowerBand.Round(4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad EMA period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Quotes.GetStarcBands(1));

        // bad ATR period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Quotes.GetStarcBands(20, 2, 1));

        // bad multiplier
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Quotes.GetStarcBands(20, 0));
    }
}
