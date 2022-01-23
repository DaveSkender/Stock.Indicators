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
        Assert.AreEqual(132286411m, Math.Round(r35.Volume, 0));
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
        Assert.AreEqual(1180981564m, Math.Round(r0.Volume, 0));
        Assert.IsTrue(r0.IsUp);

        RenkoResult r25 = results[25];
        Assert.AreEqual(240.5m, r25.Open);
        Assert.AreEqual(244.04m, r25.High);
        Assert.AreEqual(240.8m, r25.Low);
        Assert.AreEqual(243m, r25.Close);
        Assert.AreEqual(256003600m, Math.Round(r25.Volume, 0));
        Assert.IsTrue(r25.IsUp);

        RenkoResult r233 = results[233];
        Assert.AreEqual(240.5m, r233.Open);
        Assert.AreEqual(245.07m, r233.High);
        Assert.AreEqual(235.52m, r233.Low);
        Assert.AreEqual(238m, r233.Close);
        Assert.AreEqual(260180208m, Math.Round(r233.Volume, 0));
        Assert.IsFalse(r233.IsUp);

        RenkoResult r247 = results[247];
        Assert.AreEqual(245.5m, r247.Open);
        Assert.AreEqual(245.54m, r247.High);
        Assert.AreEqual(242.87m, r247.Low);
        Assert.AreEqual(243m, r247.Close);
        Assert.AreEqual(147031456m, Math.Round(r247.Volume, 0));
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
        Assert.AreEqual(212.8m, Math.Round(r0.Open, 4));
        Assert.AreEqual(220.19m, Math.Round(r0.High, 4));
        Assert.AreEqual(212.53m, Math.Round(r0.Low, 4));
        Assert.AreEqual(218.9497m, Math.Round(r0.Close, 4));
        Assert.AreEqual(2090292272m, Math.Round(r0.Volume, 0));
        Assert.IsTrue(r0.IsUp);

        RenkoResult r10 = results[10];
        Assert.AreEqual(274.2975m, Math.Round(r10.Open, 4));
        Assert.AreEqual(275.87m, Math.Round(r10.High, 4));
        Assert.AreEqual(265.25m, Math.Round(r10.Low, 4));
        Assert.AreEqual(268.1477m, Math.Round(r10.Close, 4));
        Assert.AreEqual(627270200m, Math.Round(r10.Volume, 0));
        Assert.IsFalse(r10.IsUp);

        RenkoResult r25 = results[25];
        Assert.AreEqual(268.1477m, Math.Round(r25.Open, 4));
        Assert.AreEqual(270.25m, Math.Round(r25.High, 4));
        Assert.AreEqual(261.38m, Math.Round(r25.Low, 4));
        Assert.AreEqual(261.9980m, Math.Round(r25.Close, 4));
        Assert.AreEqual(1233408112m, Math.Round(r25.Volume, 0));
        Assert.IsFalse(r25.IsUp);

        RenkoResult r42 = results[42];
        Assert.AreEqual(237.3990m, Math.Round(r42.Open, 4));
        Assert.AreEqual(246.73m, Math.Round(r42.High, 4));
        Assert.AreEqual(234.52m, Math.Round(r42.Low, 4));
        Assert.AreEqual(243.5487m, Math.Round(r42.Close, 4));
        Assert.AreEqual(492824400m, Math.Round(r42.Volume, 0));
        Assert.IsTrue(r42.IsUp);
    }

    [TestMethod]
    public void ConvertToQuotes()
    {
        List<Quote> newQuotes = quotes.GetRenko(2.5m)
            .ConvertToQuotes()
            .ToList();

        // assertions
        Assert.AreEqual(154, newQuotes.Count);

        Quote q = newQuotes[153];
        Assert.AreEqual(240.5m, q.Open);
        Assert.AreEqual(243.68m, q.High);
        Assert.AreEqual(234.52m, q.Low);
        Assert.AreEqual(243m, q.Close);
        Assert.AreEqual(189794032m, q.Volume);
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

    [TestMethod]
    public void Exceptions()
    {
        // bad arguments
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetRenko(quotes, 0));
    }
}
