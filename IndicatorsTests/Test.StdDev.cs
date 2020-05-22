using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StockIndicators.Tests
{
    [TestClass]
    public class StdDevTests : TestBase
    {

        [TestMethod()]
        public void GetStdDevTest()
        {
            int lookbackPeriod = 10;
            IEnumerable<StdDevResult> results = Indicator.GetStdDev(history, lookbackPeriod);

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count());
            Assert.AreEqual(502 - lookbackPeriod + 1, results.Where(x => x.StdDev != null).Count());

            // sample value
            StdDevResult sd = results.Where(x => x.Date == DateTime.Parse("12/31/2018")).FirstOrDefault();
            Assert.AreEqual((decimal)5.4738, Math.Round((decimal)sd.StdDev, 4));
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            Indicator.GetStdDev(history.Where(x => x.Index < 30), 30);
        }

    }
}