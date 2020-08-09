﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StockIndicators.Tests
{
    [TestClass]
    public class MfiTests : TestBase
    {

        [TestMethod()]
        public void GetMfiTest()
        {
            int lookbackPeriod = 14;
            IEnumerable<MfiResult> results = Indicator.GetMfi(history, lookbackPeriod);

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count());
            Assert.AreEqual(502 - lookbackPeriod, results.Where(x => x.Mfi != null).Count());

            // sample values
            MfiResult r1 = results.Where(x => x.Index == 502).FirstOrDefault();
            Assert.AreEqual((decimal)39.9494, Math.Round((decimal)r1.Mfi, 4));

            MfiResult r2 = results.Where(x => x.Index == 440).FirstOrDefault();
            Assert.AreEqual((decimal)69.0622, Math.Round((decimal)r2.Mfi, 4));
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(BadParameterException), "Bad lookback.")]
        public void BadLookback()
        {
            Indicator.GetMfi(history, 1);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history for N+1.")]
        public void InsufficientHistoryA()
        {
            Indicator.GetMfi(history.Where(x => x.Index < 15), 14);
        }

    }
}