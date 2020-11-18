using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Internal.Tests
{
    [TestClass]
    public class ConnorsRsiTests : TestBase
    {

        [TestMethod()]
        public void GetConnorsRsi()
        {
            int rsiPeriod = 3;
            int streakPeriod = 2;
            int rankPeriod = 100;
            int startPeriod = Math.Max(rsiPeriod, Math.Max(streakPeriod, rankPeriod)) + 2;

            List<ConnorsRsiResult> results1 = Indicator.GetConnorsRsi(history, rsiPeriod, streakPeriod, rankPeriod).ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results1.Count);
            Assert.AreEqual(502 - startPeriod + 1, results1.Where(x => x.ConnorsRsi != null).Count());

            // sample value
            ConnorsRsiResult r1 = results1[501];
            Assert.AreEqual(68.8087m, Math.Round((decimal)r1.RsiClose, 4));
            Assert.AreEqual(67.4899m, Math.Round((decimal)r1.RsiStreak, 4));
            Assert.AreEqual(88.0000m, Math.Round((decimal)r1.PercentRank, 4));
            Assert.AreEqual(74.7662m, Math.Round((decimal)r1.ConnorsRsi, 4));

            // different parameters
            List<ConnorsRsiResult> results2 = Indicator.GetConnorsRsi(history, 14, 20, 10).ToList();
            ConnorsRsiResult r2 = results2[501];
            Assert.AreEqual(42.0773m, Math.Round((decimal)r2.RsiClose, 4));
            Assert.AreEqual(52.7386m, Math.Round((decimal)r2.RsiStreak, 4));
            Assert.AreEqual(90.0000m, Math.Round((decimal)r2.PercentRank, 4));
            Assert.AreEqual(61.6053m, Math.Round((decimal)r2.ConnorsRsi, 4));

        }

        [TestMethod()]
        public void GetConnorsRsiBadData()
        {
            IEnumerable<ConnorsRsiResult> r = Indicator.GetConnorsRsi(historyBad, 4, 3, 25);
            Assert.AreEqual(502, r.Count());
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Bad RSI period.")]
        public void BadRsiPeriod()
        {
            Indicator.GetConnorsRsi(history, 1, 2, 100);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Bad Streak period.")]
        public void BadStreakPeriod()
        {
            Indicator.GetConnorsRsi(history, 3, 1, 100);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Bad Rank period.")]
        public void BadPctRankPeriod()
        {
            Indicator.GetConnorsRsi(history, 3, 2, 1);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            IEnumerable<Quote> h = History.GetHistory(101);
            Indicator.GetConnorsRsi(h, 3, 2, 100);
        }

    }
}