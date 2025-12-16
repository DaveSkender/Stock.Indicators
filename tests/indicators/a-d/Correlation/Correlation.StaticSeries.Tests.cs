namespace StaticSeries;

[TestClass]
public class Correlation : StaticSeriesTestBase
{
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        IReadOnlyList<CorrResult> results = Quotes
            .ToCorrelation(OtherQuotes, 20);

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.HasCount(502, results);
        Assert.HasCount(483, results.Where(static x => x.Correlation != null));

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
    public void Results_AreAlwaysBounded()
    {
        IReadOnlyList<CorrResult> results = Quotes
            .ToCorrelation(OtherQuotes, 20);

        TestAsserts.AlwaysBounded(results, x => x.Correlation, -1, 1);
        TestAsserts.AlwaysBounded(results, x => x.RSquared, 0, 1);
    }

    [TestMethod]
    public void UseReusable()
    {
        IReadOnlyList<CorrResult> results = Quotes
            .Use(CandlePart.Close)
            .ToCorrelation(OtherQuotes.Use(CandlePart.Close), 20);

        Assert.HasCount(502, results);
        Assert.HasCount(483, results.Where(static x => x.Correlation != null));
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToCorrelation(OtherQuotes, 20)
            .ToSma(10);

        Assert.HasCount(502, results);
        Assert.HasCount(474, results.Where(static x => x.Sma != null));
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<CorrResult> results = Quotes
            .ToSma(2)
            .ToCorrelation(OtherQuotes.ToSma(2), 20);

        Assert.HasCount(502, results);
        Assert.HasCount(482, results.Where(static x => x.Correlation != null));
        Assert.IsEmpty(results.Where(static x => x.Correlation is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<CorrResult> r = BadQuotes
            .ToCorrelation(BadQuotes, 15);

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(static x => x.Correlation is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public void BigQuoteValues_DoesNotFail()
    {
        IReadOnlyList<CorrResult> r = BigQuotes
            .ToCorrelation(BigQuotes, 150);

        Assert.HasCount(1246, r);
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<CorrResult> r0 = Noquotes
            .ToCorrelation(Noquotes, 10);

        Assert.IsEmpty(r0);

        IReadOnlyList<CorrResult> r1 = Onequote
            .ToCorrelation(Onequote, 10);

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<CorrResult> results = Quotes
            .ToCorrelation(OtherQuotes, 20)
            .RemoveWarmupPeriods();

        // assertions
        Assert.HasCount(502 - 19, results);

        CorrResult last = results[^1];
        Assert.AreEqual(0.8460, last.Correlation.Round(4));
        Assert.AreEqual(0.7157, last.RSquared.Round(4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => Quotes.ToCorrelation(OtherQuotes, 0));

        // bad eval quotes
        IReadOnlyList<Quote> eval = Data.GetCompare(300);
        Assert.ThrowsExactly<InvalidQuotesException>(
            () => Quotes.ToCorrelation(eval, 30));

        // mismatched quotes
        Assert.ThrowsExactly<InvalidQuotesException>(
            () => MismatchQuotes.ToCorrelation(OtherQuotes, 20));
    }
}
