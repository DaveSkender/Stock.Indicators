using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System.Collections.Generic;
using System.Linq;

namespace Internal.Tests
{
    [TestClass]
    public class AroonTests : TestBase
    {

        [TestMethod()]
        public void GetAroonTest()
        {
            int lookbackPeriod = 25;
            IEnumerable<AroonResult> results = Indicator.GetAroon(history, lookbackPeriod);

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count());
            Assert.AreEqual(477, results.Where(x => x.AroonUp != null).Count());
            Assert.AreEqual(477, results.Where(x => x.AroonDown != null).Count());
            Assert.AreEqual(477, results.Where(x => x.Oscillator != null).Count());

            // sample values
            AroonResult r1 = results.Where(x => x.Index == 502).FirstOrDefault();
            Assert.AreEqual(28m, r1.AroonUp);
            Assert.AreEqual(88m, r1.AroonDown);
            Assert.AreEqual(-60m, r1.Oscillator);

            AroonResult r2 = results.Where(x => x.Index == 459).FirstOrDefault();
            Assert.AreEqual(0m, r2.AroonUp);
            Assert.AreEqual(100m, r2.AroonDown);
            Assert.AreEqual(-100m, r2.Oscillator);

            AroonResult r3 = results.Where(x => x.Index == 299).FirstOrDefault();
            Assert.AreEqual(0m, r3.AroonUp);
            Assert.AreEqual(20m, r3.AroonDown);
            Assert.AreEqual(-20m, r3.Oscillator);

            AroonResult r4 = results.Where(x => x.Index == 294).FirstOrDefault();
            Assert.AreEqual(0m, r4.AroonUp);
            Assert.AreEqual(40m, r4.AroonDown);
            Assert.AreEqual(-40m, r4.Oscillator);

            AroonResult r5 = results.Where(x => x.Index == 211).FirstOrDefault();
            Assert.AreEqual(100m, r5.AroonUp);
            Assert.AreEqual(0m, r5.AroonDown);
            Assert.AreEqual(100m, r5.Oscillator);
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(BadParameterException), "Bad lookback period.")]
        public void BadLookback()
        {
            Indicator.GetAroon(history, 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            Indicator.GetAroon(history.Where(x => x.Index < 30), 30);
        }

    }
}