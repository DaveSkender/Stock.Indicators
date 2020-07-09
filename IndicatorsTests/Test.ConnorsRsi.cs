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
            Assert.AreEqual((decimal)42.0773, Math.Round((decimal)r.RsiClose, 4));
            Assert.AreEqual((decimal)42.0773, Math.Round((decimal)r.RsiStreak, 4));
            Assert.AreEqual((decimal)42.0773, Math.Round((decimal)r.PercentRank, 4));
            Assert.AreEqual((decimal)42.0773, Math.Round((decimal)r.ConnorsRsi, 4));
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(BadParameterException), "Bad lookback.")]
        public void BadLookback()
        {
            Indicator.GetConnorsRsi(history, 1);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            Indicator.GetConnorsRsi(history.Where(x => x.Index < 30), 30);
        }

    }
}