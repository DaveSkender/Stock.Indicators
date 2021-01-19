using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class Roc : TestBase
    {

        [TestMethod]
        public void Standard()
        {
            int lookbackPeriod = 20;

            List<RocResult> results = Indicator.GetRoc(history, lookbackPeriod)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(482, results.Where(x => x.Roc != null).Count());
            Assert.AreEqual(false, results.Any(x => x.RocSma != null));

            // sample values
            RocResult r1 = results[249];
            Assert.AreEqual(2.4827m, Math.Round((decimal)r1.Roc, 4));
            Assert.AreEqual(null, r1.RocSma);

            RocResult r2 = results[501];
            Assert.AreEqual(-8.2482m, Math.Round((decimal)r2.Roc, 4));
            Assert.AreEqual(null, r2.RocSma);
        }

        [TestMethod]
        public void WithSma()
        {
            int lookbackPeriod = 20;
            int smaPeriod = 5;

            List<RocResult> results = Indicator.GetRoc(history, lookbackPeriod, smaPeriod)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(482, results.Where(x => x.Roc != null).Count());
            Assert.AreEqual(478, results.Where(x => x.RocSma != null).Count());

            // sample values
            RocResult r1 = results[29];
            Assert.AreEqual(3.2936m, Math.Round((decimal)r1.Roc, 4));
            Assert.AreEqual(2.1558m, Math.Round((decimal)r1.RocSma, 4));

            RocResult r2 = results[501];
            Assert.AreEqual(-8.2482m, Math.Round((decimal)r2.Roc, 4));
            Assert.AreEqual(-8.4828m, Math.Round((decimal)r2.RocSma, 4));
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<RocResult> r = Indicator.GetRoc(historyBad, 35, 2);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad lookback period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetRoc(history, 0));

            // bad SMA period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetRoc(history, 14, 0));

            // insufficient history
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetRoc(HistoryTestData.Get(10), 10));
        }
    }
}