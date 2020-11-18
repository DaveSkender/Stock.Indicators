using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Internal.Tests
{
    [TestClass]
    public class AvgTrueRangeTests : TestBase
    {

        [TestMethod()]
        public void GetAtr()
        {
            int lookbackPeriod = 14;
            List<AtrResult> results = Indicator.GetAtr(history, lookbackPeriod).ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(502 - lookbackPeriod + 1, results.Where(x => x.Atr != null).Count());

            // sample value
            AtrResult r = results[501];
            Assert.AreEqual(2.67m, Math.Round((decimal)r.Tr, 4));
            Assert.AreEqual(6.1497m, Math.Round((decimal)r.Atr, 4));
            Assert.AreEqual(2.5072m, Math.Round((decimal)r.Atrp, 4));
        }

        [TestMethod()]
        public void GetAtrBadData()
        {
            IEnumerable<AtrResult> r = Indicator.GetAtr(historyBad, 20);
            Assert.AreEqual(502, r.Count());
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Bad lookback.")]
        public void BadLookbackPeriod()
        {
            Indicator.GetAtr(history, 1);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            IEnumerable<Quote> h = History.GetHistory(30);
            Indicator.GetAtr(h, 30);
        }

    }
}