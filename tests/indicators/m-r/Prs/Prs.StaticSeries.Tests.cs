namespace StaticSeries;

[TestClass]
public class Prs : StaticSeriesTestBase
{
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        const int lookbackPeriods = 30;

        IReadOnlyList<PrsResult> results = OtherQuotes
            .ToPrs(Quotes, lookbackPeriods);

        // proper quantities
        Assert.HasCount(502, results);
        Assert.HasCount(502, results.Where(static x => x.Prs != null));

        // sample values
        PrsResult r1 = results[8];
        Assert.AreEqual(1.108340, r1.Prs.Round(6));
        Assert.IsNull(r1.PrsPercent);

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

        Assert.HasCount(502, results);
        Assert.HasCount(502, results.Where(static x => x.Prs != null));
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<SmaResult> results = OtherQuotes
            .ToPrs(Quotes, 20)
            .ToSma(10);

        Assert.HasCount(502, results);
        Assert.HasCount(493, results.Where(static x => x.Sma != null));
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<PrsResult> results = Quotes
            .ToSma(2)
            .ToPrs(OtherQuotes.ToSma(2), 20);

        Assert.HasCount(502, results);
        Assert.HasCount(501, results.Where(static x => x.Prs != null));
        Assert.IsEmpty(results.Where(static x => x.Prs is double.NaN));
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<PrsResult> r = BadQuotes
            .ToPrs(BadQuotes, 15);

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(static x => x.Prs is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<PrsResult> r0 = Noquotes
            .ToPrs(Noquotes);

        Assert.IsEmpty(r0);

        IReadOnlyList<PrsResult> r1 = Onequote
            .ToPrs(Onequote);

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => OtherQuotes.ToPrs(Quotes, 0));

        // insufficient quotes
        Assert.ThrowsExactly<InvalidQuotesException>(
            static () => Data.GetCompare(13).ToPrs(Quotes, 14));

        // insufficient eval quotes
        Assert.ThrowsExactly<InvalidQuotesException>(
            static () => Data.GetCompare(300).ToPrs(Quotes, 14));

        // mismatch quotes
        Assert.ThrowsExactly<InvalidQuotesException>(
            static () => OtherQuotes.ToPrs(MismatchQuotes, 14));
    }
}
