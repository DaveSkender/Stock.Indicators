using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StockIndicators.Tests
{
    [TestClass]
    public class CmfTests : TestBase
    {

        [TestMethod()]
        public void GetCmfTest()
        {
            int lookbackPeriod = 20;
            IEnumerable<CmfResult> results = Indicator.GetCmf(history, lookbackPeriod);

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count());
            Assert.AreEqual(502 - lookbackPeriod + 1, results.Where(x => x.Cmf != null).Count());

            // sample value
            CmfResult r = results.Where(x => x.Index == 502).FirstOrDefault();
            Assert.AreEqual((decimal)0.8052, Math.Round(r.MoneyFlowMultiplier, 4));
            Assert.AreEqual((decimal)118396116.25, Math.Round(r.MoneyFlowVolume, 2));
            Assert.AreEqual((decimal)-20257893.60, Math.Round((decimal)r.Cmf, 2));
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(BadParameterException), "Bad lookback.")]
        public void BadLookback()
        {
            Indicator.GetCmf(history, 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            Indicator.GetCmf(history.Where(x => x.Index <= 20), 20);
        }
    }
}