using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using Tests.Common;

namespace Tests.Indicators;

[TestClass]
public class WmaTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<WmaResult> results = quotes
            .GetWma(20)
            .ToList();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.Wma != null));

        // sample values
        WmaResult r1 = results[149];
        Assert.AreEqual(235.5253, r1.Wma.Round(4));

        WmaResult r2 = results[501];
        Assert.AreEqual(246.5110, r2.Wma.Round(4));
    }

    [TestMethod]
    public void UseTuple()
    {
        List<WmaResult> results = quotes
            .Use(CandlePart.Close)
            .GetWma(20)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.Wma != null));
    }

    [TestMethod]
    public void TupleNaN()
    {
        List<WmaResult> r = tupleNanny
            .GetWma(6)
            .ToList();

        Assert.AreEqual(200, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Wma is double and double.NaN));
    }

    [TestMethod]
    public void Chainee()
    {
        List<WmaResult> results = quotes
            .GetSma(2)
            .GetWma(20)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(482, results.Count(x => x.Wma != null));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = quotes
            .GetWma(20)
            .GetSma(10)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(474, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void Chaining()
    {
        List<WmaResult> standard = quotes
            .GetWma(17)
            .ToList();

        List<WmaResult> results = quotes
            .Use(CandlePart.Close)
            .GetWma(17)
            .ToList();

        // assertions
        for (int i = 0; i < results.Count; i++)
        {
            WmaResult s = standard[i];
            WmaResult c = results[i];

            Assert.AreEqual(s.Date, c.Date);
            Assert.AreEqual(s.Wma, c.Wma);
        }
    }

    [TestMethod]
    public void BadData()
    {
        List<WmaResult> r = badQuotes
            .GetWma(15)
            .ToList();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Wma is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<WmaResult> r0 = noquotes
            .GetWma(5)
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<WmaResult> r1 = onequote
            .GetWma(5)
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        List<WmaResult> results = quotes
            .GetWma(20)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - 19, results.Count);

        WmaResult last = results.LastOrDefault();
        Assert.AreEqual(246.5110, last.Wma.Round(4));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => quotes.GetWma(0));
}
