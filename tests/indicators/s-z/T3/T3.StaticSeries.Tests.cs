namespace StaticSeries;

[TestClass]
public class T3 : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        IReadOnlyList<T3Result> results = Quotes
            .ToT3();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(502, results.Count(x => x.T3 != null));

        // sample values
        T3Result r5 = results[5];
        Assert.AreEqual(213.9654, r5.T3.Round(4));

        T3Result r24 = results[24];
        Assert.AreEqual(215.9481, r24.T3.Round(4));

        T3Result r44 = results[44];
        Assert.AreEqual(224.9412, r44.T3.Round(4));

        T3Result r149 = results[149];
        Assert.AreEqual(235.8851, r149.T3.Round(4));

        T3Result r249 = results[249];
        Assert.AreEqual(257.8735, r249.T3.Round(4));

        T3Result r501 = results[501];
        Assert.AreEqual(238.9308, r501.T3.Round(4));
    }

    [TestMethod]
    public void UseReusable()
    {
        IReadOnlyList<T3Result> results = Quotes
            .Use(CandlePart.Close)
            .ToT3();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(502, results.Count(x => x.T3 != null));
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<T3Result> results = Quotes
            .ToSma(2)
            .ToT3();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(501, results.Count(x => x.T3 != null));
    }

    [TestMethod]
    public void Chainor()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToT3()
            .ToSma(10);

        Assert.AreEqual(502, results.Count);
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<T3Result> r = BadQuotes
            .ToT3();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.T3 is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<T3Result> r0 = Noquotes
            .ToT3();

        Assert.AreEqual(0, r0.Count);

        IReadOnlyList<T3Result> r1 = Onequote
            .ToT3();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => Quotes.ToT3(0));

        // bad volume factor
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => Quotes.ToT3(25, 0));
    }
}
