using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    [Obsolete("Use GetSma() instead.")]
    public class VolSma : TestBase
    {

        [TestMethod]
        public void Standard()
        {
            List<VolSmaResult> results = quotes.GetVolSma(20)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is quotes
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(483, results.Where(x => x.VolSma != null).Count());

            // sample values
            VolSmaResult r1 = results[24];
            Assert.AreEqual(77293768.2m, r1.VolSma);

            VolSmaResult r2 = results[290];
            Assert.AreEqual(157958070.8m, r2.VolSma);

            VolSmaResult r3 = results[501];
            Assert.AreEqual(DateTime.ParseExact("12/31/2018", "MM/dd/yyyy", EnglishCulture), r3.Date);
            Assert.AreEqual(147031456m, r3.Volume);
            Assert.AreEqual(163695200m, r3.VolSma);
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<VolSmaResult> r = Indicator.GetVolSma(badQuotes, 15);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Removed()
        {
            List<VolSmaResult> results = quotes.GetVolSma(20)
                .RemoveWarmupPeriods()
                .ToList();

            // assertions
            Assert.AreEqual(502 - 19, results.Count);

            VolSmaResult last = results.LastOrDefault();
            Assert.AreEqual(147031456m, last.Volume);
            Assert.AreEqual(163695200m, last.VolSma);
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad lookback period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetVolSma(quotes, 0));

            // insufficient quotes
            Assert.ThrowsException<BadQuotesException>(() =>
                Indicator.GetVolSma(TestData.GetDefault(9), 10));
        }
    }
}
