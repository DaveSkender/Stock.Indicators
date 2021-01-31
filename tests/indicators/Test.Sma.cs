using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class Sma : TestBase
    {

        [TestMethod]
        public void Standard()
        {
            int lookbackPeriod = 20;

            List<SmaResult> results = Indicator.GetSma(history, lookbackPeriod)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(502 - lookbackPeriod + 1, results.Where(x => x.Sma != null).Count());

            // sample value
            SmaResult r = results[501];
            Assert.AreEqual(251.86m, r.Sma);
        }

        [TestMethod]
        public void Extended()
        {
            int lookbackPeriod = 20;

            List<SmaExtendedResult> results = Indicator.GetSmaExtended(history, lookbackPeriod)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(502 - lookbackPeriod + 1, results.Where(x => x.Sma != null).Count());

            // sample value
            SmaExtendedResult r = results[501];
            Assert.AreEqual(251.86m, r.Sma);
            Assert.AreEqual(9.45m, r.Mad);
            Assert.AreEqual(119.2510m, Math.Round((decimal)r.Mse, 4));
            Assert.AreEqual(0.037637m, Math.Round((decimal)r.Mape, 6));
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<SmaResult> r = Indicator.GetSmaExtended(historyBad, 15);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad lookback period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetSma(history, 0));

            // insufficient history
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetSma(HistoryTestData.Get(9), 10));
        }
    }
}
