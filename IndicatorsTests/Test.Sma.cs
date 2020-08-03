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
            IEnumerable<SmaResult> results = Indicator.GetSma(history, lookbackPeriod);

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count());
            Assert.AreEqual(502 - lookbackPeriod + 1, results.Where(x => x.Sma != null).Count());

            // sample value
            SmaResult r = results.Where(x => x.Index == 502).FirstOrDefault();
            Assert.AreEqual((decimal)251.86, r.Sma);
            Assert.AreEqual((decimal)9.45, r.Mad);
            Assert.AreEqual((double)119.2510, Math.Round((double)r.Mse, 4));
            Assert.AreEqual((double)0.037637, Math.Round((double)r.Mape, 6));
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