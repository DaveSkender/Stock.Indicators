using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using Tests.Common;

namespace Tests.Indicators;

[TestClass]
public class StochTests : TestBase
{
    [TestMethod]
    public void Standard() // Slow
    {
        int lookbackPeriods = 14;
        int signalPeriods = 3;
        int smoothPeriods = 3;

        List<StochResult> results = quotes
            .GetStoch(lookbackPeriods, signalPeriods, smoothPeriods)
            .ToList();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(487, results.Count(x => x.Oscillator != null));
        Assert.AreEqual(485, results.Count(x => x.Signal != null));

        // sample values
        StochResult r15 = results[15];
        Assert.AreEqual(81.1253, r15.Oscillator.Round(4));
        Assert.IsNull(r15.Signal);
        Assert.IsNull(r15.PercentJ);

        StochResult r17 = results[17];
        Assert.AreEqual(92.1307, r17.Oscillator.Round(4));
        Assert.AreEqual(88.4995, r17.Signal.Round(4));
        Assert.AreEqual(99.3929, r17.PercentJ.Round(4));

        StochResult r149 = results[149];
        Assert.AreEqual(81.6870, r149.Oscillator.Round(4));
        Assert.AreEqual(79.7935, r149.Signal.Round(4));
        Assert.AreEqual(85.4741, r149.PercentJ.Round(4));

        StochResult r249 = results[249];  // also testing aliases here
        Assert.AreEqual(83.2020, r249.K.Round(4));
        Assert.AreEqual(83.0813, r249.D.Round(4));
        Assert.AreEqual(83.4435, r249.J.Round(4));

        StochResult r501 = results[501];
        Assert.AreEqual(43.1353, r501.Oscillator.Round(4));
        Assert.AreEqual(35.5674, r501.Signal.Round(4));
        Assert.AreEqual(58.2712, r501.PercentJ.Round(4));

        // test boundary condition

        for (int i = 0; i < results.Count; i++)
        {
            StochResult r = results[i];

            if (r.Oscillator is not null)
            {
                Assert.IsTrue(r.Oscillator >= 0);
                Assert.IsTrue(r.Oscillator <= 100);
            }

            if (r.Signal is not null)
            {
                Assert.IsTrue(r.Signal >= 0);
                Assert.IsTrue(r.Signal <= 100);
            }

            if (r.PercentJ is not null)
            {
                Assert.IsTrue(r.Signal >= 0);
                Assert.IsTrue(r.Signal <= 100);
            }
        }
    }

