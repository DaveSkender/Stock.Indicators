using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class ParabolicSar : TestBase
    {

        [TestMethod]
        public void Standard()
        {
            decimal acclerationStep = 0.02m;
            decimal maxAccelerationFactor = 0.2m;

            List<ParabolicSarResult> results =
                history.GetParabolicSar(acclerationStep, maxAccelerationFactor)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(488, results.Where(x => x.Sar != null).Count());

            // sample values
            ParabolicSarResult r1 = results[14];
            Assert.AreEqual(212.83m, Math.Round((decimal)r1.Sar, 4));
            Assert.AreEqual(true, r1.IsReversal);

            ParabolicSarResult r2 = results[16];
            Assert.AreEqual(212.9924m, Math.Round((decimal)r2.Sar, 4));
            Assert.AreEqual(false, r2.IsReversal);

            ParabolicSarResult r3 = results[501];
            Assert.AreEqual(229.7662m, Math.Round((decimal)r3.Sar, 4));
            Assert.AreEqual(false, r3.IsReversal);
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<ParabolicSarResult> r = Indicator.GetParabolicSar(historyBad);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad acceleration step
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetParabolicSar(history, 0, 1));

            // insufficient acceleration step
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetParabolicSar(history, 0.02m, 0));

            // step larger than factor
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetParabolicSar(history, 6, 2));

            // insufficient history
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetParabolicSar(HistoryTestData.Get(1), 0.02m, 0.2m));
        }
    }
}
