using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class SmaExtended : TestBase
    {

        [TestMethod]
        public void Extended()
        {

            List<SmaExtendedResult> results = quotes.GetSmaExtended(20).ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is quotes
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(483, results.Where(x => x.Sma != null).Count());

            // sample value
            SmaExtendedResult r = results[501];
            Assert.AreEqual(251.86m, r.Sma);
            Assert.AreEqual(9.45m, r.Mad);
            Assert.AreEqual(119.2510m, Math.Round((decimal)r.Mse, 4));
            Assert.AreEqual(0.037637m, Math.Round((decimal)r.Mape, 6));
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<SmaResult> r = Indicator.GetSmaExtended(historyBad, 15);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Pruned()
        {
            List<SmaExtendedResult> results = quotes.GetSmaExtended(20)
                .PruneWarmupPeriods()
                .ToList();

            // assertions
            Assert.AreEqual(502 - 19, results.Count);
            Assert.AreEqual(251.8600m, Math.Round(results.LastOrDefault().Sma.Value, 4));
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad lookback period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetSmaExtended(quotes, 0));

            // insufficient quotes
            Assert.ThrowsException<BadQuotesException>(() =>
                Indicator.GetSmaExtended(HistoryTestData.Get(9), 10));
        }
    }
}
