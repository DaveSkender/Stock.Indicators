using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class VolatilityStop : TestBase
    {

        [TestMethod]
        public void Standard()
        {
            List<VolatilityStopResult> results =
                quotes.GetVolatilityStop(14, 3)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is quotes
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(488, results.Where(x => x.Sar != null).Count());

            // sample values
            VolatilityStopResult r1 = results[14];
            Assert.AreEqual(212.83m, Math.Round((decimal)r1.Sar, 4));
            Assert.AreEqual(true, r1.IsStop);

            VolatilityStopResult r2 = results[16];
            Assert.AreEqual(212.9924m, Math.Round((decimal)r2.Sar, 4));
            Assert.AreEqual(false, r2.IsStop);

            VolatilityStopResult r3 = results[501];
            Assert.AreEqual(229.7662m, Math.Round((decimal)r3.Sar, 4));
            Assert.AreEqual(false, r3.IsStop);
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<VolatilityStopResult> r = Indicator.GetVolatilityStop(badQuotes);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Removed()
        {
            List<VolatilityStopResult> results =
                quotes.GetVolatilityStop(14, 3)
                    .RemoveWarmupPeriods()
                    .ToList();

            // assertions
            Assert.AreEqual(488, results.Count);

            VolatilityStopResult last = results.LastOrDefault();
            Assert.AreEqual(229.7662m, Math.Round((decimal)last.Sar, 4));
            Assert.AreEqual(false, last.IsStop);
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad lookback period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                quotes.GetVolatilityStop(1));

            // bad multiplier
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetVolatilityStop(quotes, 20, 0));

            // insufficient quotes
            Assert.ThrowsException<BadQuotesException>(() =>
                Indicator.GetVolatilityStop(TestData.GetDefault(114), 15));
        }
    }
}
