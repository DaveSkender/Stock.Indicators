using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class StdDev : TestBase
    {

        [TestMethod]
        public void Standard()
        {
            int lookbackPeriod = 10;
            List<StdDevResult> results = history.GetStdDev(lookbackPeriod).ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(493, results.Where(x => x.StdDev != null).Count());
            Assert.AreEqual(493, results.Where(x => x.ZScore != null).Count());
            Assert.AreEqual(false, results.Any(x => x.StdDevSma != null));

            // sample values
            StdDevResult r1 = results[8];
            Assert.AreEqual(null, r1.StdDev);
            Assert.AreEqual(null, r1.Mean);
            Assert.AreEqual(null, r1.ZScore);
            Assert.AreEqual(null, r1.StdDevSma);

            StdDevResult r2 = results[9];
            Assert.AreEqual(0.5020m, Math.Round((decimal)r2.StdDev, 4));
            Assert.AreEqual(214.0140m, Math.Round((decimal)r2.Mean, 4));
            Assert.AreEqual(-0.525917m, Math.Round((decimal)r2.ZScore, 6));
            Assert.AreEqual(null, r2.StdDevSma);

            StdDevResult r3 = results[249];
            Assert.AreEqual(0.9827m, Math.Round((decimal)r3.StdDev, 4));
            Assert.AreEqual(257.2200m, Math.Round((decimal)r3.Mean, 4));
            Assert.AreEqual(0.783563m, Math.Round((decimal)r3.ZScore, 6));
            Assert.AreEqual(null, r3.StdDevSma);

            StdDevResult r4 = results[501];
            Assert.AreEqual(5.4738m, Math.Round((decimal)r4.StdDev, 4));
            Assert.AreEqual(242.4100m, Math.Round((decimal)r4.Mean, 4));
            Assert.AreEqual(0.524312m, Math.Round((decimal)r4.ZScore, 6));
            Assert.AreEqual(null, r4.StdDevSma);
        }

        [TestMethod]
        public void GetStdDevWithSma()
        {
            int lookbackPeriod = 10;
            int smaPeriod = 5;
            List<StdDevResult> results = Indicator.GetStdDev(history, lookbackPeriod, smaPeriod).ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(493, results.Where(x => x.StdDev != null).Count());
            Assert.AreEqual(493, results.Where(x => x.ZScore != null).Count());
            Assert.AreEqual(489, results.Where(x => x.StdDevSma != null).Count());

            // sample values
            StdDevResult r1 = results[19];
            Assert.AreEqual(1.1642m, Math.Round((decimal)r1.StdDev, 4));
            Assert.AreEqual(-0.065282m, Math.Round((decimal)r1.ZScore, 6));
            Assert.AreEqual(1.1422m, Math.Round((decimal)r1.StdDevSma, 4));

            StdDevResult r2 = results[501];
            Assert.AreEqual(5.4738m, Math.Round((decimal)r2.StdDev, 4));
            Assert.AreEqual(0.524312m, Math.Round((decimal)r2.ZScore, 6));
            Assert.AreEqual(7.6886m, Math.Round((decimal)r2.StdDevSma, 4));
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<StdDevResult> r = Indicator.GetStdDev(historyBad, 15, 3);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad lookback period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetStdDev(history, 1));

            // bad SMA period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetStdDev(history, 14, 0));

            // insufficient history
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetStdDev(HistoryTestData.Get(29), 30));
        }
    }
}
