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
            List<HurstResult> results = quotes
                .GetHurst(100)
                .ToList();

            // assertions

            // proper quantities
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(502 - 100, results.Count(x => x.HurstExponent != null));

            // sample values
            HurstResult r501 = results[501];
            Assert.AreEqual(0.492m, Math.Round((decimal)r501.HurstExponent, 3));
        }

        [TestMethod]
        public void StandardLong()
        {
            IEnumerable<Quote> longQuotes = TestData.GetSnP();
            List<HurstResult> results = longQuotes
                .GetHurst(longQuotes.Count() - 1)
                .ToList();

            // assertions

            // proper quantities
            Assert.AreEqual(15821, results.Count);
            Assert.AreEqual(1, results.Count(x => x.HurstExponent != null));

            // sample values
            HurstResult r15820 = results[15820];
            Assert.AreEqual(0.492m, Math.Round((decimal)r15820.HurstExponent, 3));
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<HurstResult> r = Indicator.GetHurst(historyBad, 150);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Removed()
        {
            List<HurstResult> results = quotes.GetHurst(100)
                .RemoveWarmupPeriods()
                .ToList();

            // assertions
            Assert.AreEqual(502 - 100, results.Count);

            HurstResult last = results.LastOrDefault();
            Assert.AreEqual(-0.123754m, Math.Round((decimal)last.HurstExponent, 6));
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad lookback period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetHurst(quotes, 99));

            // insufficient quotes
            Assert.ThrowsException<BadQuotesException>(() =>
                Indicator.GetHurst(HistoryTestData.Get(499), 500));
        }
    }
}
