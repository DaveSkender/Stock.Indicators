using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Internal.Tests
{
    [TestClass]
    public class RocTests : TestBase
    {

        [TestMethod()]
        public void GetRocTest()
        {
            int lookbackPeriod = 20;
            IEnumerable<RocResult> results = Indicator.GetRoc(history, lookbackPeriod);

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count());
            Assert.AreEqual(502 - lookbackPeriod, results.Where(x => x.Roc != null).Count());

            // sample value
            RocResult r = results.Where(x => x.Index == 502).FirstOrDefault();
            Assert.AreEqual(-8.2482m, Math.Round((decimal)r.Roc, 4));
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(BadParameterException), "Bad lookback.")]
        public void BadLookback()
        {
            Indicator.GetRoc(history, 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            Indicator.GetRoc(history.Where(x => x.Index <= 10), 10);
        }
    }
}