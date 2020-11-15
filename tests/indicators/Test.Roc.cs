using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Internal.Tests
{
    [TestClass]
    public class RocTests : TestBase
    {

        [TestMethod()]
        public void GetRocTest()
        {
            int lookbackPeriod = 20;
            IEnumerable<RocResult> results = Indicator.GetRoc(history, lookbackPeriod);

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count());
            Assert.AreEqual(482, results.Where(x => x.Roc != null).Count());
            Assert.AreEqual(false, results.Any(x => x.Sma != null));

            // sample values
            RocResult r1 = results.Where(x => x.Index == 502).FirstOrDefault();
            Assert.AreEqual(-8.2482m, Math.Round((decimal)r1.Roc, 4));
            Assert.AreEqual(null, r1.Sma);

            RocResult r2 = results.Where(x => x.Index == 250).FirstOrDefault();
            Assert.AreEqual(2.4827m, Math.Round((decimal)r2.Roc, 4));
            Assert.AreEqual(null, r2.Sma);
        }

        [TestMethod()]
        public void GetRocWithSmaTest()
        {
            int lookbackPeriod = 20;
            int smaPeriod = 5;
            IEnumerable<RocResult> results = Indicator.GetRoc(history, lookbackPeriod, smaPeriod);

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count());
            Assert.AreEqual(502 - lookbackPeriod, results.Where(x => x.Roc != null).Count());
            Assert.AreEqual(478, results.Where(x => x.Sma != null).Count());

            // sample values
            RocResult r1 = results.Where(x => x.Index == 502).FirstOrDefault();
            Assert.AreEqual(-8.2482m, Math.Round((decimal)r1.Roc, 4));
            Assert.AreEqual(-8.4828m, Math.Round((decimal)r1.Sma, 4));

            RocResult r2 = results.Where(x => x.Index == 30).FirstOrDefault();
            Assert.AreEqual(3.2936m, Math.Round((decimal)r2.Roc, 4));
            Assert.AreEqual(2.1558m, Math.Round((decimal)r2.Sma, 4));
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Bad lookback.")]
        public void BadLookbackPeriod()
        {
            Indicator.GetRoc(history, 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Bad SMA period.")]
        public void BadSmaPeriod()
        {
            Indicator.GetRoc(history, 14, 0);
        }


        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            Indicator.GetRoc(history.Where(x => x.Index <= 10), 10);
        }
    }
}