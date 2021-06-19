using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class Chandeleir : TestBase
    {

        [TestMethod]
        public void Standard()
        {
            int lookbackPeriod = 22;

            List<ChandelierResult> longResult =
                history.GetChandelier(lookbackPeriod, 3.0m)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, longResult.Count);
            Assert.AreEqual(481, longResult.Where(x => x.ChandelierExit != null).Count());

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

        [TestMethod]
        public void BadData()
        {
            IEnumerable<ChandelierResult> r = Indicator.GetChandelier(historyBad, 15, 2m);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad lookback period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetChandelier(history, 0));

            // bad multiplier
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetChandelier(history, 25, 0));

            // insufficient history
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetChandelier(HistoryTestData.Get(30), 30));
        }
    }
}
