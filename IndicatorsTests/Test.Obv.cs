﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StockIndicators.Tests
{
    [TestClass]
    public class ObvTests : TestBase
    {

        [TestMethod()]
        public void GetObvTest()
        {

            IEnumerable<ObvResult> results = Indicator.GetObv(history);

            // assertions

            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count());

            // sample value
            ObvResult r = results.Where(x => x.Date == DateTime.Parse("12/31/2018")).FirstOrDefault();
            Assert.AreEqual(539843504, r.Obv);
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            Indicator.GetObv(history.Where(x => x.Index < 2));
        }

    }
}