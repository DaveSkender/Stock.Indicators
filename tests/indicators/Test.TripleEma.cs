using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class TripleEma : TestBase
    {

        [TestMethod]
        public void Standard()
        {
            List<TemaResult> results = quotes.GetTripleEma(20).ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is quotes
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(445, results.Where(x => x.Tema != null).Count());

            // sample values
            TemaResult r1 = results[67];
            Assert.AreEqual(222.9105m, Math.Round((decimal)r1.Tema, 4));

            TemaResult r2 = results[249];
            Assert.AreEqual(258.6208m, Math.Round((decimal)r2.Tema, 4));

            TemaResult r3 = results[501];
            Assert.AreEqual(238.7690m, Math.Round((decimal)r3.Tema, 4));
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<TemaResult> r = Indicator.GetTripleEma(historyBad, 15);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Removed()
        {
            List<TemaResult> results = quotes.GetTripleEma(20)
                .RemoveWarmupPeriods()
                .ToList();

            // assertions
            Assert.AreEqual(502 - (3 * 20 + 100), results.Count);

            TemaResult last = results.LastOrDefault();
            Assert.AreEqual(238.7690m, Math.Round((decimal)last.Tema, 4));
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad lookback period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetTripleEma(quotes, 0));

            // insufficient quotes for 3*N+100
            Assert.ThrowsException<BadQuotesException>(() =>
                Indicator.GetTripleEma(TestData.GetDefault(189), 30));

            // insufficient quotes for 4×N
            Assert.ThrowsException<BadQuotesException>(() =>
                Indicator.GetTripleEma(HistoryTestData.GetLong(999), 250));
        }
    }
}
