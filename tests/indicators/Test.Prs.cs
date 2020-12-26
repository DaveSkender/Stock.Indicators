using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Internal.Tests
{
    [TestClass]
    public class PriceRelativeTests : TestBase
    {

        [TestMethod()]
        public void GetPrs()
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
            PrsResult r1 = results[501];
            Assert.AreEqual(1.356817m, Math.Round((decimal)r1.Prs, 6));
            Assert.AreEqual(1.343445m, Math.Round((decimal)r1.PrsSma, 6));
            Assert.AreEqual(0.037082m, Math.Round((decimal)r1.PrsPercent, 6));

            PrsResult r2 = results[249];
            Assert.AreEqual(1.222373m, Math.Round((decimal)r2.Prs, 6));
            Assert.AreEqual(1.275808m, Math.Round((decimal)r2.PrsSma, 6));
            Assert.AreEqual(-0.023089m, Math.Round((decimal)r2.PrsPercent, 6));

            PrsResult r3 = results[8];
            Assert.AreEqual(1.108340m, Math.Round((decimal)r3.Prs, 6));
            Assert.AreEqual(null, r3.PrsSma);
            Assert.AreEqual(null, r3.PrsPercent);
        }

        [TestMethod()]
        public void GetPrsBadData()
        {
            IEnumerable<PrsResult> r = Indicator.GetPrs(historyBad, historyBad, 15, 4);
            Assert.AreEqual(502, r.Count());
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Bad lookback period.")]
        public void BadLookbackPeriod()
        {
            Indicator.GetPrs(history, historyOther, 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Bad SMA period.")]
        public void BadSmaPeriod()
        {
            Indicator.GetPrs(history, historyOther, 14, 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            IEnumerable<Quote> h = History.GetHistoryOther(13);
            Indicator.GetPrs(history, h, 14);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Not enought Eval history.")]
        public void InsufficientEvalHistory()
        {
            IEnumerable<Quote> h = History.GetHistoryOther(300);
            Indicator.GetPrs(history, h, 14);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Mismatch history.")]
        public void MismatchHistory()
        {
            IEnumerable<Quote> historyGap = History.GetHistoryWithMismatchDates();
            Indicator.GetPrs(historyGap, historyOther, 14);
        }

    }
}