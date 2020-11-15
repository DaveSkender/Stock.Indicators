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
        public void GetPrsTest()
        {
            int lookbackPeriod = 30;
            int smaPeriod = 10;
            IEnumerable<PrsResult> results = Indicator.GetPrs(history, historyOther, lookbackPeriod, smaPeriod);

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count());
            Assert.AreEqual(502, results.Count(x => x.Prs != null));
            Assert.AreEqual(493, results.Where(x => x.Sma != null).Count());

            // sample values
            PrsResult r1 = results.Where(x => x.Index == 502).FirstOrDefault();
            Assert.AreEqual(1.356817m, Math.Round((decimal)r1.Prs, 6));
            Assert.AreEqual(1.343445m, Math.Round((decimal)r1.Sma, 6));
            Assert.AreEqual(0.037082m, Math.Round((decimal)r1.PrsPercent, 6));

            PrsResult r2 = results.Where(x => x.Index == 250).FirstOrDefault();
            Assert.AreEqual(1.222373m, Math.Round((decimal)r2.Prs, 6));
            Assert.AreEqual(1.275808m, Math.Round((decimal)r2.Sma, 6));
            Assert.AreEqual(-0.023089m, Math.Round((decimal)r2.PrsPercent, 6));

            PrsResult r3 = results.Where(x => x.Index == 9).FirstOrDefault();
            Assert.AreEqual(1.108340m, Math.Round((decimal)r3.Prs, 6));
            Assert.AreEqual(null, r3.Sma);
            Assert.AreEqual(null, r3.PrsPercent);
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
            Indicator.GetPrs(history, historyOther.Where(x => x.Index < 14), 14);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Not enought Eval history.")]
        public void InsufficientEvalHistory()
        {
            Indicator.GetPrs(history, historyOther.Where(x => x.Index <= 300), 14);
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