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
        public void GetDoubleEma()
        {
            int lookbackPeriod = 20;
            List<EmaResult> results = Indicator.GetDoubleEma(history, lookbackPeriod).ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(464, results.Where(x => x.Ema != null).Count());

            // sample values
            EmaResult r1 = results[501];
            Assert.AreEqual(241.1677m, Math.Round((decimal)r1.Ema, 4));

            EmaResult r2 = results[249];
            Assert.AreEqual(258.4452m, Math.Round((decimal)r2.Ema, 4));

            EmaResult r3 = results[51];
            Assert.AreEqual(226.0011m, Math.Round((decimal)r3.Ema, 4));
        }

        [TestMethod()]
        public void GetDoubleEmaBadData()
        {
            IEnumerable<EmaResult> r = Indicator.GetDoubleEma(historyBad, 15);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod()]
        public void GetDemaConvergence()
        {
            foreach (int qty in convergeQuantities)
            {
                IEnumerable<Quote> h = History.GetHistoryLong(130 + qty);
                IEnumerable<EmaResult> r = Indicator.GetDoubleEma(h, 15);

                EmaResult l = r.LastOrDefault();
                Console.WriteLine("DEMA on {0:d} with {1,4} periods: {2:N8}",
                    l.Date, h.Count(), l.Ema);
            }
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Bad lookback.")]
        public void BadLookbackPeriod()
        {
            Indicator.GetDoubleEma(history, 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history for 2*N+100.")]
        public void InsufficientHistoryA()
        {
            IEnumerable<Quote> h = History.GetHistory(159);
            Indicator.GetDoubleEma(h, 30);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history for 3×N.")]
        public void InsufficientHistoryB()
        {
            IEnumerable<Quote> historyLong = History.GetHistoryLong(749);
            Indicator.GetDoubleEma(historyLong, 250);
        }

    }
}