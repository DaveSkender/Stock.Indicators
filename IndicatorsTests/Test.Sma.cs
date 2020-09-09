using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StockIndicators.Tests
{
    [TestClass]
    public class SmaTests : TestBase
    {

        [TestMethod()]
        public void GetSmaTest()
        {
            int lookbackPeriod = 20;
            IEnumerable<SmaResult> results = Indicator.GetSma(history, lookbackPeriod, true);

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count());
            Assert.AreEqual(502 - lookbackPeriod + 1, results.Where(x => x.Sma != null).Count());

            // sample value
            SmaResult r = results.Where(x => x.Index == 502).FirstOrDefault();
            Assert.AreEqual(251.86m, r.Sma);
            Assert.AreEqual(9.45m, r.Mad);
            Assert.AreEqual(119.2510m, Math.Round((decimal)r.Mse, 4));
            Assert.AreEqual(0.037637m, Math.Round((decimal)r.Mape, 6));
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(BadParameterException), "Bad lookback.")]
        public void BadLookback()
        {
            Indicator.GetSma(history, 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            Indicator.GetSma(history.Where(x => x.Index < 10), 10);
        }
    }
}