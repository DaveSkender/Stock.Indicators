using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class Epma : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<EpmaResult> results = quotes.GetEpma(20).ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.Epma != null));

        // sample values
        EpmaResult r1 = results[18];
        Assert.IsNull(r1.Epma);

        EpmaResult r2 = results[19];
        Assert.AreEqual(215.6189, NullMath.Round(r2.Epma, 4));

        EpmaResult r3 = results[149];
        Assert.AreEqual(236.7060, NullMath.Round(r3.Epma, 4));

        EpmaResult r4 = results[249];
        Assert.AreEqual(258.5179, NullMath.Round(r4.Epma, 4));

        EpmaResult r5 = results[501];
        Assert.AreEqual(235.8131, NullMath.Round(r5.Epma, 4));
    }

    [TestMethod]
    public void UseTuple()
    {
        IEnumerable<EpmaResult> results = quotes
            .Use(CandlePart.Close)
            .GetEpma(20);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(483, results.Count(x => x.Epma != null));
    }

    [TestMethod]
    public void TupleNaN()
    {
        IEnumerable<EpmaResult> r = tupleNanny.GetEpma(6);

        Assert.AreEqual(200, r.Count());
        Assert.AreEqual(0, r.Count(x => x.Epma is double and double.NaN));
    }

    [TestMethod]
    public void Chainee()
    {
        IEnumerable<EpmaResult> results = quotes
            .GetSma(2)
            .GetEpma(20);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(482, results.Count(x => x.Epma != null));
    }

    [TestMethod]
    public void Chainor()
    {
        IEnumerable<SmaResult> results = quotes
            .GetEpma(20)
            .GetSma(10);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(474, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<EpmaResult> r = Indicator.GetEpma(badQuotes, 15);
        Assert.AreEqual(502, r.Count());
        Assert.AreEqual(0, r.Count(x => x.Epma is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<EpmaResult> r0 = noquotes.GetEpma(5);
        Assert.AreEqual(0, r0.Count());

        IEnumerable<EpmaResult> r1 = onequote.GetEpma(5);
        Assert.AreEqual(1, r1.Count());
    }

    [TestMethod]
    public void Removed()
    {
        List<EpmaResult> results = quotes.GetEpma(20)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - 19, results.Count);

        EpmaResult last = results.LastOrDefault();
        Assert.AreEqual(235.8131, NullMath.Round(last.Epma, 4));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => Indicator.GetEpma(quotes, 0));
}
