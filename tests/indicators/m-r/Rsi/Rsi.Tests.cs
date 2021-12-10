using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class Rsi : TestBase
    {

        [TestMethod]
        public void Standard()
        {
            List<RsiResult> results = quotes.GetRsi(14).ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is quotes
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(488, results.Where(x => x.Rsi != null).Count());

            // sample values
            RsiResult r1 = results[13];
            Assert.AreEqual(null, r1.Rsi);

            RsiResult r2 = results[14];
            Assert.AreEqual(62.0541, Math.Round((double)r2.Rsi, 4));

            RsiResult r3 = results[249];
            Assert.AreEqual(70.9368, Math.Round((double)r3.Rsi, 4));

            RsiResult r4 = results[501];
            Assert.AreEqual(42.0773, Math.Round((double)r4.Rsi, 4));
        }

        [TestMethod]
        public void SmallLookback()
        {
            int lookbackPeriods = 1;
            List<RsiResult> results = quotes.GetRsi(lookbackPeriods)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is quotes
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(501, results.Where(x => x.Rsi != null).Count());

            // sample values
            RsiResult r1 = results[28];
            Assert.AreEqual(100, r1.Rsi);

            RsiResult r2 = results[52];
            Assert.AreEqual(0, r2.Rsi);
        }

        [TestMethod]
        public void CrytoData()
        {
            IEnumerable<Quote> btc = TestData.GetBitcoin();
            IEnumerable<RsiResult> r = btc.GetRsi(1);
            Assert.AreEqual(1246, r.Count());
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<RsiResult> r = badQuotes.GetRsi(20);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void ConvertToQuotes()
        {
            // exclude nulls case
            List<Quote> results = quotes.GetRsi(14)
                .ConvertToQuotes()
                .ToList();

            // assertions

            // proper quantities
            Assert.AreEqual(488, results.Count);

            // sample values
            Quote first = results.FirstOrDefault();
            Assert.AreEqual(62.0541m, Math.Round(first.Close, 4));

            Quote last = results.LastOrDefault();
            Assert.AreEqual(42.0773m, Math.Round(last.Close, 4));
        }

        [TestMethod]
        public void Removed()
        {
            List<RsiResult> results = quotes.GetRsi(14)
                .RemoveWarmupPeriods()
                .ToList();

            // assertions
            Assert.AreEqual(502 - (10 * 14), results.Count);

            RsiResult last = results.LastOrDefault();
            Assert.AreEqual(42.0773, Math.Round((double)last.Rsi, 4));
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad lookback period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetRsi(quotes, 0));

            // insufficient quotes
            Assert.ThrowsException<BadQuotesException>(() =>
                Indicator.GetRsi(TestData.GetDefault(129), 30));
        }
    }
}
