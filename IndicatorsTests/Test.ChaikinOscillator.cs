using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StockIndicators.Tests
{
    [TestClass]
    public class ChaikinOscTests : TestBase
    {

        [TestMethod()]
        public void GetChaikinOscTest()
        {
            int fastPeriod = 3;
            int slowPeriod = 10;
            IEnumerable<ChaikinOscResult> results = Indicator.GetChaikinOsc(history, fastPeriod, slowPeriod);

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count());
            Assert.AreEqual(502 - slowPeriod + 1, results.Where(x => x.Oscillator != null).Count());

            // sample value
            ChaikinOscResult r = results.Where(x => x.Index == 502).FirstOrDefault();
            Assert.AreEqual((decimal)3439986548.42, Math.Round(r.Adl, 2));
            Assert.AreEqual((decimal)0.8052, Math.Round(r.MoneyFlowMultiplier, 4));
            Assert.AreEqual((decimal)118396116.25, Math.Round(r.MoneyFlowVolume, 2));
            Assert.AreEqual((decimal)-19135200.72, Math.Round((decimal)r.Oscillator, 2));
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(BadParameterException), "Bad fast lookback.")]
        public void BadFastLookback()
        {
            Indicator.GetChaikinOsc(history, 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadParameterException), "Bad slow lookback.")]
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