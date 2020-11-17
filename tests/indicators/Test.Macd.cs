﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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

            List<MacdResult> results =
                Indicator.GetMacd(history, fastPeriod, slowPeriod, signalPeriod)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(477, results.Where(x => x.Macd != null).Count());
            Assert.AreEqual(469, results.Where(x => x.Signal != null).Count());
            Assert.AreEqual(469, results.Where(x => x.Histogram != null).Count());

            // sample values
            MacdResult r1 = results[501];
            Assert.AreEqual(-6.2198m, Math.Round((decimal)r1.Macd, 4));
            Assert.AreEqual(-5.8569m, Math.Round((decimal)r1.Signal, 4));
            Assert.AreEqual(-0.3629m, Math.Round((decimal)r1.Histogram, 4));

            MacdResult r2 = results[49];
            Assert.AreEqual(1.7203m, Math.Round((decimal)r2.Macd, 4));
            Assert.AreEqual(1.9675m, Math.Round((decimal)r2.Signal, 4));
            Assert.AreEqual(-0.2472m, Math.Round((decimal)r2.Histogram, 4));

            MacdResult r3 = results[249];
            Assert.AreEqual(2.2353m, Math.Round((decimal)r3.Macd, 4));
            Assert.AreEqual(2.3141m, Math.Round((decimal)r3.Signal, 4));
            Assert.AreEqual(-0.0789m, Math.Round((decimal)r3.Histogram, 4));
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Fast period must be greater than 0.")]
        public void BadFastPeriod()
        {
            Indicator.GetMacd(history, 0, 26, 9);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Slow period must be greater than 0.")]
        public void BadSlowPeriod()
        {
            Indicator.GetMacd(history, 12, 0, 9);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Signal period must be greater than or equal to 0.")]
        public void BadSignalPeriod()
        {
            Indicator.GetMacd(history, 12, 26, -1);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Slow smaller than Fast period.")]
        public void BadFastAndSlowCombo()
        {
            Indicator.GetMacd(history, 26, 20, 9);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            IEnumerable<Quote> h = History.GetHistory(60);
            Indicator.GetMacd(h, 12, 26, 9);
        }

    }
}