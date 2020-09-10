using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Internal.Tests
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
            Assert.AreEqual(477, results.Where(x => x.Macd != null).Count());
            Assert.AreEqual(469, results.Where(x => x.Signal != null).Count());
            Assert.AreEqual(469, results.Where(x => x.Histogram != null).Count());

            // sample values
            MacdResult r1 = results.Where(x => x.Index == 502).FirstOrDefault();
            Assert.AreEqual(-6.2198m, Math.Round((decimal)r1.Macd, 4));
            Assert.AreEqual(-5.8569m, Math.Round((decimal)r1.Signal, 4));
            Assert.AreEqual(-0.3629m, Math.Round((decimal)r1.Histogram, 4));

            MacdResult r2 = results.Where(x => x.Index == 50).FirstOrDefault();
            Assert.AreEqual(1.7203m, Math.Round((decimal)r2.Macd, 4));
            Assert.AreEqual(1.9675m, Math.Round((decimal)r2.Signal, 4));
            Assert.AreEqual(-0.2472m, Math.Round((decimal)r2.Histogram, 4));

            MacdResult r3 = results.Where(x => x.Index == 250).FirstOrDefault();
            Assert.AreEqual(2.2353m, Math.Round((decimal)r3.Macd, 4));
            Assert.AreEqual(2.3141m, Math.Round((decimal)r3.Signal, 4));
            Assert.AreEqual(-0.0789m, Math.Round((decimal)r3.Histogram, 4));
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