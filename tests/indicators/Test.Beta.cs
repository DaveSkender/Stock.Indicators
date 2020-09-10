using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StockIndicators.Tests
{
    [TestClass]
    public class BetaTests : TestBase
    {

        [TestMethod()]
        public void GetBetaTest()
        {
            int lookbackPeriod = 20;
            IEnumerable<BetaResult> results = Indicator.GetBeta(history, historyOther, lookbackPeriod);

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count());
            Assert.AreEqual(502 - lookbackPeriod + 1, results.Where(x => x.Beta != null).Count());

            // sample value
            BetaResult r = results.Where(x => x.Index == 502).FirstOrDefault();
            Assert.AreEqual(1.6759m, Math.Round((decimal)r.Beta, 4));
        }


        [TestMethod()]
        public void GetBetaSameTest()
        {
            // Beta should be 1 if evaluating against self
            int lookbackPeriod = 20;
            IEnumerable<BetaResult> results = Indicator.GetBeta(history, history, lookbackPeriod);

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count());
            Assert.AreEqual(502 - lookbackPeriod + 1, results.Where(x => x.Beta != null).Count());

            // sample value
            BetaResult r = results.Where(x => x.Index == 502).FirstOrDefault();
            Assert.AreEqual(1, Math.Round((decimal)r.Beta, 4));
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(BadParameterException), "Bad lookback.")]
        public void BadLookback()
        {
            Indicator.GetBeta(history, historyOther, 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            Indicator.GetBeta(history.Where(x => x.Index < 30), historyOther.Where(x => x.Index < 30), 30);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Not enought Eval history.")]
        public void InsufficientEvalHistory()
        {
            Indicator.GetBeta(history, historyOther.Where(x => x.Index <= 300), 30);
        }

    }
}