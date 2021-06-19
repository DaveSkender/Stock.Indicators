using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class Pmo : TestBase
    {

        [TestMethod]
        public void Standard()
        {

            List<PmoResult> results = history.GetPmo(35, 20, 10).ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(448, results.Where(x => x.Pmo != null).Count());
            Assert.AreEqual(439, results.Where(x => x.Signal != null).Count());

            // sample values
            PmoResult r1 = results[92];
            Assert.AreEqual(0.6159m, Math.Round((decimal)r1.Pmo, 4));
            Assert.AreEqual(0.5582m, Math.Round((decimal)r1.Signal, 4));

            PmoResult r2 = results[501];
            Assert.AreEqual(-2.7016m, Math.Round((decimal)r2.Pmo, 4));
            Assert.AreEqual(-2.3117m, Math.Round((decimal)r2.Signal, 4));
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<PmoResult> r = Indicator.GetPmo(historyBad, 25, 15, 5);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad time period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetPmo(history, 1));

            // bad smoothing period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetPmo(history, 5, 0));

            // bad signal period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetPmo(history, 5, 5, 0));

            // insufficient history
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetPmo(HistoryTestData.Get(54), 35, 20, 10));
        }
    }
}
