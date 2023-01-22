using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using Tests.Common;

namespace Tests.Indicators;

[TestClass]
public class PrsTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        int lookbackPeriods = 30;
        int smaPeriods = 10;

        List<PrsResult> results =
            otherQuotes.GetPrs(quotes, lookbackPeriods, smaPeriods)
            .ToList();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(502, results.Count(x => x.Prs != null));
        Assert.AreEqual(493, results.Count(x => x.PrsSma != null));

        // sample values
        PrsResult r1 = results[8];
        Assert.AreEqual(1.108340, r1.Prs.Round(6));
        Assert.AreEqual(null, r1.PrsSma);
        Assert.AreEqual(null, r1.PrsPercent);

        PrsResult r2 = results[249];
        Assert.AreEqual(1.222373, r2.Prs.Round(6));
        Assert.AreEqual(1.275808, r2.PrsSma.Round(6));
        Assert.AreEqual(-0.023089, r2.PrsPercent.Round(6));

        PrsResult r3 = results[501];
        Assert.AreEqual(1.356817, r3.Prs.Round(6));
        Assert.AreEqual(1.343445, r3.PrsSma.Round(6));
        Assert.AreEqual(0.037082, r3.PrsPercent.Round(6));
    }

    [TestMethod]
    public void UseTuple()
    {
        List<PrsResult> results = otherQuotes
            .Use(CandlePart.Close)
            .GetPrs(quotes.Use(CandlePart.Close), 20)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(502, results.Count(x => x.Prs != null));
    }

    [TestMethod]
    public void TupleNaN()
    {
        List<PrsResult> r = tupleNanny
            .GetPrs(tupleNanny, 6)
            .ToList();

        Assert.AreEqual(200, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Prs is double and double.NaN));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = otherQuotes
            .GetPrs(quotes, 20)
            .GetSma(10)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(493, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void Chainee()
    {
        List<PrsResult> results = quotes
            .GetSma(2)
            .GetPrs(otherQuotes.GetSma(2), 20)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(501, results.Count(x => x.Prs != null));
        Assert.AreEqual(0, results.Count(x => x.Prs is double and double.NaN));
    }

    [TestMethod]
    public void BadData()
    {
        List<PrsResult> r = badQuotes
            .GetPrs(badQuotes, 15, 4)
            .ToList();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Prs is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<PrsResult> r0 = noquotes
            .GetPrs(noquotes)
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<PrsResult> r1 = onequote
            .GetPrs(onequote)
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            otherQuotes.GetPrs(quotes, 0));

        // bad SMA period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            otherQuotes.GetPrs(quotes, 14, 0));

        // insufficient quotes
        Assert.ThrowsException<InvalidQuotesException>(() =>
            TestData.GetCompare(13).GetPrs(quotes, 14));

        // insufficient eval quotes
        Assert.ThrowsException<InvalidQuotesException>(() =>
            TestData.GetCompare(300).GetPrs(quotes, 14));

        // mismatch quotes
        Assert.ThrowsException<InvalidQuotesException>(() =>
            otherQuotes.GetPrs(mismatchQuotes, 14));
    }
}
