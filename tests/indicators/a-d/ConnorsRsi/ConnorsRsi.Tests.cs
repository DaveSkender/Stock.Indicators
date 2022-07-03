using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class ConnorsRsi : TestBase
{
    [TestMethod]
    public void Standard()
    {
        int rsiPeriods = 3;
        int streakPeriods = 2;
        int rankPeriods = 100;
        int startPeriod = Math.Max(rsiPeriods, Math.Max(streakPeriods, rankPeriods)) + 2;

        List<ConnorsRsiResult> results1 =
            quotes.GetConnorsRsi(rsiPeriods, streakPeriods, rankPeriods)
            .ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results1.Count);
        Assert.AreEqual(502 - startPeriod + 1, results1.Count(x => x.ConnorsRsi != null));

        // sample value
        ConnorsRsiResult r1 = results1[501];
        Assert.AreEqual(68.8087, NullMath.Round(r1.Rsi, 4));
        Assert.AreEqual(67.4899, NullMath.Round(r1.RsiStreak, 4));
        Assert.AreEqual(88.0000, NullMath.Round(r1.PercentRank, 4));
        Assert.AreEqual(74.7662, NullMath.Round(r1.ConnorsRsi, 4));

        // different parameters
        List<ConnorsRsiResult> results2 = quotes.GetConnorsRsi(14, 20, 10).ToList();
        ConnorsRsiResult r2 = results2[501];
        Assert.AreEqual(42.0773, NullMath.Round(r2.Rsi, 4));
        Assert.AreEqual(52.7386, NullMath.Round(r2.RsiStreak, 4));
        Assert.AreEqual(90.0000, NullMath.Round(r2.PercentRank, 4));
        Assert.AreEqual(61.6053, NullMath.Round(r2.ConnorsRsi, 4));
    }

    [TestMethod]
    public void UseTuple()
    {
        IEnumerable<ConnorsRsiResult> results = quotes
            .Use(CandlePart.Close)
            .GetConnorsRsi();

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(401, results.Count(x => x.ConnorsRsi != null));
    }

    [TestMethod]
    public void TupleNaN()
    {
        IEnumerable<ConnorsRsiResult> r = tupleNanny.GetConnorsRsi();

        Assert.AreEqual(200, r.Count());
        Assert.AreEqual(0, r.Count(x => x.ConnorsRsi is double and double.NaN));
    }

    [TestMethod]
    public void Chainee()
    {
        IEnumerable<ConnorsRsiResult> results = quotes
            .GetSma(2)
            .GetConnorsRsi();

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(400, results.Count(x => x.ConnorsRsi != null));
    }

    [TestMethod]
    public void Chainor()
    {
        IEnumerable<SmaResult> results = quotes
            .GetConnorsRsi()
            .GetSma(10);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(392, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<ConnorsRsiResult> r = Indicator.GetConnorsRsi(badQuotes, 4, 3, 25);
        Assert.AreEqual(502, r.Count());
        Assert.AreEqual(0, r.Count(x => x.Rsi is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<ConnorsRsiResult> r0 = noquotes.GetConnorsRsi();
        Assert.AreEqual(0, r0.Count());

        IEnumerable<ConnorsRsiResult> r1 = onequote.GetConnorsRsi();
        Assert.AreEqual(1, r1.Count());
    }

    [TestMethod]
    public void Removed()
    {
        int rsiPeriods = 3;
        int streakPeriods = 2;
        int rankPeriods = 100;

        // TODO: I don't think this is right, inconsistent
        int removePeriods = Math.Max(rsiPeriods, Math.Max(streakPeriods, rankPeriods)) + 2;

        List<ConnorsRsiResult> results =
            quotes.GetConnorsRsi(rsiPeriods, streakPeriods, rankPeriods)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - removePeriods + 1, results.Count);

        ConnorsRsiResult last = results.LastOrDefault();
        Assert.AreEqual(68.8087, NullMath.Round(last.Rsi, 4));
        Assert.AreEqual(67.4899, NullMath.Round(last.RsiStreak, 4));
        Assert.AreEqual(88.0000, NullMath.Round(last.PercentRank, 4));
        Assert.AreEqual(74.7662, NullMath.Round(last.ConnorsRsi, 4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad RSI period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetConnorsRsi(quotes, 1, 2, 100));

        // bad Streak period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetConnorsRsi(quotes, 3, 1, 100));

        // bad Rank period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetConnorsRsi(quotes, 3, 2, 1));
    }
}
