using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StockIndicators.Tests
{
    [TestClass]
    public class ConnorsRsiTests : TestBase
    {

        [TestMethod()]
        public void GetConnorsRsiTest()
        {
            int rsiPeriod = 3;
            int streakPeriod = 2;
            int rankPeriod = 100;
            int startPeriod = Math.Max(rsiPeriod, Math.Max(streakPeriod, rankPeriod));

            IEnumerable<ConnorsRsiResult> results = Indicator.GetConnorsRsi(history, rsiPeriod, streakPeriod, rankPeriod);

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count());
            Assert.AreEqual(502 - startPeriod, results.Where(x => x.ConnorsRsi != null).Count());

            // sample value
            ConnorsRsiResult r = results.Where(x => x.Date == DateTime.Parse("12/31/2018")).FirstOrDefault();
            Assert.AreEqual((decimal)68.8087, Math.Round((decimal)r.RsiClose, 4));
            Assert.AreEqual((decimal)67.4899, Math.Round((decimal)r.RsiStreak, 4));
            Assert.AreEqual((decimal)88.0000, Math.Round((decimal)r.PercentRank, 4));
            Assert.AreEqual((decimal)74.7662, Math.Round((decimal)r.ConnorsRsi, 4));
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(BadParameterException), "Bad RSI period.")]
        public void BadRsiPeriod()
        {
            Indicator.GetConnorsRsi(history, 1, 2, 100);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadParameterException), "Bad Streak period.")]
        public void BadStreakPeriod()
        {
            Indicator.GetConnorsRsi(history, 3, 1, 100);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadParameterException), "Bad Rank period.")]
        public void BadPctRankPeriods()
        {
            Indicator.GetConnorsRsi(history, 3, 2, 1);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            Indicator.GetConnorsRsi(history.Where(x => x.Index < 102), 3, 2, 100);
        }

    }
}