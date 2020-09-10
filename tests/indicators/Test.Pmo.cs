﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StockIndicators.Tests
{
    [TestClass]
    public class PmoTests : TestBase
    {

        [TestMethod()]
        public void GetPmoTest()
        {
            IEnumerable<PmoResult> results = Indicator.GetPmo(history, 35, 20, 10);

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count());
            Assert.AreEqual(448, results.Where(x => x.Pmo != null).Count());
            Assert.AreEqual(439, results.Where(x => x.Signal != null).Count());

            // sample values
            PmoResult r1 = results.Where(x => x.Index == 93).FirstOrDefault();
            Assert.AreEqual(0.6159m, Math.Round((decimal)r1.Pmo, 4));
            Assert.AreEqual(0.5582m, Math.Round((decimal)r1.Signal, 4));

            PmoResult r2 = results.Where(x => x.Index == 502).FirstOrDefault();
            Assert.AreEqual(-2.7016m, Math.Round((decimal)r2.Pmo, 4));
            Assert.AreEqual(-2.3117m, Math.Round((decimal)r2.Signal, 4));
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(BadParameterException), "Bad time period.")]
        public void BadTimePeriod()
        {
            Indicator.GetPmo(history, 1);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadParameterException), "Bad smoothing period.")]
        public void BadSmoothingPeriod()
        {
            Indicator.GetPmo(history, 5, 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadParameterException), "Bad signal period.")]
        public void BadSignalPeriod()
        {
            Indicator.GetPmo(history, 5, 5, 0);
        }


        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            Indicator.GetPmo(history.Where(x => x.Index < 55), 35, 20, 10);
        }
    }
}