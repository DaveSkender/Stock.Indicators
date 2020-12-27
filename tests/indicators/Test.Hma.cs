﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Internal.Tests
{
    [TestClass]
    public class HmaTests : TestBase
    {

        [TestMethod()]
        public void Standard()
        {
            int lookbackPeriod = 20;
            List<HmaResult> results = Indicator.GetHma(history, lookbackPeriod).ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(480, results.Where(x => x.Hma != null).Count());

            // sample values
            HmaResult r1 = results[149];
            Assert.AreEqual(236.0835m, Math.Round((decimal)r1.Hma, 4));

            HmaResult r2 = results[501];
            Assert.AreEqual(235.6972m, Math.Round((decimal)r2.Hma, 4));
        }

        [TestMethod()]
        public void BadData()
        {
            IEnumerable<HmaResult> r = Indicator.GetHma(historyBad, 15);
            Assert.AreEqual(502, r.Count());
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Bad lookback.")]
        public void BadLookbackPeriod()
        {
            Indicator.GetHma(history, 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            IEnumerable<Quote> h = History.GetHistory(9);
            Indicator.GetHma(h, 10);
        }

    }
}