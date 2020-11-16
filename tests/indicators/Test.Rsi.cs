using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Internal.Tests
{
    [TestClass]
    public class RsiTests : TestBase
    {

        [TestMethod()]
        public void GetRsiTest()
        {
            int lookbackPeriod = 14;
            List<RsiResult> results = Indicator.GetRsi(history, lookbackPeriod).ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(488, results.Where(x => x.Rsi != null).Count());

            // sample value
            RsiResult r = results[501];
            Assert.AreEqual(42.0773m, Math.Round((decimal)r.Rsi, 4));
        }

        [TestMethod()]
        public void GetRsiSmallPeriodTest()
        {
            int lookbackPeriod = 1;
            List<RsiResult> results = Indicator.GetRsi(history, lookbackPeriod).ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(501, results.Where(x => x.Rsi != null).Count());

            // sample values
            RsiResult r1 = results[28];
            Assert.AreEqual(100m, Math.Round((decimal)r1.Rsi, 4));

            RsiResult r2 = results[52];
            Assert.AreEqual(0m, Math.Round((decimal)r2.Rsi, 4));
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Bad lookback.")]
        public void BadLookbackPeriod()
        {
            Indicator.GetRsi(history, 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            Indicator.GetRsi(history.Where(x => x.Index < 30), 30);
        }

    }
}