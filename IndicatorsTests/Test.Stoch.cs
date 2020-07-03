using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StockIndicators.Tests
{
    [TestClass]
    public class StochTests : TestBase
    {

        [TestMethod()]
        public void GetStochTest()
        {
            int lookbackPeriod = 14;
            int signalPeriod = 3;
            int smoothPeriod = 3;

            IEnumerable<StochResult> results = Indicator.GetStoch(history, lookbackPeriod, signalPeriod, smoothPeriod);

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count());
            Assert.AreEqual(502 - lookbackPeriod + 1 - smoothPeriod, results.Where(x => x.Oscillator != null).Count());
            Assert.AreEqual(502 - lookbackPeriod + 1 - smoothPeriod - signalPeriod, results.Where(x => x.Signal != null).Count());

            // sample value
            StochResult result = results.Where(x => x.Date == DateTime.Parse("12/31/2018")).FirstOrDefault();
            Assert.AreEqual((decimal)43.1354, Math.Round((decimal)result.Oscillator, 4));
            Assert.AreEqual((decimal)35.5674, Math.Round((decimal)result.Signal, 4));
            Assert.AreEqual(true, result.IsIncreasing);

            // no signal period
            IEnumerable<StochResult> results2 = Indicator.GetStoch(history, 5, 1);
            StochResult r2 = results2.Where(x => x.Date == DateTime.Parse("12/31/2018")).FirstOrDefault();
            Assert.AreEqual(r2.Oscillator, r2.Signal);

            StochResult r3 = results2.Where(x => x.Date == DateTime.Parse("12/10/2018")).FirstOrDefault();
            Assert.AreEqual(r3.Oscillator, r3.Signal);
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(BadParameterException), "Bad lookback.")]
        public void BadLookback()
        {
            Indicator.GetStoch(history, 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadParameterException), "Bad signal period.")]
        public void BadSignal()
        {
            Indicator.GetStoch(history, 14, 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadParameterException), "Bad smoothing period.")]
        public void BadSmooth()
        {
            Indicator.GetStoch(history, 14, 3, 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            Indicator.GetStoch(history.Where(x => x.Index < 30), 30);
        }

    }
}