namespace StaticSeries;

[TestClass]
public class ConnorsRsi : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        int rsiPeriods = 3;
        int streakPeriods = 2;
        int rankPeriods = 100;
        int startPeriod = Math.Max(rsiPeriods, Math.Max(streakPeriods, rankPeriods)) + 2;

        IReadOnlyList<ConnorsRsiResult> results1 = Quotes
            .ToConnorsRsi(rsiPeriods, streakPeriods, rankPeriods);

        // proper quantities
        Assert.HasCount(502, results1);
        Assert.HasCount(502 - startPeriod + 1, results1.Where(x => x.ConnorsRsi != null));

        // sample value
        ConnorsRsiResult r1 = results1[501];
        Assert.AreEqual(68.8087, r1.Rsi.Round(4));
        Assert.AreEqual(67.4899, r1.RsiStreak.Round(4));
        Assert.AreEqual(88.0000, r1.PercentRank.Round(4));
        Assert.AreEqual(74.7662, r1.ConnorsRsi.Round(4));

        // different parameters
        IReadOnlyList<ConnorsRsiResult> results2 = Quotes.ToConnorsRsi(14, 20, 10).ToList();
        ConnorsRsiResult r2 = results2[501];
        Assert.AreEqual(42.0773, r2.Rsi.Round(4));
        Assert.AreEqual(52.7386, r2.RsiStreak.Round(4));
        Assert.AreEqual(90.0000, r2.PercentRank.Round(4));
        Assert.AreEqual(61.6053, r2.ConnorsRsi.Round(4));
    }

    [TestMethod]
    public void UseReusable()
    {
        IReadOnlyList<ConnorsRsiResult> results = Quotes
            .Use(CandlePart.Close)
            .ToConnorsRsi();

        Assert.HasCount(502, results);
        Assert.AreEqual(401, results.Count(x => x.ConnorsRsi != null));
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<ConnorsRsiResult> results = Quotes
            .ToSma(2)
            .ToConnorsRsi();

        Assert.HasCount(502, results);
        Assert.AreEqual(400, results.Count(x => x.ConnorsRsi != null));
    }

    [TestMethod]
    public void Chainor()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToConnorsRsi()
            .ToSma(10);

        Assert.HasCount(502, results);
        Assert.AreEqual(392, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<ConnorsRsiResult> r = BadQuotes
            .ToConnorsRsi(4, 3, 25);

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(x => x.Rsi is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<ConnorsRsiResult> r0 = Noquotes
            .ToConnorsRsi();

        Assert.IsEmpty(r0);

        IReadOnlyList<ConnorsRsiResult> r1 = Onequote
            .ToConnorsRsi();

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Removed()
    {
        int rsiPeriods = 3;
        int streakPeriods = 2;
        int rankPeriods = 100;

        // TODO: I don't think this is right, inconsistent
        int removePeriods = Math.Max(rsiPeriods, Math.Max(streakPeriods, rankPeriods)) + 2;

        IReadOnlyList<ConnorsRsiResult> results = Quotes
            .ToConnorsRsi(rsiPeriods, streakPeriods, rankPeriods)
            .RemoveWarmupPeriods();

        // assertions
        Assert.HasCount(502 - removePeriods + 1, results);

        ConnorsRsiResult last = results[^1];
        Assert.AreEqual(68.8087, last.Rsi.Round(4));
        Assert.AreEqual(67.4899, last.RsiStreak.Round(4));
        Assert.AreEqual(88.0000, last.PercentRank.Round(4));
        Assert.AreEqual(74.7662, last.ConnorsRsi.Round(4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad RSI period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => Quotes.ToConnorsRsi(1));

        // bad Streak period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => Quotes.ToConnorsRsi(3, 1));

        // bad Rank period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => Quotes.ToConnorsRsi(3, 2, 1));
    }
}
