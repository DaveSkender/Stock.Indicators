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
            Assert.AreEqual(214.00000m, Math.Round(results[20].Jaw.Value, 5));
            Assert.AreEqual(213.97385m, Math.Round(results[21].Jaw.Value, 5));
            Assert.AreEqual(225.74192m, Math.Round(results[99].Jaw.Value, 5));
            Assert.AreEqual(260.61620m, Math.Round(results[501].Jaw.Value, 5));

            Assert.AreEqual(213.98500m, Math.Round(results[12].Teeth.Value, 5));
            Assert.AreEqual(214.05062m, Math.Round(results[13].Teeth.Value, 5));
            Assert.AreEqual(226.10575m, Math.Round(results[99].Teeth.Value, 5));
            Assert.AreEqual(252.26597m, Math.Round(results[501].Teeth.Value, 5));

            Assert.AreEqual(213.87200m, Math.Round(results[7].Lips.Value, 5));
            Assert.AreEqual(213.88760m, Math.Round(results[8].Lips.Value, 5));
            Assert.AreEqual(226.36776m, Math.Round(results[99].Lips.Value, 5));
            Assert.AreEqual(243.89603m, Math.Round(results[501].Lips.Value, 5));
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
