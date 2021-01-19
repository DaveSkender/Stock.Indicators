using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class Donchian : TestBase
    {

        [TestMethod]
        public void Standard()
        {
            int lookbackPeriod = 20;

            List<DonchianResult> results = Indicator.GetDonchian(history, lookbackPeriod)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(482, results.Where(x => x.Centerline != null).Count());
            Assert.AreEqual(482, results.Where(x => x.UpperBand != null).Count());
            Assert.AreEqual(482, results.Where(x => x.LowerBand != null).Count());
            Assert.AreEqual(482, results.Where(x => x.Width != null).Count());

            // sample values
            DonchianResult r1 = results[19];
            Assert.AreEqual(null, r1.Centerline);
            Assert.AreEqual(null, r1.UpperBand);
            Assert.AreEqual(null, r1.LowerBand);
            Assert.AreEqual(null, r1.Width);

            DonchianResult r2 = results[20];
            Assert.AreEqual(214.2700m, Math.Round((decimal)r2.Centerline, 4));
            Assert.AreEqual(217.0200m, Math.Round((decimal)r2.UpperBand, 4));
            Assert.AreEqual(211.5200m, Math.Round((decimal)r2.LowerBand, 4));
            Assert.AreEqual(0.025669m, Math.Round((decimal)r2.Width, 6));

            DonchianResult r3 = results[249];
            Assert.AreEqual(254.2850m, Math.Round((decimal)r3.Centerline, 4));
            Assert.AreEqual(258.7000m, Math.Round((decimal)r3.UpperBand, 4));
            Assert.AreEqual(249.8700m, Math.Round((decimal)r3.LowerBand, 4));
            Assert.AreEqual(0.034725m, Math.Round((decimal)r3.Width, 6));

            DonchianResult r4 = results[485];
            Assert.AreEqual(265.5350m, Math.Round((decimal)r4.Centerline, 4));
            Assert.AreEqual(274.3900m, Math.Round((decimal)r4.UpperBand, 4));
            Assert.AreEqual(256.6800m, Math.Round((decimal)r4.LowerBand, 4));
            Assert.AreEqual(0.066696m, Math.Round((decimal)r4.Width, 6));

            DonchianResult r5 = results[501];
            Assert.AreEqual(251.5050m, Math.Round((decimal)r5.Centerline, 4));
            Assert.AreEqual(273.5900m, Math.Round((decimal)r5.UpperBand, 4));
            Assert.AreEqual(229.4200m, Math.Round((decimal)r5.LowerBand, 4));
            Assert.AreEqual(0.175623m, Math.Round((decimal)r5.Width, 6));
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<DonchianResult> r = Indicator.GetDonchian(historyBad, 15);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad lookback period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetDonchian(history, 0));

            // insufficient history
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetDonchian(HistoryTestData.Get(30), 30));
        }
    }
}