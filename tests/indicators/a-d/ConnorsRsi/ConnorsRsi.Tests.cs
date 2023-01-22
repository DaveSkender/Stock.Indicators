using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using Tests.Common;

namespace Tests.Indicators;

[TestClass]
public class ConnorsRsiTests : TestBase
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

        // proper quantities
        Assert.AreEqual(502, results1.Count);
        Assert.AreEqual(502 - startPeriod + 1, results1.Count(x => x.ConnorsRsi != null));

        // sample value
        ConnorsRsiResult r1 = results1[501];
        Assert.AreEqual(68.8087, r1.Rsi.Round(4));
        Assert.AreEqual(67.4899, r1.RsiStreak.Round(4));
        Assert.AreEqual(88.0000, r1.PercentRank.Round(4));
        Assert.AreEqual(74.7662, r1.ConnorsRsi.Round(4));

        // different parameters
        List<ConnorsRsiResult> results2 = quotes.GetConnorsRsi(14, 20, 10).ToList();
        ConnorsRsiResult r2 = results2[501];
        Assert.AreEqual(42.0773, r2.Rsi.Round(4));
        Assert.AreEqual(52.7386, r2.RsiStreak.Round(4));
        Assert.AreEqual(90.0000, r2.PercentRank.Round(4));
        Assert.AreEqual(61.6053, r2.ConnorsRsi.Round(4));
    }

    [TestMethod]
    public void UseTuple()
    {
        List<ConnorsRsiResult> results = quotes
            .Use(CandlePart.Close)
            .GetConnorsRsi()
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(401, results.Count(x => x.ConnorsRsi != null));
    }

    [TestMethod]
    public void TupleNaN()
    {
        List<ConnorsRsiResult> r = tupleNanny
            .GetConnorsRsi()
            .ToList();

        Assert.AreEqual(200, r.Count);
        Assert.AreEqual(0, r.Count(x => x.ConnorsRsi is double and double.NaN));
    }

    [TestMethod]
    public void Chainee()
    {
        List<ConnorsRsiResult> results = quotes
            .GetSma(2)
            .GetConnorsRsi()
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(400, results.Count(x => x.ConnorsRsi != null));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = quotes
            .GetConnorsRsi()
            .GetSma(10)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(392, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        List<ConnorsRsiResult> r = badQuotes
            .GetConnorsRsi(4, 3, 25)
            .ToList();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Rsi is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<ConnorsRsiResult> r0 = noquotes
            .GetConnorsRsi()
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<ConnorsRsiResult> r1 = onequote
            .GetConnorsRsi()
            .ToList();

        Assert.AreEqual(1, r1.Count);
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
        Assert.AreEqual(68.8087, last.Rsi.Round(4));
        Assert.AreEqual(67.4899, last.RsiStreak.Round(4));
        Assert.AreEqual(88.0000, last.PercentRank.Round(4));
        Assert.AreEqual(74.7662, last.ConnorsRsi.Round(4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad RSI period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetConnorsRsi(1, 2, 100));

        // bad Streak period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetConnorsRsi(3, 1, 100));

        // bad Rank period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetConnorsRsi(3, 2, 1));
    }
}
