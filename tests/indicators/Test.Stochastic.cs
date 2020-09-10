using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Internal.Tests
{
    [TestClass]
    public class StochasticTests : TestBase
    {

        [TestMethod()]
        public void GetStochStandardTest()
        {
            int lookbackPeriod = 14;
            int signalPeriod = 3;
            int smoothPeriod = 3;

            IEnumerable<StochResult> results = Indicator.GetStoch(
                history, lookbackPeriod, signalPeriod, smoothPeriod);

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count());
            Assert.AreEqual(487, results.Where(x => x.Oscillator != null).Count());
            Assert.AreEqual(485, results.Where(x => x.Signal != null).Count());

            // sample value
            StochResult r = results.Where(x => x.Index == 502).FirstOrDefault();
            Assert.AreEqual(43.1353m, Math.Round((decimal)r.Oscillator, 4));
            Assert.AreEqual(35.5674m, Math.Round((decimal)r.Signal, 4));
        }

        [TestMethod()]
        public void GetStochNoSignalTest()
        {
            int lookbackPeriod = 5;
            int signalPeriod = 1;
            int smoothPeriod = 3;

            IEnumerable<StochResult> results = Indicator.GetStoch(
                history, lookbackPeriod, signalPeriod, smoothPeriod);

            // signal equals oscillator
            StochResult r1 = results.Where(x => x.Index == 502).FirstOrDefault();
            Assert.AreEqual(r1.Oscillator, r1.Signal);

            StochResult r2 = results.Where(x => x.Index == 488).FirstOrDefault();
            Assert.AreEqual(r2.Oscillator, r2.Signal);
        }

        [TestMethod()]
        public void GetStochFastTest()
        {
            int lookbackPeriod = 5;
            int signalPeriod = 10;
            int smoothPeriod = 1;

            IEnumerable<StochResult> results = Indicator.GetStoch(
                history, lookbackPeriod, signalPeriod, smoothPeriod);

            // sample values
            StochResult r1 = results.Where(x => x.Index == 502).FirstOrDefault();
            Assert.AreEqual(91.6233m, Math.Round((decimal)r1.Oscillator, 4));
            Assert.AreEqual(36.0608m, Math.Round((decimal)r1.Signal, 4));

            StochResult r2 = results.Where(x => x.Index == 488).FirstOrDefault();
            Assert.AreEqual(25.0353m, Math.Round((decimal)r2.Oscillator, 4));
            Assert.AreEqual(60.5706m, Math.Round((decimal)r2.Signal, 4));
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
            Indicator.GetStoch(history.Where(x => x.Index < 33), 30, 3, 3);
        }

    }
}