using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using Tests.Common;

namespace Tests.Indicators;

[TestClass]
public class UlcerIndexTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<UlcerIndexResult> results = quotes
            .GetUlcerIndex(14)
            .ToList();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(489, results.Count(x => x.UI != null));

        // sample value
        UlcerIndexResult r = results[501];
        Assert.AreEqual(5.7255, r.UI.Round(4));
    }

    [TestMethod]
    public void UseTuple()
    {
        List<UlcerIndexResult> results = quotes
            .Use(CandlePart.Close)
            .GetUlcerIndex(14)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(489, results.Count(x => x.UI != null));
    }

    [TestMethod]
    public void TupleNaN()
    {
        List<UlcerIndexResult> r = tupleNanny
            .GetUlcerIndex(6)
            .ToList();

        Assert.AreEqual(200, r.Count);
        Assert.AreEqual(0, r.Count(x => x.UI is double and double.NaN));
    }

    [TestMethod]
    public void Chainee()
    {
        List<UlcerIndexResult> results = quotes
            .GetSma(2)
            .GetUlcerIndex(14)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(488, results.Count(x => x.UI != null));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = quotes
            .GetUlcerIndex(14)
            .GetSma(10)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(480, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        List<UlcerIndexResult> r = badQuotes
            .GetUlcerIndex(15)
            .ToList();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.UI is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<UlcerIndexResult> r0 = noquotes
            .GetUlcerIndex()
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<UlcerIndexResult> r1 = onequote
            .GetUlcerIndex()
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        List<UlcerIndexResult> results = quotes
            .GetUlcerIndex(14)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - 13, results.Count);

        UlcerIndexResult last = results.LastOrDefault();
        Assert.AreEqual(5.7255, last.UI.Round(4));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => quotes.GetUlcerIndex(0));
}
