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
            int lookbackPeriods = 14;
            int signalPeriods = 3;
            int smoothPeriods = 3;

            List<StochResult> results =
                quotes.GetStoch(lookbackPeriods, signalPeriods, smoothPeriods)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is quotes
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(487, results.Where(x => x.Oscillator != null).Count());
            Assert.AreEqual(485, results.Where(x => x.Signal != null).Count());

            // sample values
            StochResult r15 = results[15];
            Assert.AreEqual(81.1253m, Math.Round((decimal)r15.Oscillator, 4));
            Assert.IsNull(r15.Signal);
            Assert.IsNull(r15.PercentJ);

            StochResult r17 = results[17];
            Assert.AreEqual(92.1307m, Math.Round((decimal)r17.Oscillator, 4));
            Assert.AreEqual(88.4995m, Math.Round((decimal)r17.Signal, 4));
            Assert.AreEqual(99.3929m, Math.Round((decimal)r17.PercentJ, 4));

            StochResult r149 = results[149];
            Assert.AreEqual(81.6870m, Math.Round((decimal)r149.Oscillator, 4));
            Assert.AreEqual(79.7935m, Math.Round((decimal)r149.Signal, 4));
            Assert.AreEqual(85.4741m, Math.Round((decimal)r149.PercentJ, 4));

            StochResult r249 = results[249];  // also testing aliases here
            Assert.AreEqual(83.2020m, Math.Round((decimal)r249.K, 4));
            Assert.AreEqual(83.0813m, Math.Round((decimal)r249.D, 4));
            Assert.AreEqual(83.4435m, Math.Round((decimal)r249.J, 4));

            StochResult r501 = results[501];
            Assert.AreEqual(43.1353m, Math.Round((decimal)r501.Oscillator, 4));
            Assert.AreEqual(35.5674m, Math.Round((decimal)r501.Signal, 4));
            Assert.AreEqual(58.2712m, Math.Round((decimal)r501.PercentJ, 4));
        }

        [TestMethod]
        public void Extended()  // with extra parameteres
        {

            List<StochResult> results =
                quotes.GetStoch(9, 3, 3, 5, 4, MaType.SMMA)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is quotes
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(494, results.Where(x => x.K != null).Count());
            Assert.AreEqual(494, results.Where(x => x.D != null).Count());

            // sample values
            StochResult r7 = results[7];
            Assert.IsNull(r7.K);
            Assert.IsNull(r7.D);
            Assert.IsNull(r7.J);

            StochResult r8 = results[8];
            Assert.AreEqual(81.9178m, Math.Round((decimal)r8.K, 4));
            Assert.AreEqual(81.9178m, Math.Round((decimal)r8.D, 4));
            Assert.AreEqual(81.9178m, Math.Round((decimal)r8.J, 4));

            StochResult r17 = results[17];
            Assert.AreEqual(82.5181m, Math.Round((decimal)r17.K, 4));
            Assert.AreEqual(76.2603m, Math.Round((decimal)r17.D, 4));
            Assert.AreEqual(107.5491m, Math.Round((decimal)r17.J, 4));

            StochResult r149 = results[149];
            Assert.AreEqual(77.1571m, Math.Round((decimal)r149.K, 4));
            Assert.AreEqual(72.8206m, Math.Round((decimal)r149.D, 4));
            Assert.AreEqual(94.5030m, Math.Round((decimal)r149.J, 4));

            StochResult r249 = results[249];  // also testing aliases here
            Assert.AreEqual(74.3652m, Math.Round((decimal)r249.K, 4));
            Assert.AreEqual(75.5660m, Math.Round((decimal)r249.D, 4));
            Assert.AreEqual(69.5621m, Math.Round((decimal)r249.J, 4));

            StochResult r501 = results[501];
            Assert.AreEqual(46.9807m, Math.Round((decimal)r501.K, 4));
            Assert.AreEqual(32.0413m, Math.Round((decimal)r501.D, 4));
            Assert.AreEqual(106.7382m, Math.Round((decimal)r501.J, 4));
        }

        [TestMethod]
        public void NoSignal()
        {
            int lookbackPeriods = 5;
            int signalPeriods = 1;
            int smoothPeriods = 3;

            List<StochResult> results =
                Indicator.GetStoch(quotes, lookbackPeriods, signalPeriods, smoothPeriods)
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
            int lookbackPeriods = 5;
            int signalPeriods = 10;
            int smoothPeriods = 1;

            List<StochResult> results =
                Indicator.GetStoch(quotes, lookbackPeriods, signalPeriods, smoothPeriods)
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
            int lookbackPeriods = 1;
            int signalPeriods = 10;
            int smoothPeriods = 1;

            List<StochResult> results =
                Indicator.GetStoch(quotes, lookbackPeriods, signalPeriods, smoothPeriods)
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
            IEnumerable<StochResult> r = Indicator.GetStoch(badQuotes, 15);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Removed()
        {
            int lookbackPeriods = 14;
            int signalPeriods = 3;
            int smoothPeriods = 3;

            List<StochResult> results =
                quotes.GetStoch(lookbackPeriods, signalPeriods, smoothPeriods)
                    .RemoveWarmupPeriods()
                    .ToList();

            // assertions
            Assert.AreEqual(502 - (lookbackPeriods + smoothPeriods - 2), results.Count);

            StochResult last = results.LastOrDefault();
            Assert.AreEqual(43.1353m, Math.Round((decimal)last.Oscillator, 4));
            Assert.AreEqual(35.5674m, Math.Round((decimal)last.Signal, 4));
            Assert.AreEqual(58.2712m, Math.Round((decimal)last.PercentJ, 4));
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad lookback period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetStoch(quotes, 0));

            // bad signal period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetStoch(quotes, 14, 0));

            // bad smoothing period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetStoch(quotes, 14, 3, 0));

            // bad kFactor
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                quotes.GetStoch(9, 3, 1, 0, 2, MaType.SMA));

            // bad dFactor
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                quotes.GetStoch(9, 3, 1, 3, 0, MaType.SMA));

            // bad MA type
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                quotes.GetStoch(9, 3, 3, 3, 2, MaType.ALMA));

            // insufficient quotes
            Assert.ThrowsException<BadQuotesException>(() =>
                Indicator.GetStoch(TestData.GetDefault(32), 30, 3, 3));
        }
    }
}
