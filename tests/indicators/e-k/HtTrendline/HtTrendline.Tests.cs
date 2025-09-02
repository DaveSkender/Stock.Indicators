namespace Tests.Indicators;

[TestClass]
public class HtTrendlineTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<HtlResult> results = quotes
            .GetHtTrendline()
            .ToList();

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.HasCount(502, results);
        Assert.AreEqual(495, results.Count(x => x.DcPeriods != null));
        Assert.AreEqual(502, results.Count(x => x.Trendline != null));
        Assert.AreEqual(496, results.Count(x => x.SmoothPrice != null));

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
    public void UseTuple()
    {
        List<HtlResult> results = quotes
            .Use(CandlePart.Close)
            .GetHtTrendline()
            .ToList();

        Assert.HasCount(502, results);
        Assert.AreEqual(502, results.Count(x => x.Trendline != null));
    }

    [TestMethod]
    public void TupleNaN()
    {
        List<HtlResult> r = tupleNanny
            .GetHtTrendline()
            .ToList();

        Assert.HasCount(200, r);
        Assert.IsEmpty(r.Where(x => x.Trendline is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public void Chainee()
    {
        List<HtlResult> results = quotes
            .GetSma(2)
            .GetHtTrendline()
            .ToList();

        Assert.HasCount(502, results);
        Assert.AreEqual(501, results.Count(x => x.Trendline != null));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = quotes
            .GetHtTrendline()
            .GetSma(10)
            .ToList();

        Assert.HasCount(502, results);
        Assert.AreEqual(493, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        List<HtlResult> r = badQuotes
            .GetHtTrendline()
            .ToList();

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(x => x.Trendline is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public void Removed()
    {
        List<HtlResult> results = quotes
            .GetHtTrendline()
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.HasCount(502 - 100, results);

        HtlResult last = results.LastOrDefault();
        Assert.AreEqual(252.2172, last.Trendline.Round(4));
        Assert.AreEqual(242.3435, last.SmoothPrice.Round(4));
    }

    [TestMethod]
    public void PennyData()
    {
        IEnumerable<Quote> penny = TestData.GetPenny();

        List<HtlResult> r = penny
            .GetHtTrendline()
            .ToList();

        Assert.HasCount(533, r);
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<HtlResult> r0 = noquotes
            .GetHtTrendline()
            .ToList();

        Assert.IsEmpty(r0);

        List<HtlResult> r1 = onequote
            .GetHtTrendline()
            .ToList();

        Assert.HasCount(1, r1);
    }
}
