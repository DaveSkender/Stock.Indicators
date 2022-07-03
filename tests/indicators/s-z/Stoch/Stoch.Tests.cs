using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class Stoch : TestBase
{
    [TestMethod]
    public void Standard() // Slow
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
        Assert.AreEqual(487, results.Count(x => x.Oscillator != null));
        Assert.AreEqual(485, results.Count(x => x.Signal != null));

        // sample values
        StochResult r15 = results[15];
        Assert.AreEqual(81.1253, NullMath.Round(r15.Oscillator, 4));
        Assert.IsNull(r15.Signal);
        Assert.IsNull(r15.PercentJ);

        StochResult r17 = results[17];
        Assert.AreEqual(92.1307, NullMath.Round(r17.Oscillator, 4));
        Assert.AreEqual(88.4995, NullMath.Round(r17.Signal, 4));
        Assert.AreEqual(99.3929, NullMath.Round(r17.PercentJ, 4));

        StochResult r149 = results[149];
        Assert.AreEqual(81.6870, NullMath.Round(r149.Oscillator, 4));
        Assert.AreEqual(79.7935, NullMath.Round(r149.Signal, 4));
        Assert.AreEqual(85.4741, NullMath.Round(r149.PercentJ, 4));

        StochResult r249 = results[249];  // also testing aliases here
        Assert.AreEqual(83.2020, NullMath.Round(r249.K, 4));
        Assert.AreEqual(83.0813, NullMath.Round(r249.D, 4));
        Assert.AreEqual(83.4435, NullMath.Round(r249.J, 4));

        StochResult r501 = results[501];
        Assert.AreEqual(43.1353, NullMath.Round(r501.Oscillator, 4));
        Assert.AreEqual(35.5674, NullMath.Round(r501.Signal, 4));
        Assert.AreEqual(58.2712, NullMath.Round(r501.PercentJ, 4));
    }

    [TestMethod]
    public void Extended() // with extra parameteres
    {
        List<StochResult> results =
            quotes.GetStoch(9, 3, 3, 5, 4, MaType.SMMA)
            .ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(494, results.Count(x => x.K != null));
        Assert.AreEqual(494, results.Count(x => x.D != null));

        // sample values
        StochResult r7 = results[7];
        Assert.IsNull(r7.K);
        Assert.IsNull(r7.D);
        Assert.IsNull(r7.J);

        StochResult r8 = results[8];
        Assert.AreEqual(81.9178, NullMath.Round(r8.K, 4));
        Assert.AreEqual(81.9178, NullMath.Round(r8.D, 4));
        Assert.AreEqual(81.9178, NullMath.Round(r8.J, 4));

        StochResult r17 = results[17];
        Assert.AreEqual(82.5181, NullMath.Round(r17.K, 4));
        Assert.AreEqual(76.2603, NullMath.Round(r17.D, 4));
        Assert.AreEqual(107.5491, NullMath.Round(r17.J, 4));

        StochResult r149 = results[149];
        Assert.AreEqual(77.1571, NullMath.Round(r149.K, 4));
        Assert.AreEqual(72.8206, NullMath.Round(r149.D, 4));
        Assert.AreEqual(94.5030, NullMath.Round(r149.J, 4));

        StochResult r249 = results[249];  // also testing aliases here
        Assert.AreEqual(74.3652, NullMath.Round(r249.K, 4));
        Assert.AreEqual(75.5660, NullMath.Round(r249.D, 4));
        Assert.AreEqual(69.5621, NullMath.Round(r249.J, 4));

        StochResult r501 = results[501];
        Assert.AreEqual(46.9807, NullMath.Round(r501.K, 4));
        Assert.AreEqual(32.0413, NullMath.Round(r501.D, 4));
        Assert.AreEqual(106.7382, NullMath.Round(r501.J, 4));
    }

    [TestMethod]
    public void Chainor()
    {
        IEnumerable<SmaResult> results = quotes
            .GetStoch()
            .GetSma(10);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(478, results.Count(x => x.Sma != null));
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
        Assert.AreEqual(25.0353, NullMath.Round(r1.Oscillator, 4));
        Assert.AreEqual(60.5706, NullMath.Round(r1.Signal, 4));

        StochResult r2 = results[501];
        Assert.AreEqual(91.6233, NullMath.Round(r2.Oscillator, 4));
        Assert.AreEqual(36.0608, NullMath.Round(r2.Signal, 4));
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
        Assert.AreEqual(0, r1.Oscillator);

        StochResult r2 = results[71];
        Assert.AreEqual(100, r2.Oscillator);
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<StochResult> r = Indicator.GetStoch(badQuotes, 15);
        Assert.AreEqual(502, r.Count());
        Assert.AreEqual(0, r.Count(x => x.Oscillator is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<StochResult> r0 = noquotes.GetStoch();
        Assert.AreEqual(0, r0.Count());

        IEnumerable<StochResult> r1 = onequote.GetStoch();
        Assert.AreEqual(1, r1.Count());
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
        Assert.AreEqual(43.1353, NullMath.Round(last.Oscillator, 4));
        Assert.AreEqual(35.5674, NullMath.Round(last.Signal, 4));
        Assert.AreEqual(58.2712, NullMath.Round(last.PercentJ, 4));
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
    }
}
