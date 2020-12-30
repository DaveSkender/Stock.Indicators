﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Internal.Tests
{
    [TestClass]
    public class AwesomeTests : TestBase
    {

        [TestMethod()]
        public void Standard()
        {

            List<AwesomeResult> results = Indicator.GetAwesome(history, 5, 34).ToList();

            // assertions

            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(469, results.Where(x => x.Oscillator != null).Count());

            // sample values
            AwesomeResult r1 = results[501];
            Assert.AreEqual(-17.7692m, Math.Round((decimal)r1.Oscillator, 4));
            Assert.AreEqual(-7.2763m, Math.Round((decimal)r1.Normalized, 4));

            AwesomeResult r2 = results[249];
            Assert.AreEqual(5.0618m, Math.Round((decimal)r2.Oscillator, 4));
            Assert.AreEqual(1.9634m, Math.Round((decimal)r2.Normalized, 4));

            AwesomeResult r3 = results[33];
            Assert.AreEqual(5.4756m, Math.Round((decimal)r3.Oscillator, 4));
            Assert.AreEqual(2.4548m, Math.Round((decimal)r3.Normalized, 4));

            AwesomeResult r4 = results[32];
            Assert.AreEqual(null, r4.Oscillator);
            Assert.AreEqual(null, r4.Normalized);
        }

        [TestMethod()]
        public void BadData()
        {
            IEnumerable<AwesomeResult> r = Indicator.GetAwesome(historyBad);
            Assert.AreEqual(502, r.Count());
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Bad fast period.")]
        public void BadFastPeriod()
        {
            Indicator.GetAwesome(history, 0, 34);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Bad slow period.")]
        public void BadSlowPeriod()
        {
            Indicator.GetAwesome(history, 25, 25);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            IEnumerable<Quote> h = History.GetHistory(33);
            Indicator.GetAwesome(h, 5, 34);
        }

    }
}