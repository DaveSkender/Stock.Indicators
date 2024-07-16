namespace Series;

[TestClass]
public class McGinleyDynamicTests : SeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        List<DynamicResult> results = Quotes
            .GetDynamic(14)
            .ToList();

        // assertions
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(501, results.Count(x => x.Dynamic != null));

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
        List<DynamicResult> results = Quotes
            .Use(CandlePart.Close)
            .GetDynamic(20)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(501, results.Count(x => x.Dynamic != null));
        Assert.AreEqual(0, results.Count(x => x.Dynamic is double.NaN));
    }

    [TestMethod]
    public void Chainee()
    {
        List<DynamicResult> results = Quotes
            .GetSma(10)
            .GetDynamic(14)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(492, results.Count(x => x.Dynamic != null));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = Quotes
            .GetDynamic(14)
            .GetSma(10)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(492, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        List<DynamicResult> r = BadQuotes
            .GetDynamic(15)
            .ToList();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Dynamic is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        List<DynamicResult> r0 = Noquotes
            .GetDynamic(14)
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<DynamicResult> r1 = Onequote
            .GetDynamic(14)
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => Quotes.GetDynamic(0));

        // bad k-factor
        Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => Quotes.GetDynamic(14, 0));
    }
}
