using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using Tests.Common;

namespace Tests.Indicators;

[TestClass]
public class T3Tests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<T3Result> results = quotes
            .GetT3(5, 0.7)
            .ToList();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(502, results.Count(x => x.T3 != null));

        // sample values
        T3Result r5 = results[5];
        Assert.AreEqual(213.9654, r5.T3.Round(4));

        T3Result r24 = results[24];
        Assert.AreEqual(215.9481, r24.T3.Round(4));

        T3Result r44 = results[44];
        Assert.AreEqual(224.9412, r44.T3.Round(4));

        T3Result r149 = results[149];
        Assert.AreEqual(235.8851, r149.T3.Round(4));

        T3Result r249 = results[249];
        Assert.AreEqual(257.8735, r249.T3.Round(4));

        T3Result r501 = results[501];
        Assert.AreEqual(238.9308, r501.T3.Round(4));
    }

    [TestMethod]
    public void UseTuple()
    {
        List<T3Result> results = quotes
            .Use(CandlePart.Close)
            .GetT3()
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(502, results.Count(x => x.T3 != null));
    }

    [TestMethod]
    public void TupleNaN()
    {
        List<T3Result> r = tupleNanny
            .GetT3()
            .ToList();

        Assert.AreEqual(200, r.Count);
        Assert.AreEqual(0, r.Count(x => x.T3 is double and double.NaN));
    }

    [TestMethod]
    public void Chainee()
    {
        List<T3Result> results = quotes
            .GetSma(2)
            .GetT3()
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(501, results.Count(x => x.T3 != null));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = quotes
            .GetT3()
            .GetSma(10)
            .ToList();

        Assert.AreEqual(502, results.Count);
    }

    [TestMethod]
    public void BadData()
    {
        List<T3Result> r = badQuotes
            .GetT3()
            .ToList();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.T3 is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<T3Result> r0 = noquotes
            .GetT3()
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<T3Result> r1 = onequote
            .GetT3()
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetT3(0));

        // bad volume factor
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetT3(25, 0));
    }
}
