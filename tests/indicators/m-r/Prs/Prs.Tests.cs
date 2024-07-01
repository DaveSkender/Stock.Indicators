namespace Tests.Indicators.Series;

[TestClass]
public class PrsTests : SeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        int lookbackPeriods = 30;

        List<PrsResult> results = OtherQuotes
            .GetPrs(Quotes, lookbackPeriods)
            .ToList();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(502, results.Count(x => x.Prs != null));

        // sample values
        PrsResult r1 = results[8];
        Assert.AreEqual(1.108340, r1.Prs.Round(6));
        Assert.AreEqual(null, r1.PrsPercent);

        PrsResult r2 = results[249];
        Assert.AreEqual(1.222373, r2.Prs.Round(6));
        Assert.AreEqual(-0.023089, r2.PrsPercent.Round(6));

        PrsResult r3 = results[501];
        Assert.AreEqual(1.356817, r3.Prs.Round(6));
        Assert.AreEqual(0.037082, r3.PrsPercent.Round(6));
    }

    [TestMethod]
    public void UseReusable()
    {
        List<PrsResult> results = OtherQuotes
            .Use(CandlePart.Close)
            .GetPrs(Quotes.Use(CandlePart.Close), 20)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(502, results.Count(x => x.Prs != null));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = OtherQuotes
            .GetPrs(Quotes, 20)
            .GetSma(10)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(493, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void Chainee()
    {
        List<PrsResult> results = Quotes
            .GetSma(2)
            .GetPrs(OtherQuotes.GetSma(2), 20)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(501, results.Count(x => x.Prs != null));
        Assert.AreEqual(0, results.Count(x => x.Prs is double.NaN));
    }

    [TestMethod]
    public override void BadData()
    {
        List<PrsResult> r = BadQuotes
            .GetPrs(BadQuotes, 15)
            .ToList();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Prs is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        List<PrsResult> r0 = Noquotes
            .GetPrs(Noquotes)
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<PrsResult> r1 = Onequote
            .GetPrs(Onequote)
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            OtherQuotes.GetPrs(Quotes, 0));

        // insufficient quotes
        Assert.ThrowsException<InvalidQuotesException>(() =>
            TestData.GetCompare(13).GetPrs(Quotes, 14));

        // insufficient eval quotes
        Assert.ThrowsException<InvalidQuotesException>(() =>
            TestData.GetCompare(300).GetPrs(Quotes, 14));

        // mismatch quotes
        Assert.ThrowsException<InvalidQuotesException>(() =>
            OtherQuotes.GetPrs(MismatchQuotes, 14));
    }
}
