namespace Tests.Indicators;

[TestClass]
public class StdDevTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<StdDevResult> results = quotes
            .GetStdDev(10)
            .ToList();

        // proper quantities
        Assert.HasCount(502, results);
        Assert.AreEqual(493, results.Count(x => x.StdDev != null));
        Assert.AreEqual(493, results.Count(x => x.ZScore != null));
        Assert.IsFalse(results.Any(x => x.StdDevSma != null));

        // sample values
        StdDevResult r1 = results[8];
        Assert.IsNull(r1.StdDev);
        Assert.IsNull(r1.Mean);
        Assert.IsNull(r1.ZScore);
        Assert.IsNull(r1.StdDevSma);

        StdDevResult r2 = results[9];
        Assert.AreEqual(0.5020, r2.StdDev.Round(4));
        Assert.AreEqual(214.0140, r2.Mean.Round(4));
        Assert.AreEqual(-0.525917, r2.ZScore.Round(6));
        Assert.IsNull(r2.StdDevSma);

        StdDevResult r3 = results[249];
        Assert.AreEqual(0.9827, r3.StdDev.Round(4));
        Assert.AreEqual(257.2200, r3.Mean.Round(4));
        Assert.AreEqual(0.783563, r3.ZScore.Round(6));
        Assert.IsNull(r3.StdDevSma);

        StdDevResult r4 = results[501];
        Assert.AreEqual(5.4738, r4.StdDev.Round(4));
        Assert.AreEqual(242.4100, r4.Mean.Round(4));
        Assert.AreEqual(0.524312, r4.ZScore.Round(6));
        Assert.IsNull(r4.StdDevSma);
    }

    [TestMethod]
    public void UseTuple()
    {
        List<StdDevResult> results = quotes
            .Use(CandlePart.Close)
            .GetStdDev(10)
            .ToList();

        Assert.HasCount(502, results);
        Assert.AreEqual(493, results.Count(x => x.StdDev != null));
    }

    [TestMethod]
    public void TupleNaN()
    {
        List<StdDevResult> r = tupleNanny
            .GetStdDev(6)
            .ToList();

        Assert.HasCount(200, r);
        Assert.IsEmpty(r.Where(x => x.StdDev is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public void Chainee()
    {
        List<StdDevResult> results = quotes
            .GetSma(2)
            .GetStdDev(10)
            .ToList();

        Assert.HasCount(502, results);
        Assert.AreEqual(492, results.Count(x => x.StdDev != null));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = quotes
            .GetStdDev(10)
            .GetSma(10)
            .ToList();

        Assert.HasCount(502, results);
        Assert.AreEqual(484, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void WithSma()
    {
        int lookbackPeriods = 10;
        int smaPeriods = 5;
        List<StdDevResult> results = quotes
            .GetStdDev(lookbackPeriods, smaPeriods)
            .ToList();

        // proper quantities
        Assert.HasCount(502, results);
        Assert.AreEqual(493, results.Count(x => x.StdDev != null));
        Assert.AreEqual(493, results.Count(x => x.ZScore != null));
        Assert.AreEqual(489, results.Count(x => x.StdDevSma != null));

        // sample values
        StdDevResult r1 = results[19];
        Assert.AreEqual(1.1642, r1.StdDev.Round(4));
        Assert.AreEqual(-0.065282, r1.ZScore.Round(6));
        Assert.AreEqual(1.1422, r1.StdDevSma.Round(4));

        StdDevResult r2 = results[501];
        Assert.AreEqual(5.4738, r2.StdDev.Round(4));
        Assert.AreEqual(0.524312, r2.ZScore.Round(6));
        Assert.AreEqual(7.6886, r2.StdDevSma.Round(4));
    }

    [TestMethod]
    public void BadData()
    {
        List<StdDevResult> r = badQuotes
            .GetStdDev(15, 3)
            .ToList();

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(x => x.StdDev is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public void BigData()
    {
        List<StdDevResult> r = bigQuotes
            .GetStdDev(200, 3)
            .ToList();

        Assert.HasCount(1246, r);
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<StdDevResult> r0 = noquotes
            .GetStdDev(10)
            .ToList();

        Assert.IsEmpty(r0);

        List<StdDevResult> r1 = onequote
            .GetStdDev(10)
            .ToList();

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Removed()
    {
        List<StdDevResult> results = quotes
            .GetStdDev(10)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.HasCount(502 - 9, results);

        StdDevResult last = results.LastOrDefault();
        Assert.AreEqual(5.4738, last.StdDev.Round(4));
        Assert.AreEqual(242.4100, last.Mean.Round(4));
        Assert.AreEqual(0.524312, last.ZScore.Round(6));
        Assert.IsNull(last.StdDevSma);
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            quotes.GetStdDev(1));

        // bad SMA period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            quotes.GetStdDev(14, 0));
    }
}
