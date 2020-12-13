using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Internal.Tests
{
    [TestClass]
    public class MamaTests : TestBase
    {

        [TestMethod()]
        public void GetMama()
        {
            decimal fastLimit = 0.5m;
            decimal slowLimit = 0.05m;

            List<MamaResult> results = Indicator.GetMama(history, fastLimit, slowLimit)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(497, results.Where(x => x.Mama != null).Count());

            // sample values
            MamaResult r1 = results[501];
            Assert.AreEqual(244.1092m, Math.Round((decimal)r1.Mama, 4));
            Assert.AreEqual(252.6139m, Math.Round((decimal)r1.Fama, 4));

            MamaResult r2 = results[249];
            Assert.AreEqual(256.8026m, Math.Round((decimal)r2.Mama, 4));
            Assert.AreEqual(254.0605m, Math.Round((decimal)r2.Fama, 4));

            MamaResult r3 = results[149];
            Assert.AreEqual(235.6593m, Math.Round((decimal)r3.Mama, 4));
            Assert.AreEqual(234.3660m, Math.Round((decimal)r3.Fama, 4));

            MamaResult r4 = results[25];
            Assert.AreEqual(215.9524m, Math.Round((decimal)r4.Mama, 4));
            Assert.AreEqual(215.1407m, Math.Round((decimal)r4.Fama, 4));

            MamaResult r5 = results[6];
            Assert.AreEqual(213.7850m, Math.Round((decimal)r5.Mama, 4));
            Assert.AreEqual(213.7438m, Math.Round((decimal)r5.Fama, 4));

            MamaResult r6 = results[5];
            Assert.AreEqual(213.73m, r6.Mama);
            Assert.AreEqual(213.73m, r6.Fama);

            MamaResult r7 = results[4];
            Assert.AreEqual(null, r7.Mama);
            Assert.AreEqual(null, r7.Fama);
        }

        [TestMethod()]
        public void GetMamaBadData()
        {
            IEnumerable<MamaResult> r = Indicator.GetMama(historyBad);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod()]
        public void GetMamaConvergence()
        {
            foreach (int qty in convergeQuantities.Where(q => q >= 50))
            {
                IEnumerable<Quote> h = History.GetHistoryLong(qty);
                IEnumerable<MamaResult> r = Indicator.GetMama(h);

                MamaResult l = r.LastOrDefault();
                Console.WriteLine("MAMA on {0:d} with {1,4} periods of history: {2:N8}",
                    l.Date, h.Count(), l.Mama);
            }
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Bad Fast limit.")]
        public void BadFastLimitCompare()
        {
            Indicator.GetMama(history, 0.5m, 0.5m);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Bad Fast limit.")]
        public void BadFastLimitSize()
        {
            Indicator.GetMama(history, 1m, 0.5m);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Bad Slow limit.")]
        public void BadSlowLimit()
        {
            Indicator.GetMama(history, 0.5m, 0m);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            IEnumerable<Quote> h = History.GetHistory(49);
            Indicator.GetMama(h);
        }

    }
}
