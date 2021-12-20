using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
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
            Assert.AreEqual(502 - startPeriod + 1, results1.Where(x => x.ConnorsRsi != null).Count());

            // sample value
            ConnorsRsiResult r1 = results1[501];
            Assert.AreEqual(68.8087, Math.Round((double)r1.RsiClose, 4));
            Assert.AreEqual(67.4899, Math.Round((double)r1.RsiStreak, 4));
            Assert.AreEqual(88.0000, Math.Round((double)r1.PercentRank, 4));
            Assert.AreEqual(74.7662, Math.Round((double)r1.ConnorsRsi, 4));

            // different parameters
            List<ConnorsRsiResult> results2 = quotes.GetConnorsRsi(14, 20, 10).ToList();
            ConnorsRsiResult r2 = results2[501];
            Assert.AreEqual(42.0773, Math.Round((double)r2.RsiClose, 4));
            Assert.AreEqual(52.7386, Math.Round((double)r2.RsiStreak, 4));
            Assert.AreEqual(90.0000, Math.Round((double)r2.PercentRank, 4));
            Assert.AreEqual(61.6053, Math.Round((double)r2.ConnorsRsi, 4));
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<ConnorsRsiResult> r = Indicator.GetConnorsRsi(badQuotes, 4, 3, 25);
            Assert.AreEqual(502, r.Count());
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
            Assert.AreEqual(68.8087, Math.Round((double)last.RsiClose, 4));
            Assert.AreEqual(67.4899, Math.Round((double)last.RsiStreak, 4));
            Assert.AreEqual(88.0000, Math.Round((double)last.PercentRank, 4));
            Assert.AreEqual(74.7662, Math.Round((double)last.ConnorsRsi, 4));
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

            // insufficient quotes
            Assert.ThrowsException<BadQuotesException>(() =>
                Indicator.GetConnorsRsi(TestData.GetDefault(102), 3, 2, 100));
        }
    }
}
