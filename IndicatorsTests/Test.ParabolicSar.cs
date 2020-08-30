using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StockIndicators.Tests
{
    [TestClass]
    public class ParabolicSarTests : TestBase
    {

        [TestMethod()]
        public void GetParabolicSarTest()
        {
            decimal acclerationStep = (decimal)0.02;
            decimal maxAccelerationFactor = (decimal)0.2;

            IEnumerable<ParabolicSarResult> results = Indicator.GetParabolicSar(history, acclerationStep, maxAccelerationFactor);

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count());
            Assert.AreEqual(501, results.Where(x => x.Sar != null).Count());

            // sample value
            ParabolicSarResult r = results.Where(x => x.Index == 502).FirstOrDefault();
            Assert.AreEqual(229.7662m, Math.Round((decimal)r.Sar, 4));
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(BadParameterException), "Acc Step must be greater than 0.")]
        public void BadAccelerationStep()
        {
            Indicator.GetParabolicSar(history, 0, 1);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadParameterException), "Max Acc Factor must be greater than 0.")]
        public void BadMaxAcclerationFactor()
        {
            Indicator.GetParabolicSar(history, 0.02m, 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadParameterException), "Step larger than Factor.")]
        public void BadParameterCombo()
        {
            Indicator.GetParabolicSar(history, 6, 2);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            Indicator.GetParabolicSar(history.Where(x => x.Index < 2), (decimal)0.02, (decimal)0.2);
        }

    }
}