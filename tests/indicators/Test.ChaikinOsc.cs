using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Internal.Tests
{
    [TestClass]
    public class ChaikinOscTests : TestBase
    {

        [TestMethod()]
        public void GetChaikinOscTest()
        {
            int fastPeriod = 3;
            int slowPeriod = 10;
            List<ChaikinOscResult> results = Indicator.GetChaikinOsc(history, fastPeriod, slowPeriod).ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(502 - slowPeriod + 1, results.Where(x => x.Oscillator != null).Count());

            // sample value
            ChaikinOscResult r = results[501];
            Assert.AreEqual(3439986548.42m, Math.Round(r.Adl, 2));
            Assert.AreEqual(0.8052m, Math.Round(r.MoneyFlowMultiplier, 4));
            Assert.AreEqual(118396116.25m, Math.Round(r.MoneyFlowVolume, 2));
            Assert.AreEqual(-19135200.72m, Math.Round((decimal)r.Oscillator, 2));
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Bad fast lookback.")]
        public void BadFastLookback()
        {
            Indicator.GetChaikinOsc(history, 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Bad slow lookback.")]
        public void BadSlowLookback()
        {
            Indicator.GetChaikinOsc(history, 10, 5);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history for S+100.")]
        public void InsufficientHistory100()
        {
            Indicator.GetChaikinOsc(history.Where(x => x.Index < 110), 3, 10);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history for 2×S.")]
        public void InsufficientHistory250()
        {
            Indicator.GetChaikinOsc(history.Where(x => x.Index < 500), 3, 250);
        }

    }
}