namespace StaticSeries;

[TestClass]
public class StdDev : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        IReadOnlyList<StdDevResult> results = Quotes
            .ToStdDev(10);

        // proper quantities
        Assert.HasCount(502, results);
        Assert.HasCount(493, results.Where(x => x.StdDev != null));
        Assert.HasCount(493, results.Where(x => x.ZScore != null));

        // sample values
        StdDevResult r1 = results[8];
        Assert.IsNull(r1.StdDev);
        Assert.IsNull(r1.Mean);
        Assert.IsNull(r1.ZScore);

        StdDevResult r2 = results[9];
        Assert.AreEqual(0.5020, r2.StdDev.Round(4));
        Assert.AreEqual(214.0140, r2.Mean.Round(4));
        Assert.AreEqual(-0.525917, r2.ZScore.Round(6));

        StdDevResult r3 = results[249];
        Assert.AreEqual(0.9827, r3.StdDev.Round(4));
        Assert.AreEqual(257.2200, r3.Mean.Round(4));
        Assert.AreEqual(0.783563, r3.ZScore.Round(6));

        StdDevResult r4 = results[501];
        Assert.AreEqual(5.4738, r4.StdDev.Round(4));
        Assert.AreEqual(242.4100, r4.Mean.Round(4));
        Assert.AreEqual(0.524312, r4.ZScore.Round(6));
    }

    [TestMethod]
    public void UseReusable()
    {
        IReadOnlyList<StdDevResult> results = Quotes
            .Use(CandlePart.Close)
            .ToStdDev(10);

        Assert.HasCount(502, results);
        Assert.HasCount(493, results.Where(x => x.StdDev != null));
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<StdDevResult> results = Quotes
            .ToSma(2)
            .ToStdDev(10);

        Assert.HasCount(502, results);
        Assert.HasCount(492, results.Where(x => x.StdDev != null));
    }

    [TestMethod]
    public void Chainor()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToStdDev(10)
            .ToSma(10);

        Assert.HasCount(502, results);
        Assert.HasCount(484, results.Where(x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<StdDevResult> r = BadQuotes
            .ToStdDev(15);

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(x => x.StdDev is double.NaN));
    }

    [TestMethod]
    public void BigData()
    {
        IReadOnlyList<StdDevResult> r = BigQuotes
            .ToStdDev(200);

        Assert.HasCount(1246, r);
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<StdDevResult> r0 = Noquotes
            .ToStdDev(10);

        Assert.IsEmpty(r0);

        IReadOnlyList<StdDevResult> r1 = Onequote
            .ToStdDev(10);

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<StdDevResult> results = Quotes
            .ToStdDev(10)
            .RemoveWarmupPeriods();

        // assertions
        Assert.HasCount(502 - 9, results);

        StdDevResult last = results[^1];
        Assert.AreEqual(5.4738, last.StdDev.Round(4));
        Assert.AreEqual(242.4100, last.Mean.Round(4));
        Assert.AreEqual(0.524312, last.ZScore.Round(6));
    }

    [TestMethod] // bad lookback period
    public void Exceptions()
        => Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => Quotes.ToStdDev(1));
}
