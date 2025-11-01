namespace StaticSeries;

[TestClass]
public class McGinleyDynamic : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        IReadOnlyList<DynamicResult> results = Quotes
            .ToDynamic(14);

        // assertions
        Assert.HasCount(502, results);
        Assert.HasCount(501, results.Where(static x => x.Dynamic != null));

        // sample values
        DynamicResult r1 = results[1];
        Assert.AreEqual(212.9465, r1.Dynamic.Round(4));

        DynamicResult r25 = results[25];
        Assert.AreEqual(215.4801, r25.Dynamic.Round(4));

        DynamicResult r250 = results[250];
        Assert.AreEqual(256.0554, r250.Dynamic.Round(4));

        DynamicResult r501 = results[501];
        Assert.AreEqual(245.7356, r501.Dynamic.Round(4));
    }

    [TestMethod]
    public void UseReusable()
    {
        IReadOnlyList<DynamicResult> results = Quotes
            .Use(CandlePart.Close)
            .ToDynamic(20);

        Assert.HasCount(502, results);
        Assert.HasCount(501, results.Where(static x => x.Dynamic != null));
        Assert.IsEmpty(results.Where(static x => x.Dynamic is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<DynamicResult> results = Quotes
            .ToSma(10)
            .ToDynamic(14);

        Assert.HasCount(502, results);
        Assert.HasCount(492, results.Where(static x => x.Dynamic != null));
    }

    [TestMethod]
    public void Chainor()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToDynamic(14)
            .ToSma(10);

        Assert.HasCount(502, results);
        Assert.HasCount(492, results.Where(static x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<DynamicResult> r = BadQuotes
            .ToDynamic(15);

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(static x => x.Dynamic is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<DynamicResult> r0 = Noquotes
            .ToDynamic(14);

        Assert.IsEmpty(r0);

        IReadOnlyList<DynamicResult> r1 = Onequote
            .ToDynamic(14);

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToDynamic(0));

        // bad k-factor
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToDynamic(14, 0));
    }
}
