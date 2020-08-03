using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StockIndicators.Tests
{
    [TestClass]
    public class WilliamRTests : TestBase
    {

        [TestMethod()]
        public void GetWilliamRTest()
        {
            int lookbackPeriod = 14;
            IEnumerable<WilliamResult> results = Indicator.GetWilliamR(history, lookbackPeriod);

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count());
            Assert.AreEqual(502 - lookbackPeriod + 1, results.Where(x => x.WilliamR != null).Count());

            // sample values
            WilliamResult r1 = results.Where(x => x.Index == 502).FirstOrDefault();
            Assert.AreEqual((decimal)-52.0121, Math.Round((decimal)r1.WilliamR, 4));
            Assert.AreEqual(true, r1.IsIncreasing);

            WilliamResult r2 = results.Where(x => x.Index == 344).FirstOrDefault();
            Assert.AreEqual((decimal)-19.8211, Math.Round((decimal)r2.WilliamR, 4));
            Assert.AreEqual(false, r2.IsIncreasing);
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(BadParameterException), "Bad lookback.")]
        public void BadLookback()
        {
            Indicator.GetWilliamR(history, 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            Indicator.GetWilliamR(history.Where(x => x.Index < 30), 30);
        }

    }
}