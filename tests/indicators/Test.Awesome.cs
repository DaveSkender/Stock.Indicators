using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class Awesome : TestBase
    {

        [TestMethod]
        public void Standard()
        {

            List<AwesomeResult> results = Indicator.GetAwesome(history, 5, 34)
                .ToList();

            // assertions

            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(469, results.Where(x => x.Oscillator != null).Count());

            // sample values
            AwesomeResult r1 = results[32];
            Assert.AreEqual(null, r1.Oscillator);
            Assert.AreEqual(null, r1.Normalized);

            AwesomeResult r2 = results[33];
            Assert.AreEqual(5.4756m, Math.Round((decimal)r2.Oscillator, 4));
            Assert.AreEqual(2.4548m, Math.Round((decimal)r2.Normalized, 4));

            AwesomeResult r3 = results[249];
            Assert.AreEqual(5.0618m, Math.Round((decimal)r3.Oscillator, 4));
            Assert.AreEqual(1.9634m, Math.Round((decimal)r3.Normalized, 4));

            AwesomeResult r4 = results[501];
            Assert.AreEqual(-17.7692m, Math.Round((decimal)r4.Oscillator, 4));
            Assert.AreEqual(-7.2763m, Math.Round((decimal)r4.Normalized, 4));
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<AwesomeResult> r = Indicator.GetAwesome(historyBad);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad fast period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetAwesome(history, 0, 34));

            // bad slow period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetAwesome(history, 25, 25));

            // insufficient history
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetAwesome(HistoryTestData.Get(33), 5, 34));
        }
    }
}