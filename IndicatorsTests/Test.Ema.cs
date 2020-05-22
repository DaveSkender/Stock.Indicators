using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StockIndicators.Tests
{
    [TestClass]
    public class EmaTests : TestBase
    {

        [TestMethod()]
        public void GetEmaTest()
        {
            int lookbackPeriod = 20;
            IEnumerable<EmaResult> results = Indicator.GetEma(history, lookbackPeriod);

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count());
            Assert.AreEqual(502 - lookbackPeriod + 1, results.Where(x => x.Ema != null).Count());

            // sample value
            EmaResult ema = results.Where(x => x.Date == DateTime.Parse("12/31/2018")).FirstOrDefault();
            Assert.AreEqual((decimal)249.3519, Math.Round((decimal)ema.Ema, 4));
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            Indicator.GetEma(history.Where(x => x.Index < 30), 30);
        }

    }
}