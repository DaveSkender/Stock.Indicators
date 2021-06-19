using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class ElderRay : TestBase
    {

        [TestMethod]
        public void Standard()
        {

            List<ElderRayResult> results = history.GetElderRay(13).ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(490, results.Where(x => x.BullPower != null).Count());
            Assert.AreEqual(490, results.Where(x => x.BearPower != null).Count());

            // sample values
            ElderRayResult r1 = results[11];
            Assert.IsNull(r1.Ema);
            Assert.IsNull(r1.BullPower);
            Assert.IsNull(r1.BearPower);

            ElderRayResult r2 = results[12];
            Assert.AreEqual(214.0000m, Math.Round((decimal)r2.Ema, 4));
            Assert.AreEqual(0.7500m, r2.BullPower);
            Assert.AreEqual(-0.5100m, r2.BearPower);

            ElderRayResult r3 = results[24];
            Assert.AreEqual(215.5426m, Math.Round((decimal)r3.Ema, 4));
            Assert.AreEqual(1.4274m, Math.Round((decimal)r3.BullPower, 4));
            Assert.AreEqual(0.5474m, Math.Round((decimal)r3.BearPower, 4));

            ElderRayResult r4 = results[149];
            Assert.AreEqual(235.3970m, Math.Round((decimal)r4.Ema, 4));
            Assert.AreEqual(0.9430m, Math.Round((decimal)r4.BullPower, 4));
            Assert.AreEqual(0.4730m, Math.Round((decimal)r4.BearPower, 4));

            ElderRayResult r5 = results[249];
            Assert.AreEqual(256.5206m, Math.Round((decimal)r5.Ema, 4));
            Assert.AreEqual(1.5194m, Math.Round((decimal)r5.BullPower, 4));
            Assert.AreEqual(1.0694m, Math.Round((decimal)r5.BearPower, 4));

            ElderRayResult r6 = results[501];
            Assert.AreEqual(246.0129m, Math.Round((decimal)r6.Ema, 4));
            Assert.AreEqual(-0.4729m, Math.Round((decimal)r6.BullPower, 4));
            Assert.AreEqual(-3.1429m, Math.Round((decimal)r6.BearPower, 4));
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<ElderRayResult> r = Indicator.GetElderRay(historyBad);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad lookback period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetElderRay(history, 0));

            // insufficient history for N+100
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetElderRay(HistoryTestData.Get(129), 30));

            // insufficient history for 2×N
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetElderRay(HistoryTestData.Get(499), 250));
        }
    }
}
