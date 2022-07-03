using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class SmaExtended : TestBase
{
    [TestMethod]
    public void Analysis()
    {
        List<SmaAnalysis> results = quotes.GetSmaAnalysis(20).ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.Sma != null));

        // sample value
        SmaAnalysis r = results[501];
        Assert.AreEqual(251.86, NullMath.Round(r.Sma, 6));
        Assert.AreEqual(9.450000, NullMath.Round(r.Mad, 6));
        Assert.AreEqual(119.25102, NullMath.Round(r.Mse, 6));
        Assert.AreEqual(0.037637, NullMath.Round(r.Mape, 6));
    }

    [TestMethod]
    public void UseTuple()
    {
        IEnumerable<SmaAnalysis> results = quotes
            .Use(CandlePart.Close)
            .GetSmaAnalysis(20);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(483, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void TupleNaN()
    {
        IEnumerable<SmaAnalysis> r = tupleNanny.GetSmaAnalysis(6);

        Assert.AreEqual(200, r.Count());
        Assert.AreEqual(0, r.Count(x => x.Mse is double and double.NaN));
    }

    [TestMethod]
    public void Chainee()
    {
        IEnumerable<SmaAnalysis> results = quotes
            .GetSma(2)
            .GetSmaAnalysis(20);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(482, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void Chainor()
    {
        IEnumerable<EmaResult> results = quotes
            .GetSmaAnalysis(10)
            .GetEma(10);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(484, results.Count(x => x.Ema != null));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<SmaAnalysis> r = Indicator.GetSmaAnalysis(badQuotes, 15);
        Assert.AreEqual(502, r.Count());
        Assert.AreEqual(0, r.Count(x => x.Mape is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<SmaAnalysis> r0 = noquotes.GetSmaAnalysis(6);
        Assert.AreEqual(0, r0.Count());

        IEnumerable<SmaAnalysis> r1 = onequote.GetSmaAnalysis(6);
        Assert.AreEqual(1, r1.Count());
    }

    [TestMethod]
    public void Removed()
    {
        List<SmaAnalysis> results = quotes.GetSmaAnalysis(20)
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
            => Indicator.GetSmaAnalysis(quotes, 0));
}
