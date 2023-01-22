using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using Tests.Common;

namespace Tests.Indicators;

[TestClass]
public class CorrelationTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<CorrResult> results =
            quotes.GetCorrelation(otherQuotes, 20)
            .ToList();

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.Correlation != null));

        // sample values
        CorrResult r18 = results[18];
        Assert.IsNull(r18.Correlation);
        Assert.IsNull(r18.RSquared);

        CorrResult r19 = results[19];
        Assert.AreEqual(0.6933, r19.Correlation.Round(4));
        Assert.AreEqual(0.4806, r19.RSquared.Round(4));

        CorrResult r257 = results[257];
        Assert.AreEqual(-0.1347, r257.Correlation.Round(4));
        Assert.AreEqual(0.0181, r257.RSquared.Round(4));

        CorrResult r501 = results[501];
        Assert.AreEqual(0.8460, r501.Correlation.Round(4));
        Assert.AreEqual(0.7157, r501.RSquared.Round(4));
    }

    [TestMethod]
    public void UseTuple()
    {
        List<CorrResult> results = quotes
            .Use(CandlePart.Close)
            .GetCorrelation(otherQuotes.Use(CandlePart.Close), 20)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.Correlation != null));
    }

    [TestMethod]
    public void TupleNaN()
    {
        List<CorrResult> r = tupleNanny
            .GetCorrelation(tupleNanny, 6)
            .ToList();

        Assert.AreEqual(200, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Correlation is double and double.NaN));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = quotes
            .GetCorrelation(otherQuotes, 20)
            .GetSma(10)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(474, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void Chainee()
    {
        List<CorrResult> results = quotes
            .GetSma(2)
            .GetCorrelation(otherQuotes.GetSma(2), 20)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(482, results.Count(x => x.Correlation != null));
        Assert.AreEqual(0, results.Count(x => x.Correlation is double and double.NaN));
    }

    [TestMethod]
    public void BadData()
    {
        List<CorrResult> r = badQuotes
            .GetCorrelation(badQuotes, 15)
            .ToList();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Correlation is double and double.NaN));
    }

    [TestMethod]
    public void BigData()
    {
        List<CorrResult> r = bigQuotes
            .GetCorrelation(bigQuotes, 150)
            .ToList();

        Assert.AreEqual(1246, r.Count);
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<CorrResult> r0 = noquotes
            .GetCorrelation(noquotes, 10)
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<CorrResult> r1 = onequote
            .GetCorrelation(onequote, 10)
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        List<CorrResult> results = quotes
            .GetCorrelation(otherQuotes, 20)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - 19, results.Count);

        CorrResult last = results.LastOrDefault();
        Assert.AreEqual(0.8460, last.Correlation.Round(4));
        Assert.AreEqual(0.7157, last.RSquared.Round(4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetCorrelation(otherQuotes, 0));

        // bad eval quotes
        IEnumerable<Quote> eval = TestData.GetCompare(300);
        Assert.ThrowsException<InvalidQuotesException>(() =>
            quotes.GetCorrelation(eval, 30));

        // mismatched quotes
        Assert.ThrowsException<InvalidQuotesException>(() =>
            mismatchQuotes.GetCorrelation(otherQuotes, 20));
    }
}
