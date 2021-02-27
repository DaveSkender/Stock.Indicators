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

            List<SmmaResult> results = Indicator.GetSmma(history, lookbackPeriod)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(502 - lookbackPeriod + 1, results.Where(x => x.Smma != null).Count());

            // sample value
            Assert.AreEqual(214.52500m, Math.Round(results[19].Smma.Value, 5));
            Assert.AreEqual(214.55125m, Math.Round(results[20].Smma.Value, 5));
            Assert.AreEqual(214.58319m, Math.Round(results[21].Smma.Value, 5));
            Assert.AreEqual(225.78071m, Math.Round(results[100].Smma.Value, 5));
            Assert.AreEqual(255.67462m, Math.Round(results[501].Smma.Value, 5));
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad lookback period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetSmma(history, 0));

            // insufficient history
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetSmma(HistoryTestData.Get(9), 10));
        }
    }
}
