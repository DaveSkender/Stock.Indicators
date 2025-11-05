namespace StaticSeries;

[TestClass]
public class Beta : StaticSeriesTestBase
{
    [TestMethod]
    public void All()
    {
        IReadOnlyList<BetaResult> results = OtherQuotes
            .ToBeta(Quotes, 20, BetaType.All);

        // proper quantities
        Assert.HasCount(502, results);
        Assert.HasCount(482, results.Where(static x => x.Beta != null));
        Assert.HasCount(482, results.Where(static x => x.BetaUp != null));
        Assert.HasCount(482, results.Where(static x => x.BetaDown != null));

        // sample values
        BetaResult r19 = results[19];
        Assert.IsNull(r19.Beta);
        Assert.IsNull(r19.BetaUp);
        Assert.IsNull(r19.BetaDown);
        Assert.IsNull(r19.Ratio);
        Assert.IsNull(r19.Convexity);

        BetaResult r20 = results[20];
        Assert.AreEqual(1.5139, r20.Beta.Round(4));
        Assert.AreEqual(1.8007, r20.BetaUp.Round(4));
        Assert.AreEqual(0.3292, r20.BetaDown.Round(4));
        Assert.AreEqual(5.4693, r20.Ratio.Round(4));
        Assert.AreEqual(2.1652, r20.Convexity.Round(4));
        Assert.AreEqual(-0.010678, r20.ReturnsEval.Round(6));
        Assert.AreEqual(0.000419, r20.ReturnsMrkt.Round(6));

        BetaResult r249 = results[249];
        Assert.AreEqual(1.9200, r249.Beta.Round(4));
        Assert.AreEqual(-1.2289, r249.BetaUp.Round(4));
        Assert.AreEqual(-0.3956, r249.BetaDown.Round(4));
        Assert.AreEqual(3.1066, r249.Ratio.Round(4));
        Assert.AreEqual(0.6944, r249.Convexity.Round(4));

        BetaResult r501 = results[501];
        Assert.AreEqual(1.5123, r501.Beta.Round(4));
        Assert.AreEqual(2.0721, r501.BetaUp.Round(4));
        Assert.AreEqual(1.5908, r501.BetaDown.Round(4));
        Assert.AreEqual(1.3026, r501.Ratio.Round(4));
        Assert.AreEqual(0.2316, r501.Convexity.Round(4));
    }

    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        IReadOnlyList<BetaResult> results = OtherQuotes
            .ToBeta(Quotes, 20);

        // proper quantities
        Assert.HasCount(502, results);
        Assert.HasCount(482, results.Where(static x => x.Beta != null));

