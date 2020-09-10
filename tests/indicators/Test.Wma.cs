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
        public void GetWmaTest()
        {
            int lookbackPeriod = 20;
            IEnumerable<WmaResult> results = Indicator.GetWma(history, lookbackPeriod);

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count());
            Assert.AreEqual(502 - lookbackPeriod + 1, results.Where(x => x.Wma != null).Count());

            // sample values
            WmaResult r1 = results.Where(x => x.Index == 502).FirstOrDefault();
            Assert.AreEqual(246.5110m, Math.Round((decimal)r1.Wma,4));

            WmaResult r2 = results.Where(x => x.Index == 150).FirstOrDefault();
            Assert.AreEqual(235.5253m, Math.Round((decimal)r2.Wma, 4));
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(BadParameterException), "Bad lookback.")]
        public void BadLookback()
        {
            Indicator.GetWma(history, 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            Indicator.GetWma(history.Where(x => x.Index < 10), 10);
        }
    }
}