using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class Obv : TestBase
    {

        [TestMethod]
        public void Standard()
        {

            List<ObvResult> results = history.GetObv().ToList();

            // assertions

            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(502, results.Where(x => x.ObvSma == null).Count());

            // sample values
            ObvResult r1 = results[249];
            Assert.AreEqual(1780918888m, r1.Obv);
            Assert.AreEqual(null, r1.ObvSma);

            ObvResult r2 = results[501];
            Assert.AreEqual(539843504m, r2.Obv);
            Assert.AreEqual(null, r2.ObvSma);
        }

        [TestMethod]
        public void WithSma()
        {

            List<ObvResult> results = Indicator.GetObv(history, 20).ToList();

            // assertions

            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(482, results.Where(x => x.ObvSma != null).Count());

            // sample values
            ObvResult r1 = results[501];
            Assert.AreEqual(539843504, r1.Obv);
            Assert.AreEqual(1016208844.40m, r1.ObvSma);
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<ObvResult> r = Indicator.GetObv(historyBad);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad SMA period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetObv(history, 0));

            // insufficient history
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetObv(HistoryTestData.Get(1)));
        }
    }
}
