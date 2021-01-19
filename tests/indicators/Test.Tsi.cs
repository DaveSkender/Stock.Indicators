using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class Tsi : TestBase
    {

        [TestMethod]
        public void Standard()
        {

            List<TsiResult> results = Indicator.GetTsi(history, 25, 13, 7)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(465, results.Where(x => x.Tsi != null).Count());
            Assert.AreEqual(459, results.Where(x => x.Signal != null).Count());

            // sample values
            TsiResult r2 = results[37];
            Assert.AreEqual(53.1204m, Math.Round((decimal)r2.Tsi, 4));
            Assert.AreEqual(null, r2.Signal);

            TsiResult r3a = results[43];
            Assert.AreEqual(46.0960m, Math.Round((decimal)r3a.Tsi, 4));
            Assert.AreEqual(51.6916m, Math.Round((decimal)r3a.Signal, 4));

            TsiResult r3b = results[44];
            Assert.AreEqual(42.5121m, Math.Round((decimal)r3b.Tsi, 4));
            Assert.AreEqual(49.3967m, Math.Round((decimal)r3b.Signal, 4));

            TsiResult r4 = results[149];
            Assert.AreEqual(29.0936m, Math.Round((decimal)r4.Tsi, 4));
            Assert.AreEqual(28.0134m, Math.Round((decimal)r4.Signal, 4));

            TsiResult r5 = results[249];
            Assert.AreEqual(41.9232m, Math.Round((decimal)r5.Tsi, 4));
            Assert.AreEqual(42.4063m, Math.Round((decimal)r5.Signal, 4));

            TsiResult r6 = results[501];
            Assert.AreEqual(-28.3513m, Math.Round((decimal)r6.Tsi, 4));
            Assert.AreEqual(-29.3597m, Math.Round((decimal)r6.Signal, 4));
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<TsiResult> r = Indicator.GetTsi(historyBad);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Convergence()
        {
            foreach (int qty in convergeQuantities)
            {
                IEnumerable<Quote> h = HistoryTestData.GetLong(135 + qty);
                IEnumerable<TsiResult> r = Indicator.GetTsi(h);

                TsiResult l = r.LastOrDefault();
                Console.WriteLine("TSI on {0:d} with {1,4} periods: {2:N8}",
                    l.Date, h.Count(), l.Tsi);
            }
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad lookback period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetTsi(history, 0));

            // bad smoothing period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetTsi(history, 25, 0));

            // bad signal period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetTsi(history, 25, 13, -1));

            // insufficient history
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetTsi(HistoryTestData.Get(137), 25, 13, 7));
        }
    }
}