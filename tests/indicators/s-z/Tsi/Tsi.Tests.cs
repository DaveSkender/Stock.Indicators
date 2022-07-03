using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class Tsi : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<TsiResult> results = quotes.GetTsi(25, 13, 7).ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(465, results.Count(x => x.Tsi != null));
        Assert.AreEqual(459, results.Count(x => x.Signal != null));

        // sample values
        TsiResult r2 = results[37];
        Assert.AreEqual(53.1204, NullMath.Round(r2.Tsi, 4));
        Assert.AreEqual(null, r2.Signal);

        TsiResult r3a = results[43];
        Assert.AreEqual(46.0960, NullMath.Round(r3a.Tsi, 4));
        Assert.AreEqual(51.6916, NullMath.Round(r3a.Signal, 4));

        TsiResult r3b = results[44];
        Assert.AreEqual(42.5121, NullMath.Round(r3b.Tsi, 4));
        Assert.AreEqual(49.3967, NullMath.Round(r3b.Signal, 4));

        TsiResult r4 = results[149];
        Assert.AreEqual(29.0936, NullMath.Round(r4.Tsi, 4));
        Assert.AreEqual(28.0134, NullMath.Round(r4.Signal, 4));

        TsiResult r5 = results[249];
        Assert.AreEqual(41.9232, NullMath.Round(r5.Tsi, 4));
        Assert.AreEqual(42.4063, NullMath.Round(r5.Signal, 4));

        TsiResult r6 = results[501];
        Assert.AreEqual(-28.3513, NullMath.Round(r6.Tsi, 4));
        Assert.AreEqual(-29.3597, NullMath.Round(r6.Signal, 4));
    }

    [TestMethod]
    public void UseTuple()
    {
        IEnumerable<TsiResult> results = quotes
            .Use(CandlePart.Close)
            .GetTsi();

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(465, results.Count(x => x.Tsi != null));
    }

    [TestMethod]
    public void TupleNaN()
    {
        IEnumerable<TsiResult> r = tupleNanny.GetTsi();

        Assert.AreEqual(200, r.Count());
        Assert.AreEqual(0, r.Count(x => x.Tsi is double and double.NaN));
    }

    [TestMethod]
    public void Chainee()
    {
        IEnumerable<TsiResult> results = quotes
            .GetSma(2)
            .GetTsi();

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(464, results.Count(x => x.Tsi != null));
    }

    [TestMethod]
    public void Chainor()
    {
        IEnumerable<SmaResult> results = quotes
            .GetTsi()
            .GetSma(10);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(456, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<TsiResult> r = Indicator.GetTsi(badQuotes);
        Assert.AreEqual(502, r.Count());
        Assert.AreEqual(0, r.Count(x => x.Tsi is double and double.NaN));
    }

    [TestMethod]
    public void BigData()
    {
        IEnumerable<TsiResult> r = Indicator.GetTsi(bigQuotes);
        Assert.AreEqual(1246, r.Count());
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<TsiResult> r0 = noquotes.GetTsi();
        Assert.AreEqual(0, r0.Count());

        IEnumerable<TsiResult> r1 = onequote.GetTsi();
        Assert.AreEqual(1, r1.Count());
    }

    [TestMethod]
    public void Removed()
    {
        List<TsiResult> results = quotes.GetTsi(25, 13, 7)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - (25 + 13 + 250), results.Count);

        TsiResult last = results.LastOrDefault();
        Assert.AreEqual(-28.3513, NullMath.Round(last.Tsi, 4));
        Assert.AreEqual(-29.3597, NullMath.Round(last.Signal, 4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetTsi(quotes, 0));

        // bad smoothing period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetTsi(quotes, 25, 0));

        // bad signal period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetTsi(quotes, 25, 13, -1));
    }
}
