using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class Klinger : TestBase
    {

        [TestMethod]
        public void Standard()
        {

            List<KvoResult> results =
                history.GetKvo(34, 55, 13)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(446, results.Where(x => x.Oscillator != null).Count());
            Assert.AreEqual(434, results.Where(x => x.Signal != null).Count());

            // sample values
            KvoResult r55 = results[55];
            Assert.IsNull(r55.Oscillator);
            Assert.IsNull(r55.Signal);

            KvoResult r56 = results[56];
            Assert.AreEqual(-2138454001m, Math.Round(r56.Oscillator.Value, 0));
            Assert.IsNull(r56.Signal);

            KvoResult r57 = results[57];
            Assert.AreEqual(-2265495450m, Math.Round(r57.Oscillator.Value, 0));
            Assert.IsNull(r57.Signal);

            KvoResult r68 = results[68];
            Assert.AreEqual(-1241548491m, Math.Round(r68.Oscillator.Value, 0));
            Assert.AreEqual(-1489659254m, Math.Round(r68.Signal.Value, 0));

            KvoResult r149 = results[149];
            Assert.AreEqual(-62800843m, Math.Round(r149.Oscillator.Value, 0));
            Assert.AreEqual(-18678832m, Math.Round(r149.Signal.Value, 0));

            KvoResult r249 = results[249];
            Assert.AreEqual(-51541005m, Math.Round(r249.Oscillator.Value, 0));
            Assert.AreEqual(135207969m, Math.Round(r249.Signal.Value, 0));

            KvoResult r501 = results[501];
            Assert.AreEqual(-539224047m, Math.Round(r501.Oscillator.Value, 0));
            Assert.AreEqual(-1548306127m, Math.Round(r501.Signal.Value, 0));
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<KvoResult> r = Indicator.GetKvo(historyBad);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad fast period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetKvo(history, 2));

            // bad slow period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetKvo(history, 20, 20));

            // bad signal period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetKvo(history, 34, 55, 0));

            // insufficient history
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetKvo(HistoryTestData.Get(154), 33, 55));
        }
    }
}