    [TestMethod]
    public void Extended() // with extra parameteres
    {
        List<StochResult> results =
            quotes.GetStoch(9, 3, 3, 5, 4, MaType.SMMA)
            .ToList();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(494, results.Count(x => x.K != null));
        Assert.AreEqual(494, results.Count(x => x.D != null));

        // sample values
        StochResult r7 = results[7];
        Assert.IsNull(r7.K);
        Assert.IsNull(r7.D);
        Assert.IsNull(r7.J);

        StochResult r8 = results[8];
        Assert.AreEqual(81.9178, r8.K.Round(4));
        Assert.AreEqual(81.9178, r8.D.Round(4));
        Assert.AreEqual(81.9178, r8.J.Round(4));

        StochResult r17 = results[17];
        Assert.AreEqual(82.5181, r17.K.Round(4));
        Assert.AreEqual(76.2603, r17.D.Round(4));
        Assert.AreEqual(107.5491, r17.J.Round(4));

        StochResult r149 = results[149];
        Assert.AreEqual(77.1571, r149.K.Round(4));
        Assert.AreEqual(72.8206, r149.D.Round(4));
        Assert.AreEqual(94.5030, r149.J.Round(4));

        StochResult r249 = results[249];  // also testing aliases here
        Assert.AreEqual(74.3652, r249.K.Round(4));
        Assert.AreEqual(75.5660, r249.D.Round(4));
        Assert.AreEqual(69.5621, r249.J.Round(4));

        StochResult r501 = results[501];
        Assert.AreEqual(46.9807, r501.K.Round(4));
        Assert.AreEqual(32.0413, r501.D.Round(4));
        Assert.AreEqual(106.7382, r501.J.Round(4));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = quotes
            .GetStoch()
            .GetSma(10)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(478, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void NoSignal()
    {
        int lookbackPeriods = 5;
        int signalPeriods = 1;
        int smoothPeriods = 3;

        List<StochResult> results = quotes
            .GetStoch(lookbackPeriods, signalPeriods, smoothPeriods)
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

        List<StochResult> results = quotes
            .GetStoch(lookbackPeriods, signalPeriods, smoothPeriods)
            .ToList();

        // sample values
        StochResult r1 = results[487];
        Assert.AreEqual(25.0353, r1.Oscillator.Round(4));
        Assert.AreEqual(60.5706, r1.Signal.Round(4));

        StochResult r2 = results[501];
        Assert.AreEqual(91.6233, r2.Oscillator.Round(4));
        Assert.AreEqual(36.0608, r2.Signal.Round(4));
    }

    [TestMethod]
    public void FastSmall()
    {
        int lookbackPeriods = 1;
        int signalPeriods = 10;
        int smoothPeriods = 1;

        List<StochResult> results = quotes
            .GetStoch(lookbackPeriods, signalPeriods, smoothPeriods)
            .ToList();

        // sample values
        StochResult r1 = results[70];
        Assert.AreEqual(0, r1.Oscillator);

        StochResult r2 = results[71];
        Assert.AreEqual(100, r2.Oscillator);
    }

    [TestMethod]
    public void BadData()
    {
        List<StochResult> r = badQuotes
            .GetStoch(15)
            .ToList();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Oscillator is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<StochResult> r0 = noquotes
            .GetStoch()
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<StochResult> r1 = onequote
            .GetStoch()
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        int lookbackPeriods = 14;
        int signalPeriods = 3;
        int smoothPeriods = 3;

        List<StochResult> results = quotes
            .GetStoch(lookbackPeriods, signalPeriods, smoothPeriods)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - (lookbackPeriods + smoothPeriods - 2), results.Count);

        StochResult last = results.LastOrDefault();
        Assert.AreEqual(43.1353, last.Oscillator.Round(4));
        Assert.AreEqual(35.5674, last.Signal.Round(4));
        Assert.AreEqual(58.2712, last.PercentJ.Round(4));
    }

    [TestMethod]
    public void Boundary()
    {
        int lookbackPeriods = 14;
        int signalPeriods = 3;
        int smoothPeriods = 3;

        List<StochResult> results = TestData
            .GetRandom(2500)
            .GetStoch(lookbackPeriods, signalPeriods, smoothPeriods)
            .ToList();

        // test boundary condition

        for (int i = 0; i < results.Count; i++)
        {
            StochResult r = results[i];

            if (r.Oscillator is not null)
            {
                Assert.IsTrue(r.Oscillator >= 0);
                Assert.IsTrue(r.Oscillator <= 100);
            }

            if (r.Signal is not null)
            {
                Assert.IsTrue(r.Signal >= 0);
                Assert.IsTrue(r.Signal <= 100);
            }

            if (r.PercentJ is not null)
            {
                Assert.IsTrue(r.Signal >= 0);
                Assert.IsTrue(r.Signal <= 100);
            }
        }
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetStoch(0));

        // bad signal period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetStoch(14, 0));

        // bad smoothing period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetStoch(14, 3, 0));

        // bad kFactor
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetStoch(9, 3, 1, 0, 2, MaType.SMA));

        // bad dFactor
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetStoch(9, 3, 1, 3, 0, MaType.SMA));

        // bad MA type
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetStoch(9, 3, 3, 3, 2, MaType.ALMA));
    }
}
