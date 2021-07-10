using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class Sma : TestBase
    {

        [TestMethod]
        public void Standard()
        {

            List<SmaResult> results = quotes.GetSma(20).ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is quotes
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(483, results.Where(x => x.Sma != null).Count());

            // sample values
            Assert.IsNull(results[18].Sma);
            Assert.AreEqual(214.5250m, Math.Round(results[19].Sma.Value, 4));
            Assert.AreEqual(215.0310m, Math.Round(results[24].Sma.Value, 4));
            Assert.AreEqual(234.9350m, Math.Round(results[149].Sma.Value, 4));
            Assert.AreEqual(255.5500m, Math.Round(results[249].Sma.Value, 4));
            Assert.AreEqual(251.8600m, Math.Round(results[501].Sma.Value, 4));
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<SmaResult> r = Indicator.GetSma(historyBad, 15);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Pruned()
        {
            List<SmaResult> results = quotes.GetSma(20)
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
                Indicator.GetSma(quotes, 0));

            // insufficient quotes
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetSma(HistoryTestData.Get(9), 10));
        }
    }
}
