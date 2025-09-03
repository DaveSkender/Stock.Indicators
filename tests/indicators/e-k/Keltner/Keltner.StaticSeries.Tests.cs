namespace StaticSeries;

[TestClass]
public class Keltner : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        int emaPeriods = 20;
        int multiplier = 2;
        int atrPeriods = 10;

        IReadOnlyList<KeltnerResult> results = Quotes
            .ToKeltner(emaPeriods, multiplier, atrPeriods);

        // proper quantities
        Assert.HasCount(502, results);

        int warmupPeriod = 502 - Math.Max(emaPeriods, atrPeriods) + 1;
        Assert.HasCount(warmupPeriod, results.Where(x => x.Centerline != null));
        Assert.HasCount(warmupPeriod, results.Where(x => x.UpperBand != null));
        Assert.HasCount(warmupPeriod, results.Where(x => x.LowerBand != null));
        Assert.HasCount(warmupPeriod, results.Where(x => x.Width != null));

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
    public override void BadData()
    {
        IReadOnlyList<KeltnerResult> r = BadQuotes
            .ToKeltner(10, 3, 15);

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(x => x.UpperBand is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<KeltnerResult> r0 = Noquotes
            .ToKeltner();

        Assert.IsEmpty(r0);

        IReadOnlyList<KeltnerResult> r1 = Onequote
            .ToKeltner();

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Condense()
    {
        int emaPeriods = 20;
        int multiplier = 2;
        int atrPeriods = 10;

        IReadOnlyList<KeltnerResult> results = Quotes
            .ToKeltner(emaPeriods, multiplier, atrPeriods)
            .Condense();

        // assertions
        Assert.HasCount(483, results);

        KeltnerResult last = results[^1];
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

        IReadOnlyList<KeltnerResult> results = Quotes
            .ToKeltner(emaPeriods, multiplier, atrPeriods)
            .RemoveWarmupPeriods();

        // assertions
        Assert.AreEqual(502 - Math.Max(2 * n, n + 100), results.Count);

        KeltnerResult last = results[^1];
        Assert.AreEqual(262.1873, last.UpperBand.Round(4));
        Assert.AreEqual(249.3519, last.Centerline.Round(4));
        Assert.AreEqual(236.5165, last.LowerBand.Round(4));
        Assert.AreEqual(0.102950, last.Width.Round(6));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad EMA period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => Quotes.ToKeltner(1));

        // bad ATR period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => Quotes.ToKeltner(20, 2, 1));

        // bad multiplier
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => Quotes.ToKeltner(20, 0));
    }
}
