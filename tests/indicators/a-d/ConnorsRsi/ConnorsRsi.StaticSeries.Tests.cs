namespace StaticSeries;

[TestClass]
public class ConnorsRsi : StaticSeriesTestBase
{
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        const int rsiPeriods = 3;
        const int streakPeriods = 2;
        const int rankPeriods = 100;
        int startPeriod = Math.Max(rsiPeriods, Math.Max(streakPeriods, rankPeriods)) + 2;

        IReadOnlyList<ConnorsRsiResult> results1 = Quotes
            .ToConnorsRsi(rsiPeriods, streakPeriods, rankPeriods);

        // proper quantities
        results1.Should().HaveCount(502);
        Assert.HasCount(502 - startPeriod + 1, results1.Where(static x => x.ConnorsRsi != null));

        // sample value
        ConnorsRsiResult r1 = results1[501];
        r1.Rsi.Should().BeApproximately(68.8087, Money4);
        r1.RsiStreak.Should().BeApproximately(67.4899, Money4);
        r1.PercentRank.Should().BeApproximately(88.0000, Money4);
        r1.ConnorsRsi.Should().BeApproximately(74.7662, Money4);

        // different parameters
        IReadOnlyList<ConnorsRsiResult> results2 = Quotes.ToConnorsRsi(14, 20, 10).ToList();
        ConnorsRsiResult r2 = results2[501];
        r2.Rsi.Should().BeApproximately(42.0773, Money4);
        r2.RsiStreak.Should().BeApproximately(52.7386, Money4);
        r2.PercentRank.Should().BeApproximately(90.0000, Money4);
        r2.ConnorsRsi.Should().BeApproximately(61.6053, Money4);
    }

    [TestMethod]
    public void UseReusable()
    {
        IReadOnlyList<ConnorsRsiResult> sut = Quotes
            .Use(CandlePart.Close)
            .ToConnorsRsi();

        sut.Should().HaveCount(502);
        Assert.AreEqual(401, sut.Count(static x => x.ConnorsRsi != null));
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<ConnorsRsiResult> sut = Quotes
            .ToSma(2)
            .ToConnorsRsi();

        sut.Should().HaveCount(502);
        Assert.AreEqual(400, sut.Count(static x => x.ConnorsRsi != null));
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<SmaResult> sut = Quotes
            .ToConnorsRsi()
            .ToSma(10);

        sut.Should().HaveCount(502);
        Assert.AreEqual(392, sut.Count(static x => x.Sma != null));
    }

    [TestMethod]
    public void Results_AreAlwaysBounded()
    {
        IReadOnlyList<ConnorsRsiResult> sut = Quotes.ToConnorsRsi(3, 2, 100);
        sut.IsBetween(static x => x.ConnorsRsi, 0, 100);
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<ConnorsRsiResult> r = BadQuotes
            .ToConnorsRsi(4, 3, 25);

        r.Should().HaveCount(502);
        r.Where(static x => x.Rsi is double v && double.IsNaN(v)).Should().BeEmpty();
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<ConnorsRsiResult> r0 = Noquotes
            .ToConnorsRsi();

        r0.Should().BeEmpty();

        IReadOnlyList<ConnorsRsiResult> r1 = Onequote
            .ToConnorsRsi();

        r1.Should().HaveCount(1);
    }

    [TestMethod]
    public void Removed()
    {
        const int rsiPeriods = 3;
        const int streakPeriods = 2;
        const int rankPeriods = 100;

        // TODO: I don't think this is right, inconsistent
        int removePeriods = Math.Max(rsiPeriods, Math.Max(streakPeriods, rankPeriods)) + 2;

        IReadOnlyList<ConnorsRsiResult> sut = Quotes
            .ToConnorsRsi(rsiPeriods, streakPeriods, rankPeriods)
            .RemoveWarmupPeriods();

        // assertions
        sut.Should().HaveCount(502 - removePeriods + 1);

        ConnorsRsiResult last = sut[^1];
        last.Rsi.Should().BeApproximately(68.8087, Money4);
        last.RsiStreak.Should().BeApproximately(67.4899, Money4);
        last.PercentRank.Should().BeApproximately(88.0000, Money4);
        last.ConnorsRsi.Should().BeApproximately(74.7662, Money4);
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad RSI period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToConnorsRsi(1));

        // bad Streak period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToConnorsRsi(3, 1));

        // bad Rank period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToConnorsRsi(3, 2, 1));
    }
}
