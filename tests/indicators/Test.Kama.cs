using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Internal.Tests
{
    [TestClass]
    public class Kama : TestBase
    {

        [TestMethod()]
        public void Standard()
        {
            int erPeriod = 10;
            int fastPeriod = 2;
            int slowPeriod = 30;

            List<KamaResult> results = Indicator.GetKama(history, erPeriod, fastPeriod, slowPeriod)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(493, results.Where(x => x.Kama != null).Count());

            // sample values
            KamaResult r1 = results[501];
            Assert.AreEqual(240.1138m, Math.Round((decimal)r1.Kama, 4));

            KamaResult r2 = results[249];
            Assert.AreEqual(256.0898m, Math.Round((decimal)r2.Kama, 4));

            KamaResult r3 = results[149];
            Assert.AreEqual(235.5510m, Math.Round((decimal)r3.Kama, 4));

            KamaResult r4 = results[24];
            Assert.AreEqual(214.7423m, Math.Round((decimal)r4.Kama, 4));

            KamaResult r5 = results[10];
            Assert.AreEqual(213.7713m, Math.Round((decimal)r5.Kama, 4));

            KamaResult r6 = results[9];
            Assert.AreEqual(213.75m, r6.Kama);

            KamaResult r7 = results[8];
            Assert.AreEqual(null, r7.Kama);
        }

        [TestMethod()]
        public void BadData()
        {
            IEnumerable<KamaResult> r = Indicator.GetKama(historyBad);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod()]
        public void Convergence()
        {
            foreach (int qty in convergeQuantities)
            {
                IEnumerable<Quote> h = History.GetHistoryLong(105 + qty);
                IEnumerable<KamaResult> r = Indicator.GetKama(h, 10);

                KamaResult l = r.LastOrDefault();
                Console.WriteLine("KAMA on {0:d} with {1,4} periods: {2:N8}",
                    l.Date, h.Count(), l.Kama);
            }
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Bad ER period.")]
        public void BadErPeriod()
        {
            Indicator.GetKama(history, 0, 2, 30);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Bad Fast period.")]
        public void BadFastPeriod()
        {
            Indicator.GetKama(history, 10, 0, 30);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Bad Slow period.")]
        public void BadSlowPeriod()
        {
            Indicator.GetKama(history, 10, 5, 5);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            IEnumerable<Quote> h = History.GetHistory(109);
            Indicator.GetKama(h, 10, 2, 20);
        }

    }
}