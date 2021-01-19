using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class Bop : TestBase
    {

        [TestMethod]
        public void Standard()
        {

            List<BopResult> results = Indicator.GetBop(history, 14)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(489, results.Where(x => x.Bop != null).Count());

            // sample values
            BopResult r1 = results[12];
            Assert.AreEqual(null, r1.Bop);

            BopResult r2 = results[13];
            Assert.AreEqual(0.081822m, Math.Round((decimal)r2.Bop, 6));

            BopResult r3 = results[149];
            Assert.AreEqual(-0.016203m, Math.Round((decimal)r3.Bop, 6));

            BopResult r4 = results[249];
            Assert.AreEqual(-0.058682m, Math.Round((decimal)r4.Bop, 6));

            BopResult r5 = results[501];
            Assert.AreEqual(-0.292788m, Math.Round((decimal)r5.Bop, 6));
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<BopResult> r = Indicator.GetBop(historyBad);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad smoothing period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetBop(history, 0));

            // insufficient history
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetBop(HistoryTestData.Get(24), 25));
        }
    }
}