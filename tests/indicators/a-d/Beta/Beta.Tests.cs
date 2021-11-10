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
            Assert.AreEqual(482, results.Where(x => x.BetaUp != null).Count());
            Assert.AreEqual(482, results.Where(x => x.BetaDown != null).Count());

            // sample values
            BetaResult r19 = results[19];
            Assert.AreEqual(7.5476m, Math.Round((decimal)r19.Beta, 4));
            Assert.IsNull(r19.BetaUp);
            Assert.IsNull(r19.BetaDown);
            Assert.IsNull(r19.Ratio);
            Assert.IsNull(r19.Convexity);

            BetaResult r20 = results[20];
            Assert.AreEqual(6.7488m, Math.Round((decimal)r20.Beta, 4));
            Assert.AreEqual(8.2407m, Math.Round((decimal)r20.BetaUp, 4));
            Assert.AreEqual(5.9296m, Math.Round((decimal)r20.BetaDown, 4));
            Assert.AreEqual(1.3898m, Math.Round((decimal)r20.Ratio, 4));
            Assert.AreEqual(5.3415m, Math.Round((decimal)r20.Convexity, 4));

            BetaResult r249 = results[249];
            Assert.AreEqual(3.5528m, Math.Round((decimal)r249.Beta, 4));
            Assert.AreEqual(3.1061m, Math.Round((decimal)r249.BetaUp, 4));
            Assert.AreEqual(4.0684m, Math.Round((decimal)r249.BetaDown, 4));
            Assert.AreEqual(0.7635m, Math.Round((decimal)r249.Ratio, 4));
            Assert.AreEqual(0.9260m, Math.Round((decimal)r249.Convexity, 4));

            BetaResult r501 = results[501];
            Assert.AreEqual(1.6759m, Math.Round((decimal)r501.Beta, 4));
            Assert.AreEqual(1.2838m, Math.Round((decimal)r501.BetaUp, 4));
            Assert.AreEqual(2.1034m, Math.Round((decimal)r501.BetaDown, 4));
            Assert.AreEqual(0.6104m, Math.Round((decimal)r501.Ratio, 4));
            Assert.AreEqual(0.6717m, Math.Round((decimal)r501.Convexity, 4));
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
