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
            int rsiPeriod = 3;
            int streakPeriod = 2;
            int rankPeriod = 100;
            int startPeriod = Math.Max(rsiPeriod, Math.Max(streakPeriod, rankPeriod)) + 2;

            List<ConnorsRsiResult> results1 =
                history.GetConnorsRsi(rsiPeriod, streakPeriod, rankPeriod)
                .ToList();

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
            List<ConnorsRsiResult> results2 = history.GetConnorsRsi(14, 20, 10).ToList();
            ConnorsRsiResult r2 = results2[501];
            Assert.AreEqual(42.0773m, Math.Round((decimal)r2.RsiClose, 4));
            Assert.AreEqual(52.7386m, Math.Round((decimal)r2.RsiStreak, 4));
            Assert.AreEqual(90.0000m, Math.Round((decimal)r2.PercentRank, 4));
            Assert.AreEqual(61.6053m, Math.Round((decimal)r2.ConnorsRsi, 4));
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<ConnorsRsiResult> r = Indicator.GetConnorsRsi(historyBad, 4, 3, 25);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad RSI period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetConnorsRsi(history, 1, 2, 100));

            // bad Streak period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetConnorsRsi(history, 3, 1, 100));

            // bad Rank period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetConnorsRsi(history, 3, 2, 1));

            // insufficient history
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetConnorsRsi(HistoryTestData.Get(102), 3, 2, 100));
        }
    }
}
