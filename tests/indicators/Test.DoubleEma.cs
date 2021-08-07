using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class DoubleEma : TestBase
    {

        [TestMethod]
        public void Standard()
        {
            List<DemaResult> results = quotes.GetDoubleEma(20)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is quotes
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(464, results.Where(x => x.Dema != null).Count());

            // sample values
            DemaResult r1 = results[51];
            Assert.AreEqual(226.0011m, Math.Round((decimal)r1.Dema, 4));

            DemaResult r2 = results[249];
            Assert.AreEqual(258.4452m, Math.Round((decimal)r2.Dema, 4));

            DemaResult r3 = results[501];
            Assert.AreEqual(241.1677m, Math.Round((decimal)r3.Dema, 4));
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<DemaResult> r = Indicator.GetDoubleEma(historyBad, 15);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Removed()
        {
            List<DemaResult> results = quotes.GetDoubleEma(20)
                .RemoveWarmupPeriods()
                .ToList();

            // assertions
            Assert.AreEqual(502 - (2 * 20 + 100), results.Count);

            DemaResult last = results.LastOrDefault();
            Assert.AreEqual(241.1677m, Math.Round((decimal)last.Dema, 4));
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad lookback period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetDoubleEma(quotes, 0));

            // insufficient quotes for 2*N+100
            Assert.ThrowsException<BadQuotesException>(() =>
                Indicator.GetDoubleEma(TestData.GetDefault(159), 30));

            // insufficient quotes for 3×N
            Assert.ThrowsException<BadQuotesException>(() =>
                Indicator.GetDoubleEma(HistoryTestData.GetLong(749), 250));
        }
    }
}
