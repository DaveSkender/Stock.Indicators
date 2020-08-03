using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StockIndicators.Tests
{
    [TestClass]
    public class StandardDevTests : TestBase
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
            StdDevResult r = results.Where(x => x.Index == 502).FirstOrDefault();

            Assert.AreEqual((decimal)5.4738, Math.Round((decimal)r.StdDev, 4));
            Assert.AreEqual((decimal)0.524312, Math.Round((decimal)r.ZScore, 6));
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(BadParameterException), "Bad lookback.")]
        public void BadLookback()
        {
            Indicator.GetStdDev(history, 1);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            Indicator.GetStdDev(history.Where(x => x.Index < 30), 30);
        }

    }
}