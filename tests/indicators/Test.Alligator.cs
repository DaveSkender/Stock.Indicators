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
            Assert.AreEqual(482, results.Where(x => x.Jaw != null).Count());
            Assert.AreEqual(490, results.Where(x => x.Teeth != null).Count());
            Assert.AreEqual(495, results.Where(x => x.Lips != null).Count());

            // starting calculations at proper index
            Assert.IsNull(results[19].Jaw);
            Assert.IsNotNull(results[20].Jaw);

            Assert.IsNull(results[11].Teeth);
            Assert.IsNotNull(results[12].Teeth);

            Assert.IsNull(results[6].Lips);
            Assert.IsNotNull(results[7].Lips);

            // sample values
            Assert.AreEqual(213.81269m, Math.Round(results[20].Jaw.Value, 5));
            Assert.AreEqual(213.79287m, Math.Round(results[21].Jaw.Value, 5));
            Assert.AreEqual(225.60571m, Math.Round(results[99].Jaw.Value, 5));
            Assert.AreEqual(260.98953m, Math.Round(results[501].Jaw.Value, 5));

            Assert.AreEqual(213.69938m, Math.Round(results[12].Teeth.Value, 5));
            Assert.AreEqual(213.80008m, Math.Round(results[13].Teeth.Value, 5));
            Assert.AreEqual(226.12157m, Math.Round(results[99].Teeth.Value, 5));
            Assert.AreEqual(253.53576m, Math.Round(results[501].Teeth.Value, 5));

            Assert.AreEqual(213.63500m, Math.Round(results[7].Lips.Value, 5));
            Assert.AreEqual(213.74900m, Math.Round(results[8].Lips.Value, 5));
            Assert.AreEqual(226.35353m, Math.Round(results[99].Lips.Value, 5));
            Assert.AreEqual(244.29591m, Math.Round(results[501].Lips.Value, 5));
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
            // insufficient history
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetAlligator(HistoryTestData.Get(114)));
        }
    }
}
