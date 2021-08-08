using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class Adl : TestBase
    {

        [TestMethod]
        public void Standard()
        {

            List<AdlResult> results = quotes.GetAdl().ToList();

            // assertions

            // should always be the same number of results as there is quotes
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(502, results.Where(x => x.AdlSma == null).Count());

            // sample values
            AdlResult r1 = results[249];
            Assert.AreEqual(0.7778m, Math.Round(r1.MoneyFlowMultiplier, 4));
            Assert.AreEqual(36433792.89m, Math.Round(r1.MoneyFlowVolume, 2));
            Assert.AreEqual(3266400865.74m, Math.Round(r1.Adl, 2));
            Assert.AreEqual(null, r1.AdlSma);

            AdlResult r2 = results[501];
            Assert.AreEqual(0.8052m, Math.Round(r2.MoneyFlowMultiplier, 4));
            Assert.AreEqual(118396116.25m, Math.Round(r2.MoneyFlowVolume, 2));
            Assert.AreEqual(3439986548.42m, Math.Round(r2.Adl, 2));
            Assert.AreEqual(null, r2.AdlSma);
        }

        [TestMethod]
        public void ConvertToQuotes()
        {
            List<Quote> newQuotes = quotes.GetAdl()
                .ConvertToQuotes()
                .ToList();

            Assert.AreEqual(502, newQuotes.Count);

            Quote q1 = newQuotes[249];
            Assert.AreEqual(3266400865.74m, Math.Round(q1.Close, 2));

            Quote q2 = newQuotes[501];
            Assert.AreEqual(3439986548.42m, Math.Round(q2.Close, 2));
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<AdlResult> r = Indicator.GetAdl(badQuotes);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void WithSma()
        {

            List<AdlResult> results = Indicator.GetAdl(quotes, 20).ToList();

            // assertions

            // should always be the same number of results as there is quotes
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(483, results.Where(x => x.AdlSma != null).Count());

            // sample value
            AdlResult r = results[501];
            Assert.AreEqual(0.8052m, Math.Round(r.MoneyFlowMultiplier, 4));
            Assert.AreEqual(118396116.25m, Math.Round(r.MoneyFlowVolume, 2));
            Assert.AreEqual(3439986548.42m, Math.Round(r.Adl, 2));
            Assert.AreEqual(3595352721.16m, Math.Round((decimal)r.AdlSma, 2));
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad SMA period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetAdl(quotes, 0));

            // insufficient quotes
            Assert.ThrowsException<BadQuotesException>(() =>
                Indicator.GetAdl(TestData.GetDefault(1)));
        }
    }
}
