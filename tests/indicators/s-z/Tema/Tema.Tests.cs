using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using Tests.Common;

namespace Tests.Indicators;

[TestClass]
public class TemaTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<TemaResult> results = quotes
            .GetTema(20)
            .ToList();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.Tema != null));

        // sample values
        TemaResult r25 = results[25];
        Assert.AreEqual(216.1441, r25.Tema.Round(4));

        TemaResult r67 = results[67];
        Assert.AreEqual(222.9562, r67.Tema.Round(4));

        TemaResult r249 = results[249];
        Assert.AreEqual(258.6208, r249.Tema.Round(4));

        TemaResult r501 = results[501];
        Assert.AreEqual(238.7690, r501.Tema.Round(4));
    }

    [TestMethod]
    public void UseTuple()
    {
        List<TemaResult> results = quotes
            .Use(CandlePart.Close)
            .GetTema(20)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.Tema != null));
    }

    [TestMethod]
    public void TupleNaN()
    {
        List<TemaResult> r = tupleNanny
            .GetTema(6)
            .ToList();

        Assert.AreEqual(200, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Tema is double and double.NaN));
    }

    [TestMethod]
    public void Chainee()
    {
        List<TemaResult> results = quotes
            .GetSma(2)
            .GetTema(20)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(482, results.Count(x => x.Tema != null));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = quotes
            .GetTema(20)
            .GetSma(10)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(474, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        List<TemaResult> r = badQuotes
            .GetTema(15)
            .ToList();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Tema is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<TemaResult> r0 = noquotes
            .GetTema(5)
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<TemaResult> r1 = onequote
            .GetTema(5)
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        List<TemaResult> results = quotes
            .GetTema(20)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - ((3 * 20) + 100), results.Count);

        TemaResult last = results.LastOrDefault();
        Assert.AreEqual(238.7690, last.Tema.Round(4));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => quotes.GetTema(0));
}
