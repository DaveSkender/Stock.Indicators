using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using Tests.Common;

namespace Tests.Indicators;

[TestClass]
public class SmaExtendedTests : TestBase
{
    [TestMethod]
    public void Analysis()
    {
        List<SmaAnalysis> results = quotes
            .GetSmaAnalysis(20)
            .ToList();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.Sma != null));

        // sample value
        SmaAnalysis r = results[501];
        Assert.AreEqual(251.86, r.Sma.Round(6));
        Assert.AreEqual(9.450000, r.Mad.Round(6));
        Assert.AreEqual(119.25102, r.Mse.Round(6));
        Assert.AreEqual(0.037637, r.Mape.Round(6));
    }

    [TestMethod]
    public void UseTuple()
    {
        List<SmaAnalysis> results = quotes
            .Use(CandlePart.Close)
            .GetSmaAnalysis(20)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void TupleNaN()
    {
        List<SmaAnalysis> r = tupleNanny
            .GetSmaAnalysis(6)
            .ToList();

        Assert.AreEqual(200, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Mse is double and double.NaN));
    }

    [TestMethod]
    public void Chainee()
    {
        List<SmaAnalysis> results = quotes
            .GetSma(2)
            .GetSmaAnalysis(20)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(482, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void Chainor()
    {
        List<EmaResult> results = quotes
            .GetSmaAnalysis(10)
            .GetEma(10)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(484, results.Count(x => x.Ema != null));
    }

    [TestMethod]
    public void BadData()
    {
        List<SmaAnalysis> r = badQuotes
            .GetSmaAnalysis(15)
            .ToList();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Mape is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<SmaAnalysis> r0 = noquotes
            .GetSmaAnalysis(6)
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<SmaAnalysis> r1 = onequote
            .GetSmaAnalysis(6)
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        List<SmaAnalysis> results = quotes
            .GetSmaAnalysis(20)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - 19, results.Count);
        Assert.AreEqual(251.8600, Math.Round(results.LastOrDefault().Sma.Value, 4));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => quotes.GetSmaAnalysis(0));
}
