using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StockIndicators.Tests
{
    [TestClass]
    public class ChandeleirTests : TestBase
    {

        [TestMethod()]
        public void GetChandleierTest()
        {
            int lookbackPeriod = 22;
            IEnumerable<ChandelierResult> longResult = Indicator.GetChandelier(history, lookbackPeriod, 3.0m);

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, longResult.Count());
            Assert.AreEqual(502 - lookbackPeriod + 1, longResult.Where(x => x.ChandelierExit != null).Count());

            // sample values (long)
            ChandelierResult a = longResult.Where(x => x.Index == 502).FirstOrDefault();
            Assert.AreEqual(256.5860m, Math.Round((decimal)a.ChandelierExit, 4));

            ChandelierResult b = longResult.Where(x => x.Index == 493).FirstOrDefault();
            Assert.AreEqual(259.0480m, Math.Round((decimal)b.ChandelierExit, 4));

            // short
            IEnumerable<ChandelierResult> shortResult = Indicator.GetChandelier(history, lookbackPeriod, 3.0m, ChandelierType.Short);

            ChandelierResult c = shortResult.Where(x => x.Index == 502).FirstOrDefault();
            Assert.AreEqual(246.4240m, Math.Round((decimal)c.ChandelierExit, 4));
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(BadParameterException), "Bad lookback period.")]
        public void BadLookback()
        {
            Indicator.GetChandelier(history, 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadParameterException), "Bad multiplier.")]
        public void BadMultiplier()
        {
            Indicator.GetChandelier(history, 25, 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            Indicator.GetChandelier(history.Where(x => x.Index <= 30), 30);
        }

    }
}