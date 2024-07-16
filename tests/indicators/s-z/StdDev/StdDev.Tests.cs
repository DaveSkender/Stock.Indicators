namespace Series;

[TestClass]
public class StdDevTests : SeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        List<StdDevResult> results = Quotes
            .GetStdDev(10)
            .ToList();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(493, results.Count(x => x.StdDev != null));
        Assert.AreEqual(493, results.Count(x => x.ZScore != null));

        // sample values
        StdDevResult r1 = results[8];
        Assert.AreEqual(null, r1.StdDev);
        Assert.AreEqual(null, r1.Mean);
        Assert.AreEqual(null, r1.ZScore);

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
        List<StdDevResult> results = Quotes
            .Use(CandlePart.Close)
            .GetStdDev(10)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(493, results.Count(x => x.StdDev != null));
    }

    [TestMethod]
    public void Chainee()
    {
        List<StdDevResult> results = Quotes
            .GetSma(2)
            .GetStdDev(10)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(492, results.Count(x => x.StdDev != null));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = Quotes
            .GetStdDev(10)
            .GetSma(10)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(484, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        List<StdDevResult> r = BadQuotes
            .GetStdDev(15)
            .ToList();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.StdDev is double.NaN));
    }

    [TestMethod]
    public void BigData()
    {
        List<StdDevResult> r = BigQuotes
            .GetStdDev(200)
            .ToList();

        Assert.AreEqual(1246, r.Count);
    }

    [TestMethod]
    public override void NoQuotes()
    {
        List<StdDevResult> r0 = Noquotes
            .GetStdDev(10)
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<StdDevResult> r1 = Onequote
            .GetStdDev(10)
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        List<StdDevResult> results = Quotes
            .GetStdDev(10)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - 9, results.Count);

        StdDevResult last = results.LastOrDefault();
        Assert.AreEqual(5.4738, last.StdDev.Round(4));
        Assert.AreEqual(242.4100, last.Mean.Round(4));
        Assert.AreEqual(0.524312, last.ZScore.Round(6));
    }

    [TestMethod]
    public void Exceptions() =>

        // bad lookback period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Quotes.GetStdDev(1));
}
