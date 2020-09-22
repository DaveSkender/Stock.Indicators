﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Internal.Tests
{
    [TestClass]
    public class SlopeTests : TestBase
    {

        [TestMethod()]
        public void GetSlopeTest()
        {
            int lookbackPeriod = 20;
            IEnumerable<SlopeResult> results = Indicator.GetSlope(history, lookbackPeriod);

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count());
            Assert.AreEqual(502 - lookbackPeriod + 1, results.Where(x => x.Slope != null).Count());
            Assert.AreEqual(lookbackPeriod, results.Where(x => x.Line != null).Count());

            // sample values
            SlopeResult r1 = results.Where(x => x.Index == 502).FirstOrDefault();
            Assert.AreEqual(-1.689143m, Math.Round((decimal)r1.Slope, 6));
            Assert.AreEqual(1083.7629m, Math.Round((decimal)r1.Intercept, 4));
            Assert.AreEqual(0.7955m, Math.Round((decimal)r1.RSquared, 4));
            Assert.AreEqual(235.8131m, Math.Round((decimal)r1.Line, 4));

            SlopeResult r2 = results.Where(x => x.Index == 483).FirstOrDefault();
            Assert.AreEqual(-0.337015m, Math.Round((decimal)r2.Slope, 6));
            Assert.AreEqual(425.1111m, Math.Round((decimal)r2.Intercept, 4));
            Assert.AreEqual(0.1730m, Math.Round((decimal)r2.RSquared, 4));
            Assert.AreEqual(267.9069m, Math.Round((decimal)r2.Line, 4));

            SlopeResult r3 = results.Where(x => x.Index == 250).FirstOrDefault();
            Assert.AreEqual(0.312406m, Math.Round((decimal)r3.Slope, 6));
            Assert.AreEqual(180.4164m, Math.Round((decimal)r3.Intercept, 4));
            Assert.AreEqual(0.8056m, Math.Round((decimal)r3.RSquared, 4));
            Assert.AreEqual(null, r3.Line);
        }


        /* EXCEPTIONS */
        [TestMethod()]
        [ExpectedException(typeof(BadParameterException), "Bad lookback.")]
        public void BadLookback()
        {
            Indicator.GetSlope(history, 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            Indicator.GetSlope(history.Where(x => x.Index < 30), 30);
        }
    }
}