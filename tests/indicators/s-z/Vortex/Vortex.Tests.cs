using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class Vortex : TestBase
    {

        [TestMethod]
        public void Standard()
        {
            List<VortexResult> results = quotes.GetVortex(14).ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is quotes
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(488, results.Where(x => x.Pvi != null).Count());

            // sample values
            VortexResult r1 = results[13];
            Assert.IsNull(r1.Pvi);
            Assert.IsNull(r1.Nvi);

            VortexResult r2 = results[14];
            Assert.AreEqual(1.0460, Math.Round((double)r2.Pvi, 4));
            Assert.AreEqual(0.8119, Math.Round((double)r2.Nvi, 4));

            VortexResult r3 = results[29];
            Assert.AreEqual(1.1300, Math.Round((double)r3.Pvi, 4));
            Assert.AreEqual(0.7393, Math.Round((double)r3.Nvi, 4));

            VortexResult r4 = results[249];
            Assert.AreEqual(1.1558, Math.Round((double)r4.Pvi, 4));
            Assert.AreEqual(0.6634, Math.Round((double)r4.Nvi, 4));

            VortexResult r5 = results[501];
            Assert.AreEqual(0.8712, Math.Round((double)r5.Pvi, 4));
            Assert.AreEqual(1.1163, Math.Round((double)r5.Nvi, 4));
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<VortexResult> r = Indicator.GetVortex(badQuotes, 20);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Removed()
        {
            List<VortexResult> results = quotes.GetVortex(14)
                .RemoveWarmupPeriods()
                .ToList();

            // assertions
            Assert.AreEqual(502 - 14, results.Count);

            VortexResult last = results.LastOrDefault();
            Assert.AreEqual(0.8712, Math.Round((double)last.Pvi, 4));
            Assert.AreEqual(1.1163, Math.Round((double)last.Nvi, 4));
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad lookback period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetVortex(quotes, 1));

            // insufficient quotes
            Assert.ThrowsException<BadQuotesException>(() =>
                Indicator.GetVortex(TestData.GetDefault(30), 30));
        }
    }
}
