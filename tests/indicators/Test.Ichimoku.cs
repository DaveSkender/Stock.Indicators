using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Internal.Tests
{
    [TestClass]
    public class IchimokuTests : TestBase
    {

        [TestMethod()]
        public void GetIchimokuTest()
        {
            int signalPeriod = 9;
            int shortSpanPeriod = 26;
            int longSpanPeriod = 52;

            IEnumerable<IchimokuResult> results = Indicator.GetIchimoku(
                history, signalPeriod, shortSpanPeriod, longSpanPeriod);

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count());
            Assert.AreEqual(494, results.Where(x => x.TenkanSen != null).Count());
            Assert.AreEqual(477, results.Where(x => x.KijunSen != null).Count());
            Assert.AreEqual(451, results.Where(x => x.SenkouSpanA != null).Count());
            Assert.AreEqual(425, results.Where(x => x.SenkouSpanB != null).Count());
            Assert.AreEqual(476, results.Where(x => x.ChikouSpan != null).Count());

            // sample values
            IchimokuResult r1 = results.Where(x => x.Index == 476).FirstOrDefault();
            Assert.AreEqual(265.575m, r1.TenkanSen);
            Assert.AreEqual(263.965m, r1.KijunSen);
            Assert.AreEqual(274.9475m, r1.SenkouSpanA);
            Assert.AreEqual(274.95m, r1.SenkouSpanB);
            Assert.AreEqual(245.28m, r1.ChikouSpan);

            IchimokuResult r2 = results.Where(x => x.Index == 502).FirstOrDefault();
            Assert.AreEqual(241.26m, r2.TenkanSen);
            Assert.AreEqual(251.505m, r2.KijunSen);
            Assert.AreEqual(264.77m, r2.SenkouSpanA);
            Assert.AreEqual(269.82m, r2.SenkouSpanB);
            Assert.AreEqual(null, r2.ChikouSpan);
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Bad signal period.")]
        public void BadSignalPeriod()
        {
            Indicator.GetIchimoku(history, 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Bad short span period.")]
        public void BadShortSpanPeriod()
        {
            Indicator.GetIchimoku(history, 9, 0, 52);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Bad long span period.")]
        public void BadLongSpanPeriod()
        {
            Indicator.GetIchimoku(history, 9, 26, 26);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            Indicator.GetIchimoku(history.Where(x => x.Index < 52), 9, 26, 52);
        }
    }
}