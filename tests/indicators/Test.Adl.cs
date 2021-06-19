using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class Adl : TestBase
    {

        [TestMethod]
        public void Standard()
        {

            List<AdlResult> results = history.GetAdl().ToList();

            // assertions

            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(502, results.Where(x => x.AdlSma == null).Count());

            // sample values
            AdlResult r1 = results[249];
            Assert.AreEqual(0.7778m, Math.Round(r1.MoneyFlowMultiplier, 4));
            Assert.AreEqual(36433792.89m, Math.Round(r1.MoneyFlowVolume, 2));
            Assert.AreEqual(3266400865.74m, Math.Round(r1.Adl, 2));
            Assert.AreEqual(null, r1.AdlSma);

            AdlResult r2 = results[501];
            Assert.AreEqual(0.8052m, Math.Round(r2.MoneyFlowMultiplier, 4));
            Assert.AreEqual(118396116.25m, Math.Round(r2.MoneyFlowVolume, 2));
            Assert.AreEqual(3439986548.42m, Math.Round(r2.Adl, 2));
            Assert.AreEqual(null, r2.AdlSma);
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<AdlResult> r = Indicator.GetAdl(historyBad);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void WithSma()
        {

            List<AdlResult> results = Indicator.GetAdl(history, 20).ToList();

            // assertions

            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(483, results.Where(x => x.AdlSma != null).Count());

            // sample value
            AdlResult r = results[501];
            Assert.AreEqual(0.8052m, Math.Round(r.MoneyFlowMultiplier, 4));
            Assert.AreEqual(118396116.25m, Math.Round(r.MoneyFlowVolume, 2));
            Assert.AreEqual(3439986548.42m, Math.Round(r.Adl, 2));
            Assert.AreEqual(3595352721.16m, Math.Round((decimal)r.AdlSma, 2));
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad SMA period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetAdl(history, 0));

            // insufficient history
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetAdl(HistoryTestData.Get(1)));
        }
    }
}
