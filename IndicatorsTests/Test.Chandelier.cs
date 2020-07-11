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
            ChandelierResult a = longResult.Where(x => x.Date == DateTime.Parse("12/31/2018")).FirstOrDefault();
            Assert.AreEqual((decimal)256.5860, Math.Round((decimal)a.ChandelierExit, 4));
            Assert.AreEqual(false, a.IsExitCross);
            Assert.AreEqual(true, a.IsCrossed);

            ChandelierResult b = longResult.Where(x => x.Date == DateTime.Parse("12/6/2018")).FirstOrDefault();
            Assert.AreEqual(false, b.IsExitCross);
            Assert.AreEqual(false, b.IsCrossed);

            ChandelierResult c = longResult.Where(x => x.Date == DateTime.Parse("12/7/2018")).FirstOrDefault();
            Assert.AreEqual(true, c.IsExitCross);
            Assert.AreEqual(true, c.IsCrossed);

            ChandelierResult d = longResult.Where(x => x.Date == DateTime.Parse("12/17/2018")).FirstOrDefault();
            Assert.AreEqual((decimal)259.0480, Math.Round((decimal)d.ChandelierExit, 4));
            Assert.AreEqual(false, d.IsExitCross);
            Assert.AreEqual(true, d.IsCrossed);

            // short
            IEnumerable<ChandelierResult> shortResult = Indicator.GetChandelier(history, lookbackPeriod, 3.0m, "short");

            ChandelierResult e = shortResult.Where(x => x.Date == DateTime.Parse("12/31/2018")).FirstOrDefault();
            Assert.AreEqual((decimal)246.4240, Math.Round((decimal)e.ChandelierExit, 4));
            Assert.AreEqual(false, e.IsExitCross);
            Assert.AreEqual(false, e.IsCrossed);
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
        [ExpectedException(typeof(BadParameterException), "Bad variant.")]
        public void BadVariant()
        {
            Indicator.GetChandelier(history, 25, 3, "wrongvariant");
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            Indicator.GetChandelier(history.Where(x => x.Index <= 30), 30);
        }

    }
}