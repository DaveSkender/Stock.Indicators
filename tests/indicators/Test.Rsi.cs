using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Internal.Tests
{
    [TestClass]
    public class RsiTests : TestBase
    {

        [TestMethod()]
        public void GetRsiTest()
        {
            int lookbackPeriod = 14;
            IEnumerable<RsiResult> results = Indicator.GetRsi(history, lookbackPeriod);

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count());
            Assert.AreEqual(488, results.Where(x => x.Rsi != null).Count());

            // sample value
            RsiResult r = results.Where(x => x.Index == 502).FirstOrDefault();
            Assert.AreEqual(42.0773m, Math.Round((decimal)r.Rsi, 4));
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(BadParameterException), "Bad lookback.")]
        public void BadLookback()
        {
            Indicator.GetRsi(history, 1);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            Indicator.GetRsi(history.Where(x => x.Index < 30), 30);
        }

    }
}