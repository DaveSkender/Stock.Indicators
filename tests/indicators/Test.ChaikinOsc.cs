using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Internal.Tests
{
    [TestClass]
    public class ChaikinOsc : TestBase
    {

        [TestMethod()]
        public void Standard()
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

        [TestMethod()]
        public void BadData()
        {
            IEnumerable<ChaikinOscResult> r = Indicator.GetChaikinOsc(historyBad, 5, 15);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod()]
        public void Convergence()
        {
            foreach (int qty in convergeQuantities)
            {
                IEnumerable<Quote> h = History.GetHistoryLong(110 + qty);
                IEnumerable<ChaikinOscResult> r = Indicator.GetChaikinOsc(h);

                ChaikinOscResult l = r.LastOrDefault();
                Console.WriteLine("CHAIKIN OSC on {0:d} with {1,4} periods: {2:N8}",
                    l.Date, h.Count(), l.Oscillator);
            }
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
            IEnumerable<Quote> h = History.GetHistory(109);
            Indicator.GetChaikinOsc(h, 3, 10);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history for 2×S.")]
        public void InsufficientHistory250()
        {
            IEnumerable<Quote> h = History.GetHistory(499);
            Indicator.GetChaikinOsc(h, 3, 250);
        }

    }
}