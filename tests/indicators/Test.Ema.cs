using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Internal.Tests
{
    [TestClass]
    public class EmaTests : TestBase
    {

        [TestMethod()]
        public void GetEmaTest()
        {
            int lookbackPeriod = 20;
            IEnumerable<EmaResult> results = Indicator.GetEma(history, lookbackPeriod);

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count());
            Assert.AreEqual(483, results.Where(x => x.Ema != null).Count());

            // sample values
            EmaResult r1 = results.Where(x => x.Index == 502).FirstOrDefault();
            Assert.AreEqual(249.3519m, Math.Round((decimal)r1.Ema, 4));

            EmaResult r2 = results.Where(x => x.Index == 250).FirstOrDefault();
            Assert.AreEqual(255.3873m, Math.Round((decimal)r2.Ema, 4));

            EmaResult r3 = results.Where(x => x.Index == 30).FirstOrDefault();
            Assert.AreEqual(216.6228m, Math.Round((decimal)r3.Ema, 4));
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Bad lookback.")]
        public void BadLookbackPeriod()
        {
            Indicator.GetEma(history, 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history for N+100.")]
        public void InsufficientHistoryA()
        {
            Indicator.GetEma(history.Where(x => x.Index < 130), 30);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history for 2×N.")]
        public void InsufficientHistoryB()
        {
            Indicator.GetEma(history.Where(x => x.Index < 500), 250);
        }

    }
}