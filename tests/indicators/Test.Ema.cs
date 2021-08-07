using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class Ema : TestBase
    {

        [TestMethod]
        public void Standard()
        {
            List<EmaResult> results = quotes.GetEma(20).ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is quotes
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(483, results.Where(x => x.Ema != null).Count());

            // sample values
            EmaResult r1 = results[29];
            Assert.AreEqual(216.6228m, Math.Round((decimal)r1.Ema, 4));

            EmaResult r2 = results[249];
            Assert.AreEqual(255.3873m, Math.Round((decimal)r2.Ema, 4));

            EmaResult r3 = results[501];
            Assert.AreEqual(249.3519m, Math.Round((decimal)r3.Ema, 4));
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<EmaResult> r = Indicator.GetEma(historyBad, 15);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Removed()
        {
            List<EmaResult> results = quotes.GetEma(20)
                .RemoveWarmupPeriods()
                .ToList();

            // assertions
            Assert.AreEqual(502 - (20 + 100), results.Count);

            EmaResult last = results.LastOrDefault();
            Assert.AreEqual(249.3519m, Math.Round((decimal)last.Ema, 4));
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad lookback period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetEma(quotes, 0));

            // insufficient quotes for N+100
            Assert.ThrowsException<BadQuotesException>(() =>
                Indicator.GetEma(TestData.GetDefault(129), 30));

            // insufficient quotes for 2×N
            Assert.ThrowsException<BadQuotesException>(() =>
                Indicator.GetEma(TestData.GetDefault(499), 250));
        }
    }
}
