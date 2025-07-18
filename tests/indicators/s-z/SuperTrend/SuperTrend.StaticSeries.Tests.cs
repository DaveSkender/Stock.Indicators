namespace StaticSeries;

[TestClass]
public class SuperTrend : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        const int lookbackPeriods = 14;
        const double multiplier = 3;

        IReadOnlyList<SuperTrendResult> results = Quotes
            .ToSuperTrend(lookbackPeriods, multiplier);

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(488, results.Count(x => x.SuperTrend != null));

        // sample values
        SuperTrendResult r13 = results[13];
        Assert.AreEqual(null, r13.SuperTrend);
        Assert.AreEqual(null, r13.UpperBand);
        Assert.AreEqual(null, r13.LowerBand);

        SuperTrendResult r14 = results[14];
        Assert.AreEqual(210.6157m, r14.SuperTrend.Round(4));
        Assert.AreEqual(null, r14.UpperBand);
        Assert.AreEqual(r14.SuperTrend, r14.LowerBand);

        SuperTrendResult r151 = results[151];
        Assert.AreEqual(232.8520m, r151.SuperTrend.Round(4));
        Assert.AreEqual(null, r151.UpperBand);
        Assert.AreEqual(r151.SuperTrend, r151.LowerBand);

        SuperTrendResult r152 = results[152];
        Assert.AreEqual(237.6436m, r152.SuperTrend.Round(4));
        Assert.AreEqual(r152.SuperTrend, r152.UpperBand);
        Assert.AreEqual(null, r152.LowerBand);

        SuperTrendResult r249 = results[249];
        Assert.AreEqual(253.8008m, r249.SuperTrend.Round(4));
        Assert.AreEqual(null, r249.UpperBand);
        Assert.AreEqual(r249.SuperTrend, r249.LowerBand);

        SuperTrendResult r501 = results[501];
        Assert.AreEqual(250.7954m, r501.SuperTrend.Round(4));
        Assert.AreEqual(r501.SuperTrend, r501.UpperBand);
        Assert.AreEqual(null, r501.LowerBand);
    }

    [TestMethod]
    public void Bitcoin()
    {
        IReadOnlyList<Quote> h = Data.GetBitcoin();

        IReadOnlyList<SuperTrendResult> results = h
            .ToSuperTrend();

        Assert.AreEqual(1246, results.Count);

        SuperTrendResult r = results[1208];
        Assert.AreEqual(16242.2704m, r.LowerBand.Round(4));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<SuperTrendResult> r = BadQuotes
            .ToSuperTrend(7);

        Assert.AreEqual(502, r.Count);
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<SuperTrendResult> r0 = Noquotes
            .ToSuperTrend();

        Assert.AreEqual(0, r0.Count);

        IReadOnlyList<SuperTrendResult> r1 = Onequote
            .ToSuperTrend();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Condense()
    {
        int lookbackPeriods = 14;
        double multiplier = 3;

        IReadOnlyList<SuperTrendResult> results = Quotes
            .ToSuperTrend(lookbackPeriods, multiplier)
            .Condense();

        // assertions
        Assert.AreEqual(488, results.Count);

        SuperTrendResult last = results[^1];
        Assert.AreEqual(250.7954m, last.SuperTrend.Round(4));
        Assert.AreEqual(last.SuperTrend, last.UpperBand);
        Assert.AreEqual(null, last.LowerBand);
    }

    [TestMethod]
    public void Removed()
    {
        int lookbackPeriods = 14;
        double multiplier = 3;

        IReadOnlyList<SuperTrendResult> results = Quotes
            .ToSuperTrend(lookbackPeriods, multiplier)
            .RemoveWarmupPeriods();

        // assertions
        Assert.AreEqual(488, results.Count);

        SuperTrendResult last = results[^1];
        Assert.AreEqual(250.7954m, last.SuperTrend.Round(4));
        Assert.AreEqual(last.SuperTrend, last.UpperBand);
        Assert.AreEqual(null, last.LowerBand);
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsException<ArgumentOutOfRangeException>(
            () => Quotes.ToSuperTrend(1));

        // bad multiplier
        Assert.ThrowsException<ArgumentOutOfRangeException>(
            () => Quotes.ToSuperTrend(7, 0));
    }
}
