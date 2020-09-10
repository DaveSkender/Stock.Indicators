using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Internal.Tests
{
    [TestClass]
    public class DonchianTests : TestBase
    {

        [TestMethod()]
        public void GetDonchianTest()
        {
            int lookbackPeriod = 20;
            IEnumerable<DonchianResult> results = Indicator.GetDonchian(history, lookbackPeriod);

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count());
            Assert.AreEqual(502 - lookbackPeriod + 1, results.Where(x => x.Centerline != null).Count());
            Assert.AreEqual(502 - lookbackPeriod + 1, results.Where(x => x.UpperBand != null).Count());
            Assert.AreEqual(502 - lookbackPeriod + 1, results.Where(x => x.LowerBand != null).Count());
            Assert.AreEqual(502 - lookbackPeriod + 1, results.Where(x => x.Width != null).Count());

            // sample value
            DonchianResult r1 = results.Where(x => x.Index == 502).FirstOrDefault();
            Assert.AreEqual(251.5050m, Math.Round((decimal)r1.Centerline, 4));
            Assert.AreEqual(273.5900m, Math.Round((decimal)r1.UpperBand, 4));
            Assert.AreEqual(229.4200m, Math.Round((decimal)r1.LowerBand, 4));
            Assert.AreEqual(0.175623m, Math.Round((decimal)r1.Width, 6));

            DonchianResult r2 = results.Where(x => x.Index == 486).FirstOrDefault();
            Assert.AreEqual(265.2300m, Math.Round((decimal)r2.Centerline, 4));
            Assert.AreEqual(274.3900m, Math.Round((decimal)r2.UpperBand, 4));
            Assert.AreEqual(256.0700m, Math.Round((decimal)r2.LowerBand, 4));
            Assert.AreEqual(0.069072m, Math.Round((decimal)r2.Width, 6));
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(BadParameterException), "Bad lookback period.")]
        public void BadLookback()
        {
            Indicator.GetDonchian(history, 1);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            Indicator.GetDonchian(history.Where(x => x.Index < 30), 30);
        }

    }
}