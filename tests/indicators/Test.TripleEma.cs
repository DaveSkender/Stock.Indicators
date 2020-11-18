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
        public void GetTripleEma()
        {
            int lookbackPeriod = 20;
            List<EmaResult> results = Indicator.GetTripleEma(history, lookbackPeriod).ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(445, results.Where(x => x.Ema != null).Count());

            // sample values
            EmaResult r1 = results[501];
            Assert.AreEqual(238.7690m, Math.Round((decimal)r1.Ema, 4));

            EmaResult r2 = results[249];
            Assert.AreEqual(258.6208m, Math.Round((decimal)r2.Ema, 4));

            EmaResult r3 = results[67];
            Assert.AreEqual(222.9105m, Math.Round((decimal)r3.Ema, 4));
        }

        [TestMethod()]
        public void GetTripleEmaBadData()
        {
            IEnumerable<EmaResult> r = Indicator.GetTripleEma(historyBad, 15);
            Assert.AreEqual(502, r.Count());
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Bad lookback.")]
        public void BadLookbackPeriod()
        {
            Indicator.GetTripleEma(history, 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history for 3*N+100.")]
        public void InsufficientHistoryA()
        {
            IEnumerable<Quote> h = History.GetHistory(189);
            Indicator.GetTripleEma(h, 30);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history for 4×N.")]
        public void InsufficientHistoryB()
        {
            IEnumerable<Quote> historyLong = History.GetHistoryLong(999);
            Indicator.GetTripleEma(historyLong, 250);
        }
    }
}