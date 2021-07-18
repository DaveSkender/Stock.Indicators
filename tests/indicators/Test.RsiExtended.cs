using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class RsiExtended : TestBase
    {

        [TestMethod]
        public void Standard()
        {

            List<RsiExtendedResult> results = quotes.GetRsiExtended(14)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(488, results.Where(x => x.Rsi != null).Count());

            // sample values
            RsiExtendedResult r1 = results[13];
            Assert.AreEqual(null, r1.Rsi);

            RsiExtendedResult r2 = results[14];
            Assert.AreEqual(62.0541m, Math.Round((decimal)r2.Rsi, 4));

            RsiExtendedResult r3 = results[249];
            Assert.AreEqual(70.9368m, Math.Round((decimal)r3.Rsi, 4));

            RsiResult r4 = results[501];
            Assert.AreEqual(42.0773m, Math.Round((decimal)r4.Rsi, 4));
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<RsiExtendedResult> r = historyBad.GetRsiExtended(20);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad lookback period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetRsiExtended(quotes, 0));

            // insufficient history
            Assert.ThrowsException<BadQuotesException>(() =>
                Indicator.GetRsiExtended(HistoryTestData.Get(129), 30));
        }
    }
}
