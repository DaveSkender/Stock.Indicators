using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Internal.Tests
{
    [TestClass]
    public class Chandeleir : TestBase
    {

        [TestMethod()]
        public void Standard()
        {
            int lookbackPeriod = 22;
            List<ChandelierResult> longResult =
                Indicator.GetChandelier(history, lookbackPeriod, 3.0m)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, longResult.Count);
            Assert.AreEqual(502 - lookbackPeriod + 1, longResult.Where(x => x.ChandelierExit != null).Count());

            // sample values (long)
            ChandelierResult a = longResult[501];
            Assert.AreEqual(256.5860m, Math.Round((decimal)a.ChandelierExit, 4));

            ChandelierResult b = longResult[492];
            Assert.AreEqual(259.0480m, Math.Round((decimal)b.ChandelierExit, 4));

            // short
            List<ChandelierResult> shortResult =
                Indicator.GetChandelier(history, lookbackPeriod, 3.0m, ChandelierType.Short)
                .ToList();

            ChandelierResult c = shortResult[501];
            Assert.AreEqual(246.4240m, Math.Round((decimal)c.ChandelierExit, 4));
        }

        [TestMethod()]
        public void BadData()
        {
            IEnumerable<ChandelierResult> r = Indicator.GetChandelier(historyBad, 15, 2m);
            Assert.AreEqual(502, r.Count());
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Bad lookback period.")]
        public void BadLookbackPeriod()
        {
            Indicator.GetChandelier(history, 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Bad multiplier.")]
        public void BadMultiplier()
        {
            Indicator.GetChandelier(history, 25, 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            IEnumerable<Quote> h = History.GetHistory(30);
            Indicator.GetChandelier(h, 30);
        }

    }
}