using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Internal.Tests
{
    [TestClass]
    public class AdlTests : TestBase
    {

        [TestMethod()]
        public void GetAdlTest()
        {

            IEnumerable<AdlResult> results = Indicator.GetAdl(history);

            // assertions

            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count());
            Assert.AreEqual(502, results.Where(x => x.Sma == null).Count());

            // sample values
            AdlResult r1 = results.Where(x => x.Index == 502).FirstOrDefault();
            Assert.AreEqual(0.8052m, Math.Round(r1.MoneyFlowMultiplier, 4));
            Assert.AreEqual(118396116.25m, Math.Round(r1.MoneyFlowVolume, 2));
            Assert.AreEqual(3439986548.42m, Math.Round(r1.Adl, 2));
            Assert.AreEqual(null, r1.Sma);

            AdlResult r2 = results.Where(x => x.Index == 250).FirstOrDefault();
            Assert.AreEqual(0.7778m, Math.Round(r2.MoneyFlowMultiplier, 4));
            Assert.AreEqual(36433792.89m, Math.Round(r2.MoneyFlowVolume, 2));
            Assert.AreEqual(3266400865.74m, Math.Round(r2.Adl, 2));
            Assert.AreEqual(null, r2.Sma);
        }

        [TestMethod()]
        public void GetAdlWithSmaTest()
        {

            IEnumerable<AdlResult> results = Indicator.GetAdl(history, 20);
            Assert.AreEqual(483, results.Where(x => x.Sma != null).Count());

            // assertions

            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count());

            // sample value
            AdlResult r = results.Where(x => x.Index == 502).FirstOrDefault();
            Assert.AreEqual(0.8052m, Math.Round(r.MoneyFlowMultiplier, 4));
            Assert.AreEqual(118396116.25m, Math.Round(r.MoneyFlowVolume, 2));
            Assert.AreEqual(3439986548.42m, Math.Round(r.Adl, 2));
            Assert.AreEqual(3595352721.16m, Math.Round((decimal)r.Sma, 2));
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Bad SMA period.")]
        public void BadSmaPeriod()
        {
            Indicator.GetAdl(history, 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            Indicator.GetAdl(history.Where(x => x.Index < 2));
        }

    }
}