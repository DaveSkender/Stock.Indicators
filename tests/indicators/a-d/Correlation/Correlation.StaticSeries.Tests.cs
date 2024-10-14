namespace StaticSeries;

[TestClass]
public class Correlation : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        IReadOnlyList<CorrResult> results = Quotes
            .ToCorrelation(OtherQuotes, 20);

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
    public void UseReusable()
    {
        IReadOnlyList<CorrResult> results = Quotes
            .Use(CandlePart.Close)
            .ToCorrelation(OtherQuotes.Use(CandlePart.Close), 20);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.Correlation != null));
    }

    [TestMethod]
    public void Chainor()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToCorrelation(OtherQuotes, 20)
            .ToSma(10);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(474, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<CorrResult> results = Quotes
            .ToSma(2)
            .ToCorrelation(OtherQuotes.ToSma(2), 20);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(482, results.Count(x => x.Correlation != null));
        Assert.AreEqual(0, results.Count(x => x.Correlation is double.NaN));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<CorrResult> r = BadQuotes
            .ToCorrelation(BadQuotes, 15);

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Correlation is double.NaN));
    }

    [TestMethod]
    public void BigData()
    {
        IReadOnlyList<CorrResult> r = BigQuotes
            .ToCorrelation(BigQuotes, 150);

        Assert.AreEqual(1246, r.Count);
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<CorrResult> r0 = Noquotes
            .ToCorrelation(Noquotes, 10);

        Assert.AreEqual(0, r0.Count);

        IReadOnlyList<CorrResult> r1 = Onequote
            .ToCorrelation(Onequote, 10);

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<CorrResult> results = Quotes
            .ToCorrelation(OtherQuotes, 20)
            .RemoveWarmupPeriods();

        // assertions
        Assert.AreEqual(502 - 19, results.Count);

        CorrResult last = results[^1];
        Assert.AreEqual(0.8460, last.Correlation.Round(4));
        Assert.AreEqual(0.7157, last.RSquared.Round(4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Quotes.ToCorrelation(OtherQuotes, 0));

        // bad eval quotes
        IReadOnlyList<Quote> eval = Data.GetCompare(300);
        Assert.ThrowsException<InvalidQuotesException>(() =>
            Quotes.ToCorrelation(eval, 30));

        // mismatched quotes
        Assert.ThrowsException<InvalidQuotesException>(() =>
            MismatchQuotes.ToCorrelation(OtherQuotes, 20));
    }
}
