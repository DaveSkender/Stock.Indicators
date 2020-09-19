using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System.Collections.Generic;
using System.Linq;

namespace Internal.Tests
{
    [TestClass]
    public class ObvTests : TestBase
    {

        [TestMethod()]
        public void GetObvTest()
        {

            IEnumerable<ObvResult> results = Indicator.GetObv(history);

            // assertions

            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count());
            Assert.AreEqual(502, results.Where(x => x.Sma == null).Count());

            // sample values
            ObvResult r1 = results.Where(x => x.Index == 250).FirstOrDefault();
            Assert.AreEqual(1780918888m, r1.Obv);
            Assert.AreEqual(null, r1.Sma);

            ObvResult r2 = results.Where(x => x.Index == 502).FirstOrDefault();
            Assert.AreEqual(539843504, r2.Obv);
            Assert.AreEqual(null, r2.Sma);
        }

        [TestMethod()]
        public void GetObvWithSmaTest()
        {

            IEnumerable<ObvResult> results = Indicator.GetObv(history, 20);

            // assertions

            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count());
            Assert.AreEqual(482, results.Where(x => x.Sma != null).Count());

            // sample values
            ObvResult r1 = results.Where(x => x.Index == 502).FirstOrDefault();
            Assert.AreEqual(539843504, r1.Obv);
            Assert.AreEqual(1016208844.40m, r1.Sma);
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(BadParameterException), "Bad SMA period.")]
        public void BadSmaPeriod()
        {
            Indicator.GetObv(history, 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            Indicator.GetObv(history.Where(x => x.Index < 2));
        }

    }
}