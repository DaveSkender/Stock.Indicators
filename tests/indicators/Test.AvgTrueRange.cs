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
        public void GetAtrTest()
        {
            int lookbackPeriod = 14;
            IEnumerable<AtrResult> results = Indicator.GetAtr(history, lookbackPeriod);

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count());
            Assert.AreEqual(502 - lookbackPeriod + 1, results.Where(x => x.Atr != null).Count());

            // sample value
            AtrResult r = results.Where(x => x.Index == 502).FirstOrDefault();
            Assert.AreEqual(2.67m, Math.Round((decimal)r.Tr, 4));
            Assert.AreEqual(6.1497m, Math.Round((decimal)r.Atr, 4));
            Assert.AreEqual(2.5072m, Math.Round((decimal)r.Atrp, 4));
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(BadParameterException), "Bad lookback.")]
        public void BadLookback()
        {
            Indicator.GetAtr(history, 1);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            Indicator.GetAtr(history.Where(x => x.Index < 31), 30);
        }

    }
}