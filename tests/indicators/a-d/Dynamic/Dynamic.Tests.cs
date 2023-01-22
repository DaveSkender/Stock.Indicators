using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using Tests.Common;

namespace Tests.Indicators;

[TestClass]
public class McGinleyDynamicTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<DynamicResult> results = quotes
            .GetDynamic(14)
            .ToList();

        // assertions
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(501, results.Count(x => x.Dynamic != null));

        // sample values
        DynamicResult r1 = results[1];
        Assert.AreEqual(212.9465, r1.Dynamic.Round(4));

        DynamicResult r25 = results[25];
        Assert.AreEqual(215.4801, r25.Dynamic.Round(4));

        DynamicResult r250 = results[250];
        Assert.AreEqual(256.0554, r250.Dynamic.Round(4));

        DynamicResult r501 = results[501];
        Assert.AreEqual(245.7356, r501.Dynamic.Round(4));
    }

    [TestMethod]
    public void UseTuple()
    {
        List<DynamicResult> results = quotes
            .Use(CandlePart.Close)
            .GetDynamic(20)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(501, results.Count(x => x.Dynamic != null));
        Assert.AreEqual(0, results.Count(x => x.Dynamic is double and double.NaN));
    }

    [TestMethod]
    public void TupleNaN()
    {
        List<DynamicResult> r = tupleNanny
            .GetDynamic(6)
            .ToList();

        Assert.AreEqual(200, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Dynamic is double and double.NaN));
    }

    [TestMethod]
    public void Chainee()
    {
        List<DynamicResult> results = quotes
            .GetSma(10)
            .GetDynamic(14)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(492, results.Count(x => x.Dynamic != null));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = quotes
            .GetDynamic(14)
            .GetSma(10)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(492, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        List<DynamicResult> r = badQuotes
            .GetDynamic(15)
            .ToList();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Dynamic is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<DynamicResult> r0 = noquotes
            .GetDynamic(14)
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<DynamicResult> r1 = onequote
            .GetDynamic(14)
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => quotes.GetDynamic(0));

        // bad k-factor
        Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => quotes.GetDynamic(14, 0));
    }
}
