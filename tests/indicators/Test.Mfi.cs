using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class Mfi : TestBase
    {

        [TestMethod]
        public void Standard()
        {
            int lookbackPeriod = 14;
            List<MfiResult> results = history.GetMfi(lookbackPeriod).ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(488, results.Where(x => x.Mfi != null).Count());

            // sample values
            MfiResult r1 = results[439];
            Assert.AreEqual(69.0622m, Math.Round((decimal)r1.Mfi, 4));

            MfiResult r2 = results[501];
            Assert.AreEqual(39.9494m, Math.Round((decimal)r2.Mfi, 4));
        }

        [TestMethod]
        public void SmallLookback()
        {
            int lookbackPeriod = 4;

            List<MfiResult> results = Indicator.GetMfi(history, lookbackPeriod)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(498, results.Where(x => x.Mfi != null).Count());

            // sample values
            MfiResult r1 = results[31];
            Assert.AreEqual(100m, Math.Round((decimal)r1.Mfi, 4));

            MfiResult r2 = results[43];
            Assert.AreEqual(0m, Math.Round((decimal)r2.Mfi, 4));
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<MfiResult> r = Indicator.GetMfi(historyBad, 15);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad lookback period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetMfi(history, 1));

            // insufficient history
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetMfi(HistoryTestData.Get(14), 14));
        }
    }
}
