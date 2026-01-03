namespace Tests.Indicators;

[TestClass]
public class BetaTests : TestBase
{
    [TestMethod]
    public void All()
    {
        List<BetaResult> results = otherQuotes
            .GetBeta(quotes, 20, BetaType.All)
            .ToList();

        // proper quantities
        Assert.HasCount(502, results);
        Assert.AreEqual(482, results.Count(static x => x.Beta != null));
        Assert.AreEqual(482, results.Count(static x => x.BetaUp != null));
        Assert.AreEqual(482, results.Count(static x => x.BetaDown != null));

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
    public void Standard()
    {
        List<BetaResult> results = Indicator
            .GetBeta(otherQuotes, quotes, 20, BetaType.Standard)
            .ToList();

        // proper quantities
        Assert.HasCount(502, results);
        Assert.AreEqual(482, results.Count(static x => x.Beta != null));

        // sample value
        BetaResult r = results[501];
        Assert.AreEqual(1.5123, r.Beta.Round(4));
    }

    [TestMethod]
    public void Up()
    {
        List<BetaResult> results = otherQuotes
            .GetBeta(quotes, 20, BetaType.Up)
            .ToList();

        // proper quantities
        Assert.HasCount(502, results);
        Assert.AreEqual(482, results.Count(static x => x.BetaUp != null));

        // sample value
        BetaResult r = results[501];
        Assert.AreEqual(2.0721, r.BetaUp.Round(4));
    }

    [TestMethod]
    public void Down()
    {
        List<BetaResult> results = otherQuotes
            .GetBeta(quotes, 20, BetaType.Down)
            .ToList();

        // proper quantities
        Assert.HasCount(502, results);
        Assert.AreEqual(482, results.Count(static x => x.BetaDown != null));

        // sample value
        BetaResult r = results[501];
        Assert.AreEqual(1.5908, r.BetaDown.Round(4));
    }

    [TestMethod]
    public void UseTuple()
    {
        List<BetaResult> results = otherQuotes
            .Use(CandlePart.Close)
            .GetBeta(quotes.Use(CandlePart.Close), 20)
            .ToList();

        Assert.HasCount(502, results);
        Assert.AreEqual(482, results.Count(static x => x.Beta != null));
    }

    [TestMethod]
    public void TupleNaN()
    {
        List<BetaResult> r = tupleNanny
            .GetBeta(tupleNanny, 6)
            .ToList();

        Assert.HasCount(200, r);
        Assert.IsEmpty(r.Where(static x => x.Beta is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = otherQuotes
            .GetBeta(quotes, 20)
            .GetSma(10)
            .ToList();

        Assert.HasCount(502, results);
        Assert.AreEqual(473, results.Count(static x => x.Sma != null));
    }

    [TestMethod]
    public void Chainee()
    {
        List<BetaResult> results = quotes
            .GetSma(2)
            .GetBeta(otherQuotes.GetSma(2), 20)
            .ToList();

        Assert.HasCount(502, results);
        Assert.AreEqual(481, results.Count(static x => x.Beta != null));
        Assert.IsEmpty(results.Where(static x => x.Beta is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public void BadData()
    {
        List<BetaResult> r1 = badQuotes
            .GetBeta(badQuotes, 15, BetaType.Standard)
            .ToList();

        Assert.HasCount(502, r1);
        Assert.IsEmpty(r1.Where(static x => x.Beta is double v && double.IsNaN(v)));

        List<BetaResult> r2 = badQuotes
            .GetBeta(badQuotes, 15, BetaType.Up)
            .ToList();

        Assert.HasCount(502, r2);
        Assert.IsEmpty(r2.Where(static x => x.BetaUp is double v && double.IsNaN(v)));

        List<BetaResult> r3 = badQuotes
            .GetBeta(badQuotes, 15, BetaType.Down)
            .ToList();

        Assert.HasCount(502, r3);
        Assert.IsEmpty(r3.Where(static x => x.BetaDown is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public void BigData()
    {
        List<BetaResult> r = bigQuotes
            .GetBeta(bigQuotes, 150, BetaType.All)
            .ToList();

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

        List<Quote> evalQuotes = TestData.GetMsft().ToList();
        List<Quote> mktQuotes = TestData.GetSpx().ToList();

        List<BetaResult> results = Indicator
            .GetBeta(
                evalQuotes.Aggregate(PeriodSize.Month),
                mktQuotes.Aggregate(PeriodSize.Month),
                60, BetaType.Standard)
            .ToList();

        Assert.AreEqual(0.91, results[385].Beta.Round(2));
    }

    [TestMethod]
    public void Removed()
    {
        List<BetaResult> results = otherQuotes
            .GetBeta(quotes, 20)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.HasCount(502 - 20, results);

        BetaResult last = results.LastOrDefault();
        Assert.AreEqual(1.5123, last.Beta.Round(4));
    }

    [TestMethod]
    public void SameSame()
    {
        // Beta should be 1 if evaluating against self
        List<BetaResult> results = quotes
            .GetBeta(quotes, 20)
            .ToList();

        // proper quantities
        Assert.HasCount(502, results);
        Assert.AreEqual(482, results.Count(static x => x.Beta != null));

        // sample value
        BetaResult r = results[501];
        Assert.AreEqual(1, r.Beta.Round(4));
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<BetaResult> r0 = noquotes
            .GetBeta(noquotes, 5)
            .ToList();

        Assert.IsEmpty(r0);

        List<BetaResult> r1 = onequote.GetBeta(onequote, 5).ToList();
        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void NoMatch()
    {
        List<Quote> quoteA =
        [
            new Quote { Date = DateTime.Parse("1/1/2020", EnglishCulture), Close = 1234 },
            new Quote { Date = DateTime.Parse("1/2/2020", EnglishCulture), Close = 1234 },
            new Quote { Date = DateTime.Parse("1/3/2020", EnglishCulture), Close = 1234 },
            new Quote { Date = DateTime.Parse("1/4/2020", EnglishCulture), Close = 1234 },
            new Quote { Date = DateTime.Parse("1/5/2020", EnglishCulture), Close = 1234 },
            new Quote { Date = DateTime.Parse("1/6/2020", EnglishCulture), Close = 1234 },
            new Quote { Date = DateTime.Parse("1/7/2020", EnglishCulture), Close = 1234 },
            new Quote { Date = DateTime.Parse("1/8/2020", EnglishCulture), Close = 1234 },
            new Quote { Date = DateTime.Parse("1/9/2020", EnglishCulture), Close = 1234 }
        ];

        List<Quote> quoteB =
        [
            new Quote { Date = DateTime.Parse("1/1/2020", EnglishCulture), Close = 1234 },
            new Quote { Date = DateTime.Parse("1/2/2020", EnglishCulture), Close = 1234 },
            new Quote { Date = DateTime.Parse("1/3/2020", EnglishCulture), Close = 1234 },
            new Quote { Date = DateTime.Parse("2/4/2020", EnglishCulture), Close = 1234 }, // abberrant
            new Quote { Date = DateTime.Parse("1/5/2020", EnglishCulture), Close = 1234 },
            new Quote { Date = DateTime.Parse("1/6/2020", EnglishCulture), Close = 1234 },
            new Quote { Date = DateTime.Parse("1/7/2020", EnglishCulture), Close = 1234 },
            new Quote { Date = DateTime.Parse("1/8/2020", EnglishCulture), Close = 1234 },
            new Quote { Date = DateTime.Parse("1/9/2020", EnglishCulture), Close = 1234 }
        ];

        Assert.ThrowsExactly<InvalidQuotesException>(
            () => quoteA.GetBeta(quoteB, 3));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => quotes.GetBeta(otherQuotes, 0));

        // bad evaluation quotes
        List<Quote> eval = TestData.GetCompare(300).ToList();

        Assert.ThrowsExactly<InvalidQuotesException>(
            () => quotes.GetBeta(eval, 30));
    }
}
