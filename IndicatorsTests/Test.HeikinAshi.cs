using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StockIndicators.Tests
{
    [TestClass]
    public class HeikinAshiTests : TestBase
    {

        [TestMethod()]
        public void GetHeikinAshiTest()
        {

            IEnumerable<HeikinAshiResult> results = Indicator.GetHeikinAshi(history);

            // assertions

            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count());

            // sample value
            HeikinAshiResult r = results.Where(x => x.Index == 502).FirstOrDefault();
            Assert.AreEqual((decimal)241.3018, Math.Round(r.Open, 4));
            Assert.AreEqual((decimal)245.54, Math.Round(r.High, 4));
            Assert.AreEqual((decimal)241.3018, Math.Round(r.Low, 4));
            Assert.AreEqual((decimal)244.6525, Math.Round(r.Close, 4));
            Assert.AreEqual(true, r.IsBullish);
            Assert.AreEqual(0, Math.Round(r.Weakness, 4));
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            Indicator.GetHeikinAshi(history.Where(x => x.Index < 2));
        }

    }
}