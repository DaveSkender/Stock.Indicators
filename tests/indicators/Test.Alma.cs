using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class Alma : TestBase
    {

        [TestMethod]
        public void Standard()
        {
            int lookbackPeriod = 10;
            double offset = 0.85;
            double sigma = 6;

            List<AlmaResult> results = history.GetAlma(lookbackPeriod, offset, sigma)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(493, results.Where(x => x.Alma != null).Count());

            // sample values
            AlmaResult r1 = results[8];
            Assert.AreEqual(null, r1.Alma);

            AlmaResult r2 = results[9];
            Assert.AreEqual(214.1839m, Math.Round((decimal)r2.Alma, 4));

            AlmaResult r3 = results[24];
            Assert.AreEqual(216.0619m, Math.Round((decimal)r3.Alma, 4));

            AlmaResult r4 = results[149];
            Assert.AreEqual(235.8609m, Math.Round((decimal)r4.Alma, 4));

            AlmaResult r5 = results[249];
            Assert.AreEqual(257.5787m, Math.Round((decimal)r5.Alma, 4));

            AlmaResult r6 = results[501];
            Assert.AreEqual(242.1871m, Math.Round((decimal)r6.Alma, 4));
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<AlmaResult> r = Indicator.GetAlma(historyBad, 14, 0.5, 3);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad lookback period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetAlma(history, 0, 1, 5));

            // bad offset
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetAlma(history, 15, 1.1, 3));

            // bad sigma
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetAlma(history, 10, 0.5, 0));

            // insufficient history
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetAlma(HistoryTestData.Get(10), 11, 0.5));
        }
    }
}
