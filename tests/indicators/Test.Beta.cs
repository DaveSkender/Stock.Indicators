using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class Beta : TestBase
    {

        [TestMethod]
        public void Standard()
        {

            List<BetaResult> results = Indicator.GetBeta(quotes, historyOther, 20)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is quotes
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(483, results.Where(x => x.Beta != null).Count());

            // sample value
            BetaResult r = results[501];
            Assert.AreEqual(1.6759m, Math.Round((decimal)r.Beta, 4));
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<BetaResult> r = Indicator.GetBeta(historyBad, historyBad, 15);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Pruned()
        {
            List<BetaResult> results = Indicator.GetBeta(quotes, historyOther, 20)
                .PruneWarmupPeriods()
                .ToList();

            // assertions
            Assert.AreEqual(502 - 19, results.Count);

            BetaResult last = results.LastOrDefault();
            Assert.AreEqual(1.6759m, Math.Round((decimal)last.Beta, 4));
        }

        [TestMethod]
        public void SameSame()
        {
            // Beta should be 1 if evaluating against self
            List<BetaResult> results = Indicator.GetBeta(quotes, quotes, 20)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is quotes
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(483, results.Where(x => x.Beta != null).Count());

            // sample value
            BetaResult r = results[501];
            Assert.AreEqual(1, Math.Round((decimal)r.Beta, 4));
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad lookback period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetBeta(quotes, historyOther, 0));

            // insufficient quotes
            IEnumerable<Quote> h1 = HistoryTestData.Get(29);
            IEnumerable<Quote> h2 = HistoryTestData.GetCompare(29);
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetBeta(h1, h2, 30));

            // bad evaluation quotes
            IEnumerable<Quote> eval = HistoryTestData.GetCompare(300);
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetBeta(quotes, eval, 30));
        }
    }
}
