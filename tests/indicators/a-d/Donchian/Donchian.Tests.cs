namespace Series;

[TestClass]
public class DonchianTests : SeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        IReadOnlyList<DonchianResult> results = Quotes
            .GetDonchian();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(482, results.Count(x => x.Centerline != null));
        Assert.AreEqual(482, results.Count(x => x.UpperBand != null));
        Assert.AreEqual(482, results.Count(x => x.LowerBand != null));
        Assert.AreEqual(482, results.Count(x => x.Width != null));

        // sample values
        DonchianResult r1 = results[19];
        Assert.AreEqual(null, r1.Centerline);
        Assert.AreEqual(null, r1.UpperBand);
        Assert.AreEqual(null, r1.LowerBand);
        Assert.AreEqual(null, r1.Width);

        DonchianResult r2 = results[20];
        Assert.AreEqual(214.2700m, r2.Centerline.Round(4));
        Assert.AreEqual(217.0200m, r2.UpperBand.Round(4));
        Assert.AreEqual(211.5200m, r2.LowerBand.Round(4));
        Assert.AreEqual(0.025669m, r2.Width.Round(6));

        DonchianResult r3 = results[249];
        Assert.AreEqual(254.2850m, r3.Centerline.Round(4));
        Assert.AreEqual(258.7000m, r3.UpperBand.Round(4));
        Assert.AreEqual(249.8700m, r3.LowerBand.Round(4));
        Assert.AreEqual(0.034725m, r3.Width.Round(6));

        DonchianResult r4 = results[485];
        Assert.AreEqual(265.5350m, r4.Centerline.Round(4));
        Assert.AreEqual(274.3900m, r4.UpperBand.Round(4));
        Assert.AreEqual(256.6800m, r4.LowerBand.Round(4));
        Assert.AreEqual(0.066696m, r4.Width.Round(6));

        DonchianResult r5 = results[501];
        Assert.AreEqual(251.5050m, r5.Centerline.Round(4));
        Assert.AreEqual(273.5900m, r5.UpperBand.Round(4));
        Assert.AreEqual(229.4200m, r5.LowerBand.Round(4));
        Assert.AreEqual(0.175623m, r5.Width.Round(6));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<DonchianResult> r = BadQuotes
            .GetDonchian(15);

        Assert.AreEqual(502, r.Count);
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<DonchianResult> r0 = Noquotes
            .GetDonchian();

        Assert.AreEqual(0, r0.Count);

        IReadOnlyList<DonchianResult> r1 = Onequote
            .GetDonchian();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Condense()
    {
        IReadOnlyList<DonchianResult> results = Quotes
            .GetDonchian()
            .Condense();

        // assertions
        Assert.AreEqual(502 - 20, results.Count);

        DonchianResult last = results[^1];
        Assert.AreEqual(251.5050m, last.Centerline.Round(4));
        Assert.AreEqual(273.5900m, last.UpperBand.Round(4));
        Assert.AreEqual(229.4200m, last.LowerBand.Round(4));
        Assert.AreEqual(0.175623m, last.Width.Round(6));
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<DonchianResult> results = Quotes
            .GetDonchian()
            .RemoveWarmupPeriods();

        // assertions
        Assert.AreEqual(502 - 20, results.Count);

        DonchianResult last = results[^1];
        Assert.AreEqual(251.5050m, last.Centerline.Round(4));
        Assert.AreEqual(273.5900m, last.UpperBand.Round(4));
        Assert.AreEqual(229.4200m, last.LowerBand.Round(4));
        Assert.AreEqual(0.175623m, last.Width.Round(6));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => Quotes.GetDonchian(0));
}
