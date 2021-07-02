using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class Macd : TestBase
    {

        [TestMethod]
        public void Standard()
        {
            int fastPeriod = 12;
            int slowPeriod = 26;
            int signalPeriod = 9;

            List<MacdResult> results =
                history.GetMacd(fastPeriod, slowPeriod, signalPeriod)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(477, results.Where(x => x.Macd != null).Count());
            Assert.AreEqual(469, results.Where(x => x.Signal != null).Count());
            Assert.AreEqual(469, results.Where(x => x.Histogram != null).Count());

            // sample values
            MacdResult r1 = results[49];
            Assert.AreEqual(1.7203m, Math.Round((decimal)r1.Macd, 4));
            Assert.AreEqual(1.9675m, Math.Round((decimal)r1.Signal, 4));
            Assert.AreEqual(-0.2472m, Math.Round((decimal)r1.Histogram, 4));

            MacdResult r2 = results[249];
            Assert.AreEqual(2.2353m, Math.Round((decimal)r2.Macd, 4));
            Assert.AreEqual(2.3141m, Math.Round((decimal)r2.Signal, 4));
            Assert.AreEqual(-0.0789m, Math.Round((decimal)r2.Histogram, 4));

            MacdResult r3 = results[501];
            Assert.AreEqual(-6.2198m, Math.Round((decimal)r3.Macd, 4));
            Assert.AreEqual(-5.8569m, Math.Round((decimal)r3.Signal, 4));
            Assert.AreEqual(-0.3629m, Math.Round((decimal)r3.Histogram, 4));
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<MacdResult> r = Indicator.GetMacd(historyBad, 10, 20, 5);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad fast period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetMacd(history, 0, 26, 9));

            // bad slow period must be larger than faster period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetMacd(history, 12, 12, 9));

            // bad signal period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetMacd(history, 12, 26, -1));

            // insufficient history 2×(S+P)
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetMacd(HistoryTestData.Get(409), 12, 200, 5));

            // insufficient history S+P+100
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetMacd(HistoryTestData.Get(134), 12, 26, 9));
        }
    }
}
