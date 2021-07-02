using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class StarcBands : TestBase
    {

        [TestMethod]
        public void Standard()
        {
            int smaPeriod = 20;
            int multiplier = 2;
            int atrPeriod = 14;
            int lookbackPeriod = Math.Max(smaPeriod, atrPeriod);

            List<StarcBandsResult> results =
                history.GetStarcBands(smaPeriod, multiplier, atrPeriod)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(483, results.Where(x => x.Centerline != null).Count());
            Assert.AreEqual(483, results.Where(x => x.UpperBand != null).Count());
            Assert.AreEqual(483, results.Where(x => x.LowerBand != null).Count());

            // sample value
            StarcBandsResult r1 = results[18];
            Assert.AreEqual(null, r1.Centerline);
            Assert.AreEqual(null, r1.UpperBand);
            Assert.AreEqual(null, r1.LowerBand);

            StarcBandsResult r2 = results[19];
            Assert.AreEqual(214.5250m, Math.Round((decimal)r2.Centerline, 4));
            Assert.AreEqual(217.2831m, Math.Round((decimal)r2.UpperBand, 4));
            Assert.AreEqual(211.7669m, Math.Round((decimal)r2.LowerBand, 4));

            StarcBandsResult r3 = results[249];
            Assert.AreEqual(255.5500m, Math.Round((decimal)r3.Centerline, 4));
            Assert.AreEqual(258.2261m, Math.Round((decimal)r3.UpperBand, 4));
            Assert.AreEqual(252.8739m, Math.Round((decimal)r3.LowerBand, 4));

            StarcBandsResult r4 = results[485];
            Assert.AreEqual(265.4855m, Math.Round((decimal)r4.Centerline, 4));
            Assert.AreEqual(275.1161m, Math.Round((decimal)r4.UpperBand, 4));
            Assert.AreEqual(255.8549m, Math.Round((decimal)r4.LowerBand, 4));

            StarcBandsResult r5 = results[501];
            Assert.AreEqual(251.8600m, Math.Round((decimal)r5.Centerline, 4));
            Assert.AreEqual(264.1595m, Math.Round((decimal)r5.UpperBand, 4));
            Assert.AreEqual(239.5605m, Math.Round((decimal)r5.LowerBand, 4));
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<StarcBandsResult> r = Indicator.GetStarcBands(historyBad, 10, 3, 15);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad EMA period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetStarcBands(history, 1, 2, 10));

            // bad ATR period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetStarcBands(history, 20, 2, 1));

            // bad multiplier
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetStarcBands(history, 20, 0, 10));

            // insufficient history 120
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetStarcBands(HistoryTestData.Get(119), 120, 2, 10));

            // insufficient history 250
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetStarcBands(HistoryTestData.Get(249), 20, 2, 150));
        }
    }
}
