using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using Tests.Common;

namespace Tests.Indicators;

[TestClass]
public class ObvTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<ObvResult> results = quotes
            .GetObv()
            .ToList();

        // proper quantities
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
        List<ObvResult> results = quotes
            .GetObv(20)
            .ToList();

        // proper quantities
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
        List<SmaResult> results = quotes
            .GetObv()
            .GetSma(10)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(493, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        List<ObvResult> r = badQuotes
            .GetObv()
            .ToList();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => double.IsNaN(x.Obv)));
    }

    [TestMethod]
    public void BigData()
    {
        List<ObvResult> r = bigQuotes
            .GetObv()
            .ToList();

        Assert.AreEqual(1246, r.Count);
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<ObvResult> r0 = noquotes
            .GetObv()
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<ObvResult> r1 = onequote
            .GetObv()
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }

    // bad SMA period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => quotes.GetObv(0));
}
