using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StockIndicators.Tests
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

            // sample value
            AdlResult r = results.Where(x => x.Date == DateTime.Parse("12/31/2018")).FirstOrDefault();
            Assert.AreEqual((decimal)0.8052, Math.Round(r.MoneyFlowMultiplier, 4));
            Assert.AreEqual((decimal)118396116.25, Math.Round(r.MoneyFlowVolume, 2));
            Assert.AreEqual((decimal)3439986548.42, Math.Round(r.Adl, 2));
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            Indicator.GetAdl(history.Where(x => x.Index < 2));
        }

    }
}