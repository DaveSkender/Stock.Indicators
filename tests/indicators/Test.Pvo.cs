using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class Pvo : TestBase
    {

        [TestMethod]
        public void Standard()
        {
            int fastPeriod = 12;
            int slowPeriod = 26;
            int signalPeriod = 9;

            List<PvoResult> results =
                Indicator.GetPvo(history, fastPeriod, slowPeriod, signalPeriod)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(477, results.Where(x => x.Pvo != null).Count());
            Assert.AreEqual(469, results.Where(x => x.Signal != null).Count());
            Assert.AreEqual(469, results.Where(x => x.Histogram != null).Count());

            // sample values
            PvoResult r1 = results[24];
            Assert.AreEqual(null, r1.Pvo);
            Assert.AreEqual(null, r1.Signal);
            Assert.AreEqual(null, r1.Histogram);

            PvoResult r2 = results[33];
            Assert.AreEqual(1.5795m, Math.Round((decimal)r2.Pvo, 4));
            Assert.AreEqual(-3.5530m, Math.Round((decimal)r2.Signal, 4));
            Assert.AreEqual(5.1325m, Math.Round((decimal)r2.Histogram, 4));

            PvoResult r3 = results[149];
            Assert.AreEqual(-7.1910m, Math.Round((decimal)r3.Pvo, 4));
            Assert.AreEqual(-5.1159m, Math.Round((decimal)r3.Signal, 4));
            Assert.AreEqual(-2.0751m, Math.Round((decimal)r3.Histogram, 4));

            PvoResult r4 = results[249];
            Assert.AreEqual(-6.3667m, Math.Round((decimal)r4.Pvo, 4));
            Assert.AreEqual(1.7333m, Math.Round((decimal)r4.Signal, 4));
            Assert.AreEqual(-8.1000m, Math.Round((decimal)r4.Histogram, 4));

            PvoResult r5 = results[501];
            Assert.AreEqual(10.4395m, Math.Round((decimal)r5.Pvo, 4));
            Assert.AreEqual(12.2681m, Math.Round((decimal)r5.Signal, 4));
            Assert.AreEqual(-1.8286m, Math.Round((decimal)r5.Histogram, 4));
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<PvoResult> r = Indicator.GetPvo(historyBad, 10, 20, 5);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Convergence()
        {
            foreach (int qty in convergeQuantities)
            {
                IEnumerable<Quote> h = HistoryTestData.GetLong(130 + qty);
                IEnumerable<PvoResult> r = Indicator.GetPvo(h);

                PvoResult l = r.LastOrDefault();
                Console.WriteLine("PVO on {0:d} with {1,4} periods: {2:N8}",
                    l.Date, h.Count(), l.Pvo);
            }
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad fast period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetPvo(history, 0, 26, 9));

            // bad slow period must be larger than faster period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetPvo(history, 12, 12, 9));

            // bad signal period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetPvo(history, 12, 26, -1));

            // insufficient history 2×(S+P)
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetPvo(HistoryTestData.Get(409), 12, 200, 5));

            // insufficient history S+P+100
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetPvo(HistoryTestData.Get(134), 12, 26, 9));
        }
    }
}
