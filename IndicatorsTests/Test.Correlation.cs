﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StockIndicators.Tests
{
    [TestClass]
    public class CorrelationTests : TestBase
    {

        [TestMethod()]
        public void GetCorrelationTest()
        {
            int lookbackPeriod = 20;

            IEnumerable<CorrResult> results = Indicator.GetCorrelation(history, historyOther, lookbackPeriod);

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count());
            Assert.AreEqual(502 - lookbackPeriod + 1, results.Where(x => x.Correlation != null).Count());

            // sample value
            CorrResult result = results.Where(x => x.Date == DateTime.Parse("12/31/2018")).FirstOrDefault();
            Assert.AreEqual((decimal)0.8460, Math.Round((decimal)result.Correlation, 4));
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(BadParameterException), "Bad lookback.")]
        public void BadLookback()
        {
            Indicator.GetCorrelation(history, historyOther, 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            Indicator.GetCorrelation(history.Where(x => x.Index < 30), historyOther.Where(x => x.Index < 30), 30);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Not enought Eval history.")]
        public void InsufficientEvalHistory()
        {
            Indicator.GetCorrelation(history, historyOther.Where(x => x.Index <= 300), 30);
        }

    }
}