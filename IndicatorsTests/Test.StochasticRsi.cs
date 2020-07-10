using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StockIndicators.Tests
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

            IEnumerable<StochRsiResult> results = Indicator.GetStochRsi(
                history, rsiPeriod, stochPeriod, signalPeriod, smoothPeriod);

            // assertions

            // proper quantities
            Assert.AreEqual(502, results.Count());
            Assert.AreEqual(502 - rsiPeriod - stochPeriod - smoothPeriod + 2, results.Where(x => x.StochRsi != null).Count());
            Assert.AreEqual(502 - rsiPeriod - stochPeriod - signalPeriod - smoothPeriod + 3, results.Where(x => x.Signal != null).Count());

            // this series starts with 4 periods of topped Stochastic RSI, so no direction can be determined
            Assert.AreEqual(502 - rsiPeriod - stochPeriod - smoothPeriod + 2 - 4, results.Where(x => x.IsIncreasing != null).Count());

            // sample value
            StochRsiResult r = results.Where(x => x.Date == DateTime.Parse("12/31/2018")).FirstOrDefault();
            Assert.AreEqual((decimal)97.5244, Math.Round((decimal)r.StochRsi, 4));
            Assert.AreEqual((decimal)89.8385, Math.Round((decimal)r.Signal, 4));
            Assert.AreEqual(true, r.IsIncreasing);
        }


        [TestMethod()]
        public void GetStochRsiSlowTest()
        {
            int rsiPeriod = 14;
            int stochPeriod = 14;
            int signalPeriod = 3;
            int smoothPeriod = 3;

            IEnumerable<StochRsiResult> results = Indicator.GetStochRsi(
                history, rsiPeriod, stochPeriod, signalPeriod, smoothPeriod);

            // assertions

            // proper quantities
            Assert.AreEqual(502, results.Count());
            Assert.AreEqual(502 - rsiPeriod - stochPeriod - smoothPeriod + 2, results.Where(x => x.StochRsi != null).Count());
            Assert.AreEqual(502 - rsiPeriod - stochPeriod - signalPeriod - smoothPeriod + 3, results.Where(x => x.Signal != null).Count());

            // this series starts with 2 periods of topped Stochastic RSI, so no direction can be determined
            Assert.AreEqual(502 - rsiPeriod - stochPeriod - smoothPeriod + 2 - 2, results.Where(x => x.IsIncreasing != null).Count());

            // sample value
            StochRsiResult r = results.Where(x => x.Date == DateTime.Parse("12/31/2018")).FirstOrDefault();
            Assert.AreEqual((decimal)89.8385, Math.Round((decimal)r.StochRsi, 4));
            Assert.AreEqual((decimal)73.4176, Math.Round((decimal)r.Signal, 4));
            Assert.AreEqual(true, r.IsIncreasing);
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(BadParameterException), "Bad RSI lookback.")]
        public void BadRsiLookback()
        {
            Indicator.GetStochRsi(history, 0, 14, 3, 1);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadParameterException), "Bad STO lookback.")]
        public void BadLookback()
        {
            Indicator.GetStochRsi(history, 14, 0, 3, 3);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadParameterException), "Bad STO signal period.")]
        public void BadSignal()
        {
            Indicator.GetStochRsi(history, 14, 14, 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadParameterException), "Bad STO smoothing period.")]
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