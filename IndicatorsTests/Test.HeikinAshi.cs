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
            HeikinAshiResult result = results.Where(x => x.Date == DateTime.Parse("12/31/2018")).FirstOrDefault();
            Assert.AreEqual((decimal)241.3018, Math.Round(result.Open, 4));
            Assert.AreEqual((decimal)245.54, Math.Round(result.High, 4));
            Assert.AreEqual((decimal)241.3018, Math.Round(result.Low, 4));
            Assert.AreEqual((decimal)244.6525, Math.Round(result.Close, 4));
            Assert.AreEqual(true, result.IsBullish);
            Assert.AreEqual(0, Math.Round(result.Weakness, 4));
        }
    }
}