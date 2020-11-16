using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Internal.Tests
{
    [TestClass]
    public class StochasticRsiTests : TestBase
    {

        [TestMethod()]
        public void GetStochRsiTest()
        {
            int rsiPeriod = 14;
            int stochPeriod = 14;
            int signalPeriod = 3;
            int smoothPeriod = 1;

            List<StochRsiResult> results =
                Indicator.GetStochRsi(history, rsiPeriod, stochPeriod, signalPeriod, smoothPeriod)
                .ToList();

            // assertions

            // proper quantities
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(502 - rsiPeriod - stochPeriod - smoothPeriod + 2, results.Where(x => x.StochRsi != null).Count());
            Assert.AreEqual(502 - rsiPeriod - stochPeriod - signalPeriod - smoothPeriod + 3, results.Where(x => x.Signal != null).Count());

            // sample value
            StochRsiResult r = results[501];
            Assert.AreEqual(97.5244m, Math.Round((decimal)r.StochRsi, 4));
            Assert.AreEqual(89.8385m, Math.Round((decimal)r.Signal, 4));
        }


        [TestMethod()]
        public void GetStochRsiSlowTest()
        {
            int rsiPeriod = 14;
            int stochPeriod = 14;
            int signalPeriod = 3;
            int smoothPeriod = 3;

            List<StochRsiResult> results =
                Indicator.GetStochRsi(history, rsiPeriod, stochPeriod, signalPeriod, smoothPeriod)
                .ToList();

            // assertions

            // proper quantities
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(502 - rsiPeriod - stochPeriod - smoothPeriod + 2, results.Where(x => x.StochRsi != null).Count());
            Assert.AreEqual(502 - rsiPeriod - stochPeriod - signalPeriod - smoothPeriod + 3, results.Where(x => x.Signal != null).Count());

            // sample value
            StochRsiResult r = results[501];
            Assert.AreEqual(89.8385m, Math.Round((decimal)r.StochRsi, 4));
            Assert.AreEqual(73.4176m, Math.Round((decimal)r.Signal, 4));
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Bad RSI lookback.")]
        public void BadRsiLookback()
        {
            Indicator.GetStochRsi(history, 0, 14, 3, 1);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Bad STO lookback.")]
        public void BadLookbackPeriod()
        {
            Indicator.GetStochRsi(history, 14, 0, 3, 3);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Bad STO signal period.")]
        public void BadSignal()
        {
            Indicator.GetStochRsi(history, 14, 14, 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Bad STO smoothing period.")]
        public void BadSmooth()
        {
            Indicator.GetStochRsi(history, 14, 14, 3, 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            Indicator.GetStochRsi(history.Where(x => x.Index < 60), 30, 30, 5, 5);
        }

    }
}