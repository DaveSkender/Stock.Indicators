using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Internal.Tests
{
    [TestClass]
    public class BollingerBandsTests : TestBase
    {

        [TestMethod()]
        public void GetBollingerBandsTest()
        {
            int lookbackPeriod = 20;
            int standardDeviations = 2;

            IEnumerable<BollingerBandsResult> results =
                Indicator.GetBollingerBands(history, lookbackPeriod, standardDeviations);

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count());
            Assert.AreEqual(483, results.Where(x => x.Sma != null).Count());
            Assert.AreEqual(483, results.Where(x => x.UpperBand != null).Count());
            Assert.AreEqual(483, results.Where(x => x.LowerBand != null).Count());
            Assert.AreEqual(483, results.Where(x => x.PercentB != null).Count());
            Assert.AreEqual(483, results.Where(x => x.ZScore != null).Count());
            Assert.AreEqual(483, results.Where(x => x.Width != null).Count());


            // sample values
            BollingerBandsResult r1 = results.Where(x => x.Index == 502).FirstOrDefault();
            Assert.AreEqual(251.8600m, Math.Round((decimal)r1.Sma, 4));
            Assert.AreEqual(273.7004m, Math.Round((decimal)r1.UpperBand, 4));
            Assert.AreEqual(230.0196m, Math.Round((decimal)r1.LowerBand, 4));
            Assert.AreEqual(0.349362m, Math.Round((decimal)r1.PercentB, 6));
            Assert.AreEqual(-0.602552m, Math.Round((decimal)r1.ZScore, 6));
            Assert.AreEqual(0.173433m, Math.Round((decimal)r1.Width, 6));

            BollingerBandsResult r2 = results.Where(x => x.Index == 250).FirstOrDefault();
            Assert.AreEqual(255.5500m, Math.Round((decimal)r2.Sma, 4));
            Assert.AreEqual(259.5642m, Math.Round((decimal)r2.UpperBand, 4));
            Assert.AreEqual(251.5358m, Math.Round((decimal)r2.LowerBand, 4));
            Assert.AreEqual(0.803923m, Math.Round((decimal)r2.PercentB, 6));
            Assert.AreEqual(1.215692m, Math.Round((decimal)r2.ZScore, 6));
            Assert.AreEqual(0.031416m, Math.Round((decimal)r2.Width, 6));
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