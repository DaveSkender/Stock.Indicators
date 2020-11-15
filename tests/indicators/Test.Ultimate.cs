using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Internal.Tests
{
    [TestClass]
    public class UltimateTests : TestBase
    {

        [TestMethod()]
        public void GetUltimateTest()
        {
            IEnumerable<UltimateResult> results = Indicator.GetUltimate(history, 7, 14, 28);

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count());
            Assert.AreEqual(474, results.Where(x => x.Ultimate != null).Count());

            // sample values
            UltimateResult r1 = results.Where(x => x.Index == 502).FirstOrDefault();
            Assert.AreEqual(49.5257m, Math.Round((decimal)r1.Ultimate, 4));

            UltimateResult r2 = results.Where(x => x.Index == 250).FirstOrDefault();
            Assert.AreEqual(45.3121m, Math.Round((decimal)r2.Ultimate, 4));

            UltimateResult r3 = results.Where(x => x.Index == 75).FirstOrDefault();
            Assert.AreEqual(51.7770m, Math.Round((decimal)r3.Ultimate, 4));

        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Bad short period.")]
        public void BadShortPeriod()
        {
            Indicator.GetUltimate(history, 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Bad middle period.")]
        public void BadMiddlePeriod()
        {
            Indicator.GetUltimate(history, 7, 6);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Bad long period.")]
        public void BadLongPeriod()
        {
            Indicator.GetUltimate(history, 7, 14, 11);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            Indicator.GetUltimate(history.Where(x => x.Index <= 28), 7, 14, 28);
        }
    }
}