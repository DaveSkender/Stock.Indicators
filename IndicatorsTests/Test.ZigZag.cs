using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StockIndicators.Tests
{
    [TestClass]
    public class ZigZagTests : TestBase
    {

        [TestMethod()]
        public void GetZigZagTest()
        {
            int percentChange = 5;
            IEnumerable<ZigZagResult> results = Indicator.GetZigZag(history, percentChange);

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count());
            Assert.AreEqual(502 - percentChange + 1, results.Where(x => x.ZigZag != null).Count());

            // sample value
            ZigZagResult r = results.Where(x => x.Index == 502).FirstOrDefault();
            Assert.AreEqual((decimal)251.86, r.ZigZag);
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(BadParameterException), "Bad lookback.")]
        public void BadLookback()
        {
            Indicator.GetZigZag(history, 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            Indicator.GetZigZag(history.Where(x => x.Index < 2), 5);
        }
    }
}