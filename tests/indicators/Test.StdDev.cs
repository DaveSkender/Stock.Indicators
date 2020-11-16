using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Internal.Tests
{
    [TestClass]
    public class StandardDevTests : TestBase
    {

        [TestMethod()]
        public void GetStdDevTest()
        {
            int lookbackPeriod = 10;
            List<StdDevResult> results = Indicator.GetStdDev(history, lookbackPeriod).ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(493, results.Where(x => x.StdDev != null).Count());
            Assert.AreEqual(493, results.Where(x => x.ZScore != null).Count());
            Assert.AreEqual(false, results.Any(x => x.Sma != null));

            // sample values
            StdDevResult r1 = results[501];
            Assert.AreEqual(5.4738m, Math.Round((decimal)r1.StdDev, 4));
            Assert.AreEqual(0.524312m, Math.Round((decimal)r1.ZScore, 6));
            Assert.AreEqual(null, r1.Sma);

            StdDevResult r2 = results[249];
            Assert.AreEqual(0.9827m, Math.Round((decimal)r2.StdDev, 4));
            Assert.AreEqual(0.783563m, Math.Round((decimal)r2.ZScore, 6));
            Assert.AreEqual(null, r2.Sma);
        }

        [TestMethod()]
        public void GetStdDevWithSmaTest()
        {
            int lookbackPeriod = 10;
            int smaPeriod = 5;
            List<StdDevResult> results = Indicator.GetStdDev(history, lookbackPeriod, smaPeriod).ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(493, results.Where(x => x.StdDev != null).Count());
            Assert.AreEqual(493, results.Where(x => x.ZScore != null).Count());
            Assert.AreEqual(489, results.Where(x => x.Sma != null).Count());

            // sample values
            StdDevResult r1 = results[501];
            Assert.AreEqual(5.4738m, Math.Round((decimal)r1.StdDev, 4));
            Assert.AreEqual(0.524312m, Math.Round((decimal)r1.ZScore, 6));
            Assert.AreEqual(7.6886m, Math.Round((decimal)r1.Sma, 4));

            StdDevResult r2 = results[19];
            Assert.AreEqual(1.1642m, Math.Round((decimal)r2.StdDev, 4));
            Assert.AreEqual(-0.065282m, Math.Round((decimal)r2.ZScore, 6));
            Assert.AreEqual(1.1422m, Math.Round((decimal)r2.Sma, 4));
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Bad lookback.")]
        public void BadLookbackPeriod()
        {
            Indicator.GetStdDev(history, 1);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Bad SMA period.")]
        public void BadSmaPeriod()
        {
            Indicator.GetStdDev(history, 14, 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            Indicator.GetStdDev(history.Where(x => x.Index < 30), 30);
        }

    }
}