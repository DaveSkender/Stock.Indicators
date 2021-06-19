using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class Cci : TestBase
    {

        [TestMethod]
        public void Standard()
        {

            List<CciResult> results = history.GetCci(20).ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(483, results.Where(x => x.Cci != null).Count());

            // sample value
            CciResult r = results[501];
            Assert.AreEqual(-52.9946m, Math.Round((decimal)r.Cci, 4));
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<CciResult> r = Indicator.GetCci(historyBad, 15);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad lookback period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetCci(history, 0));

            // insufficient history
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetCci(HistoryTestData.Get(30), 30));
        }
    }
}
