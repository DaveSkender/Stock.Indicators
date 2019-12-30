using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StockIndicators.Tests
{
    [TestClass]
    public class SmaTests : TestBase
    {

        [TestMethod()]
        public void GetSmaTest()
        {
            int lookbackPeriod = 20;
            IEnumerable<SmaResult> results = Indicator.GetSma(history, lookbackPeriod);

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count());
            Assert.AreEqual(502 - lookbackPeriod + 1, results.Where(x => x.Sma != null).Count());

            // sample value
            SmaResult sma = results.Where(x => x.Date == DateTime.Parse("12/31/2018")).FirstOrDefault();
            Assert.AreEqual((decimal)251.86, sma.Sma);
        }
    }
}