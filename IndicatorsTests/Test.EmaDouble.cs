using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StockIndicators.Tests
{
    [TestClass]
    public class DoubleEmaTests : TestBase
    {

        [TestMethod()]
        public void GetDoubleEmaTest()
        {
            int lookbackPeriod = 20;
            IEnumerable<EmaResult> results = Indicator.GetDoubleEma(history, lookbackPeriod);

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count());
            Assert.AreEqual(502 - 2 * lookbackPeriod + 1, results.Where(x => x.Ema != null).Count());

            // sample value
            EmaResult r = results.Where(x => x.Date == DateTime.Parse("12/31/2018")).FirstOrDefault();
            Assert.AreEqual((decimal)241.1677, Math.Round((decimal)r.Ema, 4));
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(BadParameterException), "Bad lookback.")]
        public void BadLookback()
        {
            Indicator.GetDoubleEma(history, 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history for 2*N+100.")]
        public void InsufficientHistoryA()
        {
            Indicator.GetDoubleEma(history.Where(x => x.Index < 160), 30);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history for 3×N.")]
        public void InsufficientHistoryB()
        {
            Indicator.GetDoubleEma(history.Where(x => x.Index < 750), 250);
        }

    }
}