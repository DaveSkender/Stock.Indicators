using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StockIndicators.Tests
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
            Assert.AreEqual(128, results.Where(x => x.IsDiverging == true).Count());

            // sample value
            DonchianResult r1 = results.Where(x => x.Date == DateTime.Parse("12/31/2018")).FirstOrDefault();
            Assert.AreEqual((decimal)251.5050, Math.Round((decimal)r1.Centerline, 4));
            Assert.AreEqual((decimal)273.5900, Math.Round((decimal)r1.UpperBand, 4));
            Assert.AreEqual((decimal)229.4200, Math.Round((decimal)r1.LowerBand, 4));
            Assert.AreEqual((decimal)0.175623, Math.Round((decimal)r1.Width, 6));
            Assert.IsNull(r1.IsDiverging);

            DonchianResult r2 = results.Where(x => x.Date == DateTime.Parse("12/06/2018")).FirstOrDefault();
            Assert.AreEqual((decimal)265.2300, Math.Round((decimal)r2.Centerline, 4));
            Assert.AreEqual((decimal)274.3900, Math.Round((decimal)r2.UpperBand, 4));
            Assert.AreEqual((decimal)256.0700, Math.Round((decimal)r2.LowerBand, 4));
            Assert.AreEqual((decimal)0.069072, Math.Round((decimal)r2.Width, 6));
            Assert.AreEqual(true, r2.IsDiverging);
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