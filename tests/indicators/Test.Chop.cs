using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class Chop : TestBase
    {
        [TestMethod]
        public void Standard()
        {
            int lookbackPeriod = 14;
            List<ChopResult> results = history.GetChop(lookbackPeriod)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(488, results.Where(x => x.Chop != null).Count());

            // sample values
            ChopResult r1 = results[13];
            Assert.AreEqual(null, r1.Chop);

            ChopResult r2 = results[14];
            Assert.AreEqual(69.9967m, Math.Round((decimal)r2.Chop, 4));

            ChopResult r3 = results[249];
            Assert.AreEqual(41.8499m, Math.Round((decimal)r3.Chop, 4));

            ChopResult r4 = results[501];
            Assert.AreEqual(38.6526m, Math.Round((decimal)r4.Chop, 4));
        }

        [TestMethod]
        public void SmallLookback()
        {
            int lookbackPeriod = 2;
            List<ChopResult> results = Indicator.GetChop(history, lookbackPeriod).ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(500, results.Where(x => x.Chop != null).Count());
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<ChopResult> r = Indicator.GetChop(historyBad, 20);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad lookback period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetChop(history, 1));

            // insufficient history
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetChop(HistoryTestData.Get(30), 30));
        }
    }
}
