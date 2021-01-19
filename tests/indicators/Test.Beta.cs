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

            List<BetaResult> results = Indicator.GetBeta(history, historyOther, 20)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
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
        public void SameSame()
        {
            // Beta should be 1 if evaluating against self
            List<BetaResult> results = Indicator.GetBeta(history, history, 20)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
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
                Indicator.GetBeta(history, historyOther, 0));

            // insufficient history
            IEnumerable<Quote> h1 = HistoryTestData.Get(29);
            IEnumerable<Quote> h2 = HistoryTestData.GetCompare(29);
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetBeta(h1, h2, 30));

            // bad evaluation history
            IEnumerable<Quote> eval = HistoryTestData.GetCompare(300);
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetBeta(history, eval, 30));
        }
    }
}