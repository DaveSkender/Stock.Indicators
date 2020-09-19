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
        public void GetVolSmaTest()
        {
            int lookbackPeriod = 20;
            IEnumerable<VolSmaResult> results = Indicator.GetVolSma(history, lookbackPeriod);

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count());
            Assert.AreEqual(483, results.Where(x => x.VolSma != null).Count());

            // sample values
            VolSmaResult r1 = results.Where(x => x.Index == 25).FirstOrDefault();
            Assert.AreEqual(77293768.2m, r1.VolSma);

            VolSmaResult r2 = results.Where(x => x.Index == 291).FirstOrDefault();
            Assert.AreEqual(157958070.8m, r2.VolSma);

            VolSmaResult r3 = results.Where(x => x.Index == 502).FirstOrDefault();
            Assert.AreEqual(DateTime.ParseExact("12/31/2018", "MM/dd/yyyy", englishCulture), r3.Date);
            Assert.AreEqual(147031456m, r3.Volume);
            Assert.AreEqual(163695200m, r3.VolSma);
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(BadParameterException), "Bad lookback.")]
        public void BadLookback()
        {
            Indicator.GetVolSma(history, 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            Indicator.GetVolSma(history.Where(x => x.Index < 10), 10);
        }
    }
}