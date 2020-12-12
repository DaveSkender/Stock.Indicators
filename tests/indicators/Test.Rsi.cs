using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Internal.Tests
{
    [TestClass]
    public class RsiTests : TestBase
    {

        [TestMethod()]
        public void GetRsi()
        {
            int lookbackPeriod = 14;
            List<RsiResult> results = Indicator.GetRsi(history, lookbackPeriod).ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(488, results.Where(x => x.Rsi != null).Count());

            // sample values
            RsiResult r1 = results[501];
            Assert.AreEqual(42.0773m, Math.Round((decimal)r1.Rsi, 4));

            RsiResult r2 = results[249];
            Assert.AreEqual(70.9368m, Math.Round((decimal)r2.Rsi, 4));

            RsiResult r3 = results[14];
            Assert.AreEqual(62.0541m, Math.Round((decimal)r3.Rsi, 4));

            RsiResult r4 = results[13];
            Assert.AreEqual(null, r4.Rsi);
        }

        [TestMethod()]
        public void GetRsiBadData()
        {
            IEnumerable<RsiResult> r = Indicator.GetRsi(historyBad, 20);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod()]
        public void GetRsiSmall()
        {
            int lookbackPeriod = 1;
            List<RsiResult> results = Indicator.GetRsi(history, lookbackPeriod).ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(501, results.Where(x => x.Rsi != null).Count());

            // sample values
            RsiResult r1 = results[28];
            Assert.AreEqual(100m, Math.Round((decimal)r1.Rsi, 4));

            RsiResult r2 = results[52];
            Assert.AreEqual(0m, Math.Round((decimal)r2.Rsi, 4));
        }

        [TestMethod()]
        public void GetRsiConvergence()
        {
            int lookbackPeriod = 14;

            foreach (int qty in convergeQuantities.Where(q => q > 50 - lookbackPeriod))
            {
                IEnumerable<Quote> h = History.GetHistoryLong(lookbackPeriod + qty);
                IEnumerable<RsiResult> r = Indicator.GetRsi(h, lookbackPeriod);

                RsiResult l = r.LastOrDefault();
                Console.WriteLine("RSI({0}) on {1:d} with {2,4} periods of history: {3:N8}",
                    lookbackPeriod, l.Date, h.Count(), l.Rsi);
            }
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Bad lookback.")]
        public void BadLookbackPeriod()
        {
            Indicator.GetRsi(history, 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            IEnumerable<Quote> h = History.GetHistory(79);
            Indicator.GetRsi(h, 30);
        }

    }
}