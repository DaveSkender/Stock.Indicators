using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Internal.Tests
{
    [TestClass]
    public class TemaTests : TestBase
    {

        [TestMethod()]
        public void GetTripleEmaTest()
        {
            int lookbackPeriod = 20;
            IEnumerable<EmaResult> results = Indicator.GetTripleEma(history, lookbackPeriod);

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count());
            Assert.AreEqual(445, results.Where(x => x.Ema != null).Count());

            // sample values
            EmaResult r1 = results.Where(x => x.Index == 502).FirstOrDefault();
            Assert.AreEqual(238.7690m, Math.Round((decimal)r1.Ema, 4));

            EmaResult r2 = results.Where(x => x.Index == 250).FirstOrDefault();
            Assert.AreEqual(258.6208m, Math.Round((decimal)r2.Ema, 4));

            EmaResult r3 = results.Where(x => x.Index == 68).FirstOrDefault();
            Assert.AreEqual(222.9105m, Math.Round((decimal)r3.Ema, 4));
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(BadParameterException), "Bad lookback.")]
        public void BadLookback()
        {
            Indicator.GetTripleEma(history, 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history for 3*N+100.")]
        public void InsufficientHistoryA()
        {
            Indicator.GetDoubleEma(history.Where(x => x.Index < 190), 30);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history for 4×N.")]
        public void InsufficientHistoryB()
        {
            IEnumerable<Quote> historyLong = History.GetHistoryLong(999);
            Indicator.GetDoubleEma(historyLong, 250);
        }
    }
}