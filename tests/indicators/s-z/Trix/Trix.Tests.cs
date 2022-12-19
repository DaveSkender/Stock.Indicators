using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class Trix : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<TrixResult> results = quotes
            .GetTrix(20, 5)
            .ToList();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(482, results.Count(x => x.Ema3 != null));
        Assert.AreEqual(482, results.Count(x => x.Trix != null));
        Assert.AreEqual(478, results.Count(x => x.Signal != null));

        // sample values
        TrixResult r24 = results[24];
        Assert.AreEqual(214.5486, NullMath.Round(r24.Ema3, 4));
        Assert.AreEqual(0.005047, NullMath.Round(r24.Trix, 6));
        Assert.AreEqual(0.002196, NullMath.Round(r24.Signal, 6));

        TrixResult r67 = results[67];
        Assert.AreEqual(221.7837, NullMath.Round(r67.Ema3, 4));
        Assert.AreEqual(0.050030, NullMath.Round(r67.Trix, 6));
        Assert.AreEqual(0.057064, NullMath.Round(r67.Signal, 6));

        TrixResult r249 = results[249];
        Assert.AreEqual(249.4469, NullMath.Round(r249.Ema3, 4));
        Assert.AreEqual(0.121781, NullMath.Round(r249.Trix, 6));
        Assert.AreEqual(0.119769, NullMath.Round(r249.Signal, 6));

        TrixResult r501 = results[501];
        Assert.AreEqual(263.3216, NullMath.Round(r501.Ema3, 4));
        Assert.AreEqual(-0.230742, NullMath.Round(r501.Trix, 6));
        Assert.AreEqual(-0.204536, NullMath.Round(r501.Signal, 6));
    }

    [TestMethod]
    public void UseTuple()
    {
        List<TrixResult> results = quotes
            .Use(CandlePart.Close)
            .GetTrix(20, 5)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(482, results.Count(x => x.Trix != null));
    }

    [TestMethod]
    public void TupleNaN()
    {
        List<TrixResult> r = tupleNanny
            .GetTrix(6, 2)
            .ToList();

        Assert.AreEqual(200, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Trix is double and double.NaN));
    }

    [TestMethod]
    public void Chainee()
    {
        List<TrixResult> results = quotes
            .GetSma(2)
            .GetTrix(20, 5)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(481, results.Count(x => x.Trix != null));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = quotes
            .GetTrix(20, 5)
            .GetSma(10)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(473, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        List<TrixResult> r = badQuotes
            .GetTrix(15, 2)
            .ToList();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Trix is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<TrixResult> r0 = noquotes
            .GetTrix(5)
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<TrixResult> r1 = onequote
            .GetTrix(5)
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        List<TrixResult> results = quotes
            .GetTrix(20, 5)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - ((3 * 20) + 100), results.Count);

        TrixResult last = results.LastOrDefault();
        Assert.AreEqual(263.3216, NullMath.Round(last.Ema3, 4));
        Assert.AreEqual(-0.230742, NullMath.Round(last.Trix, 6));
        Assert.AreEqual(-0.204536, NullMath.Round(last.Signal, 6));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => quotes.GetTrix(0));
}
