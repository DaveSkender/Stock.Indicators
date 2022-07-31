using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class Beta : TestBase
{
    [TestMethod]
    public void All()
    {
        List<BetaResult> results = otherQuotes
            .GetBeta(quotes, 20, BetaType.All)
            .ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(482, results.Count(x => x.Beta != null));
        Assert.AreEqual(482, results.Count(x => x.BetaUp != null));
        Assert.AreEqual(482, results.Count(x => x.BetaDown != null));

        // sample values
        BetaResult r19 = results[19];
        Assert.IsNull(r19.Beta);
        Assert.IsNull(r19.BetaUp);
        Assert.IsNull(r19.BetaDown);
        Assert.IsNull(r19.Ratio);
        Assert.IsNull(r19.Convexity);

        BetaResult r20 = results[20];
        Assert.AreEqual(1.5139, NullMath.Round(r20.Beta, 4));
        Assert.AreEqual(1.8007, NullMath.Round(r20.BetaUp, 4));
        Assert.AreEqual(0.3292, NullMath.Round(r20.BetaDown, 4));
        Assert.AreEqual(5.4693, NullMath.Round(r20.Ratio, 4));
        Assert.AreEqual(2.1652, NullMath.Round(r20.Convexity, 4));
        Assert.AreEqual(-0.010678, NullMath.Round(r20.ReturnsEval, 6));
        Assert.AreEqual(0.000419, NullMath.Round(r20.ReturnsMrkt, 6));

        BetaResult r249 = results[249];
        Assert.AreEqual(1.9200, NullMath.Round(r249.Beta, 4));
        Assert.AreEqual(-1.2289, NullMath.Round(r249.BetaUp, 4));
        Assert.AreEqual(-0.3956, NullMath.Round(r249.BetaDown, 4));
        Assert.AreEqual(3.1066, NullMath.Round(r249.Ratio, 4));
        Assert.AreEqual(0.6944, NullMath.Round(r249.Convexity, 4));

        BetaResult r501 = results[501];
        Assert.AreEqual(1.5123, NullMath.Round(r501.Beta, 4));
        Assert.AreEqual(2.0721, NullMath.Round(r501.BetaUp, 4));
        Assert.AreEqual(1.5908, NullMath.Round(r501.BetaDown, 4));
        Assert.AreEqual(1.3026, NullMath.Round(r501.Ratio, 4));
        Assert.AreEqual(0.2316, NullMath.Round(r501.Convexity, 4));
    }

    [TestMethod]
    public void Standard()
    {
        List<BetaResult> results = Indicator
            .GetBeta(otherQuotes, quotes, 20, BetaType.Standard)
            .ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(482, results.Count(x => x.Beta != null));

        // sample value
        BetaResult r = results[501];
        Assert.AreEqual(1.5123, NullMath.Round(r.Beta, 4));
    }

    [TestMethod]
    public void Up()
    {
        List<BetaResult> results = otherQuotes
            .GetBeta(quotes, 20, BetaType.Up)
            .ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(482, results.Count(x => x.BetaUp != null));

        // sample value
        BetaResult r = results[501];
        Assert.AreEqual(2.0721, NullMath.Round(r.BetaUp, 4));
    }

    [TestMethod]
    public void Down()
    {
        List<BetaResult> results = otherQuotes
            .GetBeta(quotes, 20, BetaType.Down)
            .ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(482, results.Count(x => x.BetaDown != null));

        // sample value
        BetaResult r = results[501];
        Assert.AreEqual(1.5908, NullMath.Round(r.BetaDown, 4));
    }

    [TestMethod]
    public void UseTuple()
    {
        IEnumerable<BetaResult> results = otherQuotes
            .Use(CandlePart.Close)
            .GetBeta(quotes.Use(CandlePart.Close), 20);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(482, results.Count(x => x.Beta != null));
    }

    [TestMethod]
    public void TupleNaN()
    {
        IEnumerable<BetaResult> r = tupleNanny.GetBeta(tupleNanny, 6);

        Assert.AreEqual(200, r.Count());
        Assert.AreEqual(0, r.Count(x => x.Beta is double and double.NaN));
    }

    [TestMethod]
    public void Chainor()
    {
        IEnumerable<SmaResult> results = otherQuotes
            .GetBeta(quotes, 20)
            .GetSma(10);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(473, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<BetaResult> r1 = Indicator
            .GetBeta(badQuotes, badQuotes, 15, BetaType.Standard);
        Assert.AreEqual(502, r1.Count());
        Assert.AreEqual(0, r1.Count(x => x.Beta is double and double.NaN));

        IEnumerable<BetaResult> r2 = Indicator
            .GetBeta(badQuotes, badQuotes, 15, BetaType.Up);
        Assert.AreEqual(502, r2.Count());
        Assert.AreEqual(0, r2.Count(x => x.BetaUp is double and double.NaN));

        IEnumerable<BetaResult> r3 = Indicator
            .GetBeta(badQuotes, badQuotes, 15, BetaType.Down);
        Assert.AreEqual(502, r3.Count());
        Assert.AreEqual(0, r3.Count(x => x.BetaDown is double and double.NaN));
    }

    [TestMethod]
    public void BigData()
    {
        IEnumerable<BetaResult> r = Indicator
            .GetBeta(bigQuotes, bigQuotes, 150, BetaType.All);
        Assert.AreEqual(1246, r.Count());
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

        IEnumerable<Quote> evalQuotes = TestData.GetMsft();
        IEnumerable<Quote> mktQuotes = TestData.GetSpx();

        List<BetaResult> results = Indicator
            .GetBeta(
                evalQuotes.Aggregate(PeriodSize.Month),
                mktQuotes.Aggregate(PeriodSize.Month),
                60, BetaType.Standard)
            .ToList();

        Assert.AreEqual(0.91, NullMath.Round(results[385].Beta, 2));
    }

    [TestMethod]
    public void Removed()
    {
        List<BetaResult> results = Indicator.GetBeta(otherQuotes, quotes, 20)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - 20, results.Count);

        BetaResult last = results.LastOrDefault();
        Assert.AreEqual(1.5123, NullMath.Round(last.Beta, 4));
    }

    [TestMethod]
    public void SameSame()
    {
        // Beta should be 1 if evaluating against self
        List<BetaResult> results = Indicator.GetBeta(quotes, quotes, 20)
            .ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(482, results.Count(x => x.Beta != null));

        // sample value
        BetaResult r = results[501];
        Assert.AreEqual(1, NullMath.Round(r.Beta, 4));
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<BetaResult> r0 = Indicator.GetBeta(noquotes, noquotes, 5);
        Assert.AreEqual(0, r0.Count());

        IEnumerable<BetaResult> r1 = Indicator.GetBeta(onequote, onequote, 5);
        Assert.AreEqual(1, r1.Count());
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetBeta(quotes, otherQuotes, 0));

        // bad evaluation quotes
        IEnumerable<Quote> eval = TestData.GetCompare(300);
        Assert.ThrowsException<InvalidQuotesException>(() =>
            Indicator.GetBeta(quotes, eval, 30));
    }
}
