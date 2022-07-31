using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class Obv : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<ObvResult> results = quotes.GetObv().ToList();

        // assertions

        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(502, results.Count(x => x.ObvSma == null));

        // sample values
        ObvResult r1 = results[249];
        Assert.AreEqual(1780918888, r1.Obv);
        Assert.AreEqual(null, r1.ObvSma);

        ObvResult r2 = results[501];
        Assert.AreEqual(539843504, r2.Obv);
        Assert.AreEqual(null, r2.ObvSma);
    }

    [TestMethod]
    public void WithSma()
    {
        List<ObvResult> results = Indicator.GetObv(quotes, 20).ToList();

        // assertions

        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(482, results.Count(x => x.ObvSma != null));

        // sample values
        ObvResult r1 = results[501];
        Assert.AreEqual(539843504, r1.Obv);
        Assert.AreEqual(1016208844.40, r1.ObvSma);
    }

    [TestMethod]
    public void Chainor()
    {
        IEnumerable<SmaResult> results = quotes
            .GetObv()
            .GetSma(10);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(493, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<ObvResult> r = badQuotes.GetObv();
        Assert.AreEqual(502, r.Count());
        Assert.AreEqual(0, r.Count(x => double.IsNaN(x.Obv)));
    }

    [TestMethod]
    public void BigData()
    {
        IEnumerable<ObvResult> r = bigQuotes.GetObv();
        Assert.AreEqual(1246, r.Count());
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<ObvResult> r0 = noquotes.GetObv();
        Assert.AreEqual(0, r0.Count());

        IEnumerable<ObvResult> r1 = onequote.GetObv();
        Assert.AreEqual(1, r1.Count());
    }

    // bad SMA period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => Indicator.GetObv(quotes, 0));
}
