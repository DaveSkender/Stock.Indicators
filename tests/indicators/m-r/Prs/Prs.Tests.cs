using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class Prs : TestBase
{
    [TestMethod]
    public void Standard()
    {
        int lookbackPeriods = 30;
        int smaPeriods = 10;

        List<PrsResult> results =
            otherQuotes.GetPrs(quotes, lookbackPeriods, smaPeriods)
            .ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(502, results.Count(x => x.Prs != null));
        Assert.AreEqual(493, results.Count(x => x.PrsSma != null));

        // sample values
        PrsResult r1 = results[8];
        Assert.AreEqual(1.108340, NullMath.Round(r1.Prs, 6));
        Assert.AreEqual(null, r1.PrsSma);
        Assert.AreEqual(null, r1.PrsPercent);

        PrsResult r2 = results[249];
        Assert.AreEqual(1.222373, NullMath.Round(r2.Prs, 6));
        Assert.AreEqual(1.275808, NullMath.Round(r2.PrsSma, 6));
        Assert.AreEqual(-0.023089, NullMath.Round(r2.PrsPercent, 6));

        PrsResult r3 = results[501];
        Assert.AreEqual(1.356817, NullMath.Round(r3.Prs, 6));
        Assert.AreEqual(1.343445, NullMath.Round(r3.PrsSma, 6));
        Assert.AreEqual(0.037082, NullMath.Round(r3.PrsPercent, 6));
    }

    [TestMethod]
    public void UseTuple()
    {
        IEnumerable<PrsResult> results = otherQuotes
            .Use(CandlePart.Close)
            .GetPrs(quotes.Use(CandlePart.Close), 20);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(502, results.Count(x => x.Prs != null));
    }

    [TestMethod]
    public void TupleNaN()
    {
        IEnumerable<PrsResult> r = tupleNanny.GetPrs(tupleNanny, 6);

        Assert.AreEqual(200, r.Count());
        Assert.AreEqual(0, r.Count(x => x.Prs is double and double.NaN));
    }

    [TestMethod]
    public void Chainor()
    {
        IEnumerable<SmaResult> results = otherQuotes
            .GetPrs(quotes, 20)
            .GetSma(10);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(493, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<PrsResult> r = Indicator.GetPrs(badQuotes, badQuotes, 15, 4);
        Assert.AreEqual(502, r.Count());
        Assert.AreEqual(0, r.Count(x => x.Prs is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<PrsResult> r0 = noquotes.GetPrs(noquotes);
        Assert.AreEqual(0, r0.Count());

        IEnumerable<PrsResult> r1 = onequote.GetPrs(onequote);
        Assert.AreEqual(1, r1.Count());
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetPrs(otherQuotes, quotes, 0));

        // bad SMA period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetPrs(otherQuotes, quotes, 14, 0));

        // insufficient quotes
        Assert.ThrowsException<InvalidQuotesException>(() =>
            Indicator.GetPrs(TestData.GetCompare(13), quotes, 14));

        // insufficient eval quotes
        Assert.ThrowsException<InvalidQuotesException>(() =>
            Indicator.GetPrs(TestData.GetCompare(300), quotes, 14));

        // mismatch quotes
        Assert.ThrowsException<InvalidQuotesException>(() =>
            Indicator.GetPrs(otherQuotes, mismatchQuotes, 14));
    }
}
