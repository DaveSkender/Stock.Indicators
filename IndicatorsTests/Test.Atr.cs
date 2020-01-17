using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StockIndicators.Tests
{
    [TestClass]
    public class AtrTests : TestBase
    {

        [TestMethod()]
        public void GetAtrTest()
        {
            int lookbackPeriod = 14;
            IEnumerable<AtrResult> results = Indicator.GetAtr(history, lookbackPeriod);

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count());
            Assert.AreEqual(502 - lookbackPeriod + 1, results.Where(x => x.Atr != null).Count());

            // sample value
            AtrResult atr = results.Where(x => x.Date == DateTime.Parse("12/31/2018")).FirstOrDefault();
            Assert.AreEqual((decimal)2.67, Math.Round((decimal)atr.Tr, 4));
            Assert.AreEqual((decimal)6.1497, Math.Round((decimal)atr.Atr, 4));
        }
    }
}