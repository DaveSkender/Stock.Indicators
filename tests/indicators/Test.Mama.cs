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
            Assert.AreEqual(493, results.Where(x => x.Mama != null).Count());

            // sample values
            MamaResult r1 = results[501];
            Assert.AreEqual(0m, Math.Round((decimal)r1.Mama, 4));

            MamaResult r2 = results[249];
            Assert.AreEqual(0m, Math.Round((decimal)r2.Mama, 4));

            MamaResult r3 = results[149];
            Assert.AreEqual(0m, Math.Round((decimal)r3.Mama, 4));

            MamaResult r4 = results[99];
            Assert.AreEqual(0m, Math.Round((decimal)r4.Mama, 4));

            MamaResult r5 = results[24];
            Assert.AreEqual(0m, Math.Round((decimal)r5.Mama, 4));

            MamaResult r6 = results[13];
            Assert.AreEqual(0m, r6.Mama);

            MamaResult r7 = results[12];
            Assert.AreEqual(null, r7.Mama);
        }

        [TestMethod()]
        public void GetMamaBadData()
        {
            IEnumerable<MamaResult> r = Indicator.GetMama(historyBad);
            Assert.AreEqual(502, r.Count());
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
            IEnumerable<Quote> h = History.GetHistory(5);
            Indicator.GetMama(h);
        }

    }
}
