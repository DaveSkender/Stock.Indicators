using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class SuperTrend : TestBase
    {

        [TestMethod]
        public void Standard()
        {
            int lookbackPeriod = 14;
            decimal multiplier = 3;

            List<SuperTrendResult> results = Indicator.GetSuperTrend(history, lookbackPeriod, multiplier)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(489, results.Where(x => x.SuperTrend != null).Count());

            // sample values
            SuperTrendResult r1 = results[12];
            Assert.AreEqual(null, r1.SuperTrend);
            Assert.AreEqual(null, r1.UpperBand);
            Assert.AreEqual(null, r1.LowerBand);

            SuperTrendResult r2 = results[13];
            Assert.AreEqual(209.5436m, Math.Round((decimal)r2.SuperTrend, 4));
            Assert.AreEqual(null, r2.UpperBand);
            Assert.AreEqual(r2.SuperTrend, r2.LowerBand);

            SuperTrendResult r3 = results[151];
            Assert.AreEqual(232.8519m, Math.Round((decimal)r3.SuperTrend, 4));
            Assert.AreEqual(null, r3.UpperBand);
            Assert.AreEqual(r3.SuperTrend, r3.LowerBand);

            SuperTrendResult r4 = results[152];
            Assert.AreEqual(237.6436m, Math.Round((decimal)r4.SuperTrend, 4));
            Assert.AreEqual(r4.SuperTrend, r4.UpperBand);
            Assert.AreEqual(null, r4.LowerBand);

            SuperTrendResult r5 = results[249];
            Assert.AreEqual(253.8008m, Math.Round((decimal)r5.SuperTrend, 4));
            Assert.AreEqual(null, r5.UpperBand);
            Assert.AreEqual(r5.SuperTrend, r5.LowerBand);

            SuperTrendResult r6 = results[501];
            Assert.AreEqual(250.7954m, Math.Round((decimal)r6.SuperTrend, 4));
            Assert.AreEqual(r6.SuperTrend, r6.UpperBand);
            Assert.AreEqual(null, r6.LowerBand);
        }

        [TestMethod]
        public void Bitcoin()
        {
            IEnumerable<Quote> h = HistoryTestData.GetBitcoin();
            List<SuperTrendResult> results = Indicator.GetSuperTrend(h, 10, 3)
                .ToList();
            Assert.AreEqual(1246, results.Count);

            SuperTrendResult r = results[1208];
            Assert.AreEqual(16242.2704m, Math.Round((decimal)r.LowerBand, 4));
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<SuperTrendResult> r = Indicator.GetSuperTrend(historyBad, 7);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad lookback period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetSuperTrend(history, 1));

            // bad multiplier
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetSuperTrend(history, 7, 0));

            // insufficient history
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetSuperTrend(HistoryTestData.Get(129), 30));
        }
    }
}