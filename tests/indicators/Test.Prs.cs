using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class Prs : TestBase
    {

        [TestMethod]
        public void Standard()
        {
            int lookbackPeriod = 30;
            int smaPeriod = 10;

            List<PrsResult> results =
                Indicator.GetPrs(history, historyOther, lookbackPeriod, smaPeriod)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(502, results.Count(x => x.Prs != null));
            Assert.AreEqual(493, results.Where(x => x.PrsSma != null).Count());

            // sample values
            PrsResult r1 = results[8];
            Assert.AreEqual(1.108340m, Math.Round((decimal)r1.Prs, 6));
            Assert.AreEqual(null, r1.PrsSma);
            Assert.AreEqual(null, r1.PrsPercent);

            PrsResult r2 = results[249];
            Assert.AreEqual(1.222373m, Math.Round((decimal)r2.Prs, 6));
            Assert.AreEqual(1.275808m, Math.Round((decimal)r2.PrsSma, 6));
            Assert.AreEqual(-0.023089m, Math.Round((decimal)r2.PrsPercent, 6));

            PrsResult r3 = results[501];
            Assert.AreEqual(1.356817m, Math.Round((decimal)r3.Prs, 6));
            Assert.AreEqual(1.343445m, Math.Round((decimal)r3.PrsSma, 6));
            Assert.AreEqual(0.037082m, Math.Round((decimal)r3.PrsPercent, 6));
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<PrsResult> r = Indicator.GetPrs(historyBad, historyBad, 15, 4);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad lookback period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetPrs(history, historyOther, 0));

            // bad SMA period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetPrs(history, historyOther, 14, 0));

            // insufficient history
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetPrs(history, HistoryTestData.GetCompare(13), 14));

            // insufficient eval history
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetPrs(history, HistoryTestData.GetCompare(300), 14));

            // mismatch history
            IEnumerable<Quote> historyMismatch = HistoryTestData.GetMismatchDates();
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetPrs(historyMismatch, historyOther, 14));
        }
    }
}