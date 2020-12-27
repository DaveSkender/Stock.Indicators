using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Internal.Tests
{
    [TestClass]
    public class StochasticRsiTests : TestBase
    {

        [TestMethod()]
        public void Standard()
        {
            int rsiPeriod = 14;
            int stochPeriod = 14;
            int signalPeriod = 3;
            int smoothPeriod = 1;

            List<StochRsiResult> results =
                Indicator.GetStochRsi(history, rsiPeriod, stochPeriod, signalPeriod, smoothPeriod)
                .ToList();

            // assertions

            // proper quantities
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(502 - rsiPeriod - stochPeriod - smoothPeriod + 2, results.Where(x => x.StochRsi != null).Count());
            Assert.AreEqual(502 - rsiPeriod - stochPeriod - signalPeriod - smoothPeriod + 3, results.Where(x => x.Signal != null).Count());

            // sample values
            StochRsiResult r1 = results[501];
            Assert.AreEqual(97.5244m, Math.Round((decimal)r1.StochRsi, 4));
            Assert.AreEqual(89.8385m, Math.Round((decimal)r1.Signal, 4));

            StochRsiResult r2 = results[249];
            Assert.AreEqual(36.5517m, Math.Round((decimal)r2.StochRsi, 4));
            Assert.AreEqual(27.3094m, Math.Round((decimal)r2.Signal, 4));

            StochRsiResult r3 = results[152];
            Assert.AreEqual(0m, Math.Round((decimal)r3.StochRsi, 4));
            Assert.AreEqual(0m, Math.Round((decimal)r3.Signal, 4));

            StochRsiResult r4 = results[31];
            Assert.AreEqual(93.3333m, Math.Round((decimal)r4.StochRsi, 4));
            Assert.AreEqual(97.7778m, Math.Round((decimal)r4.Signal, 4));
        }

        [TestMethod()]
        public void SlowRsi()
        {
            int rsiPeriod = 14;
            int stochPeriod = 14;
            int signalPeriod = 3;
            int smoothPeriod = 3;

            List<StochRsiResult> results =
                Indicator.GetStochRsi(history, rsiPeriod, stochPeriod, signalPeriod, smoothPeriod)
                .ToList();

            // assertions

            // proper quantities
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(502 - rsiPeriod - stochPeriod - smoothPeriod + 2, results.Where(x => x.StochRsi != null).Count());
            Assert.AreEqual(502 - rsiPeriod - stochPeriod - signalPeriod - smoothPeriod + 3, results.Where(x => x.Signal != null).Count());

            // sample values
            StochRsiResult r1 = results[501];
            Assert.AreEqual(89.8385m, Math.Round((decimal)r1.StochRsi, 4));
            Assert.AreEqual(73.4176m, Math.Round((decimal)r1.Signal, 4));

            StochRsiResult r2 = results[249];
            Assert.AreEqual(27.3094m, Math.Round((decimal)r2.StochRsi, 4));
            Assert.AreEqual(33.2716m, Math.Round((decimal)r2.Signal, 4));

            StochRsiResult r3 = results[152];
            Assert.AreEqual(0m, Math.Round((decimal)r3.StochRsi, 4));
            Assert.AreEqual(20.0263m, Math.Round((decimal)r3.Signal, 4));

            StochRsiResult r4 = results[31];
            Assert.AreEqual(97.7778m, Math.Round((decimal)r4.StochRsi, 4));
            Assert.AreEqual(99.2593m, Math.Round((decimal)r4.Signal, 4));
        }

        [TestMethod()]
        public void BadData()
        {
            IEnumerable<StochRsiResult> r = Indicator.GetStochRsi(historyBad, 15, 20, 3, 2);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod()]
        public void Convergence()
        {
            foreach (int qty in convergeQuantities.Where(x => x <= 502))
            {
                IEnumerable<Quote> h = History.GetHistory(110 + qty);
                IEnumerable<StochRsiResult> r = Indicator.GetStochRsi(h, 14, 14, 3, 1);

                StochRsiResult l = r.LastOrDefault();
                Console.WriteLine("SRSI on {0:d} with {1,4} periods: {2:N8}",
                    l.Date, h.Count(), l.StochRsi);
            }
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Bad RSI lookback.")]
        public void BadRsiLookback()
        {
            Indicator.GetStochRsi(history, 0, 14, 3, 1);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Bad STO lookback.")]
        public void BadLookbackPeriod()
        {
            Indicator.GetStochRsi(history, 14, 0, 3, 3);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Bad STO signal period.")]
        public void BadSignal()
        {
            Indicator.GetStochRsi(history, 14, 14, 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Bad STO smoothing period.")]
        public void BadSmooth()
        {
            Indicator.GetStochRsi(history, 14, 14, 3, 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            IEnumerable<Quote> h = History.GetHistory(129);
            Indicator.GetStochRsi(h, 30, 30, 5, 5);
        }

    }
}