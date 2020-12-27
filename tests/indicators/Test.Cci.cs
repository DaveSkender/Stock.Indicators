using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Internal.Tests
{
    [TestClass]
    public class CciTests : TestBase
    {

        [TestMethod()]
        public void Standard()
        {
            int lookbackPeriod = 20;

            List<CciResult> results = Indicator.GetCci(history, lookbackPeriod).ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(502 - lookbackPeriod + 1, results.Where(x => x.Cci != null).Count());

            // sample value
            CciResult r = results[501];
            Assert.AreEqual(-52.9946m, Math.Round((decimal)r.Cci, 4));
        }

        [TestMethod()]
        public void BadData()
        {
            IEnumerable<CciResult> r = Indicator.GetCci(historyBad, 15);
            Assert.AreEqual(502, r.Count());
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Bad lookback period.")]
        public void BadLookbackPeriod()
        {
            Indicator.GetCci(history, 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            IEnumerable<Quote> h = History.GetHistory(30);
            Indicator.GetCci(h, 30);
        }

    }
}