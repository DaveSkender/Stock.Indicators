using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class Beta : TestBase
    {

        [TestMethod]
        public void All()
        {

            List<BetaResult> results = Indicator
                .GetBeta(quotes, otherQuotes, 20, BetaType.All)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is quotes
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(483, results.Where(x => x.Beta != null).Count());

            // sample value
            BetaResult r = results[501];
            Assert.AreEqual(1.6759m, Math.Round((decimal)r.Beta, 4));
        }

        [TestMethod]
        public void Standard()
        {

            List<BetaResult> results = Indicator
                .GetBeta(quotes, otherQuotes, 20, BetaType.Standard)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is quotes
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(483, results.Where(x => x.Beta != null).Count());

            // sample value
            BetaResult r = results[501];
            Assert.AreEqual(1.6759m, Math.Round((decimal)r.Beta, 4));
        }

        [TestMethod]
        public void Up()
        {

            List<BetaResult> results = Indicator
                .GetBeta(quotes, otherQuotes, 20, BetaType.Up)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is quotes
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(482, results.Where(x => x.BetaUp != null).Count());

            // sample value
            BetaResult r = results[501];
            Assert.AreEqual(1.2838m, Math.Round((decimal)r.BetaUp, 4));
        }

        [TestMethod]
        public void Down()
        {

            List<BetaResult> results = Indicator
                .GetBeta(quotes, otherQuotes, 20, BetaType.Down)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is quotes
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(482, results.Where(x => x.BetaDown != null).Count());

            // sample value
            BetaResult r = results[501];
            Assert.AreEqual(2.1034m, Math.Round((decimal)r.BetaDown, 4));
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<BetaResult> r1 = Indicator
                .GetBeta(badQuotes, badQuotes, 15, BetaType.Standard);
            Assert.AreEqual(502, r1.Count());

            IEnumerable<BetaResult> r2 = Indicator
                .GetBeta(badQuotes, badQuotes, 15, BetaType.Up);
            Assert.AreEqual(502, r2.Count());

            IEnumerable<BetaResult> r3 = Indicator
                .GetBeta(badQuotes, badQuotes, 15, BetaType.Down);
            Assert.AreEqual(502, r3.Count());
        }

        [TestMethod]
        public void Removed()
        {
            List<BetaResult> results = Indicator.GetBeta(quotes, otherQuotes, 20)
                .RemoveWarmupPeriods()
                .ToList();

            // assertions
            Assert.AreEqual(502 - 19, results.Count);

            BetaResult last = results.LastOrDefault();
            Assert.AreEqual(1.6759m, Math.Round((decimal)last.Beta, 4));
        }

        [TestMethod]
        public void SameSame()
        {
            // Beta should be 1 if evaluating against self
            List<BetaResult> results = Indicator.GetBeta(quotes, quotes, 20)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is quotes
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(483, results.Where(x => x.Beta != null).Count());

            // sample value
            BetaResult r = results[501];
            Assert.AreEqual(1, Math.Round((decimal)r.Beta, 4));
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad lookback period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetBeta(quotes, otherQuotes, 0));

            // insufficient quotes
            IEnumerable<Quote> h1 = TestData.GetDefault(29);
            IEnumerable<Quote> h2 = TestData.GetCompare(29);
            Assert.ThrowsException<BadQuotesException>(() =>
                Indicator.GetBeta(h1, h2, 30));

            // bad evaluation quotes
            IEnumerable<Quote> eval = TestData.GetCompare(300);
            Assert.ThrowsException<BadQuotesException>(() =>
                Indicator.GetBeta(quotes, eval, 30));
        }
    }
}
