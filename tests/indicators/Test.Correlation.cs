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
        public void Standard()
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

        [TestMethod()]
        public void BadData()
        {
            IEnumerable<CorrResult> r = Indicator.GetCorrelation(historyBad, historyBad, 15);
            Assert.AreEqual(502, r.Count());
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
            IEnumerable<Quote> h1 = History.GetHistory(29);
            IEnumerable<Quote> h2 = History.GetHistoryOther(29);
            Indicator.GetCorrelation(h1, h2, 30);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Not enought Eval history.")]
        public void InsufficientEvalHistory()
        {
            IEnumerable<Quote> h = History.GetHistory(300);
            Indicator.GetCorrelation(history, h, 30);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Mismatch history.")]
        public void MismatchHistory()
        {
            IEnumerable<Quote> historyMismatch = History.GetHistoryWithMismatchDates();
            Indicator.GetCorrelation(historyMismatch, historyOther, 20);
        }

    }
}