        // sample value
        BetaResult r = results[501];
        Assert.AreEqual(1.5123, r.Beta.Round(4));
    }

    [TestMethod]
    public void Up()
    {
        IReadOnlyList<BetaResult> results = OtherQuotes
            .ToBeta(Quotes, 20, BetaType.Up);

        // proper quantities
        Assert.HasCount(502, results);
        Assert.HasCount(482, results.Where(static x => x.BetaUp != null));

        // sample value
        BetaResult r = results[501];
        Assert.AreEqual(2.0721, r.BetaUp.Round(4));
    }

    [TestMethod]
    public void Down()
    {
        IReadOnlyList<BetaResult> results = OtherQuotes
            .ToBeta(Quotes, 20, BetaType.Down);

        // proper quantities
        Assert.HasCount(502, results);
        Assert.HasCount(482, results.Where(static x => x.BetaDown != null));

        // sample value
        BetaResult r = results[501];
        Assert.AreEqual(1.5908, r.BetaDown.Round(4));
    }

    [TestMethod]
    public void UseReusable()
    {
        IReadOnlyList<BetaResult> results = OtherQuotes
            .Use(CandlePart.Close)
            .ToBeta(Quotes.Use(CandlePart.Close), 20);

        Assert.HasCount(502, results);
        Assert.HasCount(482, results.Where(static x => x.Beta != null));
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<SmaResult> results = OtherQuotes
            .ToBeta(Quotes, 20)
            .ToSma(10);

        Assert.HasCount(502, results);
        Assert.HasCount(473, results.Where(static x => x.Sma != null));
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<BetaResult> results = Quotes
            .ToSma(2)
            .ToBeta(OtherQuotes.ToSma(2), 20);

        Assert.HasCount(502, results);
        Assert.HasCount(481, results.Where(static x => x.Beta != null));
        Assert.IsEmpty(results.Where(static x => x.Beta is double.NaN));
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<BetaResult> r1 = BadQuotes
            .ToBeta(BadQuotes, 15);

        Assert.HasCount(502, r1);
        Assert.IsEmpty(r1.Where(static x => x.Beta is double.NaN));

        IReadOnlyList<BetaResult> r2 = BadQuotes
            .ToBeta(BadQuotes, 15, BetaType.Up);

        Assert.HasCount(502, r2);
        Assert.IsEmpty(r2.Where(static x => x.BetaUp is double.NaN));

        IReadOnlyList<BetaResult> r3 = BadQuotes
            .ToBeta(BadQuotes, 15, BetaType.Down);

        Assert.HasCount(502, r3);
        Assert.IsEmpty(r3.Where(static x => x.BetaDown is double.NaN));
    }

    [TestMethod]
    public void BigQuoteValues_DoesNotFail()
    {
        IReadOnlyList<BetaResult> r = BigQuotes
            .ToBeta(BigQuotes, 150, BetaType.All);

        Assert.HasCount(1246, r);
    }

    [TestMethod]
    public void BetaMsft()
    {
        // should produce 0.91 for 5-yr monthly values
        // multiple reputable sites show this number
        /*
          https://finance.yahoo.com/quote/MSFT
          https://www.zacks.com/stock/chart/MSFT/fundamental/beta
          https://www.nasdaq.com/market-activity/stocks/msft
        */

        IReadOnlyList<Quote> evalQuotes = Data.GetMsft();
        IReadOnlyList<Quote> mktQuotes = Data.GetSpx();

        IReadOnlyList<BetaResult> results = evalQuotes
            .Aggregate(PeriodSize.Month)
            .ToBeta(mktQuotes.Aggregate(PeriodSize.Month), 60);

        Assert.AreEqual(0.91, results[385].Beta.Round(2));
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<BetaResult> results = OtherQuotes
            .ToBeta(Quotes, 20)
            .RemoveWarmupPeriods();

        // assertions
        Assert.HasCount(502 - 20, results);

        BetaResult last = results[^1];
        Assert.AreEqual(1.5123, last.Beta.Round(4));
    }

    [TestMethod]
    public void SameSame()
    {
        // Beta should be 1 if evaluating against self
        IReadOnlyList<BetaResult> results = Quotes
            .ToBeta(Quotes, 20);

        // proper quantities
        Assert.HasCount(502, results);
        Assert.HasCount(482, results.Where(static x => x.Beta != null));

        // sample value
        BetaResult r = results[501];
        Assert.AreEqual(1, r.Beta.Round(4));
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<BetaResult> r0 = Noquotes
            .ToBeta(Noquotes, 5);

        Assert.IsEmpty(r0);

        IReadOnlyList<BetaResult> r1 = Onequote
            .ToBeta(Onequote, 5);

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void NoMatch()
    {
        IReadOnlyList<Quote> quoteA =
        [
            new(DateTime.Parse("1/1/2020", invariantCulture), 0, 0, 0, 1234, 0),
            new(DateTime.Parse("1/2/2020", invariantCulture), 0, 0, 0, 1234, 0),
            new(DateTime.Parse("1/3/2020", invariantCulture), 0, 0, 0, 1234, 0),
            new(DateTime.Parse("1/4/2020", invariantCulture), 0, 0, 0, 1234, 0),
            new(DateTime.Parse("1/5/2020", invariantCulture), 0, 0, 0, 1234, 0),
            new(DateTime.Parse("1/6/2020", invariantCulture), 0, 0, 0, 1234, 0),
            new(DateTime.Parse("1/7/2020", invariantCulture), 0, 0, 0, 1234, 0),
            new(DateTime.Parse("1/8/2020", invariantCulture), 0, 0, 0, 1234, 0),
            new(DateTime.Parse("1/9/2020", invariantCulture), 0, 0, 0, 1234, 0)
        ];

        IReadOnlyList<Quote> quoteB =
        [
            new(DateTime.Parse("1/1/2020", invariantCulture), 0, 0, 0, 1234, 0),
            new(DateTime.Parse("1/2/2020", invariantCulture), 0, 0, 0, 1234, 0),
            new(DateTime.Parse("1/3/2020", invariantCulture), 0, 0, 0, 1234, 0),
            new(DateTime.Parse("2/4/2020", invariantCulture), 0, 0, 0, 1234, 0), // abberrant
            new(DateTime.Parse("1/5/2020", invariantCulture), 0, 0, 0, 1234, 0),
            new(DateTime.Parse("1/6/2020", invariantCulture), 0, 0, 0, 1234, 0),
            new(DateTime.Parse("1/7/2020", invariantCulture), 0, 0, 0, 1234, 0),
            new(DateTime.Parse("1/8/2020", invariantCulture), 0, 0, 0, 1234, 0),
            new(DateTime.Parse("1/9/2020", invariantCulture), 0, 0, 0, 1234, 0)
        ];

        Assert.ThrowsExactly<InvalidQuotesException>(
            () => quoteA.ToBeta(quoteB, 3));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => Quotes.ToBeta(OtherQuotes, 0));

        // bad evaluation quotes
        IReadOnlyList<Quote> eval = Data.GetCompare(300).ToList();

        Assert.ThrowsExactly<InvalidQuotesException>(
            () => Quotes.ToBeta(eval, 30));
    }
}
