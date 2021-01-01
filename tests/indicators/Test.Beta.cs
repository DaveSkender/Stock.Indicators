using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Internal.Tests
{
    [TestClass]
    public class Beta : TestBase
    {

        [TestMethod()]
        public void Standard()
        {
            int lookbackPeriod = 20;
            List<BetaResult> results = Indicator.GetBeta(history, historyOther, lookbackPeriod).ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(502 - lookbackPeriod + 1, results.Where(x => x.Beta != null).Count());

            // sample value
            BetaResult r = results[501];
            Assert.AreEqual(1.6759m, Math.Round((decimal)r.Beta, 4));
        }

        [TestMethod()]
        public void BadData()
        {
            IEnumerable<BetaResult> r = Indicator.GetBeta(historyBad, historyBad, 15);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod()]
        public void SameSame()
        {
            // Beta should be 1 if evaluating against self
            int lookbackPeriod = 20;
            List<BetaResult> results = Indicator.GetBeta(history, history, lookbackPeriod).ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(502 - lookbackPeriod + 1, results.Where(x => x.Beta != null).Count());

            // sample value
            BetaResult r = results[501];
            Assert.AreEqual(1, Math.Round((decimal)r.Beta, 4));
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Bad lookback.")]
        public void BadLookbackPeriod()
        {
            Indicator.GetBeta(history, historyOther, 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            IEnumerable<Quote> h1 = History.GetHistory(29);
            IEnumerable<Quote> h2 = History.GetHistoryOther(29);
            Indicator.GetBeta(h1, h2, 30);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Not enought Eval history.")]
        public void InsufficientEvalHistory()
        {
            IEnumerable<Quote> h = History.GetHistoryOther(300);
            Indicator.GetBeta(history, h, 30);
        }

    }
}