using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class Smma : TestBase
    {
        [TestMethod]
        public void Standard()
        {
            int lookbackPeriod = 20;
            List<SmmaResult> results = history.GetSmma(lookbackPeriod).ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(483, results.Where(x => x.Smma != null).Count());

            // starting calculations at proper index
            Assert.IsNull(results[18].Smma);
            Assert.IsNotNull(results[19].Smma);

            // sample values
            Assert.AreEqual(214.52500m, Math.Round(results[19].Smma.Value, 5));
            Assert.AreEqual(214.55125m, Math.Round(results[20].Smma.Value, 5));
            Assert.AreEqual(214.58319m, Math.Round(results[21].Smma.Value, 5));
            Assert.AreEqual(225.78071m, Math.Round(results[100].Smma.Value, 5));
            Assert.AreEqual(255.67462m, Math.Round(results[501].Smma.Value, 5));
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<SmmaResult> r = Indicator.GetSmma(historyBad, 15);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad lookback period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetSmma(history, 0));

            // insufficient history for N+100
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetSmma(HistoryTestData.Get(129), 30));

            // insufficient history for 2×N
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetSmma(HistoryTestData.Get(499), 250));
        }
    }
}
