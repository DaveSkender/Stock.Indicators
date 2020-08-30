﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StockIndicators.Tests
{
    [TestClass]
    public class KeltnerTests : TestBase
    {

        [TestMethod()]
        public void GetKeltnerTest()
        {
            int emaPeriod = 20;
            int multiplier = 2;
            int atrPeriod = 10;
            int lookbackPeriod = Math.Max(emaPeriod, atrPeriod);
            IEnumerable<KeltnerResult> results = Indicator.GetKeltner(history, emaPeriod, multiplier, atrPeriod);

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count());
            Assert.AreEqual(502 - lookbackPeriod + 1, results.Where(x => x.Centerline != null).Count());
            Assert.AreEqual(502 - lookbackPeriod + 1, results.Where(x => x.UpperBand != null).Count());
            Assert.AreEqual(502 - lookbackPeriod + 1, results.Where(x => x.LowerBand != null).Count());
            Assert.AreEqual(502 - lookbackPeriod + 1, results.Where(x => x.Width != null).Count());

            // sample value
            KeltnerResult r1 = results.Where(x => x.Index == 502).FirstOrDefault();
            Assert.AreEqual(262.1873m, Math.Round((decimal)r1.UpperBand, 4));
            Assert.AreEqual(249.3519m, Math.Round((decimal)r1.Centerline, 4));
            Assert.AreEqual(236.5165m, Math.Round((decimal)r1.LowerBand, 4));
            Assert.AreEqual(0.102950m, Math.Round((decimal)r1.Width, 6));

            KeltnerResult r2 = results.Where(x => x.Index == 486).FirstOrDefault();
            Assert.AreEqual(275.4260m, Math.Round((decimal)r2.UpperBand, 4));
            Assert.AreEqual(265.4599m, Math.Round((decimal)r2.Centerline, 4));
            Assert.AreEqual(255.4938m, Math.Round((decimal)r2.LowerBand, 4));
            Assert.AreEqual(0.075085m, Math.Round((decimal)r2.Width, 6));
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(BadParameterException), "Bad EMA period.")]
        public void BadEmaPeriod()
        {
            Indicator.GetKeltner(history, 1, 2, 10);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadParameterException), "Bad ATR period.")]
        public void BadAtrPeriod()
        {
            Indicator.GetKeltner(history, 20, 2, 1);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadParameterException), "Bad multiplier.")]
        public void BadMultiplier()
        {
            Indicator.GetKeltner(history, 20, 0, 10);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history 100.")]
        public void InsufficientHistory100()
        {
            Indicator.GetKeltner(history.Where(x => x.Index < 120), 20, 2, 10);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history 250.")]
        public void InsufficientHistory250()
        {
            Indicator.GetKeltner(history.Where(x => x.Index < 500), 20, 2, 250);
        }

    }
}