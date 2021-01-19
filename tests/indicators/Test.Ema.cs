using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class Ema : TestBase
    {

        [TestMethod]
        public void Standard()
        {
            int lookbackPeriod = 20;

            List<EmaResult> results = Indicator.GetEma(history, lookbackPeriod)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(483, results.Where(x => x.Ema != null).Count());

            // sample values
            EmaResult r1 = results[29];
            Assert.AreEqual(216.6228m, Math.Round((decimal)r1.Ema, 4));

            EmaResult r2 = results[249];
            Assert.AreEqual(255.3873m, Math.Round((decimal)r2.Ema, 4));

            EmaResult r3 = results[501];
            Assert.AreEqual(249.3519m, Math.Round((decimal)r3.Ema, 4));
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<EmaResult> r = Indicator.GetEma(historyBad, 15);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Convergence()
        {
            foreach (int qty in convergeQuantities)
            {
                IEnumerable<Quote> h = HistoryTestData.GetLong(115 + qty);
                IEnumerable<EmaResult> r = Indicator.GetEma(h, 15);

                EmaResult l = r.LastOrDefault();
                Console.WriteLine("EMA on {0:d} with {1,4} periods: {2:N8}",
                    l.Date, h.Count(), l.Ema);
            }
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad lookback period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetEma(history, 0));

            // insufficient history for N+100
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetEma(HistoryTestData.Get(129), 30));

            // insufficient history for 2×N
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetEma(HistoryTestData.Get(499), 250));
        }
    }
}