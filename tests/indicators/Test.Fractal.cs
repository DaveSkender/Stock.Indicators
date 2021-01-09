using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System.Collections.Generic;
using System.Linq;

namespace Internal.Tests
{
    [TestClass]
    public class Fractal : TestBase
    {

        [TestMethod()]
        public void Standard()
        {

            List<FractalResult> results = Indicator.GetFractal(history)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(63, results.Where(x => x.FractalBear != null).Count());
            Assert.AreEqual(71, results.Where(x => x.FractalBull != null).Count());

            // sample values
            FractalResult r1 = results[1];
            Assert.AreEqual(null, r1.FractalBear);
            Assert.AreEqual(null, r1.FractalBull);

            FractalResult r2 = results[3];
            Assert.AreEqual(215.17m, r2.FractalBear);
            Assert.AreEqual(null, r2.FractalBull);

            FractalResult r3 = results[133];
            Assert.AreEqual(234.53m, r3.FractalBear);
            Assert.AreEqual(null, r3.FractalBull);

            FractalResult r4 = results[180];
            Assert.AreEqual(239.74m, r4.FractalBear);
            Assert.AreEqual(238.52m, r4.FractalBull);

            FractalResult r5 = results[250];
            Assert.AreEqual(null, r5.FractalBear);
            Assert.AreEqual(256.81m, r5.FractalBull);

            FractalResult r6 = results[500];
            Assert.AreEqual(null, r6.FractalBear);
            Assert.AreEqual(null, r6.FractalBull);
        }

        [TestMethod()]
        public void BadData()
        {
            IEnumerable<FractalResult> r = Indicator.GetFractal(historyBad);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod()]
        public void Exceptions()
        {
            // insufficient history
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetFractal(HistoryTestData.Get(4)));
        }

    }
}