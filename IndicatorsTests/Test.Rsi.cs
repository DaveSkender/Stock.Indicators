using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StockIndicators.Tests
{
    [TestClass]
    public class RsiTests : TestBase
    {

        [TestMethod()]
        public void GetRsiTest()
        {
            int lookbackPeriod = 14;

            IEnumerable<RsiResult> results = Indicator.GetRsi(history, lookbackPeriod);

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count());
            Assert.AreEqual(502 - lookbackPeriod + 1, results.Where(x => x.Rsi != null).Count());
            Assert.AreEqual(502 - lookbackPeriod + 1, results.Where(x => x.IsIncreasing != null).Count());

            // sample value
            RsiResult result = results.Where(x => x.Date == DateTime.Parse("12/31/2018")).FirstOrDefault();
            Assert.AreEqual((decimal)42.0773, Math.Round((decimal)result.Rsi, 4));
            Assert.AreEqual(true, result.IsIncreasing);
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            Indicator.GetRsi(history.Where(x => x.Index < 30), 30);
        }

    }
}