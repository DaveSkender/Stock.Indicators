using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StockIndicators.Tests
{
    [TestClass]
    public class MacdTests : TestBase
    {

        [TestMethod()]
        public void GetMacdTest()
        {
            int fastPeriod = 12;
            int slowPeriod = 26;
            int signalPeriod = 9;

            IEnumerable<MacdResult> results = Indicator.GetMacd(history, fastPeriod, slowPeriod, signalPeriod);

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count());
            Assert.AreEqual(502 - slowPeriod + 1, results.Where(x => x.Macd != null).Count());
            Assert.AreEqual(502 - slowPeriod - signalPeriod + 1, results.Where(x => x.Signal != null).Count());
            Assert.AreEqual(502 - slowPeriod - signalPeriod + 1, results.Where(x => x.Histogram != null).Count());
            Assert.AreEqual(502 - slowPeriod - signalPeriod, results.Where(x => x.IsBullish != null).Count());
            Assert.AreEqual(502 - slowPeriod - signalPeriod, results.Where(x => x.IsDiverging != null).Count());

            // sample value
            MacdResult result = results.Where(x => x.Date == DateTime.Parse("12/31/2018")).FirstOrDefault();
            Assert.AreEqual((decimal)-6.2198, Math.Round((decimal)result.Macd, 4));
            Assert.AreEqual((decimal)-5.8569, Math.Round((decimal)result.Signal, 4));
            Assert.AreEqual((decimal)-0.3629, Math.Round((decimal)result.Histogram, 4));
            Assert.AreEqual(false, result.IsBullish);
            Assert.AreEqual(false, result.IsDiverging);
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(BadParameterException), "Fast period must be greater than 0.")]
        public void BadFastPeriod()
        {
            Indicator.GetMacd(history, 0, 26, 9);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadParameterException), "Slow period must be greater than 0.")]
        public void BadSlowPeriod()
        {
            Indicator.GetMacd(history, 12, 0, 9);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadParameterException), "Signal period must be greater than or equal to 0.")]
        public void BadSignalPeriod()
        {
            Indicator.GetMacd(history, 12, 26, -1);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadParameterException), "Slow smaller than Fast period.")]
        public void BadFastAndSlowCombo()
        {
            Indicator.GetMacd(history, 26, 20, 9);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            Indicator.GetMacd(history.Where(x => x.Index < 61), 12, 26, 9);
        }

    }
}