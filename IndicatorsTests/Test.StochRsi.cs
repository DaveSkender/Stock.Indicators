using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StockIndicators.Tests
{
    [TestClass]
    public class StochRsiTests : TestBase
    {

        [TestMethod()]
        public void GetStochRsiTest()
        {
            int lookbackPeriod = 14;

            IEnumerable<StochRsiResult> results = Indicator.GetStochRsi(history, lookbackPeriod);

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count());
            Assert.AreEqual(502 - 2 * lookbackPeriod + 1, results.Where(x => x.StochRsi != null).Count());
            Assert.AreEqual(502 - 2 * lookbackPeriod + 1, results.Where(x => x.IsIncreasing != null).Count());

            // sample value
            StochRsiResult result = results.Where(x => x.Date == DateTime.Parse("12/31/2018")).FirstOrDefault();
            Assert.AreEqual((decimal)0.9752, Math.Round((decimal)result.StochRsi, 4));
            Assert.AreEqual(true, result.IsIncreasing);
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            Indicator.GetStochRsi(history.Where(x => x.Index < 60), 30);
        }

    }
}