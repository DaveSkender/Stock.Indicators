using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class Cmo : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<CmoResult> results = quotes
            .GetCmo(14)
            .ToList();

        foreach (CmoResult r in results)
        {
            Console.WriteLine($"{r.Date:d},{r.Cmo:N4}");
        }

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(488, results.Count(x => x.Cmo != null));

        // sample values
        CmoResult r13 = results[13];
        Assert.IsNull(r13.Cmo);

        CmoResult r14 = results[14];
        Assert.AreEqual(24.1081, NullMath.Round(r14.Cmo, 4));

        CmoResult r249 = results[249];
        Assert.AreEqual(48.9614, NullMath.Round(r249.Cmo, 4));

        CmoResult r501 = results[501];
        Assert.AreEqual(-26.7502, NullMath.Round(r501.Cmo, 4));
    }

    [TestMethod]
    public void UseTuple()
    {
        IEnumerable<CmoResult> results = quotes
            .Use(CandlePart.Close)
            .GetCmo(14);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(488, results.Count(x => x.Cmo != null));
    }

    [TestMethod]
    public void TupleNaN()
    {
        IEnumerable<CmoResult> r = tupleNanny.GetCmo(6);

        Assert.AreEqual(200, r.Count());
        Assert.AreEqual(0, r.Count(x => x.Cmo is double and double.NaN));
    }

    [TestMethod]
    public void Chainee()
    {
        IEnumerable<CmoResult> results = quotes
            .GetSma(2)
            .GetCmo(20);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(481, results.Count(x => x.Cmo != null));
    }

    [TestMethod]
    public void Chainor()
    {
        IEnumerable<SmaResult> results = quotes
            .GetCmo(20)
            .GetSma(10);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(473, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<CmoResult> r = badQuotes.GetCmo(35);
        Assert.AreEqual(502, r.Count());
        Assert.AreEqual(0, r.Count(x => x.Cmo is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<CmoResult> r0 = noquotes.GetCmo(5);
        Assert.AreEqual(0, r0.Count());

        IEnumerable<CmoResult> r1 = onequote.GetCmo(5);
        Assert.AreEqual(1, r1.Count());
    }

    [TestMethod]
    public void Removed()
    {
        List<CmoResult> results = quotes.GetCmo(14)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(488, results.Count);

        CmoResult last = results.LastOrDefault();
        Assert.AreEqual(-26.7502, NullMath.Round(last.Cmo, 4));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => quotes.GetCmo(0));
}
