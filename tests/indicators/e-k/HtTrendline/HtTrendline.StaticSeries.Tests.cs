namespace StaticSeries;

[TestClass]
public class HtTrendline : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        IReadOnlyList<HtlResult> results = Quotes
            .ToHtTrendline();

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.HasCount(502, results);
        Assert.HasCount(495, results.Where(static x => x.DcPeriods != null));
        Assert.HasCount(502, results.Where(static x => x.Trendline != null));
        Assert.HasCount(496, results.Where(static x => x.SmoothPrice != null));

        // sample values
        HtlResult r5 = results[5];
        Assert.IsNull(r5.DcPeriods);
        Assert.AreEqual(214.205, r5.Trendline);
        Assert.IsNull(r5.SmoothPrice);

        HtlResult r6 = results[6];
        Assert.IsNull(r6.DcPeriods);
        Assert.AreEqual(213.84, r6.Trendline);
        Assert.AreEqual(214.071, r6.SmoothPrice);

        HtlResult r7 = results[7];
        Assert.AreEqual(1, r7.DcPeriods);

        HtlResult r11 = results[11];
        Assert.AreEqual(3, r11.DcPeriods);
        Assert.AreEqual(213.9502, r11.Trendline.Round(4));
        Assert.AreEqual(213.8460, r11.SmoothPrice.Round(4));

        HtlResult r25 = results[25];
        Assert.AreEqual(14, r25.DcPeriods);
        Assert.AreEqual(215.3948, r25.Trendline.Round(4));
        Assert.AreEqual(216.3365, r25.SmoothPrice.Round(4));

        HtlResult r149 = results[149];
        Assert.AreEqual(24, r149.DcPeriods);
        Assert.AreEqual(233.9410, r149.Trendline.Round(4));
        Assert.AreEqual(235.8570, r149.SmoothPrice.Round(4));

        HtlResult r249 = results[249];
        Assert.AreEqual(25, r249.DcPeriods);
        Assert.AreEqual(253.8788, r249.Trendline.Round(4));
        Assert.AreEqual(257.5825, r249.SmoothPrice.Round(4));

        HtlResult r501 = results[501];
        Assert.AreEqual(20, r501.DcPeriods);
        Assert.AreEqual(252.2172, r501.Trendline.Round(4));
        Assert.AreEqual(242.3435, r501.SmoothPrice.Round(4));
    }

    [TestMethod]
    public void UseReusable()
    {
        IReadOnlyList<HtlResult> results = Quotes
            .Use(CandlePart.Close)
            .ToHtTrendline();

        Assert.HasCount(502, results);
        Assert.HasCount(502, results.Where(static x => x.Trendline != null));
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<HtlResult> results = Quotes
            .ToSma(2)
            .ToHtTrendline();

        Assert.HasCount(502, results);
        Assert.HasCount(501, results.Where(static x => x.Trendline != null));
    }

    [TestMethod]
    public void Chainor()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToHtTrendline()
            .ToSma(10);

        Assert.HasCount(502, results);
        Assert.HasCount(493, results.Where(static x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<HtlResult> r = BadQuotes
            .ToHtTrendline();

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(static x => x.Trendline is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<HtlResult> results = Quotes
            .ToHtTrendline()
            .RemoveWarmupPeriods();

        // assertions
        Assert.HasCount(502 - 100, results);

        HtlResult last = results[^1];
        Assert.AreEqual(252.2172, last.Trendline.Round(4));
        Assert.AreEqual(242.3435, last.SmoothPrice.Round(4));
    }

    [TestMethod]
    public void PennyData()
    {
        IReadOnlyList<Quote> penny = Data.GetPenny();

        IReadOnlyList<HtlResult> r = penny
            .ToHtTrendline();

        Assert.HasCount(533, r);
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<HtlResult> r0 = Noquotes
            .ToHtTrendline();

        Assert.IsEmpty(r0);

        IReadOnlyList<HtlResult> r1 = Onequote
            .ToHtTrendline();

        Assert.HasCount(1, r1);
    }
}
