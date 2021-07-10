using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class Hurst : TestBase
    {

        [TestMethod]
        public void Standard()
        {
            List<HurstResult> results = quotes.GetHurst(20).ToList();

            // assertions

            // proper quantities
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(483, results.Where(x => x.Hurst != null).Count());

            // sample values
            HurstResult r1 = results[49];
            Assert.AreEqual(0.350596m, Math.Round((decimal)r1.Hurst, 6));

            HurstResult r2 = results[249];
            Assert.AreEqual(-0.040226m, Math.Round((decimal)r2.Hurst, 6));

            HurstResult r3 = results[501];
            Assert.AreEqual(-0.123754m, Math.Round((decimal)r3.Hurst, 6));
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<HurstResult> r = Indicator.GetHurst(historyBad, 15);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Removed()
        {
            List<HurstResult> results = quotes.GetHurst(20)
                .RemoveWarmupPeriods()
                .ToList();

            // assertions
            Assert.AreEqual(502 - 19, results.Count);

            HurstResult last = results.LastOrDefault();
            Assert.AreEqual(-0.123754m, Math.Round((decimal)last.Hurst, 6));
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad lookback period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetHurst(quotes, 0));

            // insufficient quotes
            Assert.ThrowsException<BadQuotesException>(() =>
                Indicator.GetHurst(HistoryTestData.Get(20), 20));
        }
    }
}
