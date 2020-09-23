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
            int smaPeriod = 14;
            IEnumerable<PrsResult> results = Indicator.GetPrs(history, historyOther, smaPeriod);

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count());
            Assert.AreEqual(false, results.Any(x => x.Prs == null));
            Assert.AreEqual(489, results.Where(x => x.Sma != null).Count());

            // sample values
            PrsResult r1 = results.Where(x => x.Index == 502).FirstOrDefault();
            Assert.AreEqual(1.356817m, Math.Round((decimal)r1.Prs, 6));
            Assert.AreEqual(1.369077m, Math.Round((decimal)r1.Sma, 6));

            PrsResult r2 = results.Where(x => x.Index == 250).FirstOrDefault();
            Assert.AreEqual(1.222373m, Math.Round((decimal)r2.Prs, 6));
            Assert.AreEqual(1.281854m, Math.Round((decimal)r2.Sma, 6));

            PrsResult r3 = results.Where(x => x.Index == 10).FirstOrDefault();
            Assert.AreEqual(1.102129m, Math.Round((decimal)r3.Prs, 6));
            Assert.AreEqual(null, r3.Sma);
        }


        /* EXCEPTIONS */
        [TestMethod()]
        [ExpectedException(typeof(BadParameterException), "Bad SMA period.")]
        public void BadSmaPeriod()
        {
            Indicator.GetPrs(history, historyOther, 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            Indicator.GetPrs(history.Where(x => x.Index < 1), historyOther.Where(x => x.Index < 1));
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Not enought Eval history.")]
        public void InsufficientEvalHistory()
        {
            Indicator.GetPrs(history, historyOther.Where(x => x.Index <= 300));
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Mismatch history.")]
        public void MismatchHistory()
        {
            IEnumerable<Quote> historyGap = History.GetHistoryWithGap();
            Indicator.GetPrs(historyGap, historyOther);
        }
    }
}