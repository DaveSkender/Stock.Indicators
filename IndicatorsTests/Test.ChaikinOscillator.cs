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
            ChaikinOscResult r = results.Where(x => x.Date == DateTime.Parse("12/31/2018")).FirstOrDefault();
            Assert.AreEqual((decimal)3439986548.42, Math.Round(r.Adl, 2));
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
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            Indicator.GetChaikinOsc(history.Where(x => x.Index <= 10), 3, 10);
        }
    }
}