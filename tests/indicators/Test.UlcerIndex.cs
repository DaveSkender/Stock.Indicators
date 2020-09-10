using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StockIndicators.Tests
{
    [TestClass]
    public class UlcerIndexTests : TestBase
    {

        [TestMethod()]
        public void GetUlcerIndexTest()
        {
            int lookbackPeriod = 14;

            IEnumerable<UlcerIndexResult> results = Indicator.GetUlcerIndex(history, lookbackPeriod);

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count());
            Assert.AreEqual(502 - lookbackPeriod + 1, results.Where(x => x.UI != null).Count());

            // sample value
            UlcerIndexResult r = results.Where(x => x.Index == 502).FirstOrDefault();
            Assert.AreEqual(5.7255m, Math.Round((decimal)r.UI, 4));
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(BadParameterException), "Bad lookback.")]
        public void BadLookback()
        {
            Indicator.GetUlcerIndex(history, 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            Indicator.GetUlcerIndex(history.Where(x => x.Index < 30), 30);
        }

    }
}