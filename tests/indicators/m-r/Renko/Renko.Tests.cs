using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class Renko : TestBase
{
    [TestMethod]
    public void StandardClose()
    {
        List<RenkoResult> results = quotes
            .GetRenko(2.5m, EndType.Close)
            .ToList();

        // assertions

        // should always be the same number of results as there is quotes
        Assert.AreEqual(154, results.Count);

        // sample values
        RenkoResult r0 = results[0];
        Assert.AreEqual(213m, r0.Open);
        Assert.AreEqual(216.89m, r0.High);
        Assert.AreEqual(212.53m, r0.Low);
        Assert.AreEqual(215.5m, r0.Close);
        Assert.AreEqual(1180981564m, r0.Volume);
        Assert.IsTrue(r0.IsUp);

        RenkoResult r5 = results[5];
        Assert.AreEqual(225.5m, r5.Open);
        Assert.AreEqual(226.34m, r5.High);
        Assert.AreEqual(221.64m, r5.Low);
        Assert.AreEqual(223m, r5.Close);
        Assert.AreEqual(1150862992m, r5.Volume);
        Assert.IsFalse(r5.IsUp);

        RenkoResult r35 = results[35];
        Assert.AreEqual(270.5m, r35.Open);
        Assert.AreEqual(272.85m, r35.High);
        Assert.AreEqual(265.25m, r35.Low);
        Assert.AreEqual(268m, r35.Close);
        Assert.AreEqual(132286411m, NullMath.Round(r35.Volume, 0));
        Assert.IsFalse(r35.IsUp);

        RenkoResult r153 = results[153];
        Assert.AreEqual(240.5m, r153.Open);
        Assert.AreEqual(243.68m, r153.High);
        Assert.AreEqual(234.52m, r153.Low);
        Assert.AreEqual(243m, r153.Close);
        Assert.AreEqual(189794032m, r153.Volume);
        Assert.IsTrue(r153.IsUp);
    }

    [TestMethod]
    public void StandardHighLow()
    {
        List<RenkoResult> results = quotes
            .GetRenko(2.5m, EndType.HighLow)
            .ToList();

        // assertions

        // should always be the same number of results as there is quotes
        Assert.AreEqual(248, results.Count);

        // sample values
        RenkoResult r0 = results[0];
        Assert.AreEqual(213m, r0.Open);
        Assert.AreEqual(216.89m, r0.High);
        Assert.AreEqual(212.53m, r0.Low);
        Assert.AreEqual(215.5m, r0.Close);
        Assert.AreEqual(1180981564m, NullMath.Round(r0.Volume, 0));
        Assert.IsTrue(r0.IsUp);

        RenkoResult r25 = results[25];
        Assert.AreEqual(240.5m, r25.Open);
        Assert.AreEqual(244.04m, r25.High);
        Assert.AreEqual(240.8m, r25.Low);
        Assert.AreEqual(243m, r25.Close);
        Assert.AreEqual(256003600m, NullMath.Round(r25.Volume, 0));
        Assert.IsTrue(r25.IsUp);

        RenkoResult r233 = results[233];
        Assert.AreEqual(240.5m, r233.Open);
        Assert.AreEqual(245.07m, r233.High);
        Assert.AreEqual(235.52m, r233.Low);
        Assert.AreEqual(238m, r233.Close);
        Assert.AreEqual(260180208m, NullMath.Round(r233.Volume, 0));
        Assert.IsFalse(r233.IsUp);

        RenkoResult r247 = results[247];
        Assert.AreEqual(245.5m, r247.Open);
        Assert.AreEqual(245.54m, r247.High);
        Assert.AreEqual(242.87m, r247.Low);
        Assert.AreEqual(243m, r247.Close);
        Assert.AreEqual(147031456m, NullMath.Round(r247.Volume, 0));
        Assert.IsFalse(r247.IsUp);
    }

    [TestMethod]
    public void Atr()
    {
        List<RenkoResult> results = quotes
            .GetRenkoAtr(14, EndType.Close)
            .ToList();

        // assertions

        // should always be the same number of results as there is quotes
        Assert.AreEqual(43, results.Count);

        // sample values
        RenkoResult r0 = results[0];
        Assert.AreEqual(212.8m, NullMath.Round(r0.Open, 4));
        Assert.AreEqual(220.19m, NullMath.Round(r0.High, 4));
        Assert.AreEqual(212.53m, NullMath.Round(r0.Low, 4));
        Assert.AreEqual(218.9497m, NullMath.Round(r0.Close, 4));
        Assert.AreEqual(2090292272m, NullMath.Round(r0.Volume, 0));
        Assert.IsTrue(r0.IsUp);

        RenkoResult r10 = results[10];
        Assert.AreEqual(274.2975m, NullMath.Round(r10.Open, 4));
        Assert.AreEqual(275.87m, NullMath.Round(r10.High, 4));
        Assert.AreEqual(265.25m, NullMath.Round(r10.Low, 4));
        Assert.AreEqual(268.1477m, NullMath.Round(r10.Close, 4));
        Assert.AreEqual(627270200m, NullMath.Round(r10.Volume, 0));
        Assert.IsFalse(r10.IsUp);

        RenkoResult r25 = results[25];
        Assert.AreEqual(268.1477m, NullMath.Round(r25.Open, 4));
        Assert.AreEqual(270.25m, NullMath.Round(r25.High, 4));
        Assert.AreEqual(261.38m, NullMath.Round(r25.Low, 4));
        Assert.AreEqual(261.9980m, NullMath.Round(r25.Close, 4));
        Assert.AreEqual(1233408112m, NullMath.Round(r25.Volume, 0));
        Assert.IsFalse(r25.IsUp);

        RenkoResult r42 = results[42];
        Assert.AreEqual(237.3990m, NullMath.Round(r42.Open, 4));
        Assert.AreEqual(246.73m, NullMath.Round(r42.High, 4));
        Assert.AreEqual(234.52m, NullMath.Round(r42.Low, 4));
        Assert.AreEqual(243.5487m, NullMath.Round(r42.Close, 4));
        Assert.AreEqual(492824400m, NullMath.Round(r42.Volume, 0));
        Assert.IsTrue(r42.IsUp);
    }

    [TestMethod]
    public void UseAsQuotes()
    {
        IEnumerable<RenkoResult> renkoQuotes = quotes.GetRenko(0.5m);
        IEnumerable<SmaResult> renkoSma = renkoQuotes.GetSma(5);
        Assert.AreEqual(1124, renkoSma.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<RenkoResult> r = badQuotes.GetRenko(100m);
        Assert.AreNotEqual(0, r.Count());
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<RenkoResult> r0 = noquotes.GetRenko(0.01m);
        Assert.AreEqual(0, r0.Count());
    }

    // bad arguments
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => Indicator.GetRenko(quotes, 0));
}
