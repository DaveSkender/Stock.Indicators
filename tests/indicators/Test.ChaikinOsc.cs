using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class ChaikinOsc : TestBase
    {

        [TestMethod]
        public void Standard()
        {
            int fastPeriods = 3;
            int slowPeriods = 10;

            List<ChaikinOscResult> results = quotes.GetChaikinOsc(fastPeriods, slowPeriods)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is quotes
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(502 - slowPeriods + 1, results.Where(x => x.Oscillator != null).Count());

            // sample value
            ChaikinOscResult r = results[501];
            Assert.AreEqual(3439986548.42m, Math.Round(r.Adl, 2));
            Assert.AreEqual(0.8052m, Math.Round(r.MoneyFlowMultiplier, 4));
            Assert.AreEqual(118396116.25m, Math.Round(r.MoneyFlowVolume, 2));
            Assert.AreEqual(-19135200.72m, Math.Round((decimal)r.Oscillator, 2));
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<ChaikinOscResult> r = Indicator.GetChaikinOsc(historyBad, 5, 15);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Pruned()
        {
            int fastPeriods = 3;
            int slowPeriods = 10;

            List<ChaikinOscResult> results = quotes.GetChaikinOsc(fastPeriods, slowPeriods)
                .PruneWarmupPeriods()
                .ToList();

            // assertions
            Assert.AreEqual(502 - (slowPeriods + 100), results.Count);

            ChaikinOscResult last = results.LastOrDefault();
            Assert.AreEqual(3439986548.42m, Math.Round(last.Adl, 2));
            Assert.AreEqual(0.8052m, Math.Round(last.MoneyFlowMultiplier, 4));
            Assert.AreEqual(118396116.25m, Math.Round(last.MoneyFlowVolume, 2));
            Assert.AreEqual(-19135200.72m, Math.Round((decimal)last.Oscillator, 2));
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad fast lookback
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetChaikinOsc(quotes, 0));

            // bad slow lookback
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetChaikinOsc(quotes, 10, 5));

            // insufficient quotes S+100
            Assert.ThrowsException<BadQuotesException>(() =>
                Indicator.GetChaikinOsc(HistoryTestData.Get(109), 3, 10));

            // insufficient quotes 2×S
            Assert.ThrowsException<BadQuotesException>(() =>
                Indicator.GetChaikinOsc(HistoryTestData.Get(499), 3, 250));
        }
    }
}
