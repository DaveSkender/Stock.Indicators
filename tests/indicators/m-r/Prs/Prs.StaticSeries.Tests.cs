namespace StaticSeries;

[TestClass]
public class Prs : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        int lookbackPeriods = 30;

        IReadOnlyList<PrsResult> results = OtherQuotes
            .ToPrs(Quotes, lookbackPeriods);

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
        IReadOnlyList<PrsResult> results = OtherQuotes
            .Use(CandlePart.Close)
            .ToPrs(Quotes.Use(CandlePart.Close), 20);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(502, results.Count(x => x.Prs != null));
    }

    [TestMethod]
    public void Chainor()
    {
        IReadOnlyList<SmaResult> results = OtherQuotes
            .ToPrs(Quotes, 20)
            .ToSma(10);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(493, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<PrsResult> results = Quotes
            .ToSma(2)
            .ToPrs(OtherQuotes.ToSma(2), 20);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(501, results.Count(x => x.Prs != null));
        Assert.AreEqual(0, results.Count(x => x.Prs is double.NaN));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<PrsResult> r = BadQuotes
            .ToPrs(BadQuotes, 15);

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Prs is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<PrsResult> r0 = Noquotes
            .ToPrs(Noquotes);

        Assert.AreEqual(0, r0.Count);

        IReadOnlyList<PrsResult> r1 = Onequote
            .ToPrs(Onequote);

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsException<ArgumentOutOfRangeException>(
            () => OtherQuotes.ToPrs(Quotes, 0));

        // insufficient quotes
        Assert.ThrowsException<InvalidQuotesException>(
            () => Data.GetCompare(13).ToPrs(Quotes, 14));

        // insufficient eval quotes
        Assert.ThrowsException<InvalidQuotesException>(
            () => Data.GetCompare(300).ToPrs(Quotes, 14));

        // mismatch quotes
        Assert.ThrowsException<InvalidQuotesException>(
            () => OtherQuotes.ToPrs(MismatchQuotes, 14));
    }
}
