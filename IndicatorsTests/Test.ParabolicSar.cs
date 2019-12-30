using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StockIndicators.Tests
{
    [TestClass]
    public class ParabolicSarTests : TestBase
    {

        [TestMethod()]
        public void GetParabolicSarTest()
        {
            decimal acclerationStep = (decimal)0.02;
            decimal maxAccelerationFactor = (decimal)0.2;

            IEnumerable<ParabolicSarResult> results = Indicator.GetParabolicSar(history, acclerationStep, maxAccelerationFactor);

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count());
            Assert.AreEqual(501, results.Where(x => x.Sar != null).Count());

            // sample value
            ParabolicSarResult result = results.Where(x => x.Date == DateTime.Parse("12/31/2018")).FirstOrDefault();
            Assert.AreEqual((decimal)229.7662, Math.Round((decimal)result.Sar, 4));
            Assert.AreEqual(true, result.IsBullish);
            Assert.AreEqual(false, result.IsReversal);
        }
    }
}