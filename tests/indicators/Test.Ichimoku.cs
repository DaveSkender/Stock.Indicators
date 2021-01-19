using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class Ichimoku : TestBase
    {

        [TestMethod]
        public void Standard()
        {
            int signalPeriod = 9;
            int shortSpanPeriod = 26;
            int longSpanPeriod = 52;

            List<IchimokuResult> results = Indicator.GetIchimoku(
                history, signalPeriod, shortSpanPeriod, longSpanPeriod)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(494, results.Where(x => x.TenkanSen != null).Count());
            Assert.AreEqual(477, results.Where(x => x.KijunSen != null).Count());
            Assert.AreEqual(451, results.Where(x => x.SenkouSpanA != null).Count());
            Assert.AreEqual(425, results.Where(x => x.SenkouSpanB != null).Count());
            Assert.AreEqual(476, results.Where(x => x.ChikouSpan != null).Count());

            // sample values
            IchimokuResult r1 = results[51];
            Assert.AreEqual(224.465m, r1.TenkanSen);
            Assert.AreEqual(221.94m, r1.KijunSen);
            Assert.AreEqual(214.8325m, r1.SenkouSpanA);
            Assert.AreEqual(null, r1.SenkouSpanB);
            Assert.AreEqual(226.35m, r1.ChikouSpan);

            IchimokuResult r2 = results[249];
            Assert.AreEqual(257.15m, r2.TenkanSen);
            Assert.AreEqual(253.085m, r2.KijunSen);
            Assert.AreEqual(246.3125m, r2.SenkouSpanA);
            Assert.AreEqual(241.685m, r2.SenkouSpanB);
            Assert.AreEqual(259.21m, r2.ChikouSpan);

            IchimokuResult r3 = results[475];
            Assert.AreEqual(265.575m, r3.TenkanSen);
            Assert.AreEqual(263.965m, r3.KijunSen);
            Assert.AreEqual(274.9475m, r3.SenkouSpanA);
            Assert.AreEqual(274.95m, r3.SenkouSpanB);
            Assert.AreEqual(245.28m, r3.ChikouSpan);

            IchimokuResult r4 = results[501];
            Assert.AreEqual(241.26m, r4.TenkanSen);
            Assert.AreEqual(251.505m, r4.KijunSen);
            Assert.AreEqual(264.77m, r4.SenkouSpanA);
            Assert.AreEqual(269.82m, r4.SenkouSpanB);
            Assert.AreEqual(null, r4.ChikouSpan);
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<IchimokuResult> r = Indicator.GetIchimoku(historyBad, 8, 20, 35);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad signal period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetIchimoku(history, 0));

            // bad short span period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetIchimoku(history, 9, 0, 52));

            // bad long span period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetIchimoku(history, 9, 26, 26));

            // insufficient history
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetIchimoku(HistoryTestData.Get(51), 9, 26, 52));
        }
    }
}