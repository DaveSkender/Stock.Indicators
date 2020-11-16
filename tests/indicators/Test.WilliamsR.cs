using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Internal.Tests
{
    [TestClass]
    public class WilliamsRTests : TestBase
    {

        [TestMethod()]
        public void GetWilliamsRTest()
        {
            int lookbackPeriod = 14;
            List<WilliamsResult> results = Indicator.GetWilliamsR(history, lookbackPeriod).ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(502 - lookbackPeriod + 1, results.Where(x => x.WilliamsR != null).Count());

            // sample values
            WilliamsResult r1 = results[501];
            Assert.AreEqual(-52.0121m, Math.Round((decimal)r1.WilliamsR, 4));

            WilliamsResult r2 = results[343];
            Assert.AreEqual(-19.8211m, Math.Round((decimal)r2.WilliamsR, 4));
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Bad lookback.")]
        public void BadLookbackPeriod()
        {
            Indicator.GetWilliamsR(history, 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            Indicator.GetWilliamsR(history.Where(x => x.Index < 30), 30);
        }

    }
}