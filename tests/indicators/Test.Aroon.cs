using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Internal.Tests
{
    [TestClass]
    public class AroonTests : TestBase
    {

        [TestMethod()]
        public void GetAroonTest()
        {
            int lookbackPeriod = 25;
            IEnumerable<AroonResult> results = Indicator.GetAroon(history, lookbackPeriod);

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count());
            Assert.AreEqual(502 - lookbackPeriod + 1, results.Where(x => x.AroonUp != null).Count());

            // sample value
            AroonResult r = results.Where(x => x.Index == 502).FirstOrDefault();
            Assert.AreEqual(28.0000m, Math.Round((decimal)r.AroonUp, 4));
            Assert.AreEqual(88.0000m, Math.Round((decimal)r.AroonDown, 4));
            Assert.AreEqual(-60.0000m, Math.Round((decimal)r.Oscillator, 4));
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(BadParameterException), "Bad lookback period.")]
        public void BadLookback()
        {
            Indicator.GetAroon(history, 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            Indicator.GetAroon(history.Where(x => x.Index < 30), 30);
        }

    }
}