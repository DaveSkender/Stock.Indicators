using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StockIndicators.Tests
{
    [TestClass]
    public class BollingerBandsTests : TestBase
    {

        [TestMethod()]
        public void GetBollingerBandsTest()
        {
            int lookbackPeriod = 20;
            int standardDeviations = 2;

            IEnumerable<BollingerBandsResult> results = Indicator.GetBollingerBands(history, lookbackPeriod, standardDeviations);

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count());
            Assert.AreEqual(502 - lookbackPeriod + 1, results.Where(x => x.Sma != null).Count());
            Assert.AreEqual(502 - lookbackPeriod + 1, results.Where(x => x.UpperBand != null).Count());
            Assert.AreEqual(502 - lookbackPeriod + 1, results.Where(x => x.LowerBand != null).Count());

            // sample value
            BollingerBandsResult r = results.Where(x => x.Date == DateTime.Parse("12/31/2018")).FirstOrDefault();

            Assert.AreEqual((decimal)251.8600, Math.Round((decimal)r.Sma, 4));
            Assert.AreEqual((decimal)273.7004, Math.Round((decimal)r.UpperBand, 4));
            Assert.AreEqual((decimal)230.0196, Math.Round((decimal)r.LowerBand, 4));
            Assert.AreEqual((decimal)-0.602552, Math.Round((decimal)r.ZScore, 6));
            Assert.AreEqual((decimal)0.173433, Math.Round((decimal)r.Width, 6));
            Assert.AreEqual(false, r.IsDiverging);
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(BadParameterException), "Bad lookback period.")]
        public void BadLookback()
        {
            Indicator.GetBollingerBands(history, 1);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadParameterException), "Bad standard deviations.")]
        public void InsufficientStandardDeviations()
        {
            Indicator.GetBollingerBands(history, 2, 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            Indicator.GetBollingerBands(history.Where(x => x.Index < 30), 30, 2);
        }

    }
}