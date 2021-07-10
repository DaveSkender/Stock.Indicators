using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class Rsi : TestBase
    {

        [TestMethod]
        public void Standard()
        {
            List<RsiResult> results = quotes.GetRsi(14).ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is quotes
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(488, results.Where(x => x.Rsi != null).Count());

            // sample values
            RsiResult r1 = results[13];
            Assert.AreEqual(null, r1.Rsi);

            RsiResult r2 = results[14];
            Assert.AreEqual(62.0541m, Math.Round((decimal)r2.Rsi, 4));

            RsiResult r3 = results[249];
            Assert.AreEqual(70.9368m, Math.Round((decimal)r3.Rsi, 4));

            RsiResult r4 = results[501];
            Assert.AreEqual(42.0773m, Math.Round((decimal)r4.Rsi, 4));
        }

        [TestMethod]
        public void SmallLookback()
        {
            int lookbackPeriods = 1;
            List<RsiResult> results = Indicator.GetRsi(quotes, lookbackPeriods).ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is quotes
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(501, results.Where(x => x.Rsi != null).Count());

            // sample values
            RsiResult r1 = results[28];
            Assert.AreEqual(100m, Math.Round((decimal)r1.Rsi, 4));

            RsiResult r2 = results[52];
            Assert.AreEqual(0m, Math.Round((decimal)r2.Rsi, 4));
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<RsiResult> r = Indicator.GetRsi(historyBad, 20);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Pruned()
        {
            List<RsiResult> results = quotes.GetRsi(14)
                .PruneWarmupPeriods()
                .ToList();

            // assertions
            Assert.AreEqual(502 - (10 * 14), results.Count);

            RsiResult last = results.LastOrDefault();
            Assert.AreEqual(42.0773m, Math.Round((decimal)last.Rsi, 4));
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad lookback period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetRsi(quotes, 0));

            // insufficient quotes
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetRsi(HistoryTestData.Get(129), 30));
        }
    }
}
