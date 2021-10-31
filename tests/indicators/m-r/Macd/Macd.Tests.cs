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
            int fastPeriods = 12;
            int slowPeriods = 26;
            int signalPeriods = 9;

            List<MacdResult> results =
                quotes.GetMacd(fastPeriods, slowPeriods, signalPeriods)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is quotes
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(477, results.Where(x => x.Macd != null).Count());
            Assert.AreEqual(469, results.Where(x => x.Signal != null).Count());
            Assert.AreEqual(469, results.Where(x => x.Histogram != null).Count());

            // sample values
            MacdResult r49 = results[49];
            Assert.AreEqual(1.7203m, Math.Round((decimal)r49.Macd, 4));
            Assert.AreEqual(1.9675m, Math.Round((decimal)r49.Signal, 4));
            Assert.AreEqual(-0.2472m, Math.Round((decimal)r49.Histogram, 4));
            Assert.AreEqual(224.1840m, Math.Round((decimal)r49.FastEma, 4));
            Assert.AreEqual(222.4637m, Math.Round((decimal)r49.SlowEma, 4));

            MacdResult r249 = results[249];
            Assert.AreEqual(2.2353m, Math.Round((decimal)r249.Macd, 4));
            Assert.AreEqual(2.3141m, Math.Round((decimal)r249.Signal, 4));
            Assert.AreEqual(-0.0789m, Math.Round((decimal)r249.Histogram, 4));
            Assert.AreEqual(256.6780m, Math.Round((decimal)r249.FastEma, 4));
            Assert.AreEqual(254.4428m, Math.Round((decimal)r249.SlowEma, 4));

            MacdResult r501 = results[501];
            Assert.AreEqual(-6.2198m, Math.Round((decimal)r501.Macd, 4));
            Assert.AreEqual(-5.8569m, Math.Round((decimal)r501.Signal, 4));
            Assert.AreEqual(-0.3629m, Math.Round((decimal)r501.Histogram, 4));
            Assert.AreEqual(245.4957m, Math.Round((decimal)r501.FastEma, 4));
            Assert.AreEqual(251.7155m, Math.Round((decimal)r501.SlowEma, 4));
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<MacdResult> r = Indicator.GetMacd(badQuotes, 10, 20, 5);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Removed()
        {
            int fastPeriods = 12;
            int slowPeriods = 26;
            int signalPeriods = 9;

            List<MacdResult> results =
                quotes.GetMacd(fastPeriods, slowPeriods, signalPeriods)
                .RemoveWarmupPeriods()
                .ToList();

            // assertions
            Assert.AreEqual(502 - (slowPeriods + signalPeriods + 250), results.Count);

            MacdResult last = results.LastOrDefault();
            Assert.AreEqual(-6.2198m, Math.Round((decimal)last.Macd, 4));
            Assert.AreEqual(-5.8569m, Math.Round((decimal)last.Signal, 4));
            Assert.AreEqual(-0.3629m, Math.Round((decimal)last.Histogram, 4));
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad fast period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetMacd(quotes, 0, 26, 9));

            // bad slow periods must be larger than faster period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetMacd(quotes, 12, 12, 9));

            // bad signal period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetMacd(quotes, 12, 26, -1));

            // insufficient quotes 2×(S+P)
            Assert.ThrowsException<BadQuotesException>(() =>
                Indicator.GetMacd(TestData.GetDefault(409), 12, 200, 5));

            // insufficient quotes S+P+100
            Assert.ThrowsException<BadQuotesException>(() =>
                Indicator.GetMacd(TestData.GetDefault(134), 12, 26, 9));
        }
    }
}
