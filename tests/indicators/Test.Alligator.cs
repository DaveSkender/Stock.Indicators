using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class Alligator : TestBase
    {
        [TestMethod]
        public void Standard()
        {
            List<AlligatorResult> results = Indicator.GetAlligator(history)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(490, results.Where(x => x.Jaw != null).Count());
            Assert.AreEqual(495, results.Where(x => x.Teeth != null).Count());
            Assert.AreEqual(498, results.Where(x => x.Lips != null).Count());

            // starting calculations at proper index
            Assert.IsNull(results[11].Jaw);
            Assert.IsNotNull(results[12].Jaw);

            Assert.IsNull(results[6].Teeth);
            Assert.IsNotNull(results[7].Teeth);

            Assert.IsNull(results[3].Lips);
            Assert.IsNotNull(results[4].Lips);

            // sample values
            Assert.AreEqual(214.00000m, Math.Round(results[12].Jaw.Value, 5));
            Assert.AreEqual(213.95750m, Math.Round(results[13].Jaw.Value, 5));
            Assert.AreEqual(222.04129m, Math.Round(results[39].Jaw.Value, 5));
            Assert.AreEqual(247.02835m, Math.Round(results[501].Jaw.Value, 5));

            Assert.AreEqual(213.98500m, Math.Round(results[7].Teeth.Value, 5));
            Assert.AreEqual(214.09000m, Math.Round(results[8].Teeth.Value, 5));
            Assert.AreEqual(223.40220m, Math.Round(results[39].Teeth.Value, 5));
            Assert.AreEqual(243.99765m, Math.Round(results[501].Teeth.Value, 5));

            Assert.AreEqual(213.87200m, Math.Round(results[4].Lips.Value, 5));
            Assert.AreEqual(213.89800m, Math.Round(results[5].Lips.Value, 5));
            Assert.AreEqual(224.41956m, Math.Round(results[39].Lips.Value, 5));
            Assert.AreEqual(242.94305m, Math.Round(results[501].Lips.Value, 5));
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<AlligatorResult> r = Indicator.GetAlligator(historyBad);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad lookback period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetAlligator(history, 0, 8, 8, 5, 5, 3));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetAlligator(history, 13, 8, 0, 5, 5, 3));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetAlligator(history, 13, 8, 8, 5, 0, 3));

            // bad smoothing period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetAlligator(history, 13, 14, 8, 5, 5, 3));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetAlligator(history, 13, 8, 8, 9, 5, 3));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetAlligator(history, 13, 8, 8, 5, 5, 6));

            // insufficient history for N+100
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetAlligator(HistoryTestData.Get(112), 13, 8, 8, 5, 5, 3));
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetAlligator(HistoryTestData.Get(115), 13, 8, 16, 5, 5, 3));
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetAlligator(HistoryTestData.Get(118), 13, 8, 8, 5, 19, 3));

            // insufficient history for 2×N
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetAlligator(HistoryTestData.Get(159), 80, 8, 8, 5, 5, 3));
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetAlligator(HistoryTestData.Get(159), 13, 8, 80, 5, 5, 3));
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetAlligator(HistoryTestData.Get(159), 13, 8, 8, 5, 80, 3));
        }
    }
}
