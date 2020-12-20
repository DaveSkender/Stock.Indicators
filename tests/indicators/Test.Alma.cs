using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Internal.Tests
{
    [TestClass]
    public class AlmaTests : TestBase
    {

        [TestMethod()]
        public void GetAlma()
        {
            int lookbackPeriod = 10;
            double offset = 0.85;
            double sigma = 6;

            List<AlmaResult> results = Indicator.GetAlma(history, lookbackPeriod, offset, sigma)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(493, results.Where(x => x.Alma != null).Count());

            // sample values
            AlmaResult r1 = results[501];
            Assert.AreEqual(242.1871m, Math.Round((decimal)r1.Alma, 4));

            AlmaResult r2 = results[249];
            Assert.AreEqual(257.5787m, Math.Round((decimal)r2.Alma, 4));

            AlmaResult r3 = results[149];
            Assert.AreEqual(235.8609m, Math.Round((decimal)r3.Alma, 4));

            AlmaResult r4 = results[24];
            Assert.AreEqual(216.0619m, Math.Round((decimal)r4.Alma, 4));

            AlmaResult r5 = results[9];
            Assert.AreEqual(214.1839m, Math.Round((decimal)r5.Alma, 4));

            AlmaResult r6 = results[8];
            Assert.AreEqual(null, r6.Alma);
        }

        [TestMethod()]
        public void GetAlmaBadData()
        {
            IEnumerable<AlmaResult> r = Indicator.GetAlma(historyBad, 14, 0.5, 3);
            Assert.AreEqual(502, r.Count());
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Bad Lookback period.")]
        public void BadLookbackPeriod()
        {
            Indicator.GetAlma(history, 0, 1, 5);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Bad Offset.")]
        public void BadOffset()
        {
            Indicator.GetAlma(history, 15, 1.1, 3);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Bad Signma.")]
        public void BadSigma()
        {
            Indicator.GetAlma(history, 10, 0.5, 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            IEnumerable<Quote> h = History.GetHistory(10);
            Indicator.GetAlma(h, 11, 0.5);
        }

    }
}
