using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Internal.Tests
{
    [TestClass]
    public class DoubleEmaTests : TestBase
    {

        [TestMethod()]
        public void GetDoubleEmaTest()
        {
            int lookbackPeriod = 20;
            IEnumerable<EmaResult> results = Indicator.GetDoubleEma(history, lookbackPeriod);

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count());
            Assert.AreEqual(464, results.Where(x => x.Ema != null).Count());

            // sample values
            EmaResult r1 = results.Where(x => x.Index == 502).FirstOrDefault();
            Assert.AreEqual(241.1677m, Math.Round((decimal)r1.Ema, 4));

            EmaResult r2 = results.Where(x => x.Index == 250).FirstOrDefault();
            Assert.AreEqual(258.4452m, Math.Round((decimal)r2.Ema, 4));

            EmaResult r3 = results.Where(x => x.Index == 52).FirstOrDefault();
            Assert.AreEqual(226.0011m, Math.Round((decimal)r3.Ema, 4));
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(BadParameterException), "Bad lookback.")]
        public void BadLookback()
        {
            Indicator.GetDoubleEma(history, 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history for 2*N+100.")]
        public void InsufficientHistoryA()
        {
            Indicator.GetDoubleEma(history.Where(x => x.Index < 160), 30);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history for 3×N.")]
        public void InsufficientHistoryB()
        {
            Indicator.GetDoubleEma(history.Where(x => x.Index < 750), 250);
        }

    }
}