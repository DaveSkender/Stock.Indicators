using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class Vortex : TestBase
    {

        [TestMethod]
        public void Standard()
        {
            int lookbackPeriod = 14;
            List<VortexResult> results
                = Indicator.GetVortex(history, lookbackPeriod).ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(488, results.Where(x => x.Pvi != null).Count());

            // sample values
            VortexResult r1 = results[13];
            Assert.IsNull(r1.Pvi);
            Assert.IsNull(r1.Nvi);

            VortexResult r2 = results[14];
            Assert.AreEqual(1.0460m, Math.Round((decimal)r2.Pvi, 4));
            Assert.AreEqual(0.8119m, Math.Round((decimal)r2.Nvi, 4));

            VortexResult r3 = results[29];
            Assert.AreEqual(1.1300m, Math.Round((decimal)r3.Pvi, 4));
            Assert.AreEqual(0.7393m, Math.Round((decimal)r3.Nvi, 4));

            VortexResult r4 = results[249];
            Assert.AreEqual(1.1558m, Math.Round((decimal)r4.Pvi, 4));
            Assert.AreEqual(0.6634m, Math.Round((decimal)r4.Nvi, 4));

            VortexResult r5 = results[501];
            Assert.AreEqual(0.8712m, Math.Round((decimal)r5.Pvi, 4));
            Assert.AreEqual(1.1163m, Math.Round((decimal)r5.Nvi, 4));
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<VortexResult> r = Indicator.GetVortex(historyBad, 20);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Convergence()
        {
            foreach (int qty in convergeQuantities)
            {
                IEnumerable<Quote> h = HistoryTestData.GetLong(15 + qty);
                IEnumerable<VortexResult> r = Indicator.GetVortex(h, 14);

                VortexResult l = r.LastOrDefault();
                Console.WriteLine("VI+ on {0:d} with {1,4} periods: {2:N8}",
                    l.Date, h.Count(), l.Pvi);
            }
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad lookback period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetVortex(history, 1));

            // insufficient history
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetVortex(HistoryTestData.Get(30), 30));
        }
    }
}
