using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class Stoch : TestBase
    {

        [TestMethod]
        public void Standard()  // Slow
        {
            int lookbackPeriod = 14;
            int signalPeriod = 3;
            int smoothPeriod = 3;

            List<StochResult> results =
                Indicator.GetStoch(history, lookbackPeriod, signalPeriod, smoothPeriod)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(487, results.Where(x => x.Oscillator != null).Count());
            Assert.AreEqual(485, results.Where(x => x.Signal != null).Count());

            // sample values
            StochResult r1 = results[15];
            Assert.AreEqual(81.1253m, Math.Round((decimal)r1.Oscillator, 4));
            Assert.IsNull(r1.Signal);
            Assert.IsNull(r1.PercentJ);

            StochResult r2 = results[17];
            Assert.AreEqual(92.1307m, Math.Round((decimal)r2.Oscillator, 4));
            Assert.AreEqual(88.4995m, Math.Round((decimal)r2.Signal, 4));
            Assert.AreEqual(99.3929m, Math.Round((decimal)r2.PercentJ, 4));

            StochResult r3 = results[149];
            Assert.AreEqual(81.6870m, Math.Round((decimal)r3.Oscillator, 4));
            Assert.AreEqual(79.7935m, Math.Round((decimal)r3.Signal, 4));
            Assert.AreEqual(85.4741m, Math.Round((decimal)r3.PercentJ, 4));

            StochResult r4 = results[249];
            Assert.AreEqual(83.2020m, Math.Round((decimal)r4.Oscillator, 4));
            Assert.AreEqual(83.0813m, Math.Round((decimal)r4.Signal, 4));
            Assert.AreEqual(83.4435m, Math.Round((decimal)r4.PercentJ, 4));

            StochResult r5 = results[501];
            Assert.AreEqual(43.1353m, Math.Round((decimal)r5.Oscillator, 4));
            Assert.AreEqual(35.5674m, Math.Round((decimal)r5.Signal, 4));
            Assert.AreEqual(58.2712m, Math.Round((decimal)r5.PercentJ, 4));
        }

        [TestMethod]
        public void NoSignal()
        {
            int lookbackPeriod = 5;
            int signalPeriod = 1;
            int smoothPeriod = 3;

            List<StochResult> results =
                Indicator.GetStoch(history, lookbackPeriod, signalPeriod, smoothPeriod)
                .ToList();

            // signal equals oscillator
            StochResult r1 = results[487];
            Assert.AreEqual(r1.Oscillator, r1.Signal);

            StochResult r2 = results[501];
            Assert.AreEqual(r2.Oscillator, r2.Signal);
        }

        [TestMethod]
        public void Fast()
        {
            int lookbackPeriod = 5;
            int signalPeriod = 10;
            int smoothPeriod = 1;

            List<StochResult> results =
                Indicator.GetStoch(history, lookbackPeriod, signalPeriod, smoothPeriod)
                .ToList();

            // sample values
            StochResult r1 = results[487];
            Assert.AreEqual(25.0353m, Math.Round((decimal)r1.Oscillator, 4));
            Assert.AreEqual(60.5706m, Math.Round((decimal)r1.Signal, 4));

            StochResult r2 = results[501];
            Assert.AreEqual(91.6233m, Math.Round((decimal)r2.Oscillator, 4));
            Assert.AreEqual(36.0608m, Math.Round((decimal)r2.Signal, 4));
        }

        [TestMethod]
        public void FastSmall()
        {
            int lookbackPeriod = 1;
            int signalPeriod = 10;
            int smoothPeriod = 1;

            List<StochResult> results =
                Indicator.GetStoch(history, lookbackPeriod, signalPeriod, smoothPeriod)
                .ToList();

            // sample values
            StochResult r1 = results[70];
            Assert.AreEqual(0m, Math.Round((decimal)r1.Oscillator, 4));

            StochResult r2 = results[71];
            Assert.AreEqual(100m, Math.Round((decimal)r2.Oscillator, 4));
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<StochResult> r = Indicator.GetStoch(historyBad, 15);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad lookback period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetStoch(history, 0));

            // bad signal period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetStoch(history, 14, 0));

            // bad smoothing period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetStoch(history, 14, 3, 0));

            // insufficient history
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetStoch(HistoryTestData.Get(32), 30, 3, 3));
        }
    }
}