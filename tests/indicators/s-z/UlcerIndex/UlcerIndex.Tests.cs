using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class UlcerIndex : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<UlcerIndexResult> results = quotes.GetUlcerIndex(14)
            .ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(489, results.Where(x => x.UI != null).Count());

        // sample value
        UlcerIndexResult r = results[501];
        Assert.AreEqual(5.7255, NullMath.Round(r.UI, 4));
    }

    [TestMethod]
    public void UseTuple()
    {
        IEnumerable<UlcerIndexResult> results = quotes
            .Use(CandlePart.Close)
            .GetUlcerIndex(14);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(489, results.Where(x => x.UI != null).Count());
    }

    [TestMethod]
    public void Chainee()
    {
        IEnumerable<UlcerIndexResult> results = quotes
            .GetSma(2)
            .GetUlcerIndex(14);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(488, results.Where(x => x.UI != null).Count());
    }

    [TestMethod]
    public void Chainor()
    {
        IEnumerable<SmaResult> results = quotes
            .GetUlcerIndex(14)
            .GetSma(10);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(480, results.Where(x => x.Sma != null).Count());
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<UlcerIndexResult> r = Indicator.GetUlcerIndex(badQuotes, 15);
        Assert.AreEqual(502, r.Count());
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<UlcerIndexResult> r0 = noquotes.GetUlcerIndex();
        Assert.AreEqual(0, r0.Count());

        IEnumerable<UlcerIndexResult> r1 = onequote.GetUlcerIndex();
        Assert.AreEqual(1, r1.Count());
    }

    [TestMethod]
    public void Removed()
    {
        List<UlcerIndexResult> results = quotes.GetUlcerIndex(14)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - 13, results.Count);

        UlcerIndexResult last = results.LastOrDefault();
        Assert.AreEqual(5.7255, NullMath.Round(last.UI, 4));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => Indicator.GetUlcerIndex(quotes, 0));
}
