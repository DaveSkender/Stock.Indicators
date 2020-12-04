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

            // sample values
            AtrResult r1 = results[501];
            Assert.AreEqual(2.67m, r1.Tr);
            Assert.AreEqual(6.1497m, Math.Round((decimal)r1.Atr, 4));
            Assert.AreEqual(2.5072m, Math.Round((decimal)r1.Atrp, 4));

            AtrResult r2 = results[249];
            Assert.AreEqual(0.58m, r2.Tr);
            Assert.AreEqual(1.3381m, Math.Round((decimal)r2.Atr, 4));
            Assert.AreEqual(0.5187m, Math.Round((decimal)r2.Atrp, 4));

            AtrResult r3 = results[24];
            Assert.AreEqual(0.88m, r3.Tr);
            Assert.AreEqual(1.3201m, Math.Round((decimal)r3.Atr, 4));
            Assert.AreEqual(0.6104m, Math.Round((decimal)r3.Atrp, 4));

            AtrResult r4 = results[13];
            Assert.AreEqual(1.45m, r4.Tr);
            Assert.AreEqual(1.3371m, Math.Round((decimal)r4.Atr, 4));
            Assert.AreEqual(0.6258m, Math.Round((decimal)r4.Atrp, 4));

            AtrResult r5 = results[12];
            Assert.AreEqual(1.32m, r5.Tr);
            Assert.AreEqual(null, r5.Atr);
            Assert.AreEqual(null, r5.Atrp);
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