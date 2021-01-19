using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class TripleEma : TestBase
    {

        [TestMethod]
        public void Standard()
        {
            int lookbackPeriod = 20;

            List<EmaResult> results = Indicator.GetTripleEma(history, lookbackPeriod)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(445, results.Where(x => x.Ema != null).Count());

            // sample values
            EmaResult r1 = results[67];
            Assert.AreEqual(222.9105m, Math.Round((decimal)r1.Ema, 4));

            EmaResult r2 = results[249];
            Assert.AreEqual(258.6208m, Math.Round((decimal)r2.Ema, 4));

            EmaResult r3 = results[501];
            Assert.AreEqual(238.7690m, Math.Round((decimal)r3.Ema, 4));
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<EmaResult> r = Indicator.GetTripleEma(historyBad, 15);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Convergence()
        {
            foreach (int qty in convergeQuantities)
            {
                IEnumerable<Quote> h = HistoryTestData.GetLong(145 + qty);
                IEnumerable<EmaResult> r = Indicator.GetTripleEma(h, 15);

                EmaResult l = r.LastOrDefault();
                Console.WriteLine("TEMA on {0:d} with {1,4} periods: {2:N8}",
                    l.Date, h.Count(), l.Ema);
            }
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad lookback period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetTripleEma(history, 0));

            // insufficient history for 3*N+100
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetTripleEma(HistoryTestData.Get(189), 30));

            // insufficient history for 4×N
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetTripleEma(HistoryTestData.GetLong(999), 250));
        }
    }
}