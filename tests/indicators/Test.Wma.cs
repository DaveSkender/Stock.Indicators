using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class Wma : TestBase
    {

        [TestMethod]
        public void Standard()
        {
            int lookbackPeriod = 20;

            List<WmaResult> results = Indicator.GetWma(history, lookbackPeriod)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(483, results.Where(x => x.Wma != null).Count());

            // sample values
            WmaResult r1 = results[149];
            Assert.AreEqual(235.5253m, Math.Round((decimal)r1.Wma, 4));

            WmaResult r2 = results[501];
            Assert.AreEqual(246.5110m, Math.Round((decimal)r2.Wma, 4));
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<WmaResult> r = Indicator.GetWma(historyBad, 15);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad lookback period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetWma(history, 0));

            // insufficient history
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetWma(HistoryTestData.Get(9), 10));
        }
    }
}