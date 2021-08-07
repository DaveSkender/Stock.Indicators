using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class StochRsi : TestBase
    {

        [TestMethod]
        public void FastRsi()
        {
            int rsiPeriods = 14;
            int stochPeriods = 14;
            int signalPeriods = 3;
            int smoothPeriods = 1;

            List<StochRsiResult> results =
                quotes.GetStochRsi(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods)
                .ToList();

            // assertions

            // proper quantities
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(475, results.Where(x => x.StochRsi != null).Count());
            Assert.AreEqual(473, results.Where(x => x.Signal != null).Count());

            // sample values
            StochRsiResult r1 = results[31];
            Assert.AreEqual(93.3333m, Math.Round((decimal)r1.StochRsi, 4));
            Assert.AreEqual(97.7778m, Math.Round((decimal)r1.Signal, 4));

            StochRsiResult r2 = results[152];
            Assert.AreEqual(0m, Math.Round((decimal)r2.StochRsi, 4));
            Assert.AreEqual(0m, Math.Round((decimal)r2.Signal, 4));

            StochRsiResult r3 = results[249];
            Assert.AreEqual(36.5517m, Math.Round((decimal)r3.StochRsi, 4));
            Assert.AreEqual(27.3094m, Math.Round((decimal)r3.Signal, 4));

            StochRsiResult r4 = results[501];
            Assert.AreEqual(97.5244m, Math.Round((decimal)r4.StochRsi, 4));
            Assert.AreEqual(89.8385m, Math.Round((decimal)r4.Signal, 4));
        }

        [TestMethod]
        public void SlowRsi()
        {
            int rsiPeriods = 14;
            int stochPeriods = 14;
            int signalPeriods = 3;
            int smoothPeriods = 3;

            List<StochRsiResult> results =
                Indicator.GetStochRsi(quotes, rsiPeriods, stochPeriods, signalPeriods, smoothPeriods)
                .ToList();

            // assertions

            // proper quantities
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(473, results.Where(x => x.StochRsi != null).Count());
            Assert.AreEqual(471, results.Where(x => x.Signal != null).Count());

            // sample values
            StochRsiResult r1 = results[31];
            Assert.AreEqual(97.7778m, Math.Round((decimal)r1.StochRsi, 4));
            Assert.AreEqual(99.2593m, Math.Round((decimal)r1.Signal, 4));

            StochRsiResult r2 = results[152];
            Assert.AreEqual(0m, Math.Round((decimal)r2.StochRsi, 4));
            Assert.AreEqual(20.0263m, Math.Round((decimal)r2.Signal, 4));

            StochRsiResult r3 = results[249];
            Assert.AreEqual(27.3094m, Math.Round((decimal)r3.StochRsi, 4));
            Assert.AreEqual(33.2716m, Math.Round((decimal)r3.Signal, 4));

            StochRsiResult r4 = results[501];
            Assert.AreEqual(89.8385m, Math.Round((decimal)r4.StochRsi, 4));
            Assert.AreEqual(73.4176m, Math.Round((decimal)r4.Signal, 4));
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<StochRsiResult> r = Indicator.GetStochRsi(historyBad, 15, 20, 3, 2);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Removed()
        {
            int rsiPeriods = 14;
            int stochPeriods = 14;
            int signalPeriods = 3;
            int smoothPeriods = 3;

            List<StochRsiResult> results =
                Indicator.GetStochRsi(quotes, rsiPeriods, stochPeriods, signalPeriods, smoothPeriods)
                    .RemoveWarmupPeriods()
                    .ToList();

            // assertions
            int removeQty = rsiPeriods + stochPeriods + smoothPeriods + 100;
            Assert.AreEqual(502 - removeQty, results.Count);

            StochRsiResult last = results.LastOrDefault();
            Assert.AreEqual(89.8385m, Math.Round((decimal)last.StochRsi, 4));
            Assert.AreEqual(73.4176m, Math.Round((decimal)last.Signal, 4));
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad RSI period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetStochRsi(quotes, 0, 14, 3, 1));

            // bad STO period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetStochRsi(quotes, 14, 0, 3, 3));

            // bad STO signal period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetStochRsi(quotes, 14, 14, 0));

            // bad STO smoothing period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetStochRsi(quotes, 14, 14, 3, 0));

            // insufficient quotes
            Assert.ThrowsException<BadQuotesException>(() =>
                Indicator.GetStochRsi(TestData.GetDefault(129), 30, 30, 5, 5));
        }
    }
}
