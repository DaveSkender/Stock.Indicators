using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class Slope : TestBase
    {

        [TestMethod]
        public void Standard()
        {
            List<SlopeResult> results = quotes.GetSlope(20).ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is quotes
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(483, results.Where(x => x.Slope != null).Count());
            Assert.AreEqual(483, results.Where(x => x.StdDev != null).Count());
            Assert.AreEqual(20, results.Where(x => x.Line != null).Count());

            // sample values
            SlopeResult r1 = results[249];
            Assert.AreEqual(0.312406, Math.Round((double)r1.Slope, 6));
            Assert.AreEqual(180.4164, Math.Round((double)r1.Intercept, 4));
            Assert.AreEqual(0.8056, Math.Round((double)r1.RSquared, 4));
            Assert.AreEqual(2.0071, Math.Round((double)r1.StdDev, 4));
            Assert.AreEqual(null, r1.Line);

            SlopeResult r2 = results[482];
            Assert.AreEqual(-0.337015, Math.Round((double)r2.Slope, 6));
            Assert.AreEqual(425.1111, Math.Round((double)r2.Intercept, 4));
            Assert.AreEqual(0.1730, Math.Round((double)r2.RSquared, 4));
            Assert.AreEqual(4.6719, Math.Round((double)r2.StdDev, 4));
            Assert.AreEqual(267.9069m, Math.Round((decimal)r2.Line, 4));

            SlopeResult r3 = results[501];
            Assert.AreEqual(-1.689143, Math.Round((double)r3.Slope, 6));
            Assert.AreEqual(1083.7629, Math.Round((double)r3.Intercept, 4));
            Assert.AreEqual(0.7955, Math.Round((double)r3.RSquared, 4));
            Assert.AreEqual(10.9202, Math.Round((double)r3.StdDev, 4));
            Assert.AreEqual(235.8131m, Math.Round((decimal)r3.Line, 4));
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<SlopeResult> r = badQuotes.GetSlope(15);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void BigData()
        {
            IEnumerable<SlopeResult> r = bigQuotes.GetSlope(250);
            Assert.AreEqual(1246, r.Count());
        }

        [TestMethod]
        public void Removed()
        {
            List<SlopeResult> results = quotes.GetSlope(20)
                .RemoveWarmupPeriods()
                .ToList();

            // assertions
            Assert.AreEqual(502 - 19, results.Count);

            SlopeResult last = results.LastOrDefault();
            Assert.AreEqual(-1.689143, Math.Round((double)last.Slope, 6));
            Assert.AreEqual(1083.7629, Math.Round((double)last.Intercept, 4));
            Assert.AreEqual(0.7955, Math.Round((double)last.RSquared, 4));
            Assert.AreEqual(10.9202, Math.Round((double)last.StdDev, 4));
            Assert.AreEqual(235.8131m, Math.Round((decimal)last.Line, 4));
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad lookback period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetSlope(quotes, 0));

            // insufficient quotes
            Assert.ThrowsException<BadQuotesException>(() =>
                Indicator.GetSlope(TestData.GetDefault(29), 30));
        }
    }
}
