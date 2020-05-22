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
            BetaResult result = results.Where(x => x.Date == DateTime.Parse("12/31/2018")).FirstOrDefault();
            Assert.AreEqual((decimal)1.6759, Math.Round((decimal)result.Beta, 4));
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
            BetaResult result = results.Where(x => x.Date == DateTime.Parse("12/31/2018")).FirstOrDefault();
            Assert.AreEqual(1, Math.Round((decimal)result.Beta, 4));
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            Indicator.GetBeta(history.Where(x => x.Index < 30), historyOther.Where(x => x.Index < 30), 30);
        }

    }
}