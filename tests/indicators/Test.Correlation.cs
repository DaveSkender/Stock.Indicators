using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class Correlation : TestBase
    {

        [TestMethod]
        public void Standard()
        {
            int lookbackPeriod = 20;
            List<CorrResult> results =
                history.GetCorrelation(historyOther, lookbackPeriod)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(483, results.Where(x => x.Correlation != null).Count());

            // sample value
            CorrResult r = results[501];
            Assert.AreEqual(0.8460m, Math.Round((decimal)r.Correlation, 4));
            Assert.AreEqual(0.7157m, Math.Round((decimal)r.RSquared, 4));
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<CorrResult> r = Indicator.GetCorrelation(historyBad, historyBad, 15);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad lookback period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetCorrelation(history, historyOther, 0));

            // insufficient history
            IEnumerable<Quote> h1 = HistoryTestData.Get(29);
            IEnumerable<Quote> h2 = HistoryTestData.GetCompare(29);
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetCorrelation(h1, h2, 30));

            // bad eval history
            IEnumerable<Quote> eval = HistoryTestData.GetCompare(300);
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetCorrelation(history, eval, 30));

            // mismatched history
            IEnumerable<Quote> historyMismatch = HistoryTestData.GetMismatchDates();
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetCorrelation(historyMismatch, historyOther, 20));
        }
    }
}
