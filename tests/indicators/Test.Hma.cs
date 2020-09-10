using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Internal.Tests
{
    [TestClass]
    public class HmaTests : TestBase
    {

        [TestMethod()]
        public void GetHmaTest()
        {
            int lookbackPeriod = 20;
            IEnumerable<HmaResult> results = Indicator.GetHma(history, lookbackPeriod);

            foreach (HmaResult r in results)
            {
                Console.WriteLine("{0},{1:d},{2:N4}", r.Index, r.Date, r.Hma);  // debugging only
            }

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count());
            Assert.AreEqual(480, results.Where(x => x.Hma != null).Count());

            // sample values
            HmaResult r1 = results.Where(x => x.Index == 150).FirstOrDefault();
            Assert.AreEqual(236.0835m, Math.Round((decimal)r1.Hma, 4));

            HmaResult r2 = results.Where(x => x.Index == 502).FirstOrDefault();
            Assert.AreEqual(235.6972m, Math.Round((decimal)r2.Hma, 4));
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(BadParameterException), "Bad lookback.")]
        public void BadLookback()
        {
            Indicator.GetHma(history, 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            Indicator.GetHma(history.Where(x => x.Index < 10), 10);
        }
    }
}