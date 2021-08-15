using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class Pivots : TestBase
    {

        [TestMethod]
        public void Standard()
        {

            List<PivotsResult> results = quotes.GetPivots(4, 4, 20, EndType.HighLow)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is quotes
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(35, results.Where(x => x.HighPoint != null).Count());
            Assert.AreEqual(34, results.Where(x => x.LowPoint != null).Count());

            // sample values
            PivotsResult r1 = results[3];
            Assert.AreEqual(null, r1.HighPoint);
            Assert.AreEqual(null, r1.LowPoint);

            PivotsResult r2 = results[7];
            Assert.AreEqual(null, r2.HighPoint);
            Assert.AreEqual(212.53m, r2.LowPoint);

            PivotsResult r3 = results[120];
            Assert.AreEqual(233.02m, r3.HighPoint);
            Assert.AreEqual(null, r3.LowPoint);

            PivotsResult r4 = results[180];
            Assert.AreEqual(239.74m, r4.HighPoint);
            Assert.AreEqual(null, r4.LowPoint);

            PivotsResult r5 = results[250];
            Assert.AreEqual(null, r5.HighPoint);
            Assert.AreEqual(256.81m, r5.LowPoint);

            PivotsResult r6 = results[500];
            Assert.AreEqual(null, r6.HighPoint);
            Assert.AreEqual(null, r6.LowPoint);
        }


        [TestMethod]
        public void BadData()
        {
            IEnumerable<PivotsResult> r = Indicator.GetPivots(badQuotes);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad left span
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetPivots(quotes, 1));

            // bad right span
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetPivots(quotes, 2, 1));

            // bad lookback window
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetPivots(quotes, 20, 10, 20, EndType.Close));

            // insufficient quotes
            Assert.ThrowsException<BadQuotesException>(() =>
                Indicator.GetPivots(TestData.GetDefault(10), 5, 5));
        }
    }
}
