using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class HeikinAshi : TestBase
    {

        [TestMethod]
        public void Standard()
        {

            List<HeikinAshiResult> results = history.GetHeikinAshi().ToList();

            // assertions

            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);

            // sample value
            HeikinAshiResult r = results[501];
            Assert.AreEqual(241.3018m, Math.Round(r.Open, 4));
            Assert.AreEqual(245.54m, Math.Round(r.High, 4));
            Assert.AreEqual(241.3018m, Math.Round(r.Low, 4));
            Assert.AreEqual(244.6525m, Math.Round(r.Close, 4));
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<HeikinAshiResult> r = Indicator.GetHeikinAshi(historyBad);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Exceptions()
        {
            // insufficient history
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetHeikinAshi(HistoryTestData.Get(1)));
        }
    }
}
