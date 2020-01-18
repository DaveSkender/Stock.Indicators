using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StockIndicators.Tests
{
    [TestClass]
    public class CciTests : TestBase
    {

        [TestMethod()]
        public void GetCciTest()
        {
            int lookbackPeriod = 20;

            IEnumerable<CciResult> results = Indicator.GetCci(history, lookbackPeriod);

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count());
            Assert.AreEqual(502 - lookbackPeriod + 1, results.Where(x => x.Cci != null).Count());

            // sample value
            CciResult result = results.Where(x => x.Date == DateTime.Parse("12/31/2018")).FirstOrDefault();
            Assert.AreEqual((decimal)-52.9946, Math.Round((decimal)result.Cci, 4));
        }
    }
}