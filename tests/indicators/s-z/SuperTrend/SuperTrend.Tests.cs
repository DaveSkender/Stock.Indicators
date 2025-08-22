namespace Tests.Indicators;

[TestClass]
public class SuperTrendTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        int lookbackPeriods = 14;
        double multiplier = 3;

        List<SuperTrendResult> results = quotes
            .GetSuperTrend(lookbackPeriods, multiplier)
            .ToList();

        // proper quantities
        Assert.HasCount(502, results);
        Assert.AreEqual(488, results.Count(x => x.SuperTrend != null));

        // sample values
        SuperTrendResult r13 = results[13];
        Assert.IsNull(r13.SuperTrend);
        Assert.IsNull(r13.UpperBand);
        Assert.IsNull(r13.LowerBand);

        SuperTrendResult r14 = results[14];
        Assert.AreEqual(210.6157m, r14.SuperTrend.Round(4));
        Assert.IsNull(r14.UpperBand);
        Assert.AreEqual(r14.SuperTrend, r14.LowerBand);

        SuperTrendResult r151 = results[151];
        Assert.AreEqual(232.8520m, r151.SuperTrend.Round(4));
        Assert.IsNull(r151.UpperBand);
        Assert.AreEqual(r151.SuperTrend, r151.LowerBand);

        SuperTrendResult r152 = results[152];
        Assert.AreEqual(237.6436m, r152.SuperTrend.Round(4));
        Assert.AreEqual(r152.SuperTrend, r152.UpperBand);
        Assert.IsNull(r152.LowerBand);

        SuperTrendResult r249 = results[249];
        Assert.AreEqual(253.8008m, r249.SuperTrend.Round(4));
        Assert.IsNull(r249.UpperBand);
        Assert.AreEqual(r249.SuperTrend, r249.LowerBand);

        SuperTrendResult r501 = results[501];
        Assert.AreEqual(250.7954m, r501.SuperTrend.Round(4));
        Assert.AreEqual(r501.SuperTrend, r501.UpperBand);
        Assert.IsNull(r501.LowerBand);
    }

    [TestMethod]
    public void Bitcoin()
    {
        IEnumerable<Quote> h = TestData.GetBitcoin();

        List<SuperTrendResult> results = h
            .GetSuperTrend(10, 3)
            .ToList();

        Assert.HasCount(1246, results);

        SuperTrendResult r = results[1208];
        Assert.AreEqual(16242.2704m, r.LowerBand.Round(4));
    }

    [TestMethod]
    public void BadData()
    {
        List<SuperTrendResult> r = badQuotes
            .GetSuperTrend(7)
            .ToList();

        Assert.HasCount(502, r);
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<SuperTrendResult> r0 = noquotes
            .GetSuperTrend()
            .ToList();

        Assert.IsEmpty(r0);

        List<SuperTrendResult> r1 = onequote
            .GetSuperTrend()
            .ToList();

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Condense()
    {
        int lookbackPeriods = 14;
        double multiplier = 3;

        List<SuperTrendResult> results = quotes
            .GetSuperTrend(lookbackPeriods, multiplier)
            .Condense()
            .ToList();

        // assertions
        Assert.HasCount(488, results);

        SuperTrendResult last = results.LastOrDefault();
        Assert.AreEqual(250.7954m, last.SuperTrend.Round(4));
        Assert.AreEqual(last.SuperTrend, last.UpperBand);
        Assert.IsNull(last.LowerBand);
    }

    [TestMethod]
    public void Removed()
    {
        int lookbackPeriods = 14;
        double multiplier = 3;

        List<SuperTrendResult> results = quotes
            .GetSuperTrend(lookbackPeriods, multiplier)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.HasCount(488, results);

        SuperTrendResult last = results.LastOrDefault();
        Assert.AreEqual(250.7954m, last.SuperTrend.Round(4));
        Assert.AreEqual(last.SuperTrend, last.UpperBand);
        Assert.IsNull(last.LowerBand);
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            quotes.GetSuperTrend(1));

        // bad multiplier
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            quotes.GetSuperTrend(7, 0));
    }
}
