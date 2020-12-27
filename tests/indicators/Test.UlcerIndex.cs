using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Internal.Tests
{
    [TestClass]
    public class UlcerIndexTests : TestBase
    {

        [TestMethod()]
        public void Standard()
        {
            int lookbackPeriod = 14;
            List<UlcerIndexResult> results = Indicator.GetUlcerIndex(history, lookbackPeriod).ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(502 - lookbackPeriod + 1, results.Where(x => x.UI != null).Count());

            // sample value
            UlcerIndexResult r = results[501];
            Assert.AreEqual(5.7255m, Math.Round((decimal)r.UI, 4));
        }

        [TestMethod()]
        public void BadData()
        {
            IEnumerable<UlcerIndexResult> r = Indicator.GetUlcerIndex(historyBad, 15);
            Assert.AreEqual(502, r.Count());
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Bad lookback.")]
        public void BadLookbackPeriod()
        {
            Indicator.GetUlcerIndex(history, 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            IEnumerable<Quote> h = History.GetHistory(29);
            Indicator.GetUlcerIndex(h, 30);
        }

    }
}