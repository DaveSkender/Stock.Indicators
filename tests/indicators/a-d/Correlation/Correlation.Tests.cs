using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class Correlation : TestBase
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
        Assert.AreEqual(0.6933, NullMath.Round(r19.Correlation, 4));
        Assert.AreEqual(0.4806, NullMath.Round(r19.RSquared, 4));

        CorrResult r257 = results[257];
        Assert.AreEqual(-0.1347, NullMath.Round(r257.Correlation, 4));
        Assert.AreEqual(0.0181, NullMath.Round(r257.RSquared, 4));

        CorrResult r501 = results[501];
        Assert.AreEqual(0.8460, NullMath.Round(r501.Correlation, 4));
        Assert.AreEqual(0.7157, NullMath.Round(r501.RSquared, 4));
    }

    [TestMethod]
    public void UseTuple()
    {
        IEnumerable<CorrResult> results = quotes
            .Use(CandlePart.Close)
            .GetCorrelation(otherQuotes.Use(CandlePart.Close), 20);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(483, results.Count(x => x.Correlation != null));
    }

    [TestMethod]
    public void TupleNaN()
    {
        IEnumerable<CorrResult> r = tupleNanny.GetCorrelation(tupleNanny, 6);

        Assert.AreEqual(200, r.Count());
        Assert.AreEqual(0, r.Count(x => x.Correlation is double and double.NaN));
    }

    [TestMethod]
    public void Chainor()
    {
        IEnumerable<SmaResult> results = quotes
            .GetCorrelation(otherQuotes, 20)
            .GetSma(10);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(474, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<CorrResult> r = Indicator.GetCorrelation(badQuotes, badQuotes, 15);
        Assert.AreEqual(502, r.Count());
        Assert.AreEqual(0, r.Count(x => x.Correlation is double and double.NaN));
    }

    [TestMethod]
    public void BigData()
    {
        IEnumerable<CorrResult> r = Indicator.GetCorrelation(bigQuotes, bigQuotes, 150);
        Assert.AreEqual(1246, r.Count());
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<CorrResult> r0 = noquotes.GetCorrelation(noquotes, 10);
        Assert.AreEqual(0, r0.Count());

        IEnumerable<CorrResult> r1 = onequote.GetCorrelation(onequote, 10);
        Assert.AreEqual(1, r1.Count());
    }

    [TestMethod]
    public void Removed()
    {
        List<CorrResult> results =
            quotes.GetCorrelation(otherQuotes, 20)
                .RemoveWarmupPeriods()
                .ToList();

        // assertions
        Assert.AreEqual(502 - 19, results.Count);

        CorrResult last = results.LastOrDefault();
        Assert.AreEqual(0.8460, NullMath.Round(last.Correlation, 4));
        Assert.AreEqual(0.7157, NullMath.Round(last.RSquared, 4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetCorrelation(quotes, otherQuotes, 0));

        // bad eval quotes
        IEnumerable<Quote> eval = TestData.GetCompare(300);
        Assert.ThrowsException<InvalidQuotesException>(() =>
            Indicator.GetCorrelation(quotes, eval, 30));

        // mismatched quotes
        Assert.ThrowsException<InvalidQuotesException>(() =>
            Indicator.GetCorrelation(mismatchQuotes, otherQuotes, 20));
    }
}
