namespace StaticSeries;

[TestClass]
public class Beta : StaticSeriesTestBase
{
    [TestMethod]
    public void All()
    {
        IReadOnlyList<BetaResult> sut = OtherQuotes
            .ToBeta(Quotes, 20, BetaType.All);

        // proper quantities
        sut.Should().HaveCount(502);
        sut.Where(static x => x.Beta != null).Should().HaveCount(482);
        sut.Where(static x => x.BetaUp != null).Should().HaveCount(482);
        sut.Where(static x => x.BetaDown != null).Should().HaveCount(482);

        // sample values
        BetaResult r19 = sut[19];
        r19.Beta.Should().BeNull();
        r19.BetaUp.Should().BeNull();
        r19.BetaDown.Should().BeNull();
        r19.Ratio.Should().BeNull();
        r19.Convexity.Should().BeNull();

        BetaResult r20 = sut[20];
        r20.Beta.Should().BeApproximately(1.5139, Money4);
        r20.BetaUp.Should().BeApproximately(1.8007, Money4);
        r20.BetaDown.Should().BeApproximately(0.3292, Money4);
        r20.Ratio.Should().BeApproximately(5.4693, Money4);
        r20.Convexity.Should().BeApproximately(2.1652, Money4);
        r20.ReturnsEval.Should().BeApproximately(-0.010678, Money6);
        r20.ReturnsMrkt.Should().BeApproximately(0.000419, Money6);

        BetaResult r249 = sut[249];
        r249.Beta.Should().BeApproximately(1.9200, Money4);
        r249.BetaUp.Should().BeApproximately(-1.2289, Money4);
        r249.BetaDown.Should().BeApproximately(-0.3956, Money4);
        r249.Ratio.Should().BeApproximately(3.1066, Money4);
        r249.Convexity.Should().BeApproximately(0.6944, Money4);

        BetaResult r501 = sut[501];
        r501.Beta.Should().BeApproximately(1.5123, Money4);
        r501.BetaUp.Should().BeApproximately(2.0721, Money4);
        r501.BetaDown.Should().BeApproximately(1.5908, Money4);
        r501.Ratio.Should().BeApproximately(1.3026, Money4);
        r501.Convexity.Should().BeApproximately(0.2316, Money4);
    }

    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        IReadOnlyList<BetaResult> sut = OtherQuotes
            .ToBeta(Quotes, 20);

        // proper quantities
        sut.Should().HaveCount(502);
        sut.Where(static x => x.Beta != null).Should().HaveCount(482);

        // sample value
        BetaResult r = sut[501];
        r.Beta.Should().BeApproximately(1.5123, Money4);
    }

    [TestMethod]
    public void Up()
    {
        IReadOnlyList<BetaResult> sut = OtherQuotes
            .ToBeta(Quotes, 20, BetaType.Up);

        // proper quantities
        sut.Should().HaveCount(502);
        sut.Where(static x => x.BetaUp != null).Should().HaveCount(482);

        // sample value
        BetaResult r = sut[501];
        r.BetaUp.Should().BeApproximately(2.0721, Money4);
    }

    [TestMethod]
    public void Down()
    {
        IReadOnlyList<BetaResult> sut = OtherQuotes
            .ToBeta(Quotes, 20, BetaType.Down);

        // proper quantities
        sut.Should().HaveCount(502);
        sut.Where(static x => x.BetaDown != null).Should().HaveCount(482);

        // sample value
        BetaResult r = sut[501];
        r.BetaDown.Should().BeApproximately(1.5908, Money4);
    }

    [TestMethod]
    public void UseReusable()
    {
        IReadOnlyList<BetaResult> sut = OtherQuotes
            .Use(CandlePart.Close)
            .ToBeta(Quotes.Use(CandlePart.Close), 20);

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Beta != null).Should().HaveCount(482);
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<SmaResult> sut = OtherQuotes
            .ToBeta(Quotes, 20)
            .ToSma(10);

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Sma != null).Should().HaveCount(473);
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<BetaResult> sut = Quotes
            .ToSma(2)
            .ToBeta(OtherQuotes.ToSma(2), 20);

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Beta != null).Should().HaveCount(481);
        sut.Where(static x => x.Beta is double.NaN).Should().BeEmpty();
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<BetaResult> r1 = BadQuotes
            .ToBeta(BadQuotes, 15);

        r1.Should().HaveCount(502);
        r1.Where(static x => x.Beta is double.NaN).Should().BeEmpty();

        IReadOnlyList<BetaResult> r2 = BadQuotes
            .ToBeta(BadQuotes, 15, BetaType.Up);

        r2.Should().HaveCount(502);
        r2.Where(static x => x.BetaUp is double.NaN).Should().BeEmpty();

        IReadOnlyList<BetaResult> r3 = BadQuotes
            .ToBeta(BadQuotes, 15, BetaType.Down);

        r3.Should().HaveCount(502);
        r3.Where(static x => x.BetaDown is double.NaN).Should().BeEmpty();
    }

    [TestMethod]
    public void BigQuoteValues_DoesNotFail()
    {
        IReadOnlyList<BetaResult> r = BigQuotes
            .ToBeta(BigQuotes, 150, BetaType.All);

        r.Should().HaveCount(1246);
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

        IReadOnlyList<BetaResult> sut = evalQuotes
            .Aggregate(PeriodSize.Month)
            .ToBeta(mktQuotes.Aggregate(PeriodSize.Month), 60);

        sut[385].Beta.Should().BeApproximately(0.91, 0.005);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<BetaResult> sut = OtherQuotes
            .ToBeta(Quotes, 20)
            .RemoveWarmupPeriods();

        // assertions
        sut.Should().HaveCount(502 - 20);

        BetaResult last = sut[^1];
        last.Beta.Should().BeApproximately(1.5123, Money4);
    }

    [TestMethod]
    public void SameSame()
    {
        // Beta should be 1 if evaluating against self
        IReadOnlyList<BetaResult> sut = Quotes
            .ToBeta(Quotes, 20);

        // proper quantities
        sut.Should().HaveCount(502);
        sut.Where(static x => x.Beta != null).Should().HaveCount(482);

        // sample value
        BetaResult r = sut[501];
        r.Beta.Should().BeApproximately(1, Money4);
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<BetaResult> r0 = Noquotes
            .ToBeta(Noquotes, 5);

        r0.Should().BeEmpty();

        IReadOnlyList<BetaResult> r1 = Onequote
            .ToBeta(Onequote, 5);

        r1.Should().HaveCount(1);
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
