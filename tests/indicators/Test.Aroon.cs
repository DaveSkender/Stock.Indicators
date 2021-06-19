using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class Aroon : TestBase
    {

        [TestMethod]
        public void Standard()
        {
            int lookbackPeriod = 25;
            List<AroonResult> results = history.GetAroon(lookbackPeriod).ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(477, results.Where(x => x.AroonUp != null).Count());
            Assert.AreEqual(477, results.Where(x => x.AroonDown != null).Count());
            Assert.AreEqual(477, results.Where(x => x.Oscillator != null).Count());

            // sample values
            AroonResult r1 = results[210];
            Assert.AreEqual(100m, r1.AroonUp);
            Assert.AreEqual(000m, r1.AroonDown);
            Assert.AreEqual(100m, r1.Oscillator);

            AroonResult r2 = results[293];
            Assert.AreEqual(0m, r2.AroonUp);
            Assert.AreEqual(40m, r2.AroonDown);
            Assert.AreEqual(-40m, r2.Oscillator);

            AroonResult r3 = results[298];
            Assert.AreEqual(0m, r3.AroonUp);
            Assert.AreEqual(20m, r3.AroonDown);
            Assert.AreEqual(-20m, r3.Oscillator);

            AroonResult r4 = results[458];
            Assert.AreEqual(0m, r4.AroonUp);
            Assert.AreEqual(100m, r4.AroonDown);
            Assert.AreEqual(-100m, r4.Oscillator);

            AroonResult r5 = results[501];
            Assert.AreEqual(28m, r5.AroonUp);
            Assert.AreEqual(88m, r5.AroonDown);
            Assert.AreEqual(-60m, r5.Oscillator);
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<AroonResult> r = Indicator.GetAroon(historyBad, 20);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad lookback period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetAroon(history, 0));

            // insufficient history
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetAroon(HistoryTestData.Get(29), 30));
        }
    }
}
