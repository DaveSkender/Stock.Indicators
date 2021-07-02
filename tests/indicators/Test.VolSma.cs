using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class VolSma : TestBase
    {

        [TestMethod]
        public void Standard()
        {
            int lookbackPeriod = 20;
            List<VolSmaResult> results = history.GetVolSma(lookbackPeriod)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(483, results.Where(x => x.VolSma != null).Count());

            // sample values
            VolSmaResult r1 = results[24];
            Assert.AreEqual(77293768.2m, r1.VolSma);

            VolSmaResult r2 = results[290];
            Assert.AreEqual(157958070.8m, r2.VolSma);

            VolSmaResult r3 = results[501];
            Assert.AreEqual(DateTime.ParseExact("12/31/2018", "MM/dd/yyyy", englishCulture), r3.Date);
            Assert.AreEqual(147031456m, r3.Volume);
            Assert.AreEqual(163695200m, r3.VolSma);
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<VolSmaResult> r = Indicator.GetVolSma(historyBad, 15);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad lookback period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetVolSma(history, 0));

            // insufficient history
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetVolSma(HistoryTestData.Get(9), 10));
        }
    }
}
