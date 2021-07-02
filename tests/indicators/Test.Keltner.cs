using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class Keltner : TestBase
    {

        [TestMethod]
        public void Standard()
        {
            int emaPeriod = 20;
            int multiplier = 2;
            int atrPeriod = 10;

            List<KeltnerResult> results =
                history.GetKeltner(emaPeriod, multiplier, atrPeriod)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);

            int warmupPeriod = 502 - Math.Max(emaPeriod, atrPeriod) + 1;
            Assert.AreEqual(warmupPeriod, results.Where(x => x.Centerline != null).Count());
            Assert.AreEqual(warmupPeriod, results.Where(x => x.UpperBand != null).Count());
            Assert.AreEqual(warmupPeriod, results.Where(x => x.LowerBand != null).Count());
            Assert.AreEqual(warmupPeriod, results.Where(x => x.Width != null).Count());

            // sample value
            KeltnerResult r1 = results[485];
            Assert.AreEqual(275.4260m, Math.Round((decimal)r1.UpperBand, 4));
            Assert.AreEqual(265.4599m, Math.Round((decimal)r1.Centerline, 4));
            Assert.AreEqual(255.4938m, Math.Round((decimal)r1.LowerBand, 4));
            Assert.AreEqual(0.075085m, Math.Round((decimal)r1.Width, 6));

            KeltnerResult r2 = results[501];
            Assert.AreEqual(262.1873m, Math.Round((decimal)r2.UpperBand, 4));
            Assert.AreEqual(249.3519m, Math.Round((decimal)r2.Centerline, 4));
            Assert.AreEqual(236.5165m, Math.Round((decimal)r2.LowerBand, 4));
            Assert.AreEqual(0.102950m, Math.Round((decimal)r2.Width, 6));
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<KeltnerResult> r = Indicator.GetKeltner(historyBad, 10, 3, 15);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad EMA period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetKeltner(history, 1, 2, 10));

            // bad ATR period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetKeltner(history, 20, 2, 1));

            // bad multiplier
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetKeltner(history, 20, 0, 10));

            // insufficient history for N+100
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetKeltner(HistoryTestData.Get(119), 20, 2, 10));

            // insufficient history for 2×N
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetKeltner(HistoryTestData.Get(499), 20, 2, 250));
        }
    }
}
