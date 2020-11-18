using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Internal.Tests
{
    [TestClass]
    public class VolSmaTests : TestBase
    {

        [TestMethod()]
        public void GetVolSma()
        {
            int lookbackPeriod = 20;
            List<VolSmaResult> results = Indicator.GetVolSma(history, lookbackPeriod).ToList();

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

        [TestMethod()]
        public void GetVolSmaBadData()
        {
            IEnumerable<VolSmaResult> r = Indicator.GetVolSma(historyBad, 15);
            Assert.AreEqual(502, r.Count());
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Bad lookback.")]
        public void BadLookbackPeriod()
        {
            Indicator.GetVolSma(history, 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            IEnumerable<Quote> h = History.GetHistory(9);
            Indicator.GetVolSma(h, 10);
        }
    }
}