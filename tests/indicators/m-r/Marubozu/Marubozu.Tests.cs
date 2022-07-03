using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class Marubozu : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<CandleResult> results = quotes.GetMarubozu(95).ToList();

        // assertions

        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(6, results.Count(x => x.Match != Match.None));

        // sample values
        CandleResult r31 = results[31];
        Assert.AreEqual(null, r31.Price);
        Assert.AreEqual(0, (int)r31.Match);

        CandleResult r32 = results[32];
        Assert.AreEqual(222.10m, r32.Price);
        Assert.AreEqual(Match.BullSignal, r32.Match);

        CandleResult r33 = results[33];
        Assert.AreEqual(null, r33.Price);
        Assert.AreEqual(Match.None, r33.Match);

        CandleResult r34 = results[34];
        Assert.AreEqual(null, r34.Price);
        Assert.AreEqual(Match.None, r34.Match);

        CandleResult r274 = results[274];
        Assert.AreEqual(null, r274.Price);
        Assert.AreEqual(Match.None, r274.Match);

        CandleResult r277 = results[277];
        Assert.AreEqual(248.13m, r277.Price);
        Assert.AreEqual(Match.BearSignal, r277.Match);
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<CandleResult> r = badQuotes.GetMarubozu();
        Assert.AreEqual(502, r.Count());
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<CandleResult> r0 = noquotes.GetMarubozu();
        Assert.AreEqual(0, r0.Count());

        IEnumerable<CandleResult> r1 = onequote.GetMarubozu();
        Assert.AreEqual(1, r1.Count());
    }

    [TestMethod]
    public void Condense()
    {
        IEnumerable<CandleResult> r =
            quotes.GetMarubozu(95).Condense();

        Assert.AreEqual(6, r.Count());
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad minimum body percent values
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetMarubozu(quotes, 79.9));

        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetMarubozu(quotes, 100.1));
    }
}
