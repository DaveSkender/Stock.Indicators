using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Internal.Tests
{
    [TestClass]
    public class Stoch : TestBase
    {

        [TestMethod()]
        public void Standard()
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

            // sample value
            StochResult r = results[501];
            Assert.AreEqual(43.1353m, Math.Round((decimal)r.Oscillator, 4));
            Assert.AreEqual(35.5674m, Math.Round((decimal)r.Signal, 4));
        }

        [TestMethod()]
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

        [TestMethod()]
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

        [TestMethod()]
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

        [TestMethod()]
        public void BadData()
        {
            IEnumerable<StochResult> r = Indicator.GetStoch(historyBad, 15);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod()]
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
                Indicator.GetStoch(History.GetHistory(32), 30, 3, 3));
        }

    }
}