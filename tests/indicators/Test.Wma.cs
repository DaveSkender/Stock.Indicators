using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Internal.Tests
{
    [TestClass]
    public class WmaTests : TestBase
    {

        [TestMethod()]
        public void GetWma()
        {
            int lookbackPeriod = 20;
            List<WmaResult> results = Indicator.GetWma(history, lookbackPeriod).ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(502 - lookbackPeriod + 1, results.Where(x => x.Wma != null).Count());

            // sample values
            WmaResult r1 = results[501];
            Assert.AreEqual(246.5110m, Math.Round((decimal)r1.Wma, 4));

            WmaResult r2 = results[149];
            Assert.AreEqual(235.5253m, Math.Round((decimal)r2.Wma, 4));
        }

        [TestMethod()]
        public void GetWmaBadData()
        {
            IEnumerable<WmaResult> r = Indicator.GetWma(historyBad, 15);
            Assert.AreEqual(502, r.Count());
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Bad lookback.")]
        public void BadLookbackPeriod()
        {
            Indicator.GetWma(history, 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            IEnumerable<Quote> h = History.GetHistory(9);
            Indicator.GetWma(h, 10);
        }

    }
}