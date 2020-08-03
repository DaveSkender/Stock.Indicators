using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StockIndicators.Tests
{
    [TestClass]
    public class AvgDirectionalTests : TestBase
    {

        [TestMethod()]
        public void GetAdxTest()
        {
            int lookbackPeriod = 14;
            IEnumerable<AdxResult> results = Indicator.GetAdx(history, lookbackPeriod);

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count());
            Assert.AreEqual(502 - 2 * lookbackPeriod + 1, results.Where(x => x.Adx != null).Count());

            // sample value
            AdxResult r = results.Where(x => x.Index == 502).FirstOrDefault();
            Assert.AreEqual((decimal)17.7565, Math.Round((decimal)r.Pdi, 4));
            Assert.AreEqual((decimal)31.1510, Math.Round((decimal)r.Mdi, 4));
            Assert.AreEqual((decimal)34.2987, Math.Round((decimal)r.Adx, 4));
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(BadParameterException), "Bad lookback period.")]
        public void BadLookback()
        {
            Indicator.GetAdx(history, 1);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            Indicator.GetAdx(history.Where(x => x.Index < 61), 30);
        }

    }
}