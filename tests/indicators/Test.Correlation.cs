using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Internal.Tests
{
    [TestClass]
    public class CorrelationTests : TestBase
    {

        [TestMethod()]
        public void GetCorrelationTest()
        {
            int lookbackPeriod = 20;
            List<CorrResult> results =
                Indicator.GetCorrelation(history, historyOther, lookbackPeriod)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(502 - lookbackPeriod + 1, results.Where(x => x.Correlation != null).Count());

            // sample value
            CorrResult r = results[501];
            Assert.AreEqual(0.8460m, Math.Round((decimal)r.Correlation, 4));
            Assert.AreEqual(0.7157m, Math.Round((decimal)r.RSquared, 4));
        }


        /* EXCEPTIONS */
        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Bad lookback.")]
        public void BadLookbackPeriod()
        {
            Indicator.GetCorrelation(history, historyOther, 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            Indicator.GetCorrelation(history.Where(x => x.Index < 30), historyOther.Where(x => x.Index < 30), 30);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Not enought Eval history.")]
        public void InsufficientEvalHistory()
        {
            Indicator.GetCorrelation(history, historyOther.Where(x => x.Index <= 300), 30);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Mismatch history.")]
        public void GetCorrelationMismatchTest()
        {
            IEnumerable<Quote> historyMismatch = History.GetHistoryWithMismatchDates();
            Indicator.GetCorrelation(historyMismatch, historyOther, 20);
        }

    }
}