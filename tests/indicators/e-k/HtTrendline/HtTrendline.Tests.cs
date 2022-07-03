using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class HtTrendline : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<HtlResult> results = quotes.GetHtTrendline().ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(502, results.Count(x => x.Trendline != null));
        Assert.AreEqual(496, results.Count(x => x.SmoothPrice != null));

        // sample values
        HtlResult r1 = results[5];
        Assert.AreEqual(214.205, r1.Trendline);
        Assert.AreEqual(null, r1.SmoothPrice);

        HtlResult r2 = results[6];
        Assert.AreEqual(213.84, r2.Trendline);
        Assert.AreEqual(214.071, r2.SmoothPrice);

        HtlResult r3 = results[11];
        Assert.AreEqual(213.9502, NullMath.Round(r3.Trendline, 4));
        Assert.AreEqual(213.8460, NullMath.Round(r3.SmoothPrice, 4));

        HtlResult r4 = results[25];
        Assert.AreEqual(215.3948, NullMath.Round(r4.Trendline, 4));
        Assert.AreEqual(216.3365, NullMath.Round(r4.SmoothPrice, 4));

        HtlResult r5 = results[149];
        Assert.AreEqual(233.9410, NullMath.Round(r5.Trendline, 4));
        Assert.AreEqual(235.8570, NullMath.Round(r5.SmoothPrice, 4));

        HtlResult r6 = results[249];
        Assert.AreEqual(253.8788, NullMath.Round(r6.Trendline, 4));
        Assert.AreEqual(257.5825, NullMath.Round(r6.SmoothPrice, 4));

        HtlResult r7 = results[501];
        Assert.AreEqual(252.2172, NullMath.Round(r7.Trendline, 4));
        Assert.AreEqual(242.3435, NullMath.Round(r7.SmoothPrice, 4));
    }

    [TestMethod]
    public void UseTuple()
    {
        IEnumerable<HtlResult> results = quotes
            .Use(CandlePart.Close)
            .GetHtTrendline();

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(502, results.Count(x => x.Trendline != null));
    }

    [TestMethod]
    public void TupleNaN()
    {
        IEnumerable<HtlResult> r = tupleNanny.GetHtTrendline();

        Assert.AreEqual(200, r.Count());
        Assert.AreEqual(0, r.Count(x => x.Trendline is double and double.NaN));
    }

    [TestMethod]
    public void Chainee()
    {
        IEnumerable<HtlResult> results = quotes
            .GetSma(2)
            .GetHtTrendline();

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(501, results.Count(x => x.Trendline != null));
    }

    [TestMethod]
    public void Chainor()
    {
        IEnumerable<SmaResult> results = quotes
            .GetHtTrendline()
            .GetSma(10);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(493, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<HtlResult> r = Indicator.GetHtTrendline(badQuotes);
        Assert.AreEqual(502, r.Count());
        Assert.AreEqual(0, r.Count(x => x.Trendline is double and double.NaN));
    }

    [TestMethod]
    public void Removed()
    {
        List<HtlResult> results = quotes.GetHtTrendline()
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - 100, results.Count);

        HtlResult last = results.LastOrDefault();
        Assert.AreEqual(252.2172, NullMath.Round(last.Trendline, 4));
        Assert.AreEqual(242.3435, NullMath.Round(last.SmoothPrice, 4));
    }

    [TestMethod]
    public void PennyData()
    {
        IEnumerable<Quote> penny = TestData.GetPenny();
        IEnumerable<HtlResult> r = Indicator.GetHtTrendline(penny);
        Assert.AreEqual(533, r.Count());
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<HtlResult> r0 = noquotes.GetHtTrendline();
        Assert.AreEqual(0, r0.Count());

        IEnumerable<HtlResult> r1 = onequote.GetHtTrendline();
        Assert.AreEqual(1, r1.Count());
    }
}
