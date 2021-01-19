using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class UlcerIndex : TestBase
    {

        [TestMethod]
        public void Standard()
        {
            int lookbackPeriod = 14;

            List<UlcerIndexResult> results = Indicator.GetUlcerIndex(history, lookbackPeriod)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(489, results.Where(x => x.UI != null).Count());

            // sample value
            UlcerIndexResult r = results[501];
            Assert.AreEqual(5.7255m, Math.Round((decimal)r.UI, 4));
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<UlcerIndexResult> r = Indicator.GetUlcerIndex(historyBad, 15);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad lookback period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetUlcerIndex(history, 0));

            // insufficient history
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetUlcerIndex(HistoryTestData.Get(29), 30));
        }
    }
